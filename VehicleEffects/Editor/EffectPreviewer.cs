using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleEffects.Editor
{
    public class EffectPreviewer
    {
        private HashSet<string> m_parseErrors;
        private Dictionary<VehicleInfo, VehicleInfo.Effect[]> m_changes = new Dictionary<VehicleInfo, VehicleInfo.Effect[]>();
        private bool m_isApplied;

        public bool IsPreviewing
        {
            get
            {
                return m_isApplied;
            }
        }

        public bool ApplyPreview(VehicleEffectsDefinition definition, string packageName)
        {
            RevertPreview();

            m_parseErrors = new HashSet<string>();

            VehicleInfo vehicleInfo = ToolsModifierControl.toolController.m_editPrefabInfo as VehicleInfo;
            if(vehicleInfo != null)
            {
                // Find the VehicleInfo's currently in the scene and store them in a dictionary
                Dictionary<string, VehicleInfo> infoDict = new Dictionary<string, VehicleInfo>();
                infoDict.Add(vehicleInfo.name, vehicleInfo);

                if(vehicleInfo.m_trailers != null)
                {
                    foreach(var trailer in vehicleInfo.m_trailers)
                    {
                        if(!infoDict.ContainsKey(trailer.m_info.name))
                        {
                            infoDict.Add(trailer.m_info.name, trailer.m_info);
                        }
                    }
                }


                if(definition?.Vehicles == null || definition.Vehicles.Count == 0)
                {
                    m_parseErrors.Add("Previewer - vehicleEffectDef is null or empty.");
                }
                else
                {
                    m_isApplied = true;
                    foreach(var vehicleDef in definition.Vehicles)
                    {
                        // Check if the vehicle of this definition is in the scene and if so, apply it
                        VehicleInfo info;
                        if(infoDict.TryGetValue(vehicleDef.Name, out info))
                        {
                            VehicleEffectsMod.ParseVehicleDefinition(vehicleDef, packageName, ref m_changes, ref m_parseErrors, false, info);
                        }
                        else
                        {
                            m_parseErrors.Add("Prefab for " + vehicleDef.Name + " not found!");
                            m_parseErrors.Add(infoDict.Keys.Aggregate("List of prefabs:\n", (current, error) => current + error + "\n"));
                        }
                    }
                }
            }
            else
            {
                m_parseErrors.Add("No prefab found.");
            }

            

            if(m_parseErrors?.Count > 0)
            {
                var errorMessage = m_parseErrors.Aggregate("Error while parsing vehicle effect definition preview.\n" + "List of errors:\n", (current, error) => current + (error + '\n'));
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Vehicle Effects", errorMessage, false);
            }

            return m_isApplied;
        }

        public void RevertPreview()
        {
            if(m_isApplied)
            {
                // Restore old effect arrays
                foreach(var change in m_changes)
                {
                    change.Key.m_effects = change.Value;
                }
                m_changes.Clear();
                m_isApplied = false;
            }
        }

        public void ForceClear()
        {
            m_changes.Clear();
            m_isApplied = false;
        }
    }
}
