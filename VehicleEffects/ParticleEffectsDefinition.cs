using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace VehicleEffects
{
    public class ParticleEffectsDefinition
    {
        public List<ParticleEffectParams> Effects { get; set; }
    }

    [XmlType(TypeName = "Color")]
    public class ParticleColor
    {
        [XmlAttribute("r")]
        public float r;
        [XmlAttribute("g")]
        public float g;
        [XmlAttribute("b")]
        public float b;
        [XmlAttribute("a")]
        public float a;

        public UnityEngine.Color ToUnity()
        {
            return new UnityEngine.Color(r, g, b, a);
        }
    }

    [XmlType(TypeName = "ParticleEffect")]
    public class ParticleEffectParams
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("base")]
        public string Base { get; set; }

        // ParticleEffect
        [XmlAttribute("usebezier")]
        public bool? m_canUseBezier { get; set; }
        [XmlAttribute("usemeshdata")]
        public bool? m_canUseMeshData { get; set; }
        [XmlAttribute("usepositions")]
        public bool? m_canUsePositions { get; set; }
        [XmlAttribute("extraradius")]
        public float? m_extraRadius { get; set; }
        [XmlAttribute("maxlifetime")]
        public float? m_maxLifeTime { get; set; }
        [XmlAttribute("maxspawnangle")]
        public float? m_maxSpawnAngle { get; set; }
        [XmlAttribute("maxstartspeed")]
        public float? m_maxStartSpeed { get; set; }
        [XmlAttribute("maxvisdistance")]
        public float? m_maxVisibilityDistance { get; set; }
        [XmlAttribute("minlifetime")]
        public float? m_minLifeTime { get; set; }
        [XmlAttribute("minspawnangle")]
        public float? m_minSpawnAngle { get; set; }
        [XmlAttribute("minstartspeed")]
        public float? m_minStartSpeed { get; set; }
        [XmlAttribute("renderduration")]
        public float? m_renderDuration { get; set; }
        [XmlAttribute("usesimtime")]
        public bool? m_useSimulationTime { get; set; }

        // MovementParticleEffect
        [XmlAttribute("magaccmultiplier")]
        public float? m_magnitudeAccelerationMultiplier { get; set; }
        [XmlAttribute("magspeedmultiplier")]
        public float? m_magnitudeSpeedMultiplier { get; set; }
        [XmlAttribute("minmagnitude")]
        public float? m_minMagnitude { get; set; }

        // CustomMovementParticleEffect
        [XmlAttribute("velocitymultiplier")]
        public float? m_velocityMultiplier { get; set; }
        [XmlAttribute("spawnarearadius")]
        public float? m_spawnAreaRadius { get; set; }
        // Subtag
        public ParticleColor Color { get; set; }

        public ParticleEffectParams()
        {
            Name = "";
            Base = "";
            Color = null;
        }

        public virtual ParticleEffect CreateEffect()
        {
            return CustomParticlesManager.CreateParticleEffect(this);
        }
    }
}
