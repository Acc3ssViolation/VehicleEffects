using ColossalFramework.Math;
using UnityEngine;

namespace VehicleEffects.GameExtensions
{
    public class CustomMovementParticleEffect : MovementParticleEffect
    {
        /// <summary>
        /// Determines how much of the vehicle velocity is carried over to a particle.
        /// </summary>
        public float m_velocityMultiplier = 1.0f;
        /// <summary>
        /// Radius of spawn area.
        /// </summary>
        public float m_spawnAreaRadius = 0.25f;
        /// <summary>
        /// Overrides the particle's start color
        /// </summary>
        public Color? m_colorOverride = null;

        public override void RenderEffect(InstanceID id, EffectInfo.SpawnArea area, Vector3 velocity, float acceleration, float magnitude, float timeOffset, float timeDelta, RenderManager.CameraInfo cameraInfo)
        {
            Vector3 point = area.m_matrix.MultiplyPoint(Vector3.zero);

            if(cameraInfo.CheckRenderDistance(point, this.m_maxVisibilityDistance) && cameraInfo.Intersect(point, 100f))
            {
                Vector3 vector = new Vector3(100000f, 100000f, 100000f);
                Vector3 vector2 = new Vector3(-100000f, -100000f, -100000f);

                magnitude = Mathf.Min(magnitude, this.m_minMagnitude + velocity.magnitude * this.m_magnitudeSpeedMultiplier + acceleration * this.m_magnitudeAccelerationMultiplier);
                float particlesPerSquare = timeDelta * magnitude * 0.01f;

                velocity = velocity * m_velocityMultiplier;

                EmitParticles(id, area.m_matrix, area.m_positions, velocity, particlesPerSquare, 100, ref vector, ref vector2);
            }
        }

        public new void EmitParticles(InstanceID id, Matrix4x4 matrix, Vector4[] positions, Vector3 velocity, float particlesPerSquare, int probability, ref Vector3 min, ref Vector3 max)
        {
            Randomizer randomizer = new Randomizer(id.RawData);
            var module = m_particleSystem.emission;
            particlesPerSquare *= module.rateOverTime.constant;

            if(randomizer.Int32(100u) <= probability)
            {
                Vector3 worldPoint = matrix.MultiplyPoint(Vector3.zero);
                float w = m_spawnAreaRadius;

                float circleArea = Mathf.Max(100f, 3.14159274f * w * w);
                int particleCount = Mathf.FloorToInt(circleArea * particlesPerSquare + Random.value);
                for(int j = 0; j < particleCount; j++)
                {
                    ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
                    Vector3 position = worldPoint;
                    float startSpeed = this.m_minStartSpeed + (this.m_maxStartSpeed - this.m_minStartSpeed) * Random.value;

                    // Get random velocity vector witin a cone of 2 * m_maxSpawnAngle, in the direction of m_particleVector
                    float d = 1f / Mathf.Tan(Mathf.Deg2Rad * m_maxSpawnAngle);
                    Vector3 a = Random.insideUnitCircle;
                    a.z = d;
                    Vector3 b = Random.insideUnitCircle * m_spawnAreaRadius;
                    b.z = 0f;

                    position += matrix.MultiplyVector(b);
                    emitParams.position = position;
                    emitParams.startColor = m_colorOverride != null ? m_colorOverride.Value : m_particleSystem.main.startColor.color;
                    emitParams.velocity = velocity + matrix.MultiplyVector(a) * startSpeed;
                    emitParams.startLifetime = this.m_minLifeTime + (this.m_maxLifeTime - this.m_minLifeTime) * UnityEngine.Random.value;
                    emitParams.startSize = this.m_particleSystem.main.startSize.constant;
                    this.m_particleSystem.Emit(emitParams, 1);
                }
                min = Vector3.Min(min, worldPoint - new Vector3(w, w, w));
                max = Vector3.Max(max, worldPoint + new Vector3(w, w, w));
            }
        }

        protected override void CreateEffect()
        {
            if (this.m_effectObject == null)
            {
                // We don't have to spawn a new object as we are already a scene object
                this.m_effectObject = gameObject;
                this.m_effectComponent = this;
                this.m_particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
                this.m_particleSystem = gameObject.GetComponent<ParticleSystem>();
                var emission = this.m_particleSystem.emission;
                emission.enabled = false;
            }

            base.CreateEffect();
        }

        protected override void DestroyEffect()
        {

            if (this.m_effectObject != null)
            {
                // Since we are using our own object as the effect object we don't actually want to destroy it
                // This is because unlike the default game effects, we are not a prefab but we are normal scene object
                this.m_effectObject = null;
            }

            // This will clean up the rest
            base.DestroyEffect();
        }
    }
}
