using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VehicleEffects.GameExtensions;

namespace VehicleEffects.Effects
{
    public class DieselSmoke
    {
        public static GameObject gameObject { get; private set; }
        private const string effectName = "Diesel Smoke";

        /// <summary>
        /// Creates GameObject containing the particle system this effect needs.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject CreateEffectObject(Transform parent)
        {
            ParticleEffect templateParticleEffect = VehicleEffectsMod.FindEffect("Factory Smoke Small") as ParticleEffect;

            if(gameObject != null)
            {
                Debug.LogWarning("Creating effect object for " + effectName + " but object already exists!");
            }

            if(templateParticleEffect != null)
            {
                gameObject = new GameObject(effectName);

                gameObject.transform.parent = parent;

                // Configure particle system
                CustomMovementParticleEffect dieselSmoke = gameObject.AddComponent<CustomMovementParticleEffect>();

                // Set custom properties
                dieselSmoke.m_canUseBezier = false;
                dieselSmoke.m_canUseMeshData = false;
                dieselSmoke.m_canUsePositions = true;

                dieselSmoke.m_maxVisibilityDistance = templateParticleEffect.m_maxVisibilityDistance;

                dieselSmoke.m_minSpawnAngle = templateParticleEffect.m_minSpawnAngle;
                dieselSmoke.m_useSimulationTime = templateParticleEffect.m_useSimulationTime;

                dieselSmoke.m_velocityMultiplier = 0.0f;
                dieselSmoke.m_spawnAreaRadius = 0.25f;
                dieselSmoke.m_particleSystemOverride = templateParticleEffect.gameObject.GetComponent<ParticleSystem>();

                dieselSmoke.m_minMagnitude = 0.1f;
                dieselSmoke.m_magnitudeAccelerationMultiplier = 1.0f;
                dieselSmoke.m_magnitudeSpeedMultiplier = 0;

                dieselSmoke.m_maxStartSpeed = templateParticleEffect.m_maxStartSpeed + 0.5f;
                dieselSmoke.m_minStartSpeed = templateParticleEffect.m_minStartSpeed + 0.5f;

                dieselSmoke.m_maxLifeTime = templateParticleEffect.m_maxLifeTime;
                dieselSmoke.m_minLifeTime = templateParticleEffect.m_minLifeTime;

                dieselSmoke.m_maxSpawnAngle = 20.0f;

                return gameObject;
            }
            else
            {
                Debug.LogError("Could not find default effects used for " + effectName + "!");
                return null;
            }
        }
    }
}
