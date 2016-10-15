using System;
using ICities;
using UnityEngine;
using ColossalFramework.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.ComponentModel;
using System.IO;
using ColossalFramework.Packaging;
using ColossalFramework.UI;
using System.Linq;
using VehicleEffects.Effects;
using VehicleEffects.GameExtensions;

namespace VehicleEffects
{
    public class VehicleEffectsMod : LoadingExtensionBase, IUserMod
    {
        public HashSet<string> vehicleEffectsDefParseErrors;

        private GameObject gameObject;

        private List<VehicleEffectsDefinition.Vehicle> effectPlacementRequests;
        private Dictionary<VehicleEffectWrapper.VehicleEffectParams, EffectInfo> customEffects;
        private List<EffectChange> changes = new List<EffectChange>();
        private bool isLoaded;

        private class EffectChange
        {
            public VehicleInfo vehicle;
            public VehicleInfo.Effect[] effects;
        }

        public string Description
        {
            get
            {
                return "Allows some extra effects to be added to vehicles";
            }
        }

        public string Name
        {
            get
            {
                return "Vehicle Effects (Alpha)";
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if(mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
            {
                isLoaded = false;
                return;
            }

            gameObject = new GameObject("Vehicle Effects Mod");
            CreateCustomEffects();
            UpdateVehicleEffects();

            if(vehicleEffectsDefParseErrors?.Count > 0)
            {
                var errorMessage = vehicleEffectsDefParseErrors.Aggregate("Error while parsing light-prop definition file(s). Contact the author of the assets. \n" + "List of errors:\n", (current, error) => current + (error + '\n'));

                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Vehicle Effects", errorMessage, true);
            }

            vehicleEffectsDefParseErrors = null;

            isLoaded = true;
        }

        public override void OnLevelUnloading()
        {
            if(!isLoaded)
            {
                return;
            }

            CustomVehicleEffect.OnLevelUnloading();
            customEffects.Clear();
            ResetVehicleEffects();
            GameObject.Destroy(gameObject);
        }

        private EffectInfo GetCustomEffectVariant(VehicleEffectWrapper.VehicleEffectParams param)
        {
            EffectInfo result;
            customEffects.TryGetValue(param, out result);
            return result;
        }

        private void CreateCustomEffects()
        {
            if(customEffects == null)
            {
                customEffects = new Dictionary<VehicleEffectWrapper.VehicleEffectParams, EffectInfo>();
            }

            // Wrappers
            CustomVehicleEffect.CreateEffectObject(gameObject.transform);

            // Custom particles
            VehicleSteam.CreateEffectObject(gameObject.transform);

            // Custom sounds
            SteamTrainMovement.CreateEffectObject(gameObject.transform);
            DieselTrainMovement.CreateEffectObject(gameObject.transform);
        }

        private void ResetVehicleEffects()
        {
            foreach(var change in changes)
            {
                change.vehicle.m_effects = change.effects;
            }
            changes.Clear();
        }

        private void UpdateVehicleEffects()
        {
            try
            {
                effectPlacementRequests = new List<VehicleEffectsDefinition.Vehicle>();
                vehicleEffectsDefParseErrors = new HashSet<string>();
                var checkedPaths = new List<string>();

                for(uint i = 0; i < PrefabCollection<VehicleInfo>.LoadedCount(); i++)
                {
                    var prefab = PrefabCollection<VehicleInfo>.GetLoaded(i);

                    if(prefab == null) continue;

                    // search for VehicleEffectsDefinition.xml
                    var asset = PackageManager.FindAssetByName(prefab.name);

                    var crpPath = asset?.package?.packagePath;
                    if(crpPath == null) continue;

                    var lightPropsDefPath = Path.Combine(Path.GetDirectoryName(crpPath) ?? "",
                        "VehicleEffectsDefinition.xml");

                    // skip files which were already parsed
                    if(checkedPaths.Contains(lightPropsDefPath)) continue;
                    checkedPaths.Add(lightPropsDefPath);

                    if(!File.Exists(lightPropsDefPath)) continue;

                    VehicleEffectsDefinition vehicleEffectsDef = null;

                    var xmlSerializer = new XmlSerializer(typeof(VehicleEffectsDefinition));
                    try
                    {
                        using(var streamReader = new System.IO.StreamReader(lightPropsDefPath))
                        {
                            vehicleEffectsDef = xmlSerializer.Deserialize(streamReader) as VehicleEffectsDefinition;
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.LogException(e);
                        vehicleEffectsDefParseErrors.Add(asset.package.packageName + " - " + e.Message);
                        continue;
                    }

                    if(vehicleEffectsDef?.Vehicles == null || vehicleEffectsDef.Vehicles.Count == 0)
                    {
                        vehicleEffectsDefParseErrors.Add(asset.package.packageName + " - vehicleEffectDef is null or empty.");
                        continue;
                    }

                    foreach(var vehicleDef in vehicleEffectsDef.Vehicles)
                    {
                        if(vehicleDef?.Name == null)
                        {
                            vehicleEffectsDefParseErrors.Add(asset.package.packageName + " - Vehicle name missing.");
                            continue;
                        }

                        Debug.Log("Adding effects to " + vehicleDef.Name);

                        var vehicleDefPrefab = FindVehicle(vehicleDef.Name, asset.package.packageName);

                        if(vehicleDefPrefab == null)
                        {
                            vehicleEffectsDefParseErrors.Add(asset.package.packageName + " - Vehicle with name " + vehicleDef.Name +
                                                         " not loaded.");
                            continue;
                        }

                        if(vehicleDef.Effects == null || vehicleDef.Effects.Count == 0)
                        {
                            vehicleEffectsDefParseErrors.Add(asset.package.packageName + " - No effects specified for " +
                                                         vehicleDef.Name + ".");
                            continue;
                        }

                        var change = new EffectChange();
                        change.vehicle = vehicleDefPrefab;
                        change.effects = new VehicleInfo.Effect[vehicleDefPrefab.m_effects.Length];
                        vehicleDefPrefab.m_effects.CopyTo(change.effects, 0);
                        changes.Add(change);

                        var newEffects = new List<VehicleInfo.Effect>();
                        foreach(var effectDef in vehicleDef.Effects)
                        {
                            if(effectDef?.Name == null)
                            {
                                vehicleEffectsDefParseErrors.Add(vehicleDef.Name + " - Effect name missing.");
                                continue;
                            }

                            string effectName = (effectDef.Replacment == null) ? effectDef.Name : effectDef.Replacment;
                            effectName += (effectDef.Base != null) ? " - " + effectDef.Base : "";


                            var desiredEffectVariant = new VehicleEffectWrapper.VehicleEffectParams();
                            desiredEffectVariant.m_name = effectName;
                            desiredEffectVariant.m_position = effectDef.Position?.ToUnityVector() ?? Vector3.zero;
                            desiredEffectVariant.m_direction = effectDef.Direction?.ToUnityVector() ?? Vector3.zero;
                            desiredEffectVariant.m_maxSpeed = effectDef.MaxSpeed;
                            desiredEffectVariant.m_minSpeed = effectDef.MinSpeed;


                            var effectPrefab = FindEffect(effectName) ?? GetCustomEffectVariant(desiredEffectVariant);

                            if(effectPrefab == null)
                            {
                                // Could be a missing effect or a non-existing effect wrapper
                                if(effectName.StartsWith("Vehicle Effect Wrapper"))
                                {
                                    if(effectDef.Base != null)
                                    {
                                        var baseEffect = FindEffect(effectDef.Base);
                                        if(baseEffect != null)
                                        {
                                            effectPrefab = CustomVehicleEffect.CreateEffect(baseEffect, desiredEffectVariant);
                                            if(effectPrefab != null)
                                            {
                                                customEffects.Add(desiredEffectVariant, effectPrefab);
                                            }
                                            else
                                            {
                                                vehicleEffectsDefParseErrors.Add(vehicleDef.Name + ": An error occured trying to create a custom effect. Check debug log for details.");
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            vehicleEffectsDefParseErrors.Add(vehicleDef.Name + ": Vehicle Effect Wrapper base effect " + effectDef.Base + " was not loaded!");
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        vehicleEffectsDefParseErrors.Add(vehicleDef.Name + ": Vehicle Effect Wrapper requires a base effect but it wasn't given");
                                        continue;
                                    }
                                }
                                else
                                {
                                    vehicleEffectsDefParseErrors.Add(vehicleDef.Name + " requested non-existing effect " + effectName);
                                    continue;
                                }
                            }

                            if(effectPrefab == null)
                            {
                                vehicleEffectsDefParseErrors.Add(vehicleDef.Name + " - Effect with name " + effectDef.Name + " not loaded.");
                                continue;
                            }

                            if(effectDef.Replacment == null)
                            {
                                // TODO: Do something with flags?
                                var effect = new VehicleInfo.Effect
                                {
                                    m_effect = effectPrefab,
                                    m_parkedFlagsForbidden = VehicleParked.Flags.Created,
                                    m_parkedFlagsRequired = VehicleParked.Flags.None,
                                    m_vehicleFlagsForbidden = 0,
                                    m_vehicleFlagsRequired = Vehicle.Flags.Created
                                };

                                newEffects.Add(effect);
                            }
                            else
                            {
                                // Replace old effect with the new one
                                for(int j = 0; j < vehicleDefPrefab.m_effects.Length; j++)
                                {
                                    if(vehicleDefPrefab.m_effects[j].m_effect.name.Equals(effectDef.Name))
                                    {
                                        VehicleInfo.Effect replacementEffect = vehicleDefPrefab.m_effects[j];
                                        replacementEffect.m_effect = effectPrefab;

                                        vehicleDefPrefab.m_effects[j] = replacementEffect;
                                        Debug.Log("Replaced effect " + effectDef.Name + " with " + effectDef.Replacment);
                                    }
                                }
                            }
                        }

                        // Update the VehicleInfo with the new effects
                        VehicleInfo.Effect[] moddedEffects = new VehicleInfo.Effect[newEffects.Count + vehicleDefPrefab.m_effects.Length];
                        vehicleDefPrefab.m_effects.CopyTo(moddedEffects, 0);
                        if(newEffects.Count > 0)
                        {
                            var tmp = newEffects.ToArray();
                            tmp.CopyTo(moddedEffects, vehicleDefPrefab.m_effects.Length);
                        }

                        vehicleDefPrefab.m_effects = moddedEffects;


                        // taken from VehicleInfo.InitializePrefab
                        if(vehicleDefPrefab.m_effects != null)
                        {
                            for(int j = 0; j < vehicleDefPrefab.m_effects.Length; j++)
                            {
                                if(vehicleDefPrefab.m_effects[j].m_effect != null)
                                {
                                    vehicleDefPrefab.m_effects[j].m_effect.InitializeEffect();
                                }
                            }
                        }
                    }
                }


            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }


        public static EffectInfo FindEffect(string effectName)
        {
            // Particle Effects aren't all added to EffectCollection, so search for GameObjects as well
            var effect = EffectCollection.FindEffect(effectName) ?? GameObject.Find(effectName)?.GetComponent<EffectInfo>();
            return effect;
        }

        private static VehicleInfo FindVehicle(string prefabName, string packageName)
        {
            var prefab = PrefabCollection<VehicleInfo>.FindLoaded(prefabName) ??
                         PrefabCollection<VehicleInfo>.FindLoaded(prefabName + "_Data") ??
                         PrefabCollection<VehicleInfo>.FindLoaded(PathEscaper.Escape(prefabName) + "_Data") ??
                         PrefabCollection<VehicleInfo>.FindLoaded(packageName + "." + prefabName + "_Data") ??
                         PrefabCollection<VehicleInfo>.FindLoaded(packageName + "." + PathEscaper.Escape(prefabName) + "_Data");

            return prefab;
        }
    }
}
