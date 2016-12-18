using ColossalFramework.Globalization;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects.Editor
{
    public class UIEffectListPanel : UIPanel
    {
        public const int WIDTH = 360;
        public const int HEIGHT = 500;

        private UILabel m_label;
        private UIFastList m_effectList;
        private UITextField m_searchField;
        public UIEffectPanel m_mainPanel;

        public delegate void OnEffectSelected(EffectInfo info);
        private OnEffectSelected m_delegate;

        public static UIEffectListPanel main { get; private set; }

        public override void Start()
        {
            base.Start();

            if(main == null)
                main = this;

            UIView view = UIView.GetAView();
            width = WIDTH;
            height = HEIGHT;
            backgroundSprite = "MenuPanel2";
            name = VehicleEffectsMod.name + " Add Effects Panel";
            canFocus = true;
            isInteractive = true;
            isVisible = false;
            relativePosition = new Vector3(Mathf.FloorToInt((view.fixedWidth - width) / 2), Mathf.FloorToInt((view.fixedHeight - height) / 2));

            CreateComponents();
            PopulateList();
        }


        public void Show(OnEffectSelected callback)
        {
            m_delegate = callback;
            Show(true);
        }

        public new void Hide()
        {
            m_delegate = null;
            base.Hide();
        }


        private void CreateComponents()
        {
            m_label = AddUIComponent<UILabel>();
            m_label.text = "Select an effect";
            m_label.relativePosition = new Vector3(WIDTH / 2 - m_label.width / 2, 10);

            // Drag handle
            UIDragHandle handle = AddUIComponent<UIDragHandle>();
            handle.target = this;
            handle.constrainToScreen = true;
            handle.width = WIDTH;
            handle.height = 40;
            handle.relativePosition = Vector3.zero;

            // Name field
            UILabel label = AddUIComponent<UILabel>();
            label.text = "Name:";

            m_searchField = UIUtils.CreateTextField(this);
            m_searchField.width = 250;

            label.relativePosition = new Vector3((WIDTH - (label.width + m_searchField.width + 10)) / 2, 55);
            m_searchField.relativePosition = new Vector3((WIDTH - m_searchField.width + label.width + 10) / 2, 50);
            m_searchField.text = "";

            m_searchField.eventTextChanged += (c, s) => {
                PopulateList();
            };
            m_searchField.tooltip = "Search for an effect by name";

            // Buttons
            UIButton confirmButton = UIUtils.CreateButton(this);
            confirmButton.text = "Select";
            confirmButton.relativePosition = new Vector3(WIDTH / 2 - confirmButton.width - 10, HEIGHT - confirmButton.height - 10);
            confirmButton.eventClicked += (c, p) =>
            {
                SelectEffect();
            };

            UIButton cancelButton = UIUtils.CreateButton(this);
            cancelButton.text = "Cancel";
            cancelButton.relativePosition = new Vector3(WIDTH / 2 + 10, HEIGHT - cancelButton.height - 10);
            cancelButton.eventClicked += (c, p) =>
            {
                isVisible = false;
            };

            // Effect list
            m_effectList = UIFastList.Create<UIEffectRow>(this);
            m_effectList.backgroundSprite = "UnlockingPanel";
            m_effectList.relativePosition = new Vector3(10, 90);
            m_effectList.height = HEIGHT - 90 - 10 - cancelButton.height;
            m_effectList.width = WIDTH - 20;
            m_effectList.canSelect = true;
        }

        private void PopulateList()
        {
            m_effectList.rowsData.Clear();
            m_effectList.selectedIndex = -1;
            var effects = m_mainPanel.GetLoadedEffects();
            for(int i = 0; i < effects.Length; i++)
            {
                if(effects[i] != null &&
                    (String.IsNullOrEmpty(m_searchField.text.Trim()) || effects[i].name.ToLower().Contains(m_searchField.text.Trim().ToLower())))
                {
                    m_effectList.rowsData.Add(new UIEffectRow.EffectData { m_info = effects[i], m_showButtons = false });
                }
            }

            m_effectList.rowHeight = 40f;
            m_effectList.DisplayAt(0);
            m_effectList.selectedIndex = 0;
        }

        private void SelectEffect()
        {
            var selectedData = m_effectList.selectedItem;
            m_delegate?.Invoke(((UIEffectRow.EffectData)selectedData).m_info);
            Hide();
        }
    }
}
