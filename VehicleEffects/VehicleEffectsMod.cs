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
using ColossalFramework;

namespace VehicleEffects
{
    public partial class VehicleEffectsMod : LoadingExtensionBase, IUserMod
    {
        public const string name = "Vehicle Effects";
        public const string version = "1.9.3";

        private SavedBool showParseErrors;
        private SavedBool enableEditor;
        private SavedBool showEditorWarning;
        private SavedBool ignoreModVehicleParseErrors;

        private GameObject gameObject;
        private GameObject uiGameObject;

        private Dictionary<VehicleInfo, VehicleInfo.Effect[]> changes = new Dictionary<VehicleInfo, VehicleInfo.Effect[]>();
        private bool hasChangedPrefabs;
        private List<SoundEffectOptions> soundEffectOptions;
        private ReloadEffectsBehaviour reloadBehaviour;
        private CustomSoundsManager customSounds = new CustomSoundsManager();
        private CustomParticlesManager customParticles = new CustomParticlesManager();

        private List<ConfigLoader> configLoaders = new List<ConfigLoader>();
        private List<VehicleEffectsDefinition> loadedDefinitions = new List<VehicleEffectsDefinition>();
        private Dictionary<VehicleEffectsDefinition, string> definitionPackages = new Dictionary<VehicleEffectsDefinition, string>();
        private HashSet<string> vehicleEffectsDefParseErrors = new HashSet<string>();

        private static Dictionary<string, EffectInfo> effectDictionary = new Dictionary<string, EffectInfo>();

        public string Description
        {
            get
            {
                return "Allows extra effects to be added to vehicles. Now with custom sound effects.";
            }
        }

        public string Name
        {
            get
            {
                return name + " " + version;
            }
        }

        public VehicleEffectsMod()
        {
            if(GameSettings.FindSettingsFileByName("VehicleEffectsMod") == null)
            {
                GameSettings.AddSettingsFile(new SettingsFile[]
                {
                    new SettingsFile
                    {
                        fileName = "VehicleEffectsMod"
                    }
                });
            }

            if (effectDictionary.Count != 0)
            {
                Logging.LogWarning("Clearing effects dictionary");
                effectDictionary.Clear();
            }

            showParseErrors = new SavedBool("ShowParseErrors", "VehicleEffectsMod", true, true);
            enableEditor = new SavedBool("EnableEditor", "VehicleEffectsMod", true, true);
            showEditorWarning = new SavedBool("ShowEditorWarning", "VehicleEffectsMod", true, true);
            ignoreModVehicleParseErrors = new SavedBool("ShowModMissingVehicleErrors", "VehicleEffectsMod", true, true);

            configLoaders.Add(new VehicleEffectsLoader(loadedDefinitions, definitionPackages, vehicleEffectsDefParseErrors));
            configLoaders.Add(new SoundEffectsLoader(vehicleEffectsDefParseErrors, customSounds));
            configLoaders.Add(new ParticleEffectsLoader(vehicleEffectsDefParseErrors, customParticles));
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIHelperBase group = helper.AddGroup("Generic options");
            group.AddCheckbox("Display error messages", showParseErrors.value, (bool c) => {
                showParseErrors.value = c;
            });

            group.AddCheckbox("Enable editor", enableEditor.value, (bool c) => {
                enableEditor.value = c;
            });

            group.AddCheckbox("Display editor warning", showEditorWarning.value, (bool c) => {
                showEditorWarning.value = c;
            });

            group.AddCheckbox("No missing vehicles errors when parsing definitions included in mods", ignoreModVehicleParseErrors.value, (bool c) => {
                ignoreModVehicleParseErrors.value = c;
            });

            group = helper.AddGroup("Sound Effect Volumes");

            soundEffectOptions = new List<SoundEffectOptions>();
            soundEffectOptions.Add(new SoundEffectOptions("Train Horn", 0.7f));
            soundEffectOptions.Add(new SoundEffectOptions("Train Bell", 0.8f));
            soundEffectOptions.Add(new SoundEffectOptions("Train Whistle", 0.7f));
            //soundEffectOptions.Add(new SoundEffectOptions("Steam Train Movement Test", 0.7f));
            //soundEffectOptions.Add(new SoundEffectOptions("Diesel Train Movement", 0.5f));
            //soundEffectOptions.Add(new SoundEffectOptions("Rolling Train Movement", 0.8f));
            //soundEffectOptions.Add(new SoundEffectOptions("Propeller Aircraft Sound", 0.65f));

            foreach(var option in soundEffectOptions)
            {
                option.AddToMenu(group);
            }
            group.AddButton("Reset to defaults", () => {
                foreach(var option in soundEffectOptions)
                {
                    option.Reset();
                }
            });
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            hasChangedPrefabs = false;

            InitializeGameObjects();
            CustomVehicleEffect.Initialize(gameObject.transform);
            CustomVehicleMultiEffect.Initialize(gameObject.transform);

            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
            {
                // Editor
                if(mode == LoadMode.LoadAsset || mode == LoadMode.NewAsset)
                {
                    if(enableEditor)
                    {
                        UIView view = UIView.GetAView();
                        uiGameObject = new GameObject();
                        uiGameObject.transform.SetParent(view.transform);
                        uiGameObject.AddComponent<Editor.UIMainPanel>().SetEditorWarning(showEditorWarning);
                        uiGameObject.AddComponent<Editor.PrefabWatcher>();
                    }
                }
                return;
            }

            UpdateVehicleEffects();
        }

