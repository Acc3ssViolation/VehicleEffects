using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ExtendedAssetEditor.UI.Effects
{
    public class UIEffectRow : UIPanel, IUIFastListRow
    {
        public struct EffectData
        {
            public EffectInfo m_info;
            public bool m_showButtons;
        }

        public const int HEIGHT = 40;

        private EffectData m_data;

        private UILabel m_nameLabel;
        private UIPanel m_background;

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

            m_nameLabel.text = m_data.m_info.name;
            var le = m_data.m_info as LightEffect;
            if(le != null && le.m_positionIndex >= 0)
            {
                m_nameLabel.text += " (Light index " + le.m_positionIndex + ")";
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
        }
    }
}
