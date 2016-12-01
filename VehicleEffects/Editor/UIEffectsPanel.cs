using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects.Editor
{
    public class UIEffectsPanel : UIPanel
    {
        public const int WIDTH = 500;
        public const int HEIGHT = 400;

        private UIAddPanel m_addPanel;
        private UIButton m_previewButton;

        private VehicleEffectsDefinition m_editDefinition;
        private EffectPreviewer m_previewer;

        public new void Hide()
        {
            isVisible = false;
            m_addPanel.isVisible = false;
        }

        public new void Show()
        {
            isVisible = true;
        }

        public override void Start()
        {
            base.Start();

            UIView view = UIView.GetAView();

            name = "Effects Editor Panel";
            backgroundSprite = "MenuPanel2";
            width = WIDTH;
            height = HEIGHT;
            canFocus = true;
            isInteractive = true;
            isVisible = false;

            // Create panel for adding new items
            m_addPanel = new GameObject().AddComponent<UIAddPanel>();
            m_addPanel.transform.SetParent(transform.parent);

            // Create previewer
            m_previewer = new EffectPreviewer();

            // Start in the center
            relativePosition = new Vector3(Mathf.Floor((view.fixedWidth - width) / 2), Mathf.Floor((view.fixedHeight - height) / 2));

            PrefabWatcher.instance.prefabChanged += () => 
            {
                ToolController properties = Singleton<ToolManager>.instance.m_properties;
                if(properties != null && properties.m_editPrefabInfo != null)
                {
                    if(typeof(VehicleInfo) == properties.m_editPrefabInfo.GetType())
                    {
                        OnVehicleChanged();
                    }
                }
            };
            PrefabWatcher.instance.trailersChanged += (s) =>
            {
                ToolController properties = Singleton<ToolManager>.instance.m_properties;
                if(properties != null && properties.m_editPrefabInfo != null)
                {
                    if(typeof(VehicleInfo) == properties.m_editPrefabInfo.GetType())
                    {
                        OnTrailersChanged();
                    }
                }
            };

            ClearDefinition();
            CreateComponents();
        }

        private void OnVehicleChanged()
        {
            if(m_previewer.IsPreviewing)
            {
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Vehicle Effects", "Asset was changed with effect preview on.", false);
            }
            m_previewer.ForceClear();
            ClearDefinition();

            /*var info = Singleton<ToolManager>.instance.m_properties.m_editPrefabInfo as VehicleInfo;
            var vehicleDef = new VehicleEffectsDefinition.Vehicle();
            vehicleDef.Name = info.name;        //Fullname including package
            m_editDefinition.Vehicles.Add(vehicleDef);
            vehicleDef = new VehicleEffectsDefinition.Vehicle();
            vehicleDef.Name = info.name;        //Fullname including package
            vehicleDef.ApplyToTrailersOnly = true;
            m_editDefinition.Vehicles.Add(vehicleDef);*/
        }

        private void OnTrailersChanged()
        {
            if(m_previewer.IsPreviewing)
            {
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Vehicle Effects", "Trailer composition was changed with effect preview on.", false);
            }
            m_previewer.ForceClear();
            ClearDefinition();

            /*var info = Singleton<ToolManager>.instance.m_properties.m_editPrefabInfo as VehicleInfo;
            if(info.m_trailers != null)
            {
                // See which existing definitions we can keep
                var newVehicles = new List<VehicleEffectsDefinition.Vehicle>();
                var trailerNames = new HashSet<string>();
                foreach(var vehicleDef in m_editDefinition.Vehicles)
                {
                    for(int i = 0; i < info.m_trailers.Length; i++)
                    {
                        if(vehicleDef.Name == info.m_trailers[i].m_info.name)
                        {
                            newVehicles.Add(vehicleDef);
                            trailerNames.Add(info.m_trailers[i].m_info.name);
                            break;
                        }
                    }
                    if(vehicleDef.Name == info.name)
                    {
                        newVehicles.Add(vehicleDef);
                    }
                }

                foreach(var trailer in info.m_trailers)
                {
                    if(!trailerNames.Contains(trailer.m_info.name))
                    {
                        var vehicleDef = new VehicleEffectsDefinition.Vehicle();
                        vehicleDef.Name = trailer.m_info.name;
                        newVehicles.Add(vehicleDef);
                        trailerNames.Add(trailer.m_info.name);
                    }
                }

                foreach(var d in newVehicles)
                {
                    Debug.Log(d.Name + " - ATTO: " + (d.ApplyToTrailersOnly ? "true" : "false") + " - LENGTH: " + d.Effects.Count);
                }

                m_editDefinition.Vehicles = newVehicles;
            }*/
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if(m_addPanel != null)
            {
                GameObject.Destroy(m_addPanel);
            }
        }

        private void CreateComponents()
        {
            int headerHeight = 40;
            UIHelperBase uiHelper = new UIHelper(this);

            // Label
            UILabel label = AddUIComponent<UILabel>();
            label.text = "Vehicle Effects";
            label.relativePosition = new Vector3(WIDTH / 2 - label.width / 2, 10);

            // Drag handle
            UIDragHandle handle = AddUIComponent<UIDragHandle>();
            handle.target = this;
            handle.constrainToScreen = true;
            handle.width = WIDTH;
            handle.height = headerHeight;
            handle.relativePosition = Vector3.zero;

            UIButton addButton = UIUtils.CreateButton(this);
            addButton.text = "Add item";
            addButton.relativePosition = new Vector3(10, HEIGHT - addButton.height - 10);
            addButton.eventClicked += (c, b) =>
            {
                m_addPanel.isVisible = true;
                m_addPanel.forceZOrder = 2;
            };
        }

        private void ClearDefinition()
        {
            m_editDefinition = new VehicleEffectsDefinition();
        }
    }
}