        public override void OnLevelUnloading()
        {
            if(uiGameObject != null)
            {
                GameObject.Destroy(uiGameObject);
                uiGameObject = null;
            }
            if(hasChangedPrefabs)
            {
                ResetVehicleEffects();
            }
            CustomVehicleEffect.Destroy();
            CustomVehicleMultiEffect.Destroy();
            customSounds.Reset(gameObject.transform);
            customParticles.Reset(gameObject.transform);

            GC.Collect();
        }

        private void InitializeGameObjects()
        {
            Logging.Log("Initializing Game Objects");
            if(gameObject == null)
            {
                Logging.Log("Game Objects not created, creating new Game Objects");
                gameObject = new GameObject("Vehicle Effects Mod");
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                CreateCustomEffects();
                reloadBehaviour = gameObject.AddComponent<ReloadEffectsBehaviour>();
                reloadBehaviour.SetMod(this);
                customParticles.InitializeGameObjects(gameObject.transform);
                customSounds.InitializeGameObjects(gameObject.transform);
            }
            Logging.Log("Done initializing Game Objects");
        }

        private void CreateCustomEffects()
        {
            Logging.Log("Creating effect objects");

            var t = gameObject.transform;

            // Custom particles
            var e = VehicleSteam.CreateEffectObject(t);
            RegisterEffect(e.name, e, true);

            // Planes
            e = PropAircraftMovement.CreateEffectObject(t);
            RegisterEffect(e.name, e, true);

            // Custom lights
            e = TrainDitchLight.CreateEffectObject(t);
            RegisterEffect(e.name, e, true);

            // Initialize the options menu.
            foreach (var option in soundEffectOptions)
            {
                option.Initialize();
            }

            Logging.Log("Done creating effect objects");
        }

        public void ReloadVehicleEffects()
        {
            Logging.Log("Reloading Vehicle Effects");
            ResetVehicleEffects();
            customSounds.Reset(gameObject.transform);
            customParticles.Reset(gameObject.transform);
            CustomVehicleEffect.Destroy();
            CustomVehicleMultiEffect.Destroy();

            GC.Collect();

            CustomVehicleEffect.Initialize(gameObject.transform);
            CustomVehicleMultiEffect.Initialize(gameObject.transform);
            UpdateVehicleEffects();
        }

        private void ResetVehicleEffects()
        {
            Logging.Log("Starting effect reset");
            foreach(var change in changes)
            {
                change.Key.m_effects = change.Value;
            }
            changes.Clear();
            Logging.Log("Done resetting effects");
        }

