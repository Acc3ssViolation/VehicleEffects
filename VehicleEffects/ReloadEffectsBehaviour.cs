using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects
{
    class ReloadEffectsBehaviour : MonoBehaviour
    {
        VehicleEffectsMod mod;
        
        public void SetMod(VehicleEffectsMod mod)
        {
            this.mod = mod;
        }
        

        public void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.V))
                mod.ReloadVehicleEffects();
        }

    }
}
