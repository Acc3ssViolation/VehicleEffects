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
    public class UILoadDefPanel : UIPanel
    {
        private UITextField m_textField;
        public delegate void OnLoadFinished(VehicleEffectsDefinition definition);

        public const int WIDTH = 800;
        public const int HEIGHT = 450;
        private OnLoadFinished m_callback;

        public override void Start()
        {
            base.Start();

            UIView view = UIView.GetAView();
            width = WIDTH;
            height = HEIGHT;
            backgroundSprite = "MenuPanel2";
            name = VehicleEffectsMod.name + " Load Definition Panel";
            canFocus = true;
            isInteractive = true;
            isVisible = false;
            relativePosition = new Vector3(Mathf.FloorToInt((view.fixedWidth - width) / 2), Mathf.FloorToInt((view.fixedHeight - height) / 2));

            CreateComponents();
        }

        private void CreateComponents()
        {
            UILabel label = AddUIComponent<UILabel>();
            label.text = "Load XML";
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
            label.text = "Paste xml here:";

            m_textField = UIUtils.CreateTextField(this);
            m_textField.multiline = true;
            m_textField.horizontalAlignment = UIHorizontalAlignment.Left;
            m_textField.maxLength = int.MaxValue;
            m_textField.height = HEIGHT - 110; 
            m_textField.width = WIDTH - 20;

            label.relativePosition = new Vector3((WIDTH - label.width) / 2, 45);
            m_textField.relativePosition = new Vector3(10, 60);
            m_textField.text = "";

            m_textField.tooltip = "Paste the contents of VehicleEffectsDefinition.xml here and press Load";

            // Buttons
            UIButton confirmButton = UIUtils.CreateButton(this);
            confirmButton.text = "Load";
            confirmButton.relativePosition = new Vector3(WIDTH / 2 - confirmButton.width - 10, HEIGHT - confirmButton.height - 10);
            confirmButton.eventClicked += (c, p) =>
            {
                OnLoad();
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
            m_callback?.Invoke(null);

            isVisible = false;
            m_callback = null;
        }

        public void Show(OnLoadFinished callback = null)
        {
            Show(true);
            m_callback = callback;
        }

        void OnLoad()
        {
            VehicleEffectsDefinition definition = null;
            try
            {
                var textReader = new StringReader(m_textField.text.Trim());
                var xmlSerializer = new XmlSerializer(typeof(VehicleEffectsDefinition));
                definition = (VehicleEffectsDefinition)xmlSerializer.Deserialize(textReader);
            }
            catch(Exception e)
            {
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Error parsing definition", e.Message + "\r\n" + e.StackTrace, true);
                Logging.LogException(e);
                return;
            }

            m_callback?.Invoke(definition);
            m_callback = null;
            Hide();
        }
    }
}
