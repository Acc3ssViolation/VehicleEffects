using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleEffects
{
    class SoundEffectsLoader : ConfigLoader
    {
        private HashSet<string> vehicleEffectsDefParseErrors;

        public override string FileName
        {
            get
            {
                return "SoundEffects.xml";
            }
        }

        public SoundEffectsLoader(HashSet<string> vehicleEffectsDefParseErrors)
        {
            this.vehicleEffectsDefParseErrors = vehicleEffectsDefParseErrors;
        }

        public override void OnFileFound(string path, string name, bool isMod)
        {
        }

        public override void Prepare()
        {
            vehicleEffectsDefParseErrors.Clear();
        }
    }
}
