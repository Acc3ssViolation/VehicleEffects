using System;
using UnityEngine;
using System.IO;
using System.Reflection;

namespace VehicleEffects
{
    public class Util
    {
        public static float SpeedKmHToInternal(float kmh)
        {
            return kmh * 0.16f;
        }

        public static float SpeedInternalToKmH(float intern)
        {
            return intern / 0.16f;
        }

        public static float SpeedKmHToEffect(float kmh)
        {
            return kmh * 0.6f;
        }

        public static AudioClip LoadAudioClipFromModDir(string filename)
        {
            Assembly asm = Assembly.GetAssembly(typeof(VehicleEffectsMod));
            var pluginInfo = ColossalFramework.Plugins.PluginManager.instance.FindPluginInfo(asm);


            try
            {
                string absUri = "file:///" + pluginInfo.modPath.Replace("\\", "/") + "/" + filename;
                WWW www = new WWW(absUri);
                return www.GetAudioClip(true, false);
            }
            catch(Exception e)
            {
                Logging.Log("Exception trying to load audio file '" + filename + "'!" + e.ToString());
                return null;
            }
        }

        public static EngineSoundEffect CopyEngineSoundEffect(EngineSoundEffect template, EngineSoundEffect target)
        {
            target.m_minPitch = template.m_minPitch;
            target.m_minRange = template.m_minRange;
            target.m_pitchAccelerationMultiplier = template.m_pitchAccelerationMultiplier;
            target.m_pitchSpeedMultiplier = template.m_pitchSpeedMultiplier;
            target.m_position = template.m_position;
            target.m_range = template.m_range;
            target.m_rangeAccelerationMultiplier = template.m_rangeAccelerationMultiplier;
            target.m_rangeSpeedMultiplier = template.m_rangeSpeedMultiplier;
            target.m_audioInfo = template.m_audioInfo;

            return target;
        }

        public static Light CopyLight(Light template, Light target)
        {
            target.bounceIntensity = template.bounceIntensity;
            target.color = template.color;
            target.cookie = template.cookie;
            target.cookieSize = template.cookieSize;
            target.cullingMask = template.cullingMask;
            target.enabled = template.enabled;
            target.flare = template.flare;
            target.hideFlags = template.hideFlags;
            target.intensity = template.intensity;
            target.range = template.range;
            target.renderMode = template.renderMode;
            target.shadowBias = template.shadowBias;
            target.shadowNormalBias = template.shadowNormalBias;
            target.shadows = template.shadows;
            target.shadowStrength = template.shadowStrength;
            target.spotAngle = template.spotAngle;
            target.type = template.type;

            return target;
        }

        public static LightEffect CopyLightEffect(LightEffect template, LightEffect target)
        {
            target.m_alignment = template.m_alignment;
            target.m_batchedLight = template.m_batchedLight;
            target.m_blinkType = template.m_blinkType;
            target.m_fadeEndDistance = template.m_fadeEndDistance;
            target.m_fadeSpeed = template.m_fadeSpeed;
            target.m_fadeStartDistance = template.m_fadeStartDistance;
            target.m_offRange = template.m_offRange;
            target.m_position = template.m_position;
            target.m_positionIndex = template.m_positionIndex;
            target.m_rotationAxis = template.m_rotationAxis;
            target.m_rotationSpeed = template.m_rotationSpeed;
            target.m_spotLeaking = template.m_spotLeaking;
            target.m_variationColors = template.m_variationColors;
            return target;
        }
        /*static public void SaveAudioClipToWav(AudioClip audioClip, string filename)
        {
            FileStream fsWrite = File.Open(filename, FileMode.Create);

            BinaryWriter bw = new BinaryWriter(fsWrite);

            Byte[] header = { 82, 73, 70, 70, 22, 10, 4, 0, 87, 65, 86, 69, 102, 109, 116, 32 };
            bw.Write(header);

            Byte[] header2 = { 16, 0, 0, 0, 1, 0, 1, 0, 68, 172, 0, 0, 136, 88, 1, 0 };
            bw.Write(header2);

            Byte[] header3 = { 2, 0, 16, 0, 100, 97, 116, 97, 152, 9, 4, 0 };
            bw.Write(header3);

            float[] samples = new float[audioClip.samples];
            audioClip.GetData(samples, 0);
            int i = 0;

            while(i < audioClip.samples)
            {
                int sampleInt = (int)(32000.0 * samples[i++]);

                int msb = sampleInt / 256;
                int lsb = sampleInt - (msb * 256);

                bw.Write((Byte)lsb);
                bw.Write((Byte)msb);
            }

            fsWrite.Close();

        }*/
    }
}
