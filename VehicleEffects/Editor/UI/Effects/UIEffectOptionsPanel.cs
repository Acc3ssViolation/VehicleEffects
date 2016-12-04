using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ExtendedAssetEditor.UI.Effects
{
    public class UIEffectOptionsPanel : UIPanel
    {
        public UIEffectPanel m_mainPanel;

        private VehicleInfo.Effect m_effect;
        private UIButton m_buttonFlagsRequired;
        private UIButton m_buttonFlagsForbidden;
        private UIButton m_buttonParkedFlagsRequired;
        private UIButton m_buttonParkedFlagsForbidden;
        private int m_index;
        private UILabel m_nameLabel;
        private UILabel m_indexLabel;
        private UILabel m_subEffectsLabel;

        public void CreateComponents()
        {
            m_nameLabel = AddUIComponent<UILabel>();
            m_nameLabel.relativePosition = new Vector3(0, 0);
            
            m_indexLabel = AddUIComponent<UILabel>();
            m_indexLabel.textScale = 0.8f;
            m_indexLabel.relativePosition = new Vector3(0, 30);


            UILabel label = AddUIComponent<UILabel>();
            label.text = "Vehicle Flags";
            label.relativePosition = new Vector3(0, 55);

            m_buttonFlagsRequired = UIUtils.CreateButton(this);
            m_buttonFlagsRequired.text = "Required Flags";
            m_buttonFlagsRequired.width = 150;
            m_buttonFlagsRequired.relativePosition = new Vector3(10, 75);
            m_buttonFlagsRequired.eventClicked += (c, b) =>
            {
                if(m_mainPanel.m_flagsPanel != null && !m_mainPanel.m_flagsPanel.isVisible)
                {
                    m_mainPanel.m_flagsPanel.Show("Required Flags", m_effect.m_vehicleFlagsRequired, (flags) => {
                        m_effect.m_vehicleFlagsRequired = flags;
                        m_mainPanel.ChangeEffect(m_effect, m_index);
                        Display(m_effect, m_index);
                    });
                }
            };

            m_buttonFlagsForbidden = UIUtils.CreateButton(this);
            m_buttonFlagsForbidden.text = "Forbidden Flags";
            m_buttonFlagsForbidden.width = 150;
            m_buttonFlagsForbidden.relativePosition = new Vector3(20 + m_buttonFlagsRequired.width, 75);
            m_buttonFlagsForbidden.eventClicked += (c, b) =>
            {
                if(m_mainPanel.m_flagsPanel != null && !m_mainPanel.m_flagsPanel.isVisible)
                {
                    m_mainPanel.m_flagsPanel.Show("Forbidden Flags", m_effect.m_vehicleFlagsForbidden, (flags) => {
                        m_effect.m_vehicleFlagsForbidden = flags;
                        m_mainPanel.ChangeEffect(m_effect, m_index);
                        Display(m_effect, m_index);
                    });
                }
            };

            label = AddUIComponent<UILabel>();
            label.text = "Parked Flags";
            label.relativePosition = new Vector3(0, 115);

            m_buttonParkedFlagsRequired = UIUtils.CreateButton(this);
            m_buttonParkedFlagsRequired.text = "Required Flags";
            m_buttonParkedFlagsRequired.width = 150;
            m_buttonParkedFlagsRequired.relativePosition = new Vector3(10, 135);
            m_buttonParkedFlagsRequired.eventClicked += (c, b) =>
            {
                if(m_mainPanel.m_flagsPanel != null && !m_mainPanel.m_flagsPanel.isVisible)
                {
                    m_mainPanel.m_flagsPanel.Show("Required Parked Flags", m_effect.m_parkedFlagsRequired, (flags) => {
                        m_effect.m_parkedFlagsRequired = flags;
                        m_mainPanel.ChangeEffect(m_effect, m_index);
                        Display(m_effect, m_index);
                    });
                }
            };

            m_buttonParkedFlagsForbidden = UIUtils.CreateButton(this);
            m_buttonParkedFlagsForbidden.text = "Forbidden Flags";
            m_buttonParkedFlagsForbidden.width = 150;
            m_buttonParkedFlagsForbidden.relativePosition = new Vector3(20 + m_buttonParkedFlagsRequired.width, 135);
            m_buttonParkedFlagsForbidden.eventClicked += (c, b) =>
            {
                if(m_mainPanel.m_flagsPanel != null && !m_mainPanel.m_flagsPanel.isVisible)
                {
                    m_mainPanel.m_flagsPanel.Show("Forbidden Parked Flags", m_effect.m_parkedFlagsForbidden, (flags) => {
                        m_effect.m_parkedFlagsForbidden = flags;
                        m_mainPanel.ChangeEffect(m_effect, m_index);
                        Display(m_effect, m_index);
                    });
                }
            };

            label = AddUIComponent<UILabel>();
            label.text = "Sub Effects";
            label.relativePosition = new Vector3(0, m_buttonParkedFlagsForbidden.relativePosition.y + m_buttonParkedFlagsForbidden.height + 10);

            m_subEffectsLabel = AddUIComponent<UILabel>();
            m_subEffectsLabel.textScale = 0.9f;
            m_subEffectsLabel.relativePosition = new Vector3(0, label.relativePosition.y + 30);
        }

        public void Display(VehicleInfo.Effect effect, int index)
        {
            if(m_buttonFlagsForbidden == null)
                return;

            m_effect = effect;
            m_index = index;

            m_buttonFlagsRequired.tooltip = m_effect.m_vehicleFlagsRequired.ToString();
            m_buttonFlagsForbidden.tooltip = m_effect.m_vehicleFlagsForbidden.ToString();
            m_buttonParkedFlagsRequired.tooltip = m_effect.m_parkedFlagsRequired.ToString();
            m_buttonParkedFlagsForbidden.tooltip = m_effect.m_parkedFlagsForbidden.ToString();

            m_nameLabel.text = m_effect.m_effect != null ? m_effect.m_effect.name : "ERROR: Missing effect!";

            var le = m_effect.m_effect as LightEffect;
            if(le != null && le.m_positionIndex >=0)
            {
                m_indexLabel.text = "Light index: " + le.m_positionIndex;
            }
            else
            {
                m_indexLabel.text = "Light index: None";
            }

            var me = m_effect.m_effect as MultiEffect;
            if(me != null)
            {
                m_subEffectsLabel.text = "";
                foreach(var se in me.m_effects)
                {
                    if(se.m_effect != null)
                    {
                        m_subEffectsLabel.text += se.m_effect.name;
                        var sle = se.m_effect as LightEffect;
                        if(sle != null && sle.m_positionIndex >= 0)
                        {
                            m_subEffectsLabel.text += " (Light index " + sle.m_positionIndex + ")";
                        }
                    }
                    else
                    {
                        m_subEffectsLabel.text += "ERROR: Missing effect!";
                    }
                    m_subEffectsLabel.text += "\r\n";
                }
            }
            else
            {
                m_subEffectsLabel.text = "None";
            }
        }
    }
}
