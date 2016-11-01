/*using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects.GameExtensions
{
    public class IntensityLightEffect : LightEffect
    {
        public short m_lightIntensityIndex = -1;

        public override void RenderEffect(InstanceID id, SpawnArea area, Vector3 velocity, float acceleration, float magnitude, float timeDelta, RenderManager.CameraInfo cameraInfo)
        {
            var vehicle = VehicleManager.instance.m_vehicles.m_buffer[id.Vehicle];
            var frameData = vehicle.GetLastFrameData();

            if(m_lightIntensityIndex < 0 || m_lightIntensityIndex > 3 || frameData.m_lightIntensity[m_lightIntensityIndex] > 0)
            {
                base.RenderEffect(id, area, velocity, acceleration, magnitude, timeDelta, cameraInfo);
            }
        }
    }
}*/
