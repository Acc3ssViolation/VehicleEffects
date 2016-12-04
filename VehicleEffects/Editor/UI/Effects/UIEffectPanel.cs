using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ExtendedAssetEditor.UI.Effects
{
    /// <summary>
    /// Panel to edit effects with.
    /// </summary>
    public class UIEffectPanel : UIPanel
    {
        public const int WIDTH = 750;
        public const int HEIGHT = 500;

        private bool m_wasVisible = false;

        private UIDropDown m_vehicleDropdown;
        private UIAddEffectPanel m_addPanel;
        private UIEffectOptionsPanel m_optionsPanel;
        private UIFastList m_effectList;
        private UIButton m_addEffectButton;

        public UIFlagsPanel m_flagsPanel { get; private set; }

        private Dictionary<string, EffectInfo> m_effectDict = new Dictionary<string, EffectInfo>();
        private VehicleInfo m_vehicle;
        private VehicleInfo[] m_vehicles;

        public static UIEffectPanel main { get; private set; }


        public new void Show()
        {
            if(main == null)
                main = this;

            isVisible = true;
        }

        public new void Hide()
        {
            m_wasVisible = isVisible;
            isVisible = false;
            m_addPanel.isVisible = false;
        }

        public override void Start()
        {
            base.Start();

            UIView view = UIView.GetAView();
            width = WIDTH;
            height = HEIGHT;
            backgroundSprite = "MenuPanel2";
            name = Mod.name + " Effects Panel";
            canFocus = true;
            isInteractive = true;
            isVisible = false;
            // Start in the center of the screen
            relativePosition = new Vector3(Mathf.FloorToInt((view.fixedWidth - width) / 2), Mathf.FloorToInt((view.fixedHeight - height) / 2));

            // Create add panel
            m_addPanel = new GameObject().AddComponent<UIAddEffectPanel>();
            m_addPanel.gameObject.transform.SetParent(transform.parent);
            m_addPanel.m_mainPanel = this;

            // Create flags panel
            m_flagsPanel = new GameObject().AddComponent<UIFlagsPanel>();
            m_flagsPanel.transform.SetParent(transform.parent);

            PrefabWatcher.instance.prefabBecameVehicle += () =>
            {
                isVisible = m_wasVisible;
                PrefabWatcher.instance.prefabChanged += OnPrefabChanged;
                PrefabWatcher.instance.trailersChanged += OnTrailersChanged;
                UpdateVehicles();
            };
            PrefabWatcher.instance.prefabWasVehicle += () =>
            {
                Hide();
                PrefabWatcher.instance.prefabChanged -= OnPrefabChanged;
                PrefabWatcher.instance.trailersChanged -= OnTrailersChanged;
            };

            CompileEffectsList();
            CreateComponents();
        }

        private void OnPrefabChanged()
        {
            UpdateVehicles();
        }

        private void OnTrailersChanged(string[] names)
        {
            UpdateVehicles();
        }

        private void UpdateVehicles()
        {
            ToolController properties = Singleton<ToolManager>.instance.m_properties;
            if(properties != null)
            {
                var vehicleInfo = properties.m_editPrefabInfo as VehicleInfo;
                if(vehicleInfo != null)
                {
                    Dictionary<string, VehicleInfo> vehicles = new Dictionary<string, VehicleInfo>();
                    vehicles.Add(vehicleInfo.name, vehicleInfo);
                    if(vehicleInfo.m_trailers != null)
                    {
                        for(int i = 0; i < vehicleInfo.m_trailers.Length; i++)
                        {
                            if(!vehicles.ContainsKey(vehicleInfo.m_trailers[i].m_info.name))
                            {
                                vehicles.Add(vehicleInfo.m_trailers[i].m_info.name, vehicleInfo.m_trailers[i].m_info);
                            }
                        }
                    }
                    m_vehicles = vehicles.Values.ToArray();

                    // Update dropdown
                    m_vehicleDropdown.selectedIndex = -1;
                    var items = new string[m_vehicles.Length];
                    for(int i = 0; i < m_vehicles.Length; i++)
                    {
                        items[i] = m_vehicles[i].name;
                    }
                    m_vehicleDropdown.items = items;

                    for(int i = 0; i < m_vehicles.Length; i++)
                    {
                        if(m_vehicles[i] == m_vehicle)
                        {
                            m_vehicleDropdown.selectedIndex = i;
                            break;
                        }
                    }

                    if(m_vehicleDropdown.selectedIndex < 0)
                        m_vehicleDropdown.selectedIndex = 0;
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if(m_addPanel != null)
            {
                GameObject.Destroy(m_addPanel.gameObject);
            }
        }

        private void CreateComponents()
        {
            int headerHeight = 40;

            // Label
            UILabel label = AddUIComponent<UILabel>();
            label.text = "Effects";
            label.relativePosition = new Vector3(WIDTH / 2 - label.width / 2, 10);

            // Drag handle
            UIDragHandle handle = AddUIComponent<UIDragHandle>();
            handle.target = this;
            handle.constrainToScreen = true;
            handle.width = WIDTH;
            handle.height = headerHeight;
            handle.relativePosition = Vector3.zero;

            // close button
            UIButton closeButton = UIUtils.CreateButton(this);
            closeButton.size = new Vector2(30, 30);
            closeButton.normalBgSprite = "buttonclose";
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.pressedBgSprite = "buttonclosepressed";
            closeButton.relativePosition = new Vector3(WIDTH - 35, 5);
            closeButton.eventClicked += (c, p) => {
                isVisible = false;
            };

            // Dropdown
            m_vehicleDropdown = UIUtils.CreateDropDown(this);
            m_vehicleDropdown.width = 300;
            m_vehicleDropdown.relativePosition = new Vector3(10, headerHeight + 10);
            m_vehicleDropdown.eventSelectedIndexChanged += OnDropdownIndexChanged;
            m_vehicleDropdown.selectedIndex = -1;

            // Fastlist for effects
            m_effectList = UIFastList.Create<UIVehicleEffectRow>(this);
            m_effectList.backgroundSprite = "UnlockingPanel";
            m_effectList.width = (WIDTH - 30) / 2;
            m_effectList.height = HEIGHT - headerHeight - m_vehicleDropdown.height - 30 - 50;
            m_effectList.relativePosition = new Vector3(10, headerHeight + 20 + m_vehicleDropdown.height);
            m_effectList.canSelect = true;
            m_effectList.eventSelectedIndexChanged += OnEffectSelectionChanged;

            // Create options panel
            m_optionsPanel = AddUIComponent<UIEffectOptionsPanel>();
            m_optionsPanel.width = m_effectList.width;
            m_optionsPanel.height = m_effectList.height;
            m_optionsPanel.relativePosition = new Vector3(WIDTH / 2 + 5, headerHeight + 20 + m_vehicleDropdown.height);
            m_optionsPanel.m_mainPanel = this;
            m_optionsPanel.CreateComponents();

            // Button to add effects (footer)
            m_addEffectButton = UIUtils.CreateButton(this);
            m_addEffectButton.text = "Add effect";
            m_addEffectButton.width = 120;
            m_addEffectButton.relativePosition = new Vector3(10, HEIGHT - 40);
            m_addEffectButton.eventClicked += (c, b) =>
            {
                if(!m_addPanel.isVisible)
                    m_addPanel.isVisible = true;
            };
        }

        private void OnEffectSelectionChanged(UIComponent component, int value)
        {
            if(m_optionsPanel != null)
            {
                if(value < 0 || m_vehicle == null || m_vehicle.m_effects == null || value < 0 || value >= m_vehicle.m_effects.Length)
                {
                    m_optionsPanel.isVisible = false;
                    return;
                }

                m_optionsPanel.isVisible = true;
                m_optionsPanel.Display(m_vehicle.m_effects[value], value);
            }
        }

        private void OnDropdownIndexChanged(UIComponent component, int value)
        {
            if(value < 0)
                return;

            m_vehicle = m_vehicles[value];
            PopulateList();
        }

        private void PopulateList()
        {
            m_effectList.rowsData.Clear();
            m_effectList.selectedIndex = -1;
            var effects = m_vehicle.m_effects;
            for(int i = 0; i < effects.Length; i++)
            {
                m_effectList.rowsData.Add(new UIVehicleEffectRow.EffectData { m_effect = effects[i], m_showButtons = true, m_index = i });
            }

            m_effectList.rowHeight = UIVehicleEffectRow.HEIGHT;
            m_effectList.DisplayAt(0);
            m_effectList.selectedIndex = 0;
        }

        private void CompileEffectsList()
        {
            m_effectDict.Clear();
            EffectInfo[] infos = Resources.FindObjectsOfTypeAll<EffectInfo>();
            for(int i = 0; i < infos.Length; i++)
            {
                m_effectDict[infos[i].name] = infos[i];
            }
        }

        public void AddEffect(EffectInfo effect)
        {
            AddEffect(new VehicleInfo.Effect
            {
                m_effect = effect,
                m_parkedFlagsForbidden = VehicleParked.Flags.Created,
                m_parkedFlagsRequired = VehicleParked.Flags.None,
                m_vehicleFlagsForbidden = 0,
                m_vehicleFlagsRequired = Vehicle.Flags.Created | Vehicle.Flags.Spawned
            });
        }

        public void AddEffect(VehicleInfo.Effect effectData)
        {
            if(m_vehicle != null)
            {
                var array = new VehicleInfo.Effect[m_vehicle.m_effects.Length + 1];
                m_vehicle.m_effects.CopyTo(array, 0);
                array[m_vehicle.m_effects.Length] = effectData;
                m_vehicle.m_effects = array;
                PopulateList();
                m_effectList.DisplayAt(m_vehicle.m_effects.Length - 1);
                m_effectList.selectedIndex = m_vehicle.m_effects.Length - 1;
            }
        }

        public void ChangeEffect(VehicleInfo.Effect data, int index)
        {
            if(m_vehicle != null)
            {
                if(m_vehicle.m_effects == null || index < 0 || index >= m_vehicle.m_effects.Length)
                    return;

                m_vehicle.m_effects[index] = data;
            }
        }
       
        public void RemoveEffect(int index)
        {
            if(m_vehicle != null)
            {
                if(m_vehicle.m_effects == null || index < 0 || index >= m_vehicle.m_effects.Length)
                    return;

                /*ConfirmPanel.ShowModal(Mod.name, "Are you sure you want to remove the effect?", delegate (UIComponent comp, int ret)
                {
                    if(ret == 1)
                    {
                        
                    }
                });*/

                List<VehicleInfo.Effect> list = new List<VehicleInfo.Effect>();
                list.AddRange(m_vehicle.m_effects);
                list.RemoveAt(index);
                m_vehicle.m_effects = list.ToArray();
                PopulateList();
            }
        }

        public EffectInfo[] GetEffects()
        {
            var effectArray = new EffectInfo[m_effectDict.Count];
            m_effectDict.Values.CopyTo(effectArray, 0);
            return effectArray;
        }
    }
}
