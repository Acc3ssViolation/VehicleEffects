using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects.GameExtensions
{
    public class VehicleEffectWrapper : EffectInfo
    {
        public struct VehicleEffectParams
        {
            public String m_name;
            public Vector3 m_position;
            public Vector3 m_direction;

            public float m_maxSpeed;
            public float m_minSpeed;
        }


        public EffectInfo m_wrappedEffect;
        private bool m_isInitialized;

        public VehicleEffectParams m_params = new VehicleEffectParams();


        protected override void CreateEffect()
        {
            if(!m_isInitialized)
            {
                m_wrappedEffect.InitializeEffect();
                m_isInitialized = true;
            }
        }

        protected override void DestroyEffect()
        {
            if(m_isInitialized)
            {
                m_wrappedEffect.ReleaseEffect();
                m_isInitialized = false;
            }
        }

        public override void RenderEffect(InstanceID id, SpawnArea area, Vector3 velocity, float acceleration, float magnitude, float timeDelta, RenderManager.CameraInfo cameraInfo)
        {
            if(velocity.magnitude >= m_params.m_minSpeed && velocity.magnitude <= m_params.m_maxSpeed)
            {
                // Add pos, dir and scale to matrix
                area.m_matrix = area.m_matrix * Matrix4x4.TRS(m_params.m_position, Quaternion.LookRotation(m_params.m_direction), Vector3.one);
                m_wrappedEffect.RenderEffect(id, area, velocity, acceleration, magnitude, timeDelta, cameraInfo);
            }
        }

        public override void PlayEffect(InstanceID id, SpawnArea area, Vector3 velocity, float acceleration, float magnitude, AudioManager.ListenerInfo listenerInfo, AudioGroup audioGroup)
        {
            if(velocity.magnitude >= m_params.m_minSpeed && velocity.magnitude <= m_params.m_maxSpeed)
            {
                area.m_matrix = area.m_matrix * Matrix4x4.TRS(m_params.m_position, Quaternion.LookRotation(m_params.m_direction), Vector3.one);
                m_wrappedEffect.PlayEffect(id, area, velocity, acceleration, magnitude, listenerInfo, audioGroup);
            }
        }

    }
}
