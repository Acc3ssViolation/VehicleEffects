using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects.Editor
{
    public class PrefabWatcher : MonoBehaviour
    {
        public static PrefabWatcher instance { get; private set; }

        public delegate void OnPrefabChanged();
        public delegate void OnTrailersChanged(string[] names);

        public event OnPrefabChanged prefabBecameVehicle;
        public event OnPrefabChanged prefabWasVehicle;
        public event OnPrefabChanged prefabBecameBuilding;
        public event OnPrefabChanged prefabWasBuilding;
        public event OnPrefabChanged prefabBecameProp;
        public event OnPrefabChanged prefabWasProp;

        public event OnPrefabChanged prefabChanged;
        public event OnTrailersChanged trailersChanged;

        private Type m_typeOfPrefab;
        private string m_prefabName;

        private string[] m_trailerNames;

        void Awake()
        {
            if(instance != null)
            {
                Debug.LogWarning("More than 1 PrefabWatcher active!");
                return;
            }
            m_trailerNames = new string[0];
            instance = this;
        }

        void LateUpdate()
        {
            ToolController properties = Singleton<ToolManager>.instance.m_properties;
            if(properties != null && properties.m_editPrefabInfo != null)
            {
                if(m_typeOfPrefab != properties.m_editPrefabInfo.GetType())
                {
                    if(m_typeOfPrefab == typeof(VehicleInfo))
                    {
                        prefabWasVehicle?.Invoke();
                    }
                    else if(m_typeOfPrefab == typeof(BuildingInfo))
                    {
                        prefabWasBuilding?.Invoke();
                    }
                    else if(m_typeOfPrefab == typeof(PropInfo))
                    {
                        prefabWasProp?.Invoke();
                    }

                    m_typeOfPrefab = properties.m_editPrefabInfo.GetType();

                    if(m_typeOfPrefab == typeof(VehicleInfo))
                    {
                        prefabBecameVehicle?.Invoke();
                    }
                    else if(m_typeOfPrefab == typeof(BuildingInfo))
                    {
                        prefabBecameBuilding?.Invoke();
                    }
                    else if(m_typeOfPrefab == typeof(PropInfo))
                    {
                        prefabBecameProp?.Invoke();
                    }
                }

                if(m_prefabName != properties.m_editPrefabInfo.name)
                {
                    m_prefabName = properties.m_editPrefabInfo.name;
                    prefabChanged?.Invoke();
                    Debug.Log(m_prefabName);
                }

                int trailerCount = ((properties.m_editPrefabInfo as VehicleInfo)?.m_trailers != null) ? (properties.m_editPrefabInfo as VehicleInfo).m_trailers.Length : 0;
                bool trailersHaveChanged = false;
                if(m_trailerNames.Length != trailerCount)
                {
                    trailersHaveChanged = true;
                }
                else
                {
                    for(int i = 0; i < trailerCount; i++)
                    {
                        if(m_trailerNames[i] != (properties.m_editPrefabInfo as VehicleInfo).m_trailers[i].m_info.name)
                        {
                            trailersHaveChanged = true;
                            break;
                        }
                    }
                }
                if(trailersHaveChanged)
                {
                    m_trailerNames = new string[trailerCount];
                    for(int i = 0; i < trailerCount; i++)
                    {
                        m_trailerNames[i] = (properties.m_editPrefabInfo as VehicleInfo).m_trailers[i].m_info.name;
                    }
                    trailersChanged?.Invoke(m_trailerNames);
                }
            }
        }
    }
}
