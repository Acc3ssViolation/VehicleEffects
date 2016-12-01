using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace VehicleEffects.Editor
{
    public class UIAddPanel : UIPanel
    {
        public const int WIDTH = 500;
        public const int HEIGHT = 400;

        public override void Start()
        {
            base.Start();
            UIView view = UIView.GetAView();

            name = "Effects Editor New Item Panel";
            backgroundSprite = "MenuPanel2";
            width = WIDTH;
            height = HEIGHT;
            canFocus = true;
            isInteractive = true;
            isVisible = false;

            relativePosition = new Vector3(Mathf.Floor((view.fixedWidth - width) / 2), Mathf.Floor((view.fixedHeight - height) / 2));

            CreateComponents();
        }

        private void CreateComponents()
        {
            int headerHeight = 40;
            UIHelperBase uiHelper = new UIHelper(this);

            // Label
            UILabel label = AddUIComponent<UILabel>();
            label.text = "Add new effect definition";
            label.relativePosition = new Vector3(WIDTH / 2 - label.width / 2, 10);

            // Drag handle
            UIDragHandle handle = AddUIComponent<UIDragHandle>();
            handle.target = this;
            handle.constrainToScreen = true;
            handle.width = WIDTH;
            handle.height = headerHeight;
            handle.relativePosition = Vector3.zero;

            // Buttons
            UIButton confirmButton = UIUtils.CreateButton(this);
            confirmButton.text = "Add";
            confirmButton.relativePosition = new Vector3(WIDTH / 2 - confirmButton.width - 10, HEIGHT - confirmButton.height - 10);
            confirmButton.eventClicked += (c, p) =>
            {
                OnConfirm();
            };
            UIButton cancelButton = UIUtils.CreateButton(this);
            cancelButton.text = "Cancel";
            cancelButton.relativePosition = new Vector3(WIDTH / 2 + 10, HEIGHT - cancelButton.height - 10);
            cancelButton.eventClicked += (c, p) =>
            {
                isVisible = false;
            };


        }

        private void OnConfirm()
        {
            isVisible = false;
        }
    }
}
