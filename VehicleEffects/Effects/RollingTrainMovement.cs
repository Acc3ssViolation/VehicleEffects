using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects.Effects
{
    public class RollingTrainMovement
    {
        private const string effectName = "Rolling Train Movement";

        public static EffectInfo CreateEffectObject(Transform parent)
        {
            EngineSoundEffect defaultEngineSound = VehicleEffectsMod.FindEffect("Train Movement") as EngineSoundEffect;

            if(defaultEngineSound != null)
            {
                GameObject obj = new GameObject(effectName);
                obj.transform.parent = parent;

                EngineSoundEffect newSoundEffect = Util.CopyEngineSoundEffect(defaultEngineSound, obj.AddComponent<EngineSoundEffect>());
                newSoundEffect.name = effectName;



                // Create a copy of audioInfo
                AudioInfo audioInfo = UnityEngine.Object.Instantiate(defaultEngineSound.m_audioInfo) as AudioInfo;

                audioInfo.name = effectName;
                audioInfo.m_volume = 0.8f;

                // Load new audio clip

                var clip = Util.LoadAudioClipFromModDir("Sounds/rolling-stock-moving.ogg");

                if(clip != null)
                {
                    audioInfo.m_clip = clip;
                }
                else
                {
                    return null;
                }

                newSoundEffect.m_audioInfo = audioInfo;


                //defaultEngineSound.m_audioInfo = audioInfo;


                return newSoundEffect;
            }
            else
            {
                Debug.LogError("Could not find default train sound effect!");
                return null;
            }
        }
    }
}
