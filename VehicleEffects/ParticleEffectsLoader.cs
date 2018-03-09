using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace VehicleEffects
{
    class ParticleEffectsLoader : ConfigLoader
    {
        private HashSet<string> vehicleEffectsDefParseErrors;
        private CustomParticlesManager manager;

        public override string FileName
        {
            get
            {
                return "ParticleEffects.xml";
            }
        }

        public ParticleEffectsLoader(HashSet<string> vehicleEffectsDefParseErrors, CustomParticlesManager manager)
        {
            this.vehicleEffectsDefParseErrors = vehicleEffectsDefParseErrors;
            this.manager = manager;
        }

        public override void OnFileFound(string path, string name, bool isMod)
        {
            ParticleEffectsDefinition particleDef = null;

            var xmlSerializer = new XmlSerializer(typeof(ParticleEffectsDefinition));
            try
            {
                using(var streamReader = new System.IO.StreamReader(path))
                {
                    particleDef = xmlSerializer.Deserialize(streamReader) as ParticleEffectsDefinition;
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
            foreach(var effect in particleDef.Effects)
            {
                var particles = effect.CreateEffect();
                if(particles == null)
                {
                    vehicleEffectsDefParseErrors.Add(name + " - Unable to create particle effect, see output_log.txt for more details.");
                    continue;
                }

                if(!manager.AddEffect(particles))
                {
                    if(isMod)
                    {
                        // Only show duplicate effects included with mods as errors to allow authors to include the same effect with multiple assets
                        // Also, mods are loaded first so a mod containing the same effect as an asset will always take priority
                        vehicleEffectsDefParseErrors.Add(name + " - Duplicate particle effect name, ignoring effect.");
                    }
                    Logging.LogWarning("Duplicate Custom Particle Effect " + particles.name + " found and ignored from " + name);
                    GameObject.Destroy(particles.gameObject);
                    continue;
                }

                Logging.Log("Loaded Custom Particle Effect " + particles.name + " from " + name);
            }
        }

        public override void Prepare()
        {
            vehicleEffectsDefParseErrors.Clear();
        }
    }
}
