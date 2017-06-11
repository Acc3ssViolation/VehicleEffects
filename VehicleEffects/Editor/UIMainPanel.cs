using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace VehicleEffects.Editor
{
    public class UIMainPanel : UIPanel
    {
        private UIButton m_toggleButton;
        private UIEffectPanel m_effectsPanel;
        private bool m_showWarning;

        public void SetEditorWarning(bool enabled)
        {
            m_showWarning = enabled;
        }

        public override void Start()
        {
            base.Start();

            name = "Vehicle Effects Main Panel";
            width = 0;
            height = 0;
            isVisible = false;

            PrefabWatcher.instance.prefabWasVehicle += () =>
            {
                isVisible = false;
                m_effectsPanel.Hide();
            };
            PrefabWatcher.instance.prefabBecameVehicle += () =>
            {
                isVisible = true;
            };

            UIView view = UIView.GetAView();
            relativePosition = Vector3.zero;

            m_effectsPanel = new GameObject().AddComponent<UIEffectPanel>();
            m_effectsPanel.transform.SetParent(transform);

            m_toggleButton = UIUtils.CreateButton(this);
            m_toggleButton.text = "Vehicle Effects";
            m_toggleButton.width = 120;
            m_toggleButton.tooltip = "Opens the Vehicle Effects editor";
            m_toggleButton.relativePosition = new Vector3(10, 10);
            m_toggleButton.eventClicked += (c, b) =>
            {
                if(m_showWarning)
                {
                    m_showWarning = false;
                    UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Vehicle Effects", "Do not save, load or swap an asset (or trailer) while the effect preview is enabled. The asset will be left modified which can lead to unexpected results.\r\nThis warning can be disabled in the mod options menu.", false);
                }
                if(m_effectsPanel.isVisible)
                {
                    m_effectsPanel.Hide();
                }
                else
                {
                    m_effectsPanel.Show();
                }
            };
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if(m_effectsPanel != null)
            {
                GameObject.Destroy(m_effectsPanel.gameObject);
            }
        }
    }
}
