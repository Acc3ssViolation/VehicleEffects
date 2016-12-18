using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects.Editor
{
    public class UIEffectOptionsPanel : UIPanel
    {
        public UIEffectPanel m_mainPanel;

        public VehicleEffectsDefinition.Effect m_effect;
        private UIButton m_buttonFlagsRequired;
        private UIButton m_buttonFlagsForbidden;
        private UILabel m_nameLabel;

        private UICheckBox m_wrapperCheckbox;
        private UIFloatField m_posX;
        private UIFloatField m_posY;
        private UIFloatField m_posZ;
        private UIFloatField m_dirX;
        private UIFloatField m_dirY;
        private UIFloatField m_dirZ;

        private UIFloatField m_minSpeed;
        private UIFloatField m_maxSpeed;

        private UITextField m_replacementField;
        private UIButton m_replacementButton;

        private UITextField m_fallbackField;
        private UIButton m_fallbackButton;

        public bool m_allowEditing = true;
        private UIPanel m_posPanel;
        private UIPanel m_dirPanel;

        public new void Hide()
        {
            m_effect = null;
            base.Hide();
        }

        public void CreateComponents()
        {
            m_nameLabel = AddUIComponent<UILabel>();
            m_nameLabel.relativePosition = new Vector3(0, 0);

            float nextFreeY = 25f;
            float yPadding = 5f;
            float xPaddingStart = 0f;
            float xPadding = 10f;

            // Wrapper
            m_wrapperCheckbox = UIUtils.CreateCheckBox(this);
            m_wrapperCheckbox.relativePosition = new Vector3(xPaddingStart, nextFreeY);
            m_wrapperCheckbox.text = "Use Wrapper";
            m_wrapperCheckbox.tooltip = "Wrappers are required for non-vehicle lights and positioning";
            m_wrapperCheckbox.eventCheckChanged += (c, b) =>
            {
                OnWrapperCheck(b);
            };

            nextFreeY += m_wrapperCheckbox.height + yPadding;

            // Direction

            m_posPanel = AddUIComponent<UIPanel>();
            m_posPanel.relativePosition = new Vector3(xPaddingStart, nextFreeY);

            // min speed
            m_minSpeed = UIFloatField.CreateField("Min Speed:", m_posPanel);
            m_minSpeed.panel.relativePosition = new Vector3(0, 0);
            m_minSpeed.textField.eventTextChanged += (c, s) => {
                if(!m_allowEditing) { return; }
                float x = m_effect.MinSpeed;
                m_effect.MinSpeed = UIFloatField.FloatFieldHandler(m_minSpeed.textField, s, ref x);
            };
            m_minSpeed.buttonUp.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_minSpeed.SetValue(m_effect.MinSpeed + 5f);
            };
            m_minSpeed.buttonDown.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_minSpeed.SetValue(m_effect.MinSpeed - 5f);
            };
            m_minSpeed.textField.tooltip = "Minimum speed the vehicle has to go for this effect to show (km/h)";

            // pos x
            m_posX = UIFloatField.CreateField("Pos X:", m_posPanel);
            m_posX.panel.relativePosition = new Vector3(0, 30);
            m_posX.textField.eventTextChanged += (c, s) => {
                if(!m_allowEditing) { return; }
                float x = m_effect.Position.X;
                m_effect.Position.X = UIFloatField.FloatFieldHandler(m_posX.textField, s, ref x);
            };
            m_posX.buttonUp.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_posX.SetValue(m_effect.Position.X + 0.1f);
            };
            m_posX.buttonDown.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_posX.SetValue(m_effect.Position.X - 0.1f);
            };

            // pos y
            m_posY = UIFloatField.CreateField("Pos Y:", m_posPanel);
            m_posY.panel.relativePosition = new Vector3(0, 60);
            m_posY.textField.eventTextChanged += (c, s) => {
                if(!m_allowEditing) { return; }
                float x = m_effect.Position.Y;
                m_effect.Position.Y = UIFloatField.FloatFieldHandler(m_posY.textField, s, ref x);
            };
            m_posY.buttonUp.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_posY.SetValue(m_effect.Position.Y + 0.1f);
            };
            m_posY.buttonDown.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_posY.SetValue(m_effect.Position.Y - 0.1f);
            };

            // pos z
            m_posZ = UIFloatField.CreateField("Pos Z:", m_posPanel);
            m_posZ.panel.relativePosition = new Vector3(0, 90);
            m_posZ.textField.eventTextChanged += (c, s) => {
                if(!m_allowEditing) { return; }
                float x = m_effect.Position.Z;
                m_effect.Position.Z = UIFloatField.FloatFieldHandler(m_posZ.textField, s, ref x);
            };
            m_posZ.buttonUp.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_posZ.SetValue(m_effect.Position.Z + 0.1f);
            };
            m_posZ.buttonDown.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_posZ.SetValue(m_effect.Position.Z - 0.1f);
            };

            m_posPanel.size = new Vector2(m_minSpeed.panel.width, m_posZ.panel.relativePosition.y + m_posZ.panel.height);


            // Direction


            m_dirPanel = AddUIComponent<UIPanel>();
            m_dirPanel.relativePosition = new Vector3(xPaddingStart + m_posPanel.size.x + xPadding, nextFreeY);

            // max speed
            m_maxSpeed = UIFloatField.CreateField("Max Speed:", m_dirPanel);
            m_maxSpeed.panel.relativePosition = new Vector3(0, 0);
            m_maxSpeed.textField.eventTextChanged += (c, s) => {
                if(!m_allowEditing) { return; }
                float x = m_effect.MaxSpeed;
                m_effect.MaxSpeed = UIFloatField.FloatFieldHandler(m_maxSpeed.textField, s, ref x);
            };
            m_maxSpeed.buttonUp.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_maxSpeed.SetValue(m_effect.MaxSpeed + 5f);
            };
            m_maxSpeed.buttonDown.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_maxSpeed.SetValue(m_effect.MaxSpeed - 5f);
            };
            m_maxSpeed.textField.tooltip = "Maximum speed the vehicle is allowed to go for this effect to show (km/h)";

            // pos x
            m_dirX = UIFloatField.CreateField("Dir X:", m_dirPanel);
            m_dirX.panel.relativePosition = new Vector3(0, 30);
            m_dirX.textField.eventTextChanged += (c, s) => {
                if(!m_allowEditing) { return; }
                float x = m_effect.Direction.X;
                m_effect.Direction.X = UIFloatField.FloatFieldHandler(m_dirX.textField, s, ref x);
            };
            m_dirX.buttonUp.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_dirX.SetValue(m_effect.Direction.X + 0.1f);
            };
            m_dirX.buttonDown.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_dirX.SetValue(m_effect.Direction.X - 0.1f);
            };

            // dir y
            m_dirY = UIFloatField.CreateField("Dir Y:", m_dirPanel);
            m_dirY.panel.relativePosition = new Vector3(0, 60);
            m_dirY.textField.eventTextChanged += (c, s) => {
                if(!m_allowEditing) { return; }
                float x = m_effect.Direction.Y;
                m_effect.Direction.Y = UIFloatField.FloatFieldHandler(m_dirY.textField, s, ref x);
            };
            m_dirY.buttonUp.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_dirY.SetValue(m_effect.Direction.Y + 0.1f);
            };
            m_dirY.buttonDown.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_dirY.SetValue(m_effect.Direction.Y - 0.1f);
            };

            // dir z
            m_dirZ = UIFloatField.CreateField("Dir Z:", m_dirPanel);
            m_dirZ.panel.relativePosition = new Vector3(0, 90);
            m_dirZ.textField.eventTextChanged += (c, s) => {
                if(!m_allowEditing) { return; }
                float x = m_effect.Direction.Z;
                m_effect.Direction.Z = UIFloatField.FloatFieldHandler(m_dirZ.textField, s, ref x);
            };
            m_dirZ.buttonUp.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_dirZ.SetValue(m_effect.Direction.Z + 0.1f);
            };
            m_dirZ.buttonDown.eventClicked += (c, b) => {
                if(!m_allowEditing) { return; }
                m_dirZ.SetValue(m_effect.Direction.Z - 0.1f);
            };

            m_dirPanel.size = new Vector2(m_maxSpeed.panel.width, m_dirZ.panel.relativePosition.y + m_dirZ.panel.height);
            nextFreeY += m_dirPanel.height + yPadding;

            // Replacment
            UILabel label = AddUIComponent<UILabel>();
            label.text = "Replaces";
            label.relativePosition = new Vector3(0, nextFreeY);
            nextFreeY += label.height + yPadding;

            m_replacementField = UIUtils.CreateTextField(this);
            m_replacementField.width = 250;
            m_replacementField.relativePosition = new Vector3(xPaddingStart, nextFreeY);
            m_replacementField.eventTextChanged += (c, t) => {
                OnReplacmentTextChanged(t);
            };
            m_replacementField.tooltip = "Effect this effect will replace";

            m_replacementButton = UIUtils.CreateButton(this);
            m_replacementButton.text = "Search";
            m_replacementButton.relativePosition = new Vector3(xPaddingStart + m_replacementField.width + xPadding, nextFreeY);
            m_replacementButton.eventClicked += (c, b) => {
                if(!UIEffectListPanel.main.isVisible)
                {
                    UIEffectListPanel.main.Show((info) => {
                        m_replacementField.text = info.name;
                    });
                }
            };

            nextFreeY += Mathf.Max(m_replacementButton.height, m_replacementField.height) + yPadding;

            // Fallback
            label = AddUIComponent<UILabel>();
            label.text = "Fallback";
            label.relativePosition = new Vector3(0, nextFreeY);
            nextFreeY += label.height + yPadding;

            m_fallbackField = UIUtils.CreateTextField(this);
            m_fallbackField.width = 250;
            m_fallbackField.relativePosition = new Vector3(xPaddingStart, nextFreeY);
            m_fallbackField.eventTextChanged += (c, t) => {
                OnFallbackTextChanged(t);
            };
            m_fallbackField.tooltip = "Effect that is used if this effect can't be found.";

            m_fallbackButton = UIUtils.CreateButton(this);
            m_fallbackButton.text = "Search";
            m_fallbackButton.relativePosition = new Vector3(xPaddingStart + m_fallbackField.width + xPadding, nextFreeY);
            m_fallbackButton.eventClicked += (c, b) => {
                if(!UIEffectListPanel.main.isVisible)
                {
                    UIEffectListPanel.main.Show((info) => {
                        m_fallbackField.text = info.name;
                    });
                }
            };

            nextFreeY += Mathf.Max(m_fallbackButton.height, m_fallbackField.height) + yPadding;

            // Fags
            label = AddUIComponent<UILabel>();
            label.text = "Vehicle Flags";
            label.relativePosition = new Vector3(0, nextFreeY);
            nextFreeY += label.height + yPadding;

            m_buttonFlagsRequired = UIUtils.CreateButton(this);
            m_buttonFlagsRequired.text = "Required Flags";
            m_buttonFlagsRequired.width = 150;
            m_buttonFlagsRequired.relativePosition = new Vector3(xPaddingStart, nextFreeY);
            m_buttonFlagsRequired.eventClicked += (c, b) =>
            {
                if(m_mainPanel.m_flagsPanel != null && !m_mainPanel.m_flagsPanel.isVisible)
                {
                    m_mainPanel.m_flagsPanel.Show("Required Flags", (Vehicle.Flags)m_effect.RequiredFlags, (flags) => {
                        if(!m_allowEditing)
                            return;

                        m_effect.RequiredFlags = (VehicleEffectsDefinition.Effect.Flags)flags;
                        Display(m_effect);
                    }, m_allowEditing);
                }
            };

            m_buttonFlagsForbidden = UIUtils.CreateButton(this);
            m_buttonFlagsForbidden.text = "Forbidden Flags";
            m_buttonFlagsForbidden.width = 150;
            m_buttonFlagsForbidden.relativePosition = new Vector3(xPaddingStart + xPadding + m_buttonFlagsRequired.width, nextFreeY);
            m_buttonFlagsForbidden.eventClicked += (c, b) =>
            {
                if(m_mainPanel.m_flagsPanel != null && !m_mainPanel.m_flagsPanel.isVisible)
                {
                    m_mainPanel.m_flagsPanel.Show("Forbidden Flags", (Vehicle.Flags)m_effect.ForbiddenFlags, (flags) =>
                    {
                        if(!m_allowEditing)
                            return;

                        m_effect.ForbiddenFlags = (VehicleEffectsDefinition.Effect.Flags)flags;
                        Display(m_effect);
                    }, m_allowEditing);
                }
            };

            nextFreeY += m_buttonFlagsForbidden.height + yPadding;
        }

        private void OnFallbackTextChanged(string text)
        {
            string trimmedText = text.Trim();
            if(string.IsNullOrEmpty(trimmedText))
            {
                m_effect.Fallback = null;
            }
            else
            {
                m_effect.Fallback = text;
            }
        }

        private void OnReplacmentTextChanged(string text)
        {
            string trimmedText = text.Trim();
            if(string.IsNullOrEmpty(trimmedText))
            {
                if(m_effect.Replacment != null)
                {
                    // Change back to being an addition
                    m_effect.Name = m_effect.Replacment;
                    m_effect.Replacment = null;
                }
            }
            else
            {
                if(m_effect.Replacment == null)
                {
                    // Change from an addition to a replacment
                    m_effect.Replacment = m_effect.Name;
                }
                m_effect.Name = text;
            }
        }

        private void OnWrapperCheck(bool b)
        {
            if(b)
            {
                if(m_effect.Base == null)
                {
                    if(m_effect.Direction == null)
                        m_effect.Direction = new VehicleEffectsDefinition.Vector();
                    if(m_effect.Position == null)
                        m_effect.Position = new VehicleEffectsDefinition.Vector();

                    string effectName = m_effect.Replacment ?? m_effect.Name;
                    m_effect.Base = effectName;
                    if(m_effect.Replacment != null)
                    {
                        m_effect.Replacment = "Vehicle Effect Wrapper";
                    }
                    else
                    {
                        m_effect.Name = "Vehicle Effect Wrapper";
                    }

                    // Update fields
                    m_minSpeed.SetValue(m_effect.MinSpeed);
                    m_maxSpeed.SetValue(m_effect.MaxSpeed);
                    m_posX.SetValue(m_effect.Position.X);
                    m_posY.SetValue(m_effect.Position.Y);
                    m_posZ.SetValue(m_effect.Position.Z);
                    m_dirX.SetValue(m_effect.Direction.X);
                    m_dirY.SetValue(m_effect.Direction.Y);
                    m_dirZ.SetValue(m_effect.Direction.Z);
                }
            }
            else
            {
                if(m_effect.Base != null)
                {
                    if(m_effect.Replacment != null)
                    {
                        m_effect.Replacment = m_effect.Base;
                    }
                    else
                    {
                        m_effect.Name = m_effect.Base;
                    }
                    m_effect.Base = null;
                    m_effect.Position = null;
                    m_effect.Direction = null;
                }
            }
            m_posPanel.isVisible = b;
            m_dirPanel.isVisible = b;
        }

        public void Display(VehicleEffectsDefinition.Effect effect)
        {
            if(m_buttonFlagsForbidden == null)
                return;

            m_effect = effect;

            m_buttonFlagsRequired.tooltip = m_effect.RequiredFlags.ToString();
            m_buttonFlagsForbidden.tooltip = m_effect.ForbiddenFlags.ToString();

            string effectName = m_effect.Base ?? m_effect.Replacment ?? m_effect.Name;
            bool usesWrapper = m_effect.Base != null;

            m_wrapperCheckbox.isChecked = usesWrapper;

            // Update fields
            if(usesWrapper)
            {
                m_minSpeed.SetValue(m_effect.MinSpeed);
                m_maxSpeed.SetValue(m_effect.MaxSpeed);
                m_posX.SetValue(m_effect.Position.X);
                m_posY.SetValue(m_effect.Position.Y);
                m_posZ.SetValue(m_effect.Position.Z);
                m_dirX.SetValue(m_effect.Direction.X);
                m_dirY.SetValue(m_effect.Direction.Y);
                m_dirZ.SetValue(m_effect.Direction.Z);
            }

            m_posPanel.isVisible = usesWrapper;
            m_dirPanel.isVisible = usesWrapper;


            if(m_effect.Replacment != null)
            {
                m_replacementField.text = m_effect.Name;
            }
            else
            {
                m_replacementField.text = "";
            }

            if(m_effect.Fallback != null)
            {
                m_fallbackField.text = m_effect.Fallback;
            }
            else
            {
                m_fallbackField.text = "";
            }

            m_nameLabel.text = effectName;
        }
    }
}
