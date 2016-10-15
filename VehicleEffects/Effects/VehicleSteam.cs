using ColossalFramework.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using VehicleEffects.GameExtensions;

namespace VehicleEffects.Effects
{
    public class VehicleSteam
    {
        public static GameObject gameObject { get; private set; }
        private const string effectName = "Vehicle Steam";

        /// <summary>
        /// Creates GameObject containing the particle system this effect needs.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject CreateEffectObject(Transform parent)
        {
            ParticleEffect templateParticleEffect = VehicleEffectsMod.FindEffect("Factory Steam") as ParticleEffect;
            MovementParticleEffect templateMovementEffect = VehicleEffectsMod.FindEffect("Gravel Dust") as MovementParticleEffect;

            if(gameObject != null)
            {
                Debug.LogWarning("Creating effect object for " + effectName + " but object already exists!");
            }

            if(templateParticleEffect != null && templateMovementEffect != null)
            {
                // Load particle system from AssetBundle because we cannot access ParticleSystem modifiers from code.
                // The AssetBundle used contains a GameObject prefab with a ParticleSystem attached. The following modifiers are active:
                // Scale over time: scaling from ~7% to 100%.
                // Color over time: fade out near the end of the lifespan.
                // Velocity limit over time is active as well.

                Assembly asm = Assembly.GetAssembly(typeof(VehicleEffectsMod));
                var pluginInfo = PluginManager.instance.FindPluginInfo(asm);

                GameObject obj = null;

                try
                {
                    string absUri = "file:///" + pluginInfo.modPath.Replace("\\", "/") + "/AssetBundles/particlesystems";
                    WWW www = new WWW(absUri);
                    AssetBundle bundle = www.assetBundle;


                    Debug.Log("Bundle loading " + ((bundle == null) ? "failed " + www.error : "succeeded"));
                    UnityEngine.Object a = bundle.LoadAsset("ParticleSystemSteam");
                    Debug.Log("Asset unpacking " + ((a == null) ? "failed " : "succeeded"));
                    obj = GameObject.Instantiate(a) as GameObject;
                    bundle.Unload(false);
                }
                catch(Exception e)
                {
                    Debug.Log("Exception trying to load bundle file!" + e.ToString());
                }

                if(obj != null)
                {
                    obj.name = effectName;
                }
                else
                {
                    Debug.LogWarning("Could not use bundle, creating new game object.");
                    obj = new GameObject(effectName);
                }

                obj.transform.parent = parent;

                // Configure particle system
                var particleSystem = templateParticleEffect.GetComponent<ParticleSystem>();
                var particleRenderer = templateParticleEffect.GetComponent<ParticleSystemRenderer>();
                var particleRendererMov = templateMovementEffect.GetComponent<ParticleSystemRenderer>();

                var psCopy = obj.GetComponent<ParticleSystem>() ?? obj.AddComponent<ParticleSystem>();
                psCopy.gravityModifier = 0.2f;
                psCopy.startSize = 8;

                psCopy.time = 0;
                psCopy.startSpeed = 10;

                psCopy.startRotation = 6.283185f;
                psCopy.startLifetime = 5;
                psCopy.startDelay = 0;
                psCopy.startColor = Color.white;
                psCopy.simulationSpace = ParticleSystemSimulationSpace.World;
                psCopy.randomSeed = 0;
                psCopy.playOnAwake = false;
                psCopy.playbackSpeed = 0;
                psCopy.maxParticles = 10000;
                psCopy.loop = true;
                psCopy.hideFlags = HideFlags.None;
                psCopy.enableEmission = true;
                psCopy.emissionRate = 10;


                var renderCopy = obj.GetComponent<ParticleSystemRenderer>();
                renderCopy.cameraVelocityScale = 0;
                renderCopy.lengthScale = 2;
                renderCopy.maxParticleSize = 20.0f;
                renderCopy.mesh = null;
                renderCopy.probeAnchor = null;
                renderCopy.receiveShadows = true;
                renderCopy.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;
                renderCopy.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderCopy.sharedMaterial = particleRenderer.sharedMaterial;
                renderCopy.useLightProbes = false;
                renderCopy.velocityScale = 0;

                renderCopy.enabled = true;
                renderCopy.hideFlags = HideFlags.None;
                renderCopy.lightmapIndex = -1;
                renderCopy.lightmapScaleOffset = new Vector4(1, 1, 0, 0);
                renderCopy.sortingLayerID = 0;
                renderCopy.sortingLayerName = "Default";
                renderCopy.sortingOrder = 0;
                renderCopy.renderMode = ParticleSystemRenderMode.Billboard;


                CustomMovementParticleEffect vehicleSteam = obj.AddComponent<CustomMovementParticleEffect>();

                // Set custom properties
                vehicleSteam.m_canUseBezier = false;
                vehicleSteam.m_canUseMeshData = false;
                vehicleSteam.m_canUsePositions = true;

                vehicleSteam.m_maxVisibilityDistance = 1800f;

                vehicleSteam.m_minSpawnAngle = templateParticleEffect.m_minSpawnAngle;
                vehicleSteam.m_useSimulationTime = templateParticleEffect.m_useSimulationTime;


                vehicleSteam.m_velocityMultiplier = 1.0f;
                vehicleSteam.m_spawnAreaRadius = 0.25f;

                vehicleSteam.m_minMagnitude = 0.2f;                     // Gives occasional puffs of smoke when standing still
                vehicleSteam.m_magnitudeAccelerationMultiplier = 10;
                vehicleSteam.m_magnitudeSpeedMultiplier = 20;

                vehicleSteam.m_maxStartSpeed = 1.7f;
                vehicleSteam.m_minStartSpeed = 1.5f;

                vehicleSteam.m_maxLifeTime = 1.3f;
                vehicleSteam.m_minLifeTime = 1.1f;

                vehicleSteam.m_maxSpawnAngle = 4.0f;

                gameObject = obj;

                return obj;
            }
            else
            {
                Debug.LogError("Could not find default effects used for Vehicle Steam Effect!");
                return null;
            }
        }
    }
}
