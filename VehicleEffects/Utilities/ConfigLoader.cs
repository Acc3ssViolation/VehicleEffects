using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace VehicleEffects
{
    public abstract class ConfigLoader
    {
        /// <summary>
        /// The filename of this type of config file.
        /// </summary>
        public abstract string FileName
        {
            get;
        }

        /// <summary>
        /// Called when a loader should prepare for a new loading run.
        /// Reset state here.
        /// </summary>
        public abstract void Prepare();

        /// <summary>
        /// Called when a file was found in a mod or asset directory.
        /// </summary>
        /// <param name="path">Full path to the found file</param>
        /// <param name="name">Name of the asset/mod</param>
        /// <param name="isMod">Indicates if it's from a mod directory</param>
        public abstract void OnFileFound(string path, string name, bool isMod);

        /// <summary>
        /// Tries to deserialize an xml file, returns null on failure.
        /// Exceptions are logged via Logging.
        /// </summary>
        /// <typeparam name="T">The type to deserialize</typeparam>
        /// <param name="path">Path of the .xml file</param>
        /// <returns>Deserialized object or null on failure</returns>
        protected T XMLDeserialize<T>(string path) where T : class
        {
            T result = null;
            var xmlSerializer = new XmlSerializer(typeof(T));
            try
            {
                using(var streamReader = new System.IO.StreamReader(path))
                {
                    result = xmlSerializer.Deserialize(streamReader) as T;
                }
            }
            catch(Exception e)
            {
                Logging.LogException(e);
            }
            return result;
        }
    }
}
