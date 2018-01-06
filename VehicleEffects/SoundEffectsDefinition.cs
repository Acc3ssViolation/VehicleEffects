using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace VehicleEffects
{
    /// <summary>
    /// Container for serializing sound effect parameters.
    /// </summary>
    public class SoundEffectsDefinition
    {
        public List<SoundEffectParams> Effects { get; set; }

        public static SoundEffectsDefinition Deserialize(string path)
        {
            try
            {
                XmlSerializer s = new XmlSerializer(typeof(SoundEffectsDefinition));
                using(var reader = new StreamReader(path))
                {
                    return s.Deserialize(reader) as SoundEffectsDefinition;
                }
            }
            catch(Exception e)
            {
                Logging.LogError("Exception thrown when deserializing config file " + path + "\r\n" + e.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// Indicates the type (class) of sound effect.
    /// </summary>
    public enum SoundEffectType
    {
        SoundEffect,
        EngineSoundEffect,
    }

    /// <summary>
    /// Sound Effect Parameters defining a sound effect. Negative values for floats
    /// are ignored and the base effect's values will be used instead.
    /// TODO: Use nullable values and see if that works, it's probably better
    /// </summary>
    [XmlType(TypeName = "SoundEffect")]
    public class SoundEffectParams
    {
        [XmlAttribute("type")]
        public SoundEffectType Type { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("base")]
        public string Base { get; set; }
        [XmlAttribute("file")]
        public string SoundFile { get; set; }
        [XmlAttribute("range")]
        public float? Range { get; set; }
        [XmlAttribute("volume")]
        public float? Volume { get; set; }
        [XmlAttribute("pitch")]
        public float? Pitch { get; set; }
        [XmlAttribute("fadelength")]
        public float? FadeLength { get; set; }
        [XmlAttribute("loop")]
        public bool? Loop { get; set; }
        [XmlAttribute("is3d")]
        public bool? Is3D { get; set; }
        [XmlAttribute("randomtime")]
        public bool? RandomTime { get; set; }
        [XmlAttribute("minpitch")]
        public float? MinPitch { get; set; }
        [XmlAttribute("minrange")]
        public float? MinRange { get; set; }
        [XmlAttribute("pitcham")]
        public float? PitchAccelerationMultiplier { get; set; }
        [XmlAttribute("pitchsm")]
        public float? PitchSpeedMultiplier { get; set; }
        [XmlAttribute("rangeam")]
        public float? RangeAccelerationMultiplier { get; set; }
        [XmlAttribute("rangesm")]
        public float? RangeSpeedMultiplier { get; set; }

        public SoundEffectParams()
        {
            Name = "";
            Base = "";
            SoundFile = "";
            Type = SoundEffectType.SoundEffect;
        }

        public virtual SoundEffect CreateEffect()
        {
            return CustomSoundsManager.CreateSoundEffect(this);
        }
    }
}
