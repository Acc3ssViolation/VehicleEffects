using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.IO;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace VehicleEffects.Editor
{
    public class UISaveDefPanel : UIPanel
    {
        private UITextField m_textField;
        private VehicleEffectsDefinition m_definition;
        public delegate void OnSaveFinished();

        public const int WIDTH = 360;
        public const int HEIGHT = 150;
        private OnSaveFinished m_callback;

        public override void Start()
        {
            base.Start();

            UIView view = UIView.GetAView();
            width = WIDTH;
            height = HEIGHT;
            backgroundSprite = "MenuPanel2";
            name = VehicleEffectsMod.name + " Save Effects Panel";
            canFocus = true;
            isInteractive = true;
            isVisible = false;
            relativePosition = new Vector3(Mathf.FloorToInt((view.fixedWidth - width) / 2), Mathf.FloorToInt((view.fixedHeight - height) / 2));

            CreateComponents();
        }

        private void CreateComponents()
        {
            UILabel label = AddUIComponent<UILabel>();
            label.text = "Save XML";
            label.relativePosition = new Vector3(WIDTH / 2 - label.width / 2, 10);

            // Drag handle
            UIDragHandle handle = AddUIComponent<UIDragHandle>();
            handle.target = this;
            handle.constrainToScreen = true;
            handle.width = WIDTH;
            handle.height = 40;
            handle.relativePosition = Vector3.zero;

            // Name field
            label = AddUIComponent<UILabel>();
            label.text = "Subfolder:";

            m_textField = UIUtils.CreateTextField(this);
            m_textField.width = 250;

            label.relativePosition = new Vector3((WIDTH - (label.width + m_textField.width + 10)) / 2, 65);
            m_textField.relativePosition = new Vector3((WIDTH - m_textField.width + label.width + 10) / 2, 60);
            m_textField.text = "";

            m_textField.tooltip = "Subfolder this will be saved to. Existing definition file will be overwritten.";

            // Buttons
            UIButton confirmButton = UIUtils.CreateButton(this);
            confirmButton.text = "Save";
            confirmButton.relativePosition = new Vector3(WIDTH / 2 - confirmButton.width - 10, HEIGHT - confirmButton.height - 10);
            confirmButton.eventClicked += (c, p) =>
            {
                OnSave();
            };

            UIButton cancelButton = UIUtils.CreateButton(this);
            cancelButton.text = "Cancel";
            cancelButton.relativePosition = new Vector3(WIDTH / 2 + 10, HEIGHT - cancelButton.height - 10);
            cancelButton.eventClicked += (c, p) =>
            {
                Hide();
            };
        }

        public new void Hide()
        {
            m_callback?.Invoke();

            isVisible = false;
            m_definition = null;
            m_callback = null;
        }

        public void Show(string packageName, VehicleEffectsDefinition definition, OnSaveFinished callback = null)
        {
            Show(true);
            m_textField.text = packageName;
            m_callback = callback;
            m_definition = definition;
        }

        void OnSave()
        {
            if(m_definition == null)
            {
                Logging.LogError("Trying to save null definition!");
                return;
            }

            try
            {
                string packageName = m_textField.text.Trim();
                string saveDir = Path.Combine(DataLocation.addonsPath, "Vehicle Effects");
                if(!String.IsNullOrEmpty(packageName))
                {
                    saveDir = Path.Combine(saveDir, packageName);
                }
                if(!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }
                string savePath = Path.Combine(saveDir, VehicleEffectsMod.filename);
                //int i = 0;
                while(File.Exists(savePath))
                {
                    File.Delete(savePath);

                    //savePath = savePath.Substring(0, savePath.Length - 3) + i + ".xml";
                    //i++;
                }

                using(StreamWriter streamWriter = new StreamWriter(savePath, false, Encoding.UTF8))
                {
                    var xmlSerializer = new XmlSerializer(typeof(VehicleEffectsDefinition));
                    xmlSerializer.Serialize(streamWriter, m_definition);
                }

                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Vehicle Effects", "Saved definition to: \r\n" + savePath, false);
            }
            catch(Exception e)
            {
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Vehicle Effects", e.Message + "\r\n" + e.StackTrace, true);
                Logging.LogException(e);
                return;
            }

            Hide();
        }
    }
}
