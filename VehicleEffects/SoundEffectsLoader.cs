using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace VehicleEffects
{
    class SoundEffectsLoader : ConfigLoader
    {
        private HashSet<string> vehicleEffectsDefParseErrors;
        private CustomSoundsManager manager;

        public override string FileName
        {
            get
            {
                return "SoundEffects.xml";
            }
        }

        public SoundEffectsLoader(HashSet<string> vehicleEffectsDefParseErrors, CustomSoundsManager manager)
        {
            this.vehicleEffectsDefParseErrors = vehicleEffectsDefParseErrors;
            this.manager = manager;
        }

        public override void OnFileFound(string path, string name, bool isMod)
        {
            SoundEffectsDefinition soundDef = null;

            var xmlSerializer = new XmlSerializer(typeof(SoundEffectsDefinition));
            try
            {
                using(var streamReader = new System.IO.StreamReader(path))
                {
                    soundDef = xmlSerializer.Deserialize(streamReader) as SoundEffectsDefinition;
                }
            }
            catch(Exception e)
            {
                Logging.LogException(e);
                vehicleEffectsDefParseErrors.Add(name + " - " + e.Message);
                return;
            }

            // We can create the effects during loading. This also means that they will be ready when the
            // vehicle definitions get parsed because that only happens after all files have been loaded
            foreach(var effect in soundDef.Effects)
            {
                effect.SoundFile = Path.Combine(Path.GetDirectoryName(path), effect.SoundFile);
                var sound = effect.CreateEffect();
                if(sound == null)
                {
                    vehicleEffectsDefParseErrors.Add(name + " - Unable to create sound effect, see output_log.txt for more details.");
                    continue;
                }

                if(!manager.AddEffect(sound))
                {
                    if(isMod)
                    {
                        // Only show duplicate effects included with mods as errors to allow authors to include the same effect with multiple assets
                        // Also, mods are loaded first so a mod containing the same effect as an asset will always take priority
                        vehicleEffectsDefParseErrors.Add(name + " - Duplicate sound effect name, ignoring effect.");
                    }
                    Logging.LogWarning("Duplicate Custom Sound Effect " + sound.name + " found and ignored from " + name);
                    GameObject.Destroy(sound.gameObject);
                    continue;
                }

                Logging.Log("Loaded Custom Sound Effect " + sound.name + " from " + name);
            }
        }

        public override void Prepare()
        {
            vehicleEffectsDefParseErrors.Clear();
        }
    }
}
