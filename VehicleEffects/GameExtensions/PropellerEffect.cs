using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VehicleEffects.Effects;

namespace VehicleEffects.GameExtensions
{
    /*public class PropellerEffect : EffectInfo
    {
        public Mesh m_mesh;

        public Vector3 m_position;
        public Vector3 m_rotationAxis;

        protected override void CreateEffect()
        {
            base.CreateEffect();
        }

        public override void RenderEffect(InstanceID id, SpawnArea area, Vector3 velocity, float acceleration, float magnitude, float timeOffset, float timeDelta, RenderManager.CameraInfo cameraInfo)
        {
            int rotationSpeed = 1;

            float angle = (float)(rotationSpeed * 6) * Singleton<SimulationManager>.instance.m_simulationTimer;
            Quaternion rotation = Quaternion.AngleAxis(angle, m_rotationAxis);

            var materialBlock = PropellerEffectManager.MaterialBlock;
            materialBlock.Clear();
            materialBlock.SetMatrix(Singleton<VehicleManager>.instance.ID_TyreMatrix, Matrix4x4.identity);
            materialBlock.SetVector(Singleton<VehicleManager>.instance.ID_TyrePosition, Vector4.zero);
            materialBlock.SetVector(Singleton<VehicleManager>.instance.ID_LightState, Vector4.zero);

            area.m_matrix = area.m_matrix * Matrix4x4.TRS(m_position, rotation, Vector3.one);

            Graphics.DrawMesh(m_mesh, area.m_matrix, PropellerEffectManager.Material, 0, cameraInfo.m_camera, 0, materialBlock);
        }
    }*/
}
