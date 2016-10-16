using ColossalFramework.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace VehicleEffects.Effects
{
    public class TrainDitchLight
    {
        private const string effectName = "Train Ditch Light";

        public static EffectInfo CreateEffectObject(Transform parent)
        {
            var templateEffect = VehicleEffectsMod.FindEffect("Large Car Light Right2") as LightEffect;

            if(templateEffect != null)
            {
                GameObject obj = new GameObject(effectName);
                obj.transform.parent = parent;

                var ditchLightEffect = obj.AddComponent<LightEffect>();
                Util.CopyLight(templateEffect.GetComponent<Light>(), obj.AddComponent<Light>());
                Util.CopyLightEffect(templateEffect, ditchLightEffect);

                ditchLightEffect.m_batchedLight = false;
                ditchLightEffect.m_positionIndex = -1;

                ditchLightEffect.m_offRange = new Vector2(1000, 1000);
                ditchLightEffect.m_position = Vector3.zero;

                return ditchLightEffect;
            }
            else
            {
                Debug.Log("Could not find template for " + effectName);
                return null;
            }
        }
    }
}
