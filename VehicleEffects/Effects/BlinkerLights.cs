/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VehicleEffects.GameExtensions;

namespace VehicleEffects.Effects
{
    public class BlinkerLights
    {
        public static GameObject gameObject { get; private set; }
        private const string effectName = "Blinker Light";

        public static GameObject CreateEffectObject(Transform parent)
        {
            if(gameObject != null)
            {
                Logging.LogWarning("Creating effect object for " + effectName + " but object already exists!");
            }

            gameObject = new GameObject(effectName + " Effects");
            gameObject.transform.parent = parent;

            CreateEffectVariants();

            return gameObject;
        }

        private static void CreateEffectVariants()
        {
            LightEffect baseLightEffect = VehicleEffectsMod.FindEffect("Light Pole Orange") as LightEffect;
            IntensityLightEffect newLightEffect;

            GameObject lightObject = new GameObject("Blinker Light Left");
            lightObject.transform.parent = gameObject.transform;

            var templateLight = baseLightEffect.GetComponent<Light>();
            var light = lightObject.AddComponent<Light>();
            Util.CopyLight(templateLight, light);
            newLightEffect = Util.CopyLightEffect(baseLightEffect, lightObject.AddComponent<IntensityLightEffect>()) as IntensityLightEffect;
            newLightEffect.m_batchedLight = false;
            newLightEffect.m_positionIndex = -1;
            newLightEffect.m_position = Vector3.zero;
            newLightEffect.m_lightIntensityIndex = 2;


            lightObject = new GameObject("Blinker Light Right");
            lightObject.transform.parent = gameObject.transform;

            templateLight = baseLightEffect.GetComponent<Light>();
            light = lightObject.AddComponent<Light>();
            Util.CopyLight(templateLight, light);
            newLightEffect = Util.CopyLightEffect(baseLightEffect, lightObject.AddComponent<IntensityLightEffect>()) as IntensityLightEffect;
            newLightEffect.m_batchedLight = false;
            newLightEffect.m_positionIndex = -1;
            newLightEffect.m_position = Vector3.zero;
            newLightEffect.m_lightIntensityIndex = 3;
        }
    }
}*/
