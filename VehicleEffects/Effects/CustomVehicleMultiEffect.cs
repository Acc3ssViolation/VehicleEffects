using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VehicleEffects.GameExtensions;

namespace VehicleEffects.Effects
{
    public class CustomVehicleMultiEffect
    {
        public static GameObject gameObject { get; private set; }
        private const string effectName = "Custom Multi Effects";
        private static List<CustomMultiEffect> createdEffects;

        public static GameObject CreateEffectObject(Transform parent)
        {
            if(gameObject != null)
            {
                Debug.LogWarning("Creating effect object for " + effectName + " but object already exists!");
            }

            createdEffects = new List<CustomMultiEffect>();
            gameObject = new GameObject(effectName);
            gameObject.transform.parent = parent;
            return gameObject;
        }


        public static EffectInfo CreateEffect(VehicleEffectsDefinition.Effect effectDef, MultiEffect.SubEffect[] subEffects, float duration, bool useSimulationTime)
        {
            if(gameObject == null)
            {
                Debug.LogError("Tried to create EffectInfo for " + effectName + " but GameObject was not created!");
                return null;
            }

            var effectParameters = new VehicleEffectWrapper.VehicleEffectParams();
            effectParameters.m_name = ((effectDef.Replacment == null) ? effectDef.Name : effectDef.Replacment) + ((effectDef.Base != null) ? " - " + effectDef.Base : "");
            effectParameters.m_position = effectDef.Position?.ToUnityVector() ?? Vector3.zero;
            effectParameters.m_direction = effectDef.Direction?.ToUnityVector() ?? Vector3.zero;
            effectParameters.m_maxSpeed = Util.SpeedKmHToEffect(effectDef.MaxSpeed);
            effectParameters.m_minSpeed = Util.SpeedKmHToEffect(effectDef.MinSpeed);


            CustomMultiEffect effect = null;


            var i = 0;
            while(effect == null && i < createdEffects.Count)
            {
                if(createdEffects[i].m_params.Equals(effectParameters))
                {
                    if(createdEffects[i].m_duration == duration && createdEffects[i].m_useSimulationTime == useSimulationTime)
                    {
                        if(createdEffects[i].m_effects.SequenceEqual(subEffects))
                        {
                            effect = createdEffects[i];
                        }
                    }
                }
                i++;
            }

            if(effect == null)
            {
                effect = gameObject.AddComponent<CustomMultiEffect>();

                effect.m_params = effectParameters;
                effect.m_duration = duration;
                effect.m_useSimulationTime = useSimulationTime;
                effect.m_effects = subEffects;

                createdEffects.Add(effect);
            }

            return effect;
        }
    }
}
