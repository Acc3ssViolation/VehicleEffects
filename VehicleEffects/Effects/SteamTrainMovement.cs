//using ColossalFramework.Plugins;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using UnityEngine;

//namespace VehicleEffects.Effects
//{
//    public class SteamTrainMovement
//    {
//        private const string effectName = "Steam Train Movement";

//        public static EffectInfo CreateEffectObject(Transform parent)
//        {
//            EngineSoundEffect defaultEngineSound = VehicleEffectsMod.FindEffect("Train Movement") as EngineSoundEffect;

//            if(defaultEngineSound != null)
//            {
//                GameObject obj = new GameObject(effectName);
//                obj.transform.parent = parent;

//                EngineSoundEffect steamEngineSound = Util.CopyEngineSoundEffect(defaultEngineSound, obj.AddComponent<EngineSoundEffect>());
//                steamEngineSound.name = effectName;

//                // Create a copy of audioInfo
//                AudioInfo audioInfo = UnityEngine.Object.Instantiate(defaultEngineSound.m_audioInfo) as AudioInfo;

//                audioInfo.name = effectName;
//                audioInfo.m_volume = 0.7f;

//                // Load new audio clip

//                var clip = Util.LoadAudioClipFromModDir("Sounds/steam-engine-a3-moving.ogg");

//                if(clip != null)
//                {
//                    audioInfo.m_clip = clip;
//                }
//                else
//                {
//                    return null;
//                }

//                steamEngineSound.m_audioInfo = audioInfo;

//                return steamEngineSound;
//            }
//            else
//            {
//                Logging.LogError("Could not find default train sound effect!");
//                return null;
//            }
//        }
//    }
//}
