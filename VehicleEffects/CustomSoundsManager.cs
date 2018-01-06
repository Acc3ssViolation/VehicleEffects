using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects
{
    /// <summary>
    /// Handles the collection of custom sound effects loaded via config files.
    /// </summary>
    class CustomSoundsManager
    {
        private Dictionary<string, SoundEffect> effects = new Dictionary<string, SoundEffect>();
        private GameObject effectRoot;

        public void InitializeGameObjects(Transform parent)
        {
            if(effectRoot == null)
            {
                effectRoot = new GameObject("Custom Sounds");
                effectRoot.transform.SetParent(parent);
                GameObject.DontDestroyOnLoad(effectRoot);
            }
        }

        public void Destroy()
        {
            if(effectRoot != null)
            {
                GameObject.Destroy(effectRoot);
                effectRoot = null;
                effects.Clear();
            }
        }

        public void Reset(Transform parent)
        {
            Destroy();
            InitializeGameObjects(parent);
        }

        public bool AddEffect(SoundEffect effect)
        {
            if(effects.ContainsKey(effect.name))
            {
                return false;
            }
            effects[effect.name] = effect;
            effect.transform.SetParent(effectRoot.transform);
            return true;
        }

        private static void ApplyNullableSetting<T>(ref T target, T? setting) where T : struct
        {
            if(setting != null)
            {
                target = setting.Value;
            }
        }

        public static SoundEffect CreateSoundEffect(SoundEffectParams settings)
        {
            if(settings == null)
            {
                Logging.LogError("Null passed to CreateSoundEffect");
                return null;
            }

            switch(settings.Type)
            {
                case SoundEffectType.SoundEffect:
                    return CreateSoundEffect<SoundEffect>(settings);
                case SoundEffectType.EngineSoundEffect:
                    return CreateEngineSoundEffect(settings);
                default:
                    return null;
            }
        }

        private static T CreateSoundEffect<T>(SoundEffectParams settings) where T : SoundEffect
        {
            if(settings == null || string.IsNullOrEmpty(settings.Base) || string.IsNullOrEmpty(settings.Name))
            {
                Logging.LogError("Invalid sound effect settings, name or base is empty");
                return null;
            }

            var baseEffect = VehicleEffectsMod.FindEffect(settings.Base) as T;
            if(baseEffect == null)
            {
                Logging.LogError("Unable to find base sound effect " + settings.Base + " for effect " + settings.Name);
                return null;
            }

            if(!File.Exists(settings.SoundFile))
            {
                Logging.LogError("Unable to find sound file for " + settings.Name + " at " + settings.SoundFile);
                return null;
            }

            var clip = LoadAudioClip(settings.SoundFile);
            if(clip == null)
            {
                Logging.LogError("Unable to load sound file for " + settings.Name + " at " + settings.SoundFile);
                return null;
            }

            // Create effect
            var effect = new GameObject().AddComponent<T>();
            Util.CopySoundEffect(baseEffect, effect, false);
            effect.m_audioInfo = UnityEngine.Object.Instantiate(baseEffect.m_audioInfo) as AudioInfo;
            effect.m_audioInfo.m_clip = clip;

            // Apply general settings
            ApplySoundEffectSettings(effect, settings);

            return effect;
        }

        private static EngineSoundEffect CreateEngineSoundEffect(SoundEffectParams settings)
        {
            var effect = CreateSoundEffect<EngineSoundEffect>(settings);
            if(effect == null)
            {
                return null;
            }
            // Apply base settings, ignoring the audio info
            Util.CopyEngineSoundEffect(VehicleEffectsMod.FindEffect(settings.Base) as EngineSoundEffect, effect, false);
            // Reapply our settings
            ApplySoundEffectSettings(effect, settings);
            ApplyNullableSetting(ref effect.m_minPitch, settings.MinPitch);
            ApplyNullableSetting(ref effect.m_minRange, settings.MinRange);
            ApplyNullableSetting(ref effect.m_pitchAccelerationMultiplier, settings.PitchAccelerationMultiplier);
            ApplyNullableSetting(ref effect.m_pitchSpeedMultiplier, settings.PitchSpeedMultiplier);
            ApplyNullableSetting(ref effect.m_rangeAccelerationMultiplier, settings.RangeAccelerationMultiplier);
            ApplyNullableSetting(ref effect.m_rangeSpeedMultiplier, settings.RangeSpeedMultiplier);

            return effect;
        }

        private static void ApplySoundEffectSettings(SoundEffect effect, SoundEffectParams settings)
        {
            effect.name = settings.Name;
            ApplyNullableSetting(ref effect.m_audioInfo.m_pitch, settings.Pitch);
            ApplyNullableSetting(ref effect.m_audioInfo.m_fadeLength, settings.FadeLength);
            ApplyNullableSetting(ref effect.m_audioInfo.m_volume, settings.Volume);
            ApplyNullableSetting(ref effect.m_audioInfo.m_loop, settings.Loop);
            ApplyNullableSetting(ref effect.m_audioInfo.m_is3D, settings.Is3D);
            ApplyNullableSetting(ref effect.m_audioInfo.m_randomTime, settings.RandomTime);

            ApplyNullableSetting(ref effect.m_range, settings.Range);
        }

        private static AudioClip LoadAudioClip(string filename)
        {
            try
            {
                string absUri = "file:///" + filename.Replace("\\", "/");
                WWW www = new WWW(absUri);
                return www.GetAudioClip(true, false);
            }
            catch(Exception e)
            {
                Logging.LogError("Exception trying to load audio file '" + filename + "'!\r\n" + e.ToString());
                return null;
            }
        }
    }
}
