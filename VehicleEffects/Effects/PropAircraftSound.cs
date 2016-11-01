using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects.Effects
{
    public class PropAircraftSound
    {
        private const string effectName = "Propeller Aircraft Sound";

        public static EffectInfo CreateEffectObject(Transform parent)
        {
            EngineSoundEffect defaultEngineSound = VehicleEffectsMod.FindEffect("Aircraft Sound") as EngineSoundEffect;

            if(defaultEngineSound != null)
            {
                GameObject obj = new GameObject(effectName);
                obj.transform.parent = parent;

                EngineSoundEffect newEngineSoundEffect = Util.CopyEngineSoundEffect(defaultEngineSound, obj.AddComponent<EngineSoundEffect>());
                newEngineSoundEffect.name = effectName;
                newEngineSoundEffect.m_minPitch = 0.65f;

                // Create a copy of audioInfo
                AudioInfo audioInfo = UnityEngine.Object.Instantiate(defaultEngineSound.m_audioInfo) as AudioInfo;

                audioInfo.name = effectName;
                audioInfo.m_volume = 0.65f;

                // Load new audio clip

                var clip = Util.LoadAudioClipFromModDir("Sounds/prop-plane-moving.ogg");

                if(clip != null)
                {
                    audioInfo.m_clip = clip;
                }
                else
                {
                    return null;
                }

                newEngineSoundEffect.m_audioInfo = audioInfo;

                return newEngineSoundEffect;
            }
            else
            {
                Debug.LogError("Could not find default plane sound effect!");
                return null;
            }
        }
    }
}