        private void UpdateVehicleEffects()
        {
            OnVehicleUpdateStart();
            try
            {
                var checkedPaths = new List<string>();

                foreach(var loader in configLoaders)
                {
                    loader.Prepare();
                }

                // Load definitions from mod folders
                foreach(var current in ColossalFramework.Plugins.PluginManager.instance.GetPluginsInfo())
                {
                    if(current.isEnabled)
                    {
                        foreach(var loader in configLoaders)
                        {
                            var path = Path.Combine(current.modPath, loader.FileName);
                            // skip files which were already parsed
                            if(checkedPaths.Contains(path)) continue;
                            checkedPaths.Add(path);
                            if(!File.Exists(path)) continue;
                            loader.OnFileFound(path, current.name, true);
                        }
                    }                    
                }

                // Using a sub function here since we have to do this for various prefab types
                void LoadConfigsForPrefab(PrefabInfo prefab)
                {
                    // Check if asset is valid
                    if (prefab == null) return;

                    var asset = PackageManager.FindAssetByName(prefab.name);

                    var crpPath = asset?.package?.packagePath;
                    if (crpPath == null)
                    {
                        Logging.Log("No package path for prefab " + prefab.name + " can not load configs from its directory");
                        return;
                    }

                    foreach (var loader in configLoaders)
                    {
                        var path = Path.Combine(Path.GetDirectoryName(crpPath) ?? "", loader.FileName);
                        // skip files which were already parsed
                        if (checkedPaths.Contains(path)) continue;
                        checkedPaths.Add(path);
                        if (!File.Exists(path)) continue;
                        loader.OnFileFound(path, asset.package.packageName, false);
                    }
                }

                // Load definitions from prefabs
                for(uint i = 0; i < PrefabCollection<VehicleInfo>.LoadedCount(); i++)
                {
                    var prefab = PrefabCollection<VehicleInfo>.GetLoaded(i);
                    LoadConfigsForPrefab(prefab);
                }
                // Workaround for configs not loading when props are included in the workshop upload
                for (uint i = 0; i < PrefabCollection<PropInfo>.LoadedCount(); i++)
                {
                    var prefab = PrefabCollection<PropInfo>.GetLoaded(i);
                    LoadConfigsForPrefab(prefab);
                }

                // Dump loaded packages in log
                foreach (var loadedPackage in definitionPackages)
                {
                    Logging.Log("Package " + loadedPackage.Value + ": " + loadedPackage.Key.Vehicles.Count + " vehicles");
                }

                // Apply the effects
                foreach(var vehicleEffectsDef in loadedDefinitions)
                {
                    if(vehicleEffectsDef?.Vehicles == null || vehicleEffectsDef.Vehicles.Count == 0)
                    {
                        vehicleEffectsDefParseErrors.Add(definitionPackages[vehicleEffectsDef] + " - vehicleEffectDef is null or empty.");
                        continue;
                    }

                    foreach(var vehicleDef in vehicleEffectsDef.Vehicles)
                    {
                        ParseVehicleDefinition(vehicleDef, definitionPackages[vehicleEffectsDef], ref changes, ref vehicleEffectsDefParseErrors, ignoreModVehicleParseErrors ? vehicleEffectsDef.LoadedFromMod : false);  // Mods can be used as packs, not all vehicles may be loaded so we surpress vehicle parse errors
                    }
                }
            }
            catch(Exception e)
            {
                Logging.LogException(e);
            }

            if(vehicleEffectsDefParseErrors.Count > 0 && showParseErrors)
            {
                // Log all parse errors to the log file as well, after processing. This makes them easier to find instead of them being spread out throughout the entire loading process
                Logging.LogError($"Found {vehicleEffectsDefParseErrors.Count} errors while loading effects:");

                foreach (var parseError in vehicleEffectsDefParseErrors)
                {
                    Logging.LogError(parseError);
                }

                if (showParseErrors)
                {
                    var errorMessage = vehicleEffectsDefParseErrors.Aggregate("Error while parsing vehicle effect definition file(s). Assets will work but may have effects missing. Contact the author of the asset(s). \n" + "List of errors:\n", (current, error) => current + (error + '\n'));

                    UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Vehicle Effects", errorMessage, false);
                }
            }

            // Initialize the options menu here to allow this mod's own sound effects to be loaded via xml
            foreach(var option in soundEffectOptions)
            {
                option.Initialize();
            }

            hasChangedPrefabs = true;
            OnVehicleUpdateFinished();
        }

