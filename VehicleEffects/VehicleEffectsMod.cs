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

            InitializeGameObjects();

            UpdateVehicleEffects();

            if(vehicleEffectsDefParseErrors?.Count > 0)
            {
                var errorMessage = vehicleEffectsDefParseErrors.Aggregate("Error while parsing vehicle effect definition file(s). Contact the author of the assets. \n" + "List of errors:\n", (current, error) => current + (error + '\n'));

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

            ResetVehicleEffects();
        }

        private void InitializeGameObjects()
        {
            Debug.Log("Vehicle Effects Mod - Initializing Game Objects");
            if(gameObject == null)
            {
                Debug.Log("Vehicle Effects Mod - Game Objects not created, creating new Game Objects");
                gameObject = new GameObject("Vehicle Effects Mod");
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                CreateCustomEffects();
            }
            Debug.Log("Vehicle Effects Mod - Done initializing Game Objects");
        }

        private void CreateCustomEffects()
        {
            Debug.Log("Vehicle Effects Mod - Creating effect objects");

            // Wrappers
            CustomVehicleEffect.CreateEffectObject(gameObject.transform);
            CustomVehicleMultiEffect.CreateEffectObject(gameObject.transform);

            // Custom particles
            VehicleSteam.CreateEffectObject(gameObject.transform);
            DieselSmoke.CreateEffectObject(gameObject.transform);

            // Custom sounds
            SteamTrainMovement.CreateEffectObject(gameObject.transform);
            DieselTrainMovement.CreateEffectObject(gameObject.transform);
            RollingTrainMovement.CreateEffectObject(gameObject.transform);
            TrainHorn.CreateEffectObject(gameObject.transform);
            TrainWhistle.CreateEffectObject(gameObject.transform);
            TrainBell.CreateEffectObject(gameObject.transform);

            // Custom lights
            TrainDitchLight.CreateEffectObject(gameObject.transform);

            Debug.Log("Vehicle Effects Mod - Done creating effect objects");
        }

        private void ResetVehicleEffects()
        {
            Debug.Log("Vehicle Effects Mod - Starting effect reset");
            foreach(var change in changes)
            {
                change.vehicle.m_effects = change.effects;
            }
            changes.Clear();
            Debug.Log("Vehicle Effects Mod - Done resetting effects");
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

                        var vehicleEffectsList = new List<VehicleInfo.Effect>();
                        vehicleEffectsList.AddRange(vehicleDefPrefab.m_effects);

                        foreach(var effectDef in vehicleDef.Effects)
                        {
                            ParseEffectDefinition(vehicleDef, effectDef, vehicleEffectsList);
                        }

                        // Update the VehicleInfo with the new effects
                        vehicleDefPrefab.m_effects = vehicleEffectsList.ToArray();


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

        private void ParseEffectDefinition(VehicleEffectsDefinition.Vehicle vehicleDef, VehicleEffectsDefinition.Effect effectDef, List<VehicleInfo.Effect> effectsList)
        {
            if(effectDef?.Name == null)
            {
                vehicleEffectsDefParseErrors.Add(vehicleDef.Name + " - Effect name missing.");
                return;
            }

            string effectName = (effectDef.Replacment == null) ? effectDef.Name : effectDef.Replacment;
            effectName += (effectDef.Base != null) ? " - " + effectDef.Base : "";

            // Parameters for wrappers
            var desiredEffectVariant = new VehicleEffectWrapper.VehicleEffectParams();
            desiredEffectVariant.m_name = effectName;
            desiredEffectVariant.m_position = effectDef.Position?.ToUnityVector() ?? Vector3.zero;
            desiredEffectVariant.m_direction = effectDef.Direction?.ToUnityVector() ?? Vector3.zero;
            desiredEffectVariant.m_maxSpeed = Util.SpeedKmHToEffect(effectDef.MaxSpeed);
            desiredEffectVariant.m_minSpeed = Util.SpeedKmHToEffect(effectDef.MinSpeed);


            var effectPrefab = FindEffect(effectName);

            if(effectPrefab == null)
            {
                //This effect is either a 'special' effect or simply non existent

                if(effectName.StartsWith("Vehicle Effect Wrapper"))
                {
                    // Wrapper for effects
                    if(effectDef.Base != null)
                    {
                        var baseEffect = FindEffect(effectDef.Base);
                        if(baseEffect != null)
                        {
                            effectPrefab = CustomVehicleEffect.CreateEffect(baseEffect, desiredEffectVariant);
                            if(effectPrefab == null)
                            {
                                vehicleEffectsDefParseErrors.Add(vehicleDef.Name + ": An error occured trying to create a custom effect. Check debug log for details.");
                                return;
                            }
                        }
                        else
                        {
                            vehicleEffectsDefParseErrors.Add(vehicleDef.Name + ": Vehicle Effect Wrapper base effect " + effectDef.Base + " was not loaded!");
                            return;
                        }
                    }
                    else
                    {
                        vehicleEffectsDefParseErrors.Add(vehicleDef.Name + ": Vehicle Effect Wrapper requires a base effect but it wasn't given");
                        return;
                    }
                }
                else if(effectName.StartsWith("Vehicle Effect Multi"))
                {
                    // Custom MultiEffect defined in the xml
                    if(effectDef.SubEffects != null)
                    {
                        List<MultiEffect.SubEffect> loadedSubEffects = new List<MultiEffect.SubEffect>();
                        foreach(var subEffect in effectDef.SubEffects)
                        {
                            var dummyList = new List<VehicleInfo.Effect>();
                            ParseEffectDefinition(vehicleDef, subEffect.Effect, dummyList);
                            if(dummyList.Count > 0)
                            {
                                loadedSubEffects.Add(new MultiEffect.SubEffect()
                                {
                                    m_effect = dummyList[0].m_effect,
                                    m_endTime = subEffect.EndTime,
                                    m_startTime = subEffect.StartTime,
                                    m_probability = subEffect.Probability
                                });
                            }
                        }
                        effectPrefab = CustomVehicleMultiEffect.CreateEffect(desiredEffectVariant, loadedSubEffects.ToArray(), effectDef.Duration, effectDef.UseSimulationTime);
                    }
                    else
                    {
                        vehicleEffectsDefParseErrors.Add(vehicleDef.Name + " Vehicle Effect Multi with no sub effects!");
                        return;
                    }
                }
                else if(effectName.StartsWith("None"))
                {
                    // Not a real effect, but keyword used for removal of an existing effect.
                    effectPrefab = null;
                }
                else
                {
                    vehicleEffectsDefParseErrors.Add(vehicleDef.Name + " requested non-existing effect " + effectName);
                    return;
                }
            }

            if(effectPrefab == null && !effectName.Equals("None"))
            {
                vehicleEffectsDefParseErrors.Add(vehicleDef.Name + " - Effect with name " + effectDef.Name + " not loaded.");
                return;
            }

            var vehicleInfoEffect = new VehicleInfo.Effect
            {
                m_effect = effectPrefab,
                m_parkedFlagsForbidden = VehicleParked.Flags.Created,
                m_parkedFlagsRequired = VehicleParked.Flags.None,
                m_vehicleFlagsForbidden = (Vehicle.Flags)effectDef.ForbiddenFlags,
                m_vehicleFlagsRequired = (Vehicle.Flags)effectDef.RequiredFlags
            };

            if(effectDef.Replacment == null)
            {
                effectsList.Add(vehicleInfoEffect);
            }
            else
            {
                int indexToRemove = -1;

                for(int i = 0; i < effectsList.Count; i++)
                {
                    if(effectsList[i].m_effect.name.Equals(effectDef.Name))
                    {
                        if(effectDef.Replacment.Equals("None"))
                        {
                            indexToRemove = i;
                            break;
                        }

                        effectsList[i] = vehicleInfoEffect;
                        break;
                    }
                }

                if(indexToRemove >= 0)
                {
                    effectsList.RemoveAt(indexToRemove);
                }
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
