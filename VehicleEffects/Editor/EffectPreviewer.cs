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

        public void ApplyPreview(VehicleEffectsDefinition definition, string packageName)
        {
            RevertPreview();

            m_parseErrors = new HashSet<string>();

            if(definition?.Vehicles == null || definition.Vehicles.Count == 0)
            {
                m_parseErrors.Add("Previewer - vehicleEffectDef is null or empty.");
            }
            else
            {
                m_isApplied = true;
                foreach(var vehicleDef in definition.Vehicles)
                {
                    VehicleEffectsMod.ParseVehicleDefinition(vehicleDef, packageName, ref m_changes, ref m_parseErrors);
                }
            }

            if(m_parseErrors?.Count > 0)
            {
                var errorMessage = m_parseErrors.Aggregate("Error while parsing vehicle effect definition preview.\n" + "List of errors:\n", (current, error) => current + (error + '\n'));
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Vehicle Effects", errorMessage, false);
            }
        }

        public void RevertPreview()
        {
            if(m_isApplied)
            {
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
