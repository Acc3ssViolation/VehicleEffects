using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VehicleEffects.GameExtensions;

namespace VehicleEffects.Effects
{
    /*public class PropellerEffectManager
    {
        private const string gameObjectName = "Propeller Effect Manager";
        private static GameObject gameObject;

        public static Mesh Mesh { get; private set; }
        public static Material Material { get; private set; }
        public static MaterialPropertyBlock MaterialBlock { get; private set; }

        public static GameObject CreateEffectObject(Transform parent)
        {
            var prefab = Util.LoadGameObjectFromAssetBundle("propellers", "PropellerPrefab");
            if(prefab != null)
            {
                prefab.SetActive(false);
                var go = new GameObject(gameObjectName);
                go.transform.SetParent(parent);
                prefab.transform.SetParent(go.transform);

                Mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
                Material = prefab.GetComponent<MeshRenderer>().sharedMaterial;
                MaterialBlock = new MaterialPropertyBlock();

                gameObject = go;
                return go;
            }
            else
            {
                Logging.LogError("Failed to load propeller prefab!");
                return null;
            }
        }

        public static PropellerEffect CreatePropeller(Vector3 position, Vector3 axis, float diameter)
        {
            var go = new GameObject("Prop " + position + ", " + axis + ", " + diameter);
            go.transform.SetParent(gameObject.transform);

            var effect = go.AddComponent<PropellerEffect>();

            effect.m_mesh = CreateMesh(diameter);
            effect.m_position = position;
            effect.m_rotationAxis = axis;

            return effect;
        }

        private static Mesh CreateMesh(float diameter)
        {
            Mesh newMesh = new Mesh();

            Vector3[] verts = Mesh.vertices;
            for(int i = 0; i < verts.Length; i++)
            {
                verts[i].Scale(new Vector3(diameter, diameter, 1));
            }

            newMesh.vertices = verts;
            newMesh.uv = Mesh.uv;
            newMesh.triangles = Mesh.triangles;
            newMesh.RecalculateNormals();
            newMesh.RecalculateBounds();
            newMesh.UploadMeshData(true);

            return newMesh;
        }
    }*/
}
