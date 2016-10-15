using ColossalFramework.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace VehicleEffects.Effects
{
    public class SteamTrainMovement
    {
        private const string effectName = "Steam Train Movement";

        public static EffectInfo CreateEffectObject(Transform parent)
        {
            EngineSoundEffect defaultEngineSound = VehicleEffectsMod.FindEffect("Train Movement") as EngineSoundEffect;

            if(defaultEngineSound != null)
            {
                GameObject obj = new GameObject(effectName);
                obj.transform.parent = parent;

                EngineSoundEffect steamEngineSound = Util.CopyEngineSoundEffect(defaultEngineSound, obj.AddComponent<EngineSoundEffect>());
                steamEngineSound.name = effectName;

                // Create a copy of audioInfo
                AudioInfo audioInfo = UnityEngine.Object.Instantiate(defaultEngineSound.m_audioInfo) as AudioInfo;

                audioInfo.name = effectName;

                // Load new audio clip

                Assembly asm = Assembly.GetAssembly(typeof(VehicleEffectsMod));
                var pluginInfo = PluginManager.instance.FindPluginInfo(asm);

                Debug.Log(pluginInfo.modPath);

                try
                {
                    string absUri = "file:///" + pluginInfo.modPath.Replace("\\", "/") + "/Sounds/steam-engine-a3-moving.ogg";
                    Debug.Log(absUri);
                    WWW www = new WWW(absUri);
                    audioInfo.m_clip = www.GetAudioClip(true, false);
                }
                catch(Exception e)
                {
                    Debug.Log("Exception trying to load audio file!" + e.ToString());
                    return null;
                }

                steamEngineSound.m_audioInfo = audioInfo;

                return steamEngineSound;
            }
            else
            {
                Debug.Log("Could not find default train sound effect!");
                return null;
            }
        }
    }
}