        /// <summary>
        /// Applies a vehicle definition vehicle entry.
        /// </summary>
        /// <param name="vehicleDef">Vehicle definition to parse.</param>
        /// <param name="packageName">Package name used in error messages.</param>
        /// <param name="backup">Used to store original effect array.</param>
        /// <param name="parseErrors">HashSet to add parse errors to.</param>
        /// <param name="noParseErrors">Optional: If true no vehicle related parse errors are given.</param>
        /// <param name="vehicleDefPrefab">Optional: VehicleInfo to apply definition to.</param>
        public static void ParseVehicleDefinition(VehicleEffectsDefinition.Vehicle vehicleDef, string packageName, ref Dictionary<VehicleInfo, VehicleInfo.Effect[]> backup, ref HashSet<string> parseErrors, bool noParseErrors = false, VehicleInfo vehicleDefPrefab = null)
        {
            if(vehicleDef?.Name == null)
            {
                if(!noParseErrors) parseErrors.Add(packageName + " - Vehicle name missing.");
                return;
            }

            Logging.Log("Parsing definition for " + vehicleDef.Name + " with parse errors " + (noParseErrors ? "Disabled" : "Enabled"));

            if(vehicleDefPrefab == null)
            {
                vehicleDefPrefab = FindVehicle(vehicleDef.Name, packageName);
            }

            if(vehicleDefPrefab == null)
            {
                if(!noParseErrors) parseErrors.Add(packageName + " - Vehicle with name " + vehicleDef.Name + " not loaded.");
                return;
            }


            if(vehicleDef.Effects == null || vehicleDef.Effects.Count == 0)
            {
                if(!noParseErrors) parseErrors.Add(packageName + " - No effects specified for " + vehicleDef.Name + ".");
                return;
            }

            if(vehicleDef.ApplyToTrailersOnly)
            {
                HashSet<VehicleInfo> trailers = new HashSet<VehicleInfo>();
                foreach(var trailer in vehicleDefPrefab.m_trailers)
                {
                    if(!trailers.Contains(trailer.m_info))
                    {
                        trailers.Add(trailer.m_info);
                    }
                }

                foreach(var trailer in trailers)
                {
                    var trailerDef = new VehicleEffectsDefinition.Vehicle();
                    trailerDef.Name = trailer.name;
                    trailerDef.Effects = vehicleDef.Effects;
                    ParseVehicleDefinition(trailerDef, packageName, ref backup, ref parseErrors, noParseErrors, trailer);
                }

                return;
            }

            // Backup default effects array
            if(!backup.ContainsKey(vehicleDefPrefab))
            {
                var effects = new VehicleInfo.Effect[vehicleDefPrefab.m_effects.Length];
                vehicleDefPrefab.m_effects.CopyTo(effects, 0);
                backup.Add(vehicleDefPrefab, effects);
            }


            var vehicleEffectsList = new List<VehicleInfo.Effect>();
            vehicleEffectsList.AddRange(vehicleDefPrefab.m_effects);

            foreach(var effectDef in vehicleDef.Effects)
            {
                ParseEffectDefinition(vehicleDef, effectDef, vehicleEffectsList, ref parseErrors);
            }

            // Remove and log null effects
            for(int i = vehicleEffectsList.Count - 1; i >= 0; i--)
            {
                if(vehicleEffectsList[i].m_effect == null)
                {
                    Logging.LogWarning("Found effect that is NULL for vehicle " + vehicleDef.Name + " at index " + i + ", removing.");
                    vehicleEffectsList.RemoveAt(i);
                }
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

            Logging.Log("Finished applying definition for " + vehicleDef.Name);
        }

        public static void ParseEffectDefinition(VehicleEffectsDefinition.Vehicle vehicleDef, VehicleEffectsDefinition.Effect effectDef, List<VehicleInfo.Effect> effectsList, ref HashSet<string> parseErrors)
        {
            if(effectDef?.Name == null)
            {
                parseErrors.Add(vehicleDef.Name + " - Effect name missing.");
                return;
            }

            string effectName = (effectDef.Replacment == null) ? effectDef.Name : effectDef.Replacment;
            effectName += (effectDef.Base != null) ? " - " + effectDef.Base : "";

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
                            effectPrefab = CustomVehicleEffect.CreateEffect(effectDef, baseEffect);
                            if(effectPrefab == null)
                            {
                                parseErrors.Add(vehicleDef.Name + ": An error occured trying to create a custom effect. Check debug log for details.");
                                return;
                            }
                        }
                        else
                        {
                            // Try fallback
                            if(effectDef.Fallback != null)
                            {
                                Logging.Log(vehicleDef.Name + ": trying to use fallback " + effectDef.Fallback + " for " + effectName);
                                if(effectDef.Fallback.StartsWith("None"))
                                {
                                    // None specified as fallback, just exit
                                    return;
                                }
                                else if(effectDef.Fallback.StartsWith("Remove") && effectDef.Replacment != null)
                                {
                                    // Remove the existing effect
                                    effectName = "Remove";
                                    effectPrefab = null;
                                }
                                else
                                {
                                    baseEffect = FindEffect(effectDef.Fallback);
                                    if (baseEffect != null)
                                    {
                                        effectPrefab = CustomVehicleEffect.CreateEffect(effectDef, baseEffect);
                                        if (effectPrefab == null)
                                        {
                                            parseErrors.Add(vehicleDef.Name + ": An error occured trying to create a custom effect. Check debug log for details.");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        parseErrors.Add(vehicleDef.Name + ": Vehicle Effect Wrapper fallback " + effectDef.Fallback + " was needed for " + effectDef.Base + " but not loaded either!");
                                    }
                                }
                            }
                            else
                            {
                                parseErrors.Add(vehicleDef.Name + ": Vehicle Effect Wrapper base effect " + effectDef.Base + " was not loaded and no fallback was specified!");
                                return;
                            }
                        }
                    }
                    else
                    {
                        parseErrors.Add(vehicleDef.Name + ": Vehicle Effect Wrapper requires a base effect but it wasn't given");
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
                            ParseEffectDefinition(vehicleDef, subEffect.Effect, dummyList, ref parseErrors);
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
                        effectPrefab = CustomVehicleMultiEffect.CreateEffect(effectDef, loadedSubEffects.ToArray(), effectDef.Duration, effectDef.UseSimulationTime);
                    }
                    else
                    {
                        parseErrors.Add(vehicleDef.Name + " Vehicle Effect Multi with no sub effects!");
                        return;
                    }
                }
                else if(effectName.StartsWith("None") || effectName.StartsWith("Remove"))
                {
                    // Not a real effect, but keywords used for removal of an existing effect.
                    // 'None' included for < 1.2c compatibility
                    effectName = "Remove";
                    effectPrefab = null;
                }
                else
                {
                    // Try fallback
                    if(effectDef.Fallback != null)
                    {
                        Logging.Log(vehicleDef.Name + ": trying to use fallback " + effectDef.Fallback + " for " + effectName);
                        if(effectDef.Fallback.StartsWith("None"))
                        {
                            // None specified as fallback, just exit
                            return;
                        }
                        else if(effectDef.Fallback.StartsWith("Remove") && effectDef.Replacment != null)
                        {
                            // Remove the existing effect
                            effectName = "Remove";
                            effectPrefab = null;
                        }
                        else
                        {
                            effectPrefab = FindEffect(effectDef.Fallback);
                            if (effectPrefab == null)
                            {
                                parseErrors.Add(vehicleDef.Name + " requested non-existing effect " + effectName + " and fallback " + effectDef.Fallback + " could not be found either!");
                                return;
                            }
                        }
                    }
                    else
                    {
                        parseErrors.Add(vehicleDef.Name + " requested non-existing effect " + effectName + " and no fallback was specified");
                        return;
                    }
                    
                }
            }

