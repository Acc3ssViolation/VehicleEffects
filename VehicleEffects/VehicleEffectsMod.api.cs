using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleEffects
{
    /*
	 * This file contains the public plugin api code for Vehicle Effects. It would have been better to just put it in another class to begin
	 * with but I didn't so this will have to do as far as organizing things goes.
	 */
    public partial class VehicleEffectsMod
    {

        public delegate void OnVehicleUpdate();
        public static event OnVehicleUpdate eventVehicleUpdateStart;
        public static event OnVehicleUpdate eventVehicleUpdateFinished;

		private void OnVehicleUpdateStart()
        {
            try
            {
                eventVehicleUpdateStart?.Invoke();
            }
            catch(Exception e)
            {
                Logging.LogWarning("Caught an exception when running a VehicleUpdateStart event on a plugin\nThe exception was:");
                Logging.LogException(e);
            }
        }

		private void OnVehicleUpdateFinished()
        {
            try
            {
                eventVehicleUpdateFinished?.Invoke();
            }
            catch(Exception e)
            {
                Logging.LogWarning("Caught an exception when running a VehicleUpdateFinished event on a plugin\nThe exception was:");
                Logging.LogException(e);
            }
        }

        public static bool RegisterEffect(string name, EffectInfo effect, bool forceOverwrite = false)
        {
            if (!effectDictionary.ContainsKey(name))
            {
                effectDictionary.Add(name, effect);
                Logging.Log($"Added effect dictionary entry for {name}");
                return true;
            }
            else
            {
                Logging.LogWarning($"Overwriting effect dictionary entry for {name}");
                effectDictionary[name] = effect;
            }
            return false;
        }

        public static void UnregisterEffect(string name)
        {
            if (effectDictionary.Remove(name))
            {
                Logging.Log($"Removed effect dictionary entry for {name}");
            }
        }
    }
}
