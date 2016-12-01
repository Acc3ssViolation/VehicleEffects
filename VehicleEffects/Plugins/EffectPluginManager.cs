using ICities;
using System.Collections.Generic;

namespace VehicleEffects.Plugins
{
    /// <summary>
    /// Ensures that vehicles are not updated until all custom effects from other mods (plugins) are loaded.
    /// This is only needed if your effects are created in OnLevelLoaded.
    /// 
    /// To correctly register your plugin do the following:
    ///     In your loading extension's OnCreated make a call to RegisterPlugin and keep the returned ID.
    ///     When you have finished creating effects in OnLevelLoaded call FinishedLoadingPlugin and pass the ID you got earlier.
    /// 
    /// IMPORTANT: In case the effect creation fails, call DeregisterPlugin with your ID! VE will not apply effect definitions if a plugin hasn't loaded!
    /// If this happens, check output_log.txt for the last entry that starts with "Plugin Manager - Still waiting for:" for a list of plugins that failed to indicate that they have loaded.
    /// </summary>
    public class EffectPluginManager : LoadingExtensionBase
    {
        private class PluginData
        {
            public string name;
            public bool created;
        }

        private static Dictionary<int, PluginData> m_registeredPlugins = new Dictionary<int, PluginData>();
        private static System.Random m_random = new System.Random();
        private static int m_loadedPlugins = 0;

        public delegate void OnAllPluginsFinished();

        /// <summary>
        /// Gets triggered when all plugins are loaded, if at least 1 plugin finished loading.
        /// </summary>
        public static event OnAllPluginsFinished eventAllPluginsFinished;

        /// <summary>
        /// Registers a plugin and returns the plugin ID.
        /// </summary>
        /// <param name="pluginName">Name to identify the plugin (does not need to be unique)</param>
        /// <returns>Plugin ID</returns>
        public static int RegisterPlugin(string pluginName)
        {
            int id;
            do
            {
                id = m_random.Next();
            } while(m_registeredPlugins.ContainsKey(id));

            m_registeredPlugins.Add(id, new PluginData
            {
                name = pluginName,
                created = false
            });

            Logging.Log("Plugin Manager - Registered plugin " + pluginName + " at ID " + id);

            return id;
        }

        /// <summary>
        /// Deregisters the plugin with id.
        /// </summary>
        /// <param name="id">The id of the plugin to deregister</param>
        public static void DeregisterPlugin(int id)
        {
            if(m_registeredPlugins.Remove(id))
            {
                Logging.Log("Plugin Manager - Removed plugin with id " + id);

                bool allLoaded = true;
                foreach(var plugin in m_registeredPlugins.Values)
                {
                    if(plugin.created == false)
                    {
                        allLoaded = false;
                        break;
                    }
                }
                if(allLoaded)
                {
                    eventAllPluginsFinished?.Invoke();
                }

                return;
            }
            Logging.LogWarning("Plugin Manager - Tried to remove plugin with id " + id + " but this id doesn't exist");
        }

        /// <summary>
        /// Call this to indicate that plugin with id has finished loading (e.g. its effects are created).
        /// </summary>
        /// <param name="id">Id of plugin.</param>
        public static void FinishedLoadingPlugin(int id)
        {
            PluginData pluginData;
            if(m_registeredPlugins.TryGetValue(id, out pluginData))
            {
                pluginData.created = true;
                Logging.Log("Plugin Manager - Finished loading plugin " + pluginData.name + " with id " + id);

                m_loadedPlugins++;
                if(m_loadedPlugins == m_registeredPlugins.Count)
                {
                    Logging.Log("Plugin Manager - Finished loading all plugins");
                    eventAllPluginsFinished?.Invoke();
                }
                else
                {
                    string str = "Plugin Manager - Still waiting for:\r\n";
                    foreach(var plugin in m_registeredPlugins)
                    {
                        if(plugin.Value.created == false)
                            str += plugin.Value.name + "\r\n";
                    }
                    Logging.Log(str);
                }
            }
            else
            {
                Logging.LogWarning("Plugin Manager - Call for loading finished on plugin " + id + " but this id doesn't exist");
            }
        }

        /// <summary>
        /// Returns true if any plugins are registered.
        /// </summary>
        public static bool HasPlugins()
        {
            if(m_registeredPlugins.Count > 0)
            {
                return true;
            }
            return false;
        }

        public override void OnLevelUnloading()
        {
            m_registeredPlugins.Clear();
            m_loadedPlugins = 0;
        }
    }
}