            if(effectPrefab == null && !effectName.Equals("Remove"))
            {
                parseErrors.Add(vehicleDef.Name + " - Effect with name " + effectDef.Name + " not loaded.");
                return;
            }

            var vehicleInfoEffect = new VehicleInfo.Effect
            {
                m_effect = effectPrefab,
                m_parkedFlagsForbidden = VehicleParked.Flags.Created,
                m_parkedFlagsRequired = VehicleParked.Flags.None,
                m_vehicleFlagsForbidden = (Vehicle.Flags)effectDef.ForbiddenFlags,
                m_vehicleFlagsRequired = ((Vehicle.Flags)effectDef.RequiredFlags) | Vehicle.Flags.Created
            };

            if(effectDef.Replacment == null)
            {
                effectsList.Add(vehicleInfoEffect);

                if (effectDef.Base != null)
                {
                    Logging.Log("Added effect " + effectDef.Name + " (" + effectDef.Base + ") to vehicle " + vehicleDef.Name);
                }
                else
                {
                    Logging.Log("Added effect " + effectDef.Name + " to vehicle " + vehicleDef.Name);
                }
            }
            else
            {
                int indexToRemove = -1;

                for(int i = 0; i < effectsList.Count; i++)
                {
                    if(effectsList[i].m_effect.name.Equals(effectDef.Name))
                    {
                        if(effectDef.Replacment.Equals("Remove"))
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
                    Logging.Log("Removed effect " + effectDef.Name + " from vehicle " + vehicleDef.Name);
                }
                else
                {
                    if (effectDef.Base != null)
                    {
                        Logging.Log("Replaced effect " + effectDef.Name + " with " + effectDef.Replacment + " (" + effectDef.Base + ") for vehicle " + vehicleDef.Name);
                    }
                    else
                    {
                        Logging.Log("Replaced effect " + effectDef.Name + " with " + effectDef.Replacment + " for vehicle " + vehicleDef.Name);
                    }
                    
                }
            }
        }

        public static EffectInfo FindEffect(string effectName)
        {
            // Particle Effects aren't all added to EffectCollection, so search for GameObjects as well
            var effect = EffectCollection.FindEffect(effectName);
            if (effect == null)
            {
                if (!effectDictionary.TryGetValue(effectName, out effect))
                {
                    Logging.Log($"Unknown effect {effectName}, falling back to GameObject.Find");
                    var go = GameObject.Find(effectName);
                    if (go != null)
                    {
                        effect = go.GetComponent<EffectInfo>();
                    }
                }
            }
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
