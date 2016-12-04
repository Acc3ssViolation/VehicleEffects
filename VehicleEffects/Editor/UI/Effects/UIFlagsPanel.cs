using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ExtendedAssetEditor.UI.Effects
{
    public class UIFlagsPanel : UIPanel
    {
        public const int WIDTH = 500;
        public const int HEIGHT = 550;

        public Dictionary<UICheckBox, Vehicle.Flags> m_boxFlagDict = new Dictionary<UICheckBox, Vehicle.Flags>();
        public Dictionary<Vehicle.Flags, UICheckBox> m_flagBoxDict = new Dictionary<Vehicle.Flags, UICheckBox>();

        public Dictionary<UICheckBox, VehicleParked.Flags> m_boxFlagDictAlt = new Dictionary<UICheckBox, VehicleParked.Flags>();
        public Dictionary<VehicleParked.Flags, UICheckBox> m_flagBoxDictAlt = new Dictionary<VehicleParked.Flags, UICheckBox>();

        private UILabel m_label;
        private UIPanel m_flagsPanel;
        private UIPanel m_parkedFlagsPanel;

        public delegate void OnFlagsSet(Vehicle.Flags flags);
        public delegate void OnParkedFlagsSet(VehicleParked.Flags flags);

        private OnFlagsSet m_callback1;
        private OnParkedFlagsSet m_callback2;

        public override void Start()
        {
            base.Start();

            UIView view = UIView.GetAView();
            width = WIDTH;
            height = HEIGHT;
            backgroundSprite = "MenuPanel2";
            name = Mod.name + " Flags Panel";
            canFocus = true;
            isInteractive = true;
            isVisible = false;
            relativePosition = new Vector3(Mathf.FloorToInt((view.fixedWidth - width) / 2), Mathf.FloorToInt((view.fixedHeight - height) / 2));

            CreateComponents();
        }

        public void Show(string title, Vehicle.Flags checkedFlags, OnFlagsSet callback)
        {
            m_callback1 = callback;
            m_callback2 = null;
            m_label.text = title;

            m_flagsPanel.isVisible = true;
            m_parkedFlagsPanel.isVisible = false;

            var flags = (Vehicle.Flags[])Enum.GetValues(typeof(Vehicle.Flags));
            for(int i = 0; i < flags.Length; i++)
            {
                if((checkedFlags & flags[i]) > 0)
                {
                    Debug.Log("True");
                    m_flagBoxDict[flags[i]].isChecked = true;
                }
                else
                {
                    Debug.Log("False");
                    m_flagBoxDict[flags[i]].isChecked = false;
                }
            }
            
            isVisible = true;
        }

        public void Show(string title, VehicleParked.Flags checkedFlags, OnParkedFlagsSet callback)
        {
            m_callback1 = null;
            m_callback2 = callback;
            m_label.text = title;

            m_flagsPanel.isVisible = false;
            m_parkedFlagsPanel.isVisible = true;

            var flags = (VehicleParked.Flags[])Enum.GetValues(typeof(VehicleParked.Flags));
            for(int i = 0; i < flags.Length; i++)
            {
                if((checkedFlags & flags[i]) > 0)
                {
                    m_flagBoxDictAlt[flags[i]].isChecked = true;
                }
                else
                {
                    m_flagBoxDictAlt[flags[i]].isChecked = false;
                }
            }

            isVisible = true;
            m_label.relativePosition = new Vector3(WIDTH / 2 - m_label.width / 2, 10);
        }

        private void CreateComponents()
        {
            m_label = AddUIComponent<UILabel>();
            m_label.text = "Flag options";
            m_label.relativePosition = new Vector3(WIDTH / 2 - m_label.width / 2, 10);

            // Drag handle
            UIDragHandle handle = AddUIComponent<UIDragHandle>();
            handle.target = this;
            handle.constrainToScreen = true;
            handle.width = WIDTH;
            handle.height = 40;
            handle.relativePosition = Vector3.zero;

            m_flagsPanel = CreateFlagCheckboxes(new Vector3(10, handle.height + 10), 250, HEIGHT - 100, m_boxFlagDict, m_flagBoxDict);
            m_parkedFlagsPanel = CreateFlagCheckboxes(new Vector3(10, handle.height + 10), 250, HEIGHT - 100, m_boxFlagDictAlt, m_flagBoxDictAlt);

            // Buttons
            UIButton confirmButton = UIUtils.CreateButton(this);
            confirmButton.text = "Done";
            confirmButton.relativePosition = new Vector3(WIDTH / 2 - confirmButton.width - 10, HEIGHT - confirmButton.height - 10);
            confirmButton.eventClicked += (c, p) =>
            {
                Done();
            };

            UIButton cancelButton = UIUtils.CreateButton(this);
            cancelButton.text = "Cancel";
            cancelButton.relativePosition = new Vector3(WIDTH / 2 + 10, HEIGHT - cancelButton.height - 10);
            cancelButton.eventClicked += (c, p) =>
            {
                isVisible = false;
            };
        }

        private void Done()
        {
            if(m_callback1 != null)
            {
                Vehicle.Flags flags = 0;
                foreach(var v in m_boxFlagDict)
                {
                    if(v.Key.isChecked)
                    {
                        flags |= v.Value;
                    }
                }
                Debug.LogWarning(flags.ToString());
                m_callback1.Invoke(flags);
            }
            else if(m_callback2 != null)
            {
                VehicleParked.Flags flags = 0;
                foreach(var v in m_boxFlagDictAlt)
                {
                    if(v.Key.isChecked)
                    {
                        flags |= v.Value;
                    }
                }
                Debug.LogWarning(flags.ToString());
                m_callback2.Invoke(flags);
            }
            isVisible = false;
        }

        private UIPanel CreateFlagCheckboxes(Vector3 startLocation, float halfWidth, float maxHeight, Dictionary<UICheckBox, Vehicle.Flags> checkboxDict, Dictionary<Vehicle.Flags, UICheckBox> flagDict)
        {
            UIPanel panel = AddUIComponent<UIPanel>();
            panel.width = halfWidth;
            panel.relativePosition = startLocation;

            float y = 0;
            float x = 0;

            var flags = (Vehicle.Flags[])Enum.GetValues(typeof(Vehicle.Flags));
            for(int i = 0; i < flags.Length; i++)
            {
                UICheckBox checkbox = UIUtils.CreateCheckBox(panel);
                checkbox.text = flags[i].ToString();
                checkbox.isChecked = false;
                checkbox.width = halfWidth - 10;
                checkbox.relativePosition = new Vector3(x, y);
                y += checkbox.height + 5;
 
                if(y >= maxHeight + checkbox.height)
                {
                    y = 0;
                    x += halfWidth;
                    panel.width += halfWidth;
                }

                flagDict.Add(flags[i], checkbox);
                checkboxDict.Add(checkbox, flags[i]);
            }

            panel.height = y;
            return panel;
        }

        private UIPanel CreateFlagCheckboxes(Vector3 startLocation, float halfWidth, float maxHeight, Dictionary<UICheckBox, VehicleParked.Flags> checkboxDict, Dictionary<VehicleParked.Flags, UICheckBox> flagDict)
        {
            UIPanel panel = AddUIComponent<UIPanel>();
            panel.width = halfWidth;
            panel.relativePosition = startLocation;

            float y = 0;
            float x = 0;

            var flags = (VehicleParked.Flags[])Enum.GetValues(typeof(VehicleParked.Flags));
            for(int i = 0; i < flags.Length; i++)
            {
                UICheckBox checkbox = UIUtils.CreateCheckBox(panel);
                checkbox.text = flags[i].ToString();
                checkbox.isChecked = false;
                checkbox.width = halfWidth - 10;
                checkbox.relativePosition = new Vector3(x, y);
                y += checkbox.height + 5;

                if(y >= maxHeight + checkbox.height)
                {
                    y = 0;
                    x += halfWidth;
                    panel.width += halfWidth;
                }

                flagDict.Add(flags[i], checkbox);
                checkboxDict.Add(checkbox, flags[i]);
            }

            panel.height = y;
            return panel;
        }
    }
}
