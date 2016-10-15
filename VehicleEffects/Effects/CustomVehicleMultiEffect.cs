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
        private const string effectName = "Vehicle Multi Effect Wrapper";
        private static Dictionary<String, EffectInfo> modifiedEffects;


        public static GameObject CreateEffectObject(Transform parent)
        {
            if(gameObject != null)
            {
                Debug.LogWarning("Creating effect object for " + effectName + " but object already exists!");
            }

            modifiedEffects = new Dictionary<string, EffectInfo>();
            gameObject = new GameObject(effectName + " Effects");
            gameObject.transform.parent = parent;
            return gameObject;
        }


        public static EffectInfo CreateEffect(VehicleEffectWrapper.VehicleEffectParams parameters, MultiEffect.SubEffect[] subEffects, float duration)
        {
            if(gameObject == null)
            {
                Debug.LogError("Tried to create EffectInfo for " + effectName + " but GameObject was not created!");
                return null;
            }

            var effect = gameObject.AddComponent<CustomMultiEffect>();

            effect.m_params = parameters;
            effect.m_duration = duration;
            effect.m_useSimulationTime = true;
            effect.m_effects = subEffects;

            return effect;
        }

        public static void OnLevelUnloading()
        {
            modifiedEffects.Clear();
        }

        private static EffectInfo GetModifiedEffect(string effectName)
        {
            EffectInfo result;
            modifiedEffects.TryGetValue(effectName, out result);
            return result;
        }
    }
}
