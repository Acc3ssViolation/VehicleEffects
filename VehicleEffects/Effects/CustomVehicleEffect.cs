using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VehicleEffects.GameExtensions;

namespace VehicleEffects.Effects
{
    public class CustomVehicleEffect
    {
        public static GameObject gameObject { get; private set; }
        private const string effectName = "Vehicle Effect Wrapper";
        private static Dictionary<string, EffectInfo> modifiedEffects;
        private static Dictionary<VehicleEffectWrapper.VehicleEffectParams, VehicleEffectWrapper> createdEffects;


        public static GameObject CreateEffectObject(Transform parent)
        {
            if(gameObject != null)
            {
                Logging.LogWarning("Creating effect object for " + effectName + " but object already exists!");
            }

            modifiedEffects = new Dictionary<string, EffectInfo>();
            createdEffects = new Dictionary<VehicleEffectWrapper.VehicleEffectParams, VehicleEffectWrapper>();

            gameObject = new GameObject(effectName + " Effects");
            gameObject.transform.parent = parent;
            return gameObject;
        }


        public static EffectInfo CreateEffect(VehicleEffectsDefinition.Effect effectDef, EffectInfo baseEffect)
        {
            if(gameObject == null)
            {
                Logging.LogError("Tried to create EffectInfo for " + effectName + " but GameObject was not created!");
                return null;
            }

            VehicleEffectWrapper effectWrapper = null;

            var effectParameters = new VehicleEffectWrapper.VehicleEffectParams();
            effectParameters.m_name = ((effectDef.Replacment == null) ? effectDef.Name : effectDef.Replacment) + ((effectDef.Base != null) ? " - " + effectDef.Base : "");
            effectParameters.m_position = effectDef.Position?.ToUnityVector() ?? Vector3.zero;
            effectParameters.m_direction = effectDef.Direction?.ToUnityVector() ?? Vector3.zero;
            effectParameters.m_maxSpeed = Util.SpeedKmHToEffect(effectDef.MaxSpeed);
            effectParameters.m_minSpeed = Util.SpeedKmHToEffect(effectDef.MinSpeed);

            createdEffects.TryGetValue(effectParameters, out effectWrapper);

            if(effectWrapper == null)
            {
                effectWrapper = gameObject.AddComponent<VehicleEffectWrapper>();

                if(baseEffect is LightEffect)
                {
                    var lightEffect = baseEffect as LightEffect;

                    // There are some cases in which the effect won't render, so we need a copy that can be rendered the way we need it
                    if(lightEffect.m_batchedLight || lightEffect.m_positionIndex >= 0)
                    {
                        var effect2 = GetModifiedEffect(lightEffect.name) as LightEffect;
                        if(effect2 == null)
                        {
                            GameObject lightObject = new GameObject(lightEffect.name + " - Modified");
                            lightObject.transform.parent = gameObject.transform;

                            var templateLight = lightEffect.GetComponent<Light>();
                            var light = lightObject.AddComponent<Light>();
                            Util.CopyLight(templateLight, light);
                            effect2 = Util.CopyLightEffect(lightEffect, lightObject.AddComponent<LightEffect>());
                            effect2.m_batchedLight = false;
                            effect2.m_positionIndex = -1;
                            effect2.m_position = Vector3.zero;

                            modifiedEffects.Add(lightEffect.name, effect2);
                        }
                        baseEffect = effect2;
                    }
                }

                effectWrapper.m_wrappedEffect = baseEffect;
                effectWrapper.m_params = effectParameters;

                createdEffects.Add(effectParameters, effectWrapper);
            }

            return effectWrapper;
        }

        private static EffectInfo GetModifiedEffect(string effectName)
        {
            EffectInfo result;
            modifiedEffects.TryGetValue(effectName, out result);
            return result;
        }
    }
}
