using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace VehicleEffects
{
    class VehicleEffectsLoader : ConfigLoader
    {
        private List<VehicleEffectsDefinition> loadedDefinitions;
        private Dictionary<VehicleEffectsDefinition, string> definitionPackages;
        private HashSet<string> vehicleEffectsDefParseErrors;

        public override string FileName
        {
            get
            {
                return "VehicleEffectsDefinition.xml";
            }
        }

        public VehicleEffectsLoader(List<VehicleEffectsDefinition> loadedDefinitions,
            Dictionary<VehicleEffectsDefinition, string> definitionPackages,
            HashSet<string> vehicleEffectsDefParseErrors)
        {
            this.vehicleEffectsDefParseErrors = vehicleEffectsDefParseErrors;
            this.loadedDefinitions = loadedDefinitions;
            this.definitionPackages = definitionPackages;
        }

        public override void OnFileFound(string path, string name, bool isMod)
        {
            VehicleEffectsDefinition vehicleEffectsDef = null;

            var xmlSerializer = new XmlSerializer(typeof(VehicleEffectsDefinition));
            try
            {
                using(var streamReader = new System.IO.StreamReader(path))
                {
                    vehicleEffectsDef = xmlSerializer.Deserialize(streamReader) as VehicleEffectsDefinition;
                }
            }
            catch(Exception e)
            {
                Logging.LogException(e);
                vehicleEffectsDefParseErrors.Add(name + " - " + e.Message);
                return;
            }

            // Add config to loaded list
            vehicleEffectsDef.LoadedFromMod = isMod;
            loadedDefinitions.Add(vehicleEffectsDef);
            definitionPackages.Add(vehicleEffectsDef, name);
        }

        public override void Prepare()
        {
            loadedDefinitions.Clear();
            definitionPackages.Clear();
            vehicleEffectsDefParseErrors.Clear();
        }
    }
}
