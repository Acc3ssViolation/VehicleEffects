using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ExtendedAssetEditor.UI.Effects
{
    public class UIVehicleEffectRow : UIPanel, IUIFastListRow
    {
        public struct EffectData
        {
            public VehicleInfo.Effect m_effect;
            [DefaultValue(-1)]
            public int m_index;
            public bool m_showButtons;
        }

        public const int HEIGHT = 40;

        private EffectData m_data;

        private UILabel m_nameLabel;
        private UIPanel m_background;
        private UIButton m_removeButton;

        public UIPanel background
        {
            get
            {
                if(m_background == null)
                {
                    m_background = AddUIComponent<UIPanel>();
                    m_background.width = width;
                    m_background.height = HEIGHT;
                    m_background.relativePosition = Vector2.zero;

                    m_background.zOrder = 0;
                }

                return m_background;
            }
        }

        public override void Start()
        {
            base.Start();

            width = parent.width;
            height = HEIGHT;
            m_removeButton.relativePosition = new Vector3(width - 40, 5);

            if(m_nameLabel == null)
                CreateComponents();
        }

        public void Deselect(bool isRowOdd)
        {
            if(m_nameLabel == null)
                return;

            m_nameLabel.textColor = new Color32(255, 255, 255, 255);
            if(isRowOdd)
            {
                background.backgroundSprite = "ListItemHover";
                background.color = new Color32(0, 0, 0, 128);
            }
            else
            {
                background.backgroundSprite = null;
            }
        }

        public void Select(bool isRowOdd)
        {
            if(m_nameLabel == null)
                return;

            m_nameLabel.textColor = new Color32(255, 255, 255, 255);

            background.backgroundSprite = "ListItemHighlight";
            background.color = new Color32(255, 255, 255, 255);
        }

        public void Display(object data, bool isRowOdd)
        {
            if(!(data is EffectData))
                return;

            m_data = (EffectData)data;

            if(m_nameLabel == null)
                CreateComponents();

            m_removeButton.isVisible = m_data.m_showButtons;

            if(m_data.m_effect.m_effect == null)
            {
                m_nameLabel.textColor = Color.red;
                m_nameLabel.text = "ERROR: Missing effect!";
            }
            else
            {
                m_nameLabel.textColor = Color.white;
                m_nameLabel.text = m_data.m_effect.m_effect.name;
                var le = m_data.m_effect.m_effect as LightEffect;
                if(le != null && le.m_positionIndex >= 0)
                {
                    m_nameLabel.text += " (Light index " + le.m_positionIndex + ")";
                }
            }
            m_nameLabel.tooltip = m_nameLabel.text;


            if(isRowOdd)
            {
                background.backgroundSprite = "ListItemHover";
                background.color = new Color32(0, 0, 0, 128);
            }
            else
            {
                background.backgroundSprite = null;
            }
        }

        private void CreateComponents()
        {
            m_nameLabel = AddUIComponent<UILabel>();
            m_nameLabel.relativePosition = new Vector3(10, 10);

            UIButton closeButton = UIUtils.CreateButton(this);
            closeButton.size = new Vector2(30, 30);
            closeButton.normalBgSprite = "buttonclose";
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.pressedBgSprite = "buttonclosepressed";
            closeButton.relativePosition = new Vector3(width - 40, 5);
            closeButton.eventClicked += (c, p) => {
                UIEffectPanel.main.RemoveEffect(m_data.m_index);
            };

            m_removeButton = closeButton;
        }
    }
}
