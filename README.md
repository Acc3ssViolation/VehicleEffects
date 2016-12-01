# Vehicle Effects
Mod for Cities: Skylines

## Plugins

If you want to make a plugin to add new effects you can use the following example:
```c#
using System;
using ICities;
using VehicleEffects;
using UnityEngine;
using ColossalFramework.UI;

namespace VehicleEffectsPack
{
    public class VehicleEffectsPackMod : LoadingExtensionBase, IUserMod
    {
        private GameObject m_effectObject;

        public string Description
        {
            get
            {
                return "Test effect pack for Vehicle Effects";
            }
        }

        public string Name
        {
            get
            {
                return "Effects Pack Test";
            }
        }

        public override void OnCreated(ILoading loading)
        {
            if(!SubscribedToVehicleEffects())
                return;

            VehicleEffectsMod.eventVehicleUpdateStart += OnVehicleUpdateStart;
            VehicleEffectsMod.eventVehicleUpdateFinished += OnVehicleUpdateFinished;
        }

        public override void OnReleased()
        {
            if(!SubscribedToVehicleEffects())
                return;

            VehicleEffectsMod.eventVehicleUpdateStart -= OnVehicleUpdateStart;
            VehicleEffectsMod.eventVehicleUpdateFinished -= OnVehicleUpdateFinished;
        }

        private void OnVehicleUpdateFinished()
        {
            // Do something fun
        }

        private void OnVehicleUpdateStart()
        {
            // Make sure that the effects are ready for use when exiting this method
            CreateEffectObjects();
        }

        private void CreateEffectObjects()
        {
            if(m_effectObject == null)
            {
                try
                {
                    // Create the effect object
                    m_effectObject = new GameObject(Name);
                    var effect = m_effectObject.AddComponent<MultiEffect>();
                    effect.m_duration = 0.0f;
                    effect.m_useSimulationTime = true;
                    effect.m_effects = new MultiEffect.SubEffect[2];
                    effect.m_effects[0].m_effect = EffectCollection.FindEffect("Train Light Left");
                    effect.m_effects[0].m_probability = 1.0f;
                    effect.m_effects[1].m_effect = EffectCollection.FindEffect("Train Light Right");
                    effect.m_effects[1].m_probability = 1.0f;
                }
                catch(Exception e)
                {
                    Debug.Log(e.Message + "\r\n" + e.StackTrace);
                }
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if(mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;

            // Display warning if player is not subscribed to Vehicle Effects
            if(!SubscribedToVehicleEffects())
            {
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(
                    "Missing dependency",
                    Name + " requires the 'Vehicle Effects' mod to work properly. Please subscribe to the mod and restart the game!",
                    false);
                return;
            }
        }

        // Checks if the player subscribed to the Vehicle Effects mod.
        private bool SubscribedToVehicleEffects()
        {
            foreach(ColossalFramework.Plugins.PluginManager.PluginInfo current in ColossalFramework.Plugins.PluginManager.instance.GetPluginsInfo())
            {
                if(current.publishedFileID.AsUInt64 == 780720853uL) return true;
            }
            return false;
        }
    }
}
```