using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects.GameExtensions
{
    public class CustomMultiEffect : MultiEffect
    {
        public VehicleEffectWrapper.VehicleEffectParams m_params = new VehicleEffectWrapper.VehicleEffectParams();

        public override void RenderEffect(InstanceID id, SpawnArea area, Vector3 velocity, float acceleration, float magnitude, float timeDelta, RenderManager.CameraInfo cameraInfo)
        {
            if(velocity.magnitude >= m_params.m_minSpeed && velocity.magnitude <= m_params.m_maxSpeed)
            {
                base.RenderEffect(id, area, velocity, acceleration, magnitude, timeDelta, cameraInfo);
            }
        }

        public override void PlayEffect(InstanceID id, SpawnArea area, Vector3 velocity, float acceleration, float magnitude, AudioManager.ListenerInfo listenerInfo, AudioGroup audioGroup)
        {
            if(velocity.magnitude >= m_params.m_minSpeed && velocity.magnitude <= m_params.m_maxSpeed)
            {
                base.PlayEffect(id, area, velocity, acceleration, magnitude, listenerInfo, audioGroup);
            }
        }
    }
}
