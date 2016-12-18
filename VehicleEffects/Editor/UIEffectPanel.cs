using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects.Editor
{
    /// <summary>
    /// Panel to edit effects with.
    /// </summary>
    public class UIEffectPanel : UIPanel
    {
        public const int WIDTH = 950;
        public const int HEIGHT = 550;
        private const string ALL_TRAILER_POSTFIX = " (All Trailers Only)";

        private bool m_wasVisible = false;

        private UIDropDown m_vehicleDropdown;
        private UIEffectListPanel m_effectListPanel;
        private UISaveDefPanel m_savePanel;
        private UILoadDefPanel m_loadPanel;
        private UIEffectOptionsPanel m_optionsPanel;
        private UIFastList m_veEffectList;
        private UIButton m_addEffectButton;
        private UIButton m_saveDefinitionButton;
        private UIButton m_loadDefinitionButton;
        private UIButton m_previewButton;

        public UIFlagsPanel m_flagsPanel { get; private set; }

        private Dictionary<string, EffectInfo> m_effectDict = new Dictionary<string, EffectInfo>();
        private string m_vehicle;
        private string[] m_vehicles;
        public VehicleEffectsDefinition m_definition;

        private EffectPreviewer m_previewer;

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
            m_effectListPanel.isVisible = false;
        }

        public override void Start()
        {
            base.Start();

            UIView view = UIView.GetAView();
            width = WIDTH;
            height = HEIGHT;
            backgroundSprite = "MenuPanel2";
            name = VehicleEffectsMod.name + " Effects Panel";
            canFocus = true;
            isInteractive = true;
            isVisible = false;
            // Start in the center of the screen
            relativePosition = new Vector3(Mathf.FloorToInt((view.fixedWidth - width) / 2), Mathf.FloorToInt((view.fixedHeight - height) / 2));

            // Create add panel
            m_effectListPanel = new GameObject().AddComponent<UIEffectListPanel>();
            m_effectListPanel.gameObject.transform.SetParent(transform.parent);
            m_effectListPanel.m_mainPanel = this;

            // Create save panel
            m_savePanel = new GameObject().AddComponent<UISaveDefPanel>();
            m_savePanel.gameObject.transform.SetParent(transform.parent);

            // Create load panel
            m_loadPanel = new GameObject().AddComponent<UILoadDefPanel>();
            m_loadPanel.gameObject.transform.SetParent(transform.parent);

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

            m_previewer = new EffectPreviewer();

            CompileEffectsList();
            CreateComponents();
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
            m_vehicleDropdown.width = 400;
            m_vehicleDropdown.relativePosition = new Vector3(10, headerHeight + 10);
            m_vehicleDropdown.eventSelectedIndexChanged += OnDropdownIndexChanged;
            m_vehicleDropdown.selectedIndex = -1;

            float listHeight = HEIGHT - headerHeight - m_vehicleDropdown.height - 30 - 50;
            float listWidth = 300;
            float padding = 10;
            float optionsWidth = WIDTH - (listWidth - 3 * padding);
            float listTop = headerHeight + 20 + m_vehicleDropdown.height;

            // Fastlist for Vehicle Effects definitions
            m_veEffectList = UIFastList.Create<UIEffectDefinitionRow>(this);
            m_veEffectList.backgroundSprite = "UnlockingPanel";
            m_veEffectList.width = listWidth;
            m_veEffectList.height = listHeight;
            m_veEffectList.relativePosition = new Vector3(padding, listTop);
            m_veEffectList.canSelect = true;
            m_veEffectList.eventSelectedIndexChanged += OnVEEffectSelectionChanged;

            // Create options panel
            m_optionsPanel = AddUIComponent<UIEffectOptionsPanel>();
            m_optionsPanel.width = optionsWidth;
            m_optionsPanel.height = listHeight;
            m_optionsPanel.relativePosition = new Vector3(listWidth + padding * 2, listTop);
            m_optionsPanel.m_mainPanel = this;
            m_optionsPanel.CreateComponents();

            float footerX = 10;

            // Button to add effects (footer)
            m_addEffectButton = UIUtils.CreateButton(this);
            m_addEffectButton.text = "Add effect";
            m_addEffectButton.width = 120;
            m_addEffectButton.relativePosition = new Vector3(footerX, HEIGHT - 40);
            m_addEffectButton.eventClicked += (c, b) =>
            {
                if(!m_effectListPanel.isVisible)
                {
                    m_effectListPanel.Show((info) => {
                        VehicleEffectsDefinition.Effect effect = new VehicleEffectsDefinition.Effect();
                        effect.Name = info.name;
                        AddEffect(effect);
                    });
                }
            };
            footerX += m_addEffectButton.width + 10;

            // Button to save definition (footer)
            m_saveDefinitionButton = UIUtils.CreateButton(this);
            m_saveDefinitionButton.text = "Save XML";
            m_saveDefinitionButton.width = 120;
            m_saveDefinitionButton.relativePosition = new Vector3(footerX, HEIGHT - 40);
            m_saveDefinitionButton.eventClicked += (c, b) =>
            {
                if(!m_savePanel.isVisible)
                {
                    m_savePanel.Show(Util.GetPackageName(m_vehicles[0]), GetCleanedDefinition());
                }
            };
            footerX += m_saveDefinitionButton.width + 10;

            // Button to load definition (footer)
            m_loadDefinitionButton = UIUtils.CreateButton(this);
            m_loadDefinitionButton.text = "Load XML";
            m_loadDefinitionButton.width = 120;
            m_loadDefinitionButton.relativePosition = new Vector3(footerX, HEIGHT - 40);
            m_loadDefinitionButton.eventClicked += (c, b) =>
            {
                if(!m_loadPanel.isVisible)
                {
                    m_loadPanel.Show((definition) => {
                        if(definition != null)
                        {
                            m_definition = definition;
                            AddMissingSceneVehicles();
                            PopulateVEList();
                        }
                    });
                }
            };
            footerX += m_loadDefinitionButton.width + 10;

            // Button to preview definition (footer)
            m_previewButton = UIUtils.CreateButton(this);
            m_previewButton.text = "Enable Preview";
            m_previewButton.width = 180;
            m_previewButton.relativePosition = new Vector3(WIDTH - 10 - m_previewButton.width, HEIGHT - 40);
            m_previewButton.eventClicked += (c, b) =>
            {
                if(m_previewer.IsPreviewing)
                {
                    m_previewButton.text = "Enable Preview";
                    m_previewer.RevertPreview();
                }
                else
                {
                    if(m_previewer.ApplyPreview(GetCleanedDefinition(), "Vehicle Effects Previewer"))
                    {
                        m_previewButton.text = "Disable Preview";
                    }
                }
            };
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
                    HashSet<string> vehicles = new HashSet<string>();
                    vehicles.Add(vehicleInfo.name);
                    vehicles.Add(vehicleInfo.name + ALL_TRAILER_POSTFIX);


                    if(vehicleInfo.m_trailers != null)
                    {
                        for(int i = 0; i < vehicleInfo.m_trailers.Length; i++)
                        {
                            if(!vehicles.Contains(vehicleInfo.m_trailers[i].m_info.name))
                            {
                                vehicles.Add(vehicleInfo.m_trailers[i].m_info.name);
                            }
                        }
                    }
                    m_vehicles = vehicles.ToArray();

                    // Update dropdown
                    m_vehicleDropdown.selectedIndex = -1;
                    var items = new string[m_vehicles.Length];
                    for(int i = 0; i < m_vehicles.Length; i++)
                    {
                        items[i] = m_vehicles[i];
                    }
                    m_vehicleDropdown.items = items;

                    AddMissingSceneVehicles();

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

        private void AddMissingSceneVehicles()
        {
            if(m_definition == null)
            {
                m_definition = new VehicleEffectsDefinition();
            }

            // Loop trough scene vehicles
            for(int i = 0; i < m_vehicles.Length; i++)
            {
                if(FindVehicleDefinition(m_vehicles[i]) == null)
                {
                    var vehicleDef = new VehicleEffectsDefinition.Vehicle();
                    Logging.Log("Adding definition for " + m_vehicles[i]);
                    if(i == 1)
                    {
                        // Special case for global trailer option
                        vehicleDef.Name = m_vehicles[0];
                        vehicleDef.ApplyToTrailersOnly = true;
                    }
                    else
                    {
                        vehicleDef.Name = m_vehicles[i];
                    }
                    m_definition.Vehicles.Add(vehicleDef);
                }
            }
        }

        private VehicleEffectsDefinition.Vehicle FindVehicleDefinition(string name)
        {
            if(m_definition != null)
            {
                bool trailersOnly = false;
                if(name.Contains(ALL_TRAILER_POSTFIX))
                {
                    trailersOnly = true;
                    name = name.Substring(0, name.Length - ALL_TRAILER_POSTFIX.Length);
                }
                foreach(var vehicleDef in m_definition.Vehicles)
                {
                    if(vehicleDef.Name == name && vehicleDef.ApplyToTrailersOnly == trailersOnly)
                        return vehicleDef;
                }
            }
            return null;
        }

        private VehicleEffectsDefinition GetCleanedDefinition()
        {
            if(m_definition == null)
            {
                m_definition = new VehicleEffectsDefinition();
            }

            var cleanedDef = m_definition.Copy();

            for(int i = cleanedDef.Vehicles.Count - 1; i >= 0; i--)
            {
                Logging.LogWarning(i + " " + cleanedDef.Vehicles[i]);
                string name = cleanedDef.Vehicles[i].ApplyToTrailersOnly ? cleanedDef.Vehicles[i].Name + ALL_TRAILER_POSTFIX : cleanedDef.Vehicles[i].Name;
                bool inScene = false;
                foreach(var vehicle in m_vehicles)
                {
                    if(name == vehicle)
                    {
                        inScene = true;
                        break;
                    }
                }
                if(!inScene || cleanedDef.Vehicles[i].Effects.Count < 1)
                {
                    Logging.Log("Removing definition for " + cleanedDef.Vehicles[i].Name + "\nInScene: " + inScene.ToString());
                    cleanedDef.Vehicles.RemoveAt(i);
                }
            }

            return cleanedDef;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if(m_effectListPanel != null)
            {
                GameObject.Destroy(m_effectListPanel.gameObject);
            }
            if(m_savePanel != null)
            {
                GameObject.Destroy(m_savePanel.gameObject);
            }
            if(m_loadPanel != null)
            {
                GameObject.Destroy(m_loadPanel.gameObject);
            }
            if(m_flagsPanel != null)
            {
                GameObject.Destroy(m_flagsPanel.gameObject);
            }
        }

        private void OnVEEffectSelectionChanged(UIComponent component, int value)
        {
            if(m_optionsPanel != null)
            {
                if(value < 0)
                {
                    m_optionsPanel.Hide();
                    return;
                }

                m_optionsPanel.isVisible = true;
                m_optionsPanel.m_allowEditing = true;
                m_optionsPanel.Display(FindVehicleDefinition(m_vehicle).Effects[value]);
            }
        }

        private void OnDropdownIndexChanged(UIComponent component, int value)
        {
            if(value < 0)
            {
                m_optionsPanel.Hide();
                return;
            }

            m_vehicleDropdown.tooltip = m_vehicleDropdown.items[value];
            m_vehicle = m_vehicles[value];
            PopulateVEList();
        }

        private void PopulateVEList()
        {
            m_veEffectList.rowsData.Clear();
            m_veEffectList.selectedIndex = -1;
            var effects = FindVehicleDefinition(m_vehicle).Effects;
            for(int i = 0; i < effects.Count; i++)
            {
                m_veEffectList.rowsData.Add(new UIEffectDefinitionRow.EffectData { m_effect = effects[i], m_showButtons = true, m_index = i });
            }

            m_veEffectList.rowHeight = UIEffectDefinitionRow.HEIGHT;
            m_veEffectList.DisplayAt(0);
            if(effects.Count > 0)
            {
                m_veEffectList.selectedIndex = 0;
            }
            else
            {
                m_optionsPanel.Hide();
            }
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

        public void AddEffect(VehicleEffectsDefinition.Effect effectDef)
        {
            if(m_vehicle != null)
            {
                VehicleEffectsDefinition.Vehicle vehicle = FindVehicleDefinition(m_vehicle);
                if(vehicle != null)
                {
                    vehicle.Effects.Add(effectDef);
                    PopulateVEList();
                }
            }
        }
       
        public void RemoveEffect(int index)
        {
            if(m_vehicle != null)
            {
                VehicleEffectsDefinition.Vehicle vehicle = FindVehicleDefinition(m_vehicle);
                if(vehicle != null)
                {
                    vehicle.Effects.RemoveAt(index);
                    PopulateVEList();
                }
            }
        }

        public EffectInfo[] GetLoadedEffects()
        {
            var effectArray = new EffectInfo[m_effectDict.Count];
            m_effectDict.Values.CopyTo(effectArray, 0);
            return effectArray;
        }
    }
}
