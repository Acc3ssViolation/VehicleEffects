# Vehicle Effects
Mod for Cities: Skylines

# Plugins

This mod contains some custom effects. You can also create a mod to create your own custom effects.

To make sure that your effect objects are created before the vehicle effect definitions are parsed, you must use this mod's plugin system.
It will wait with parsing the effect definitions until all registered plugins are either loaded or deregistered.

There are two steps to using the plugin system:

* Register your plugin with `RegisterPlugin` and keep track of the id you get back.
* Indicate that your plugin is done loading by calling `FinishedLoadingPlugin`.

If for some reason you fail to create your effect object, call `DeregisterPlugin` to ensure Vehicle Effects will not keep waiting for you.

And always make sure to check if Vehicle Effects is even subscribed to.

Example code:
--------
```c#
using System;
using ICities;
using VehicleEffects.Plugins;
using UnityEngine;
using ColossalFramework.UI;

namespace VehicleEffectsPack1
{
    public class VehicleEffectsPackMod : LoadingExtensionBase, IUserMod
    {
        private GameObject m_effectObject;
        private int m_id;

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

            base.OnCreated(loading);

            m_id = EffectPluginManager.RegisterPlugin("Effects Pack Test");
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
                    EffectPluginManager.DeregisterPlugin(m_id);
                    Debug.Log(e.Message + "\r\n" + e.StackTrace);
                }
            }

            EffectPluginManager.FinishedLoadingPlugin(m_id);
        }

        // checks if the player subscribed to the Vehicle Effects mod
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
