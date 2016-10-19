using ColossalFramework.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace VehicleEffects.Effects
{
    public class TrainHorn
    {
        private const string effectName = "Train Horn";

        public static EffectInfo CreateEffectObject(Transform parent)
        {
            var obj = new GameObject(effectName);
            obj.transform.parent = parent;
            SoundEffect effect = obj.AddComponent<SoundEffect>();
            effect.m_position = Vector3.zero;

            // Create a copy of an audioInfo
            var templateSound = VehicleEffectsMod.FindEffect("Train Movement") as SoundEffect;
            AudioInfo audioInfo = UnityEngine.Object.Instantiate(templateSound.m_audioInfo) as AudioInfo;
            audioInfo.name = effectName;
            audioInfo.m_fadeLength = 0.18f;
            audioInfo.m_loop = true;
            audioInfo.m_pitch = 1.0f;
            audioInfo.m_volume = 0.7f;
            audioInfo.m_randomTime = false;

            // Load new audio clip

            var clip = Util.LoadAudioClipFromModDir("Sounds/train-horn-loop.ogg");

            if(clip != null)
            {
                audioInfo.m_clip = clip;
            }
            else
            {
                return null;
            }

            effect.m_audioInfo = audioInfo;

            return effect;
        }
    }
}
