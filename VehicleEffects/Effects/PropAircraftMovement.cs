using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects.Effects
{
    public class PropAircraftMovement
    {
        private const string effectName = "Propeller Aircraft Movement";

        public static EffectInfo CreateEffectObject(Transform parent)
        {
            MultiEffect defaultMultiEffect = VehicleEffectsMod.FindEffect("Aircraft Movement") as MultiEffect;

            if(defaultMultiEffect != null)
            {
                MultiEffect newMultiEffect = GameObject.Instantiate(defaultMultiEffect);
                newMultiEffect.name = effectName;
                newMultiEffect.transform.SetParent(parent);

                newMultiEffect.m_effects[0].m_effect = PropAircraftSound.CreateEffectObject(parent);

                return newMultiEffect;
            }
            else
            {
                Logging.LogError("Could not find default plane movement effect!");
                return null;
            }
        }
    }
}
