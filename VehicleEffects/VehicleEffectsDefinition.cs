using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace VehicleEffects
{
    public class VehicleEffectsDefinition
    {
        public List<Vehicle> Vehicles { get; set; }

        public VehicleEffectsDefinition()
        {
            Vehicles = new List<Vehicle>();
        }

        public class Vehicle
        {
            [XmlAttribute("name"), DefaultValue(null)]
            public string Name { get; set; }

            public List<Effect> Effects { get; set; }

            public Vehicle()
            {
                Effects = new List<Effect>();
            }
        }

        public class Effect
        {
            /// <summary>
            /// Name of effect to add or replace
            /// </summary>
            [XmlAttribute("name"), DefaultValue(null)]
            public string Name { get; set; }

            /// <summary>
            /// Name of effect to replace with (optional)
            /// </summary>
            [XmlAttribute("replacement"), DefaultValue(null)]
            public string Replacment { get; set; }

            /// <summary>
            /// Base effect for wrappers etc. (optional)
            /// </summary>
            [XmlAttribute("base"), DefaultValue(null)]
            public string Base { get; set; }

            public Vector Position { get; set; }
            public Vector Direction { get; set; }

            public int MinSpeed { get; set; }

            public int MaxSpeed { get; set; }

            public Effect()
            {
                // Set some default values
                MinSpeed = 0;
                MaxSpeed = 10000;
            }
        }

        public class Vector
        {
            [XmlAttribute("x"), DefaultValue(0f)]
            public float X { get; set; }

            [XmlAttribute("y"), DefaultValue(0f)]
            public float Y { get; set; }

            [XmlAttribute("z"), DefaultValue(0f)]
            public float Z { get; set; }

            public Vector3 ToUnityVector()
            {
                return new Vector3(X, Y, Z);
            }
        }
    }
}
