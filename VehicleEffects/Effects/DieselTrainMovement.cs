using ColossalFramework.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace VehicleEffects.Effects
{
    public class DieselTrainMovement
    {
        private const string effectName = "Diesel Train Movement";

        public static EffectInfo CreateEffectObject(Transform parent)
        {
            EngineSoundEffect defaultEngineSound = VehicleEffectsMod.FindEffect("Train Movement") as EngineSoundEffect;

            if(defaultEngineSound != null)
            {
                GameObject obj = new GameObject(effectName);
                obj.transform.parent = parent;

                EngineSoundEffect dieselEngineSound = Util.CopyEngineSoundEffect(defaultEngineSound, obj.AddComponent<EngineSoundEffect>());
                dieselEngineSound.name = effectName;

                // Create a copy of audioInfo
                AudioInfo audioInfo = UnityEngine.Object.Instantiate(defaultEngineSound.m_audioInfo) as AudioInfo;

                audioInfo.name = effectName;

                // Load new audio clip

                var clip = Util.LoadAudioClipFromModDir("Sounds/diesel-engine-sd45-moving.ogg");

                if(clip != null)
                {
                    audioInfo.m_clip = clip;
                }
                else
                {
                    return null;
                }

                dieselEngineSound.m_audioInfo = audioInfo;

                return dieselEngineSound;
            }
            else
            {
                Debug.Log("Could not find default train sound effect!");
                return null;
            }
        }
    }
}
