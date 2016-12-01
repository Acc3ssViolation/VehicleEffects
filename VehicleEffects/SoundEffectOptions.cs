using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects
{
    class SoundEffectOptions
    {
        public string effectName;
        public SoundEffect effect;
        public float defaultVolume;

        public SavedFloat savedVolume;

        private UISlider slider;
        
        public SoundEffectOptions(string effectName, float defaultVolume)
        {
            this.effectName = effectName;
            this.defaultVolume = defaultVolume;
            savedVolume = new SavedFloat(effectName.Replace(" ", "") + "Volume", "VehicleEffectsMod", defaultVolume, true);
        }

        public void Reset()
        {
            slider.value = defaultVolume;
        }

        public void AddToMenu(UIHelperBase helper)
        {
            slider = helper.AddSlider(effectName, 0.0f, 1.0f, 0.05f, savedVolume.value, EventSlide) as UISlider;
        }

        public void Initialize()
        {
            effect = VehicleEffectsMod.FindEffect(effectName) as SoundEffect;
            if(effect == null)
            {
                Logging.LogWarning("Could not find effect: " + effectName + " for sound effect options");
                return;
            }
            EventSlide(savedVolume.value);
        }

        private void EventSlide(float c)
        {
            savedVolume.value = c;
            if(effect != null)
            {
                effect.m_audioInfo.m_volume = c;
            }
        }
    }
}
