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

            // VehicleEffectWrapper params
            public Vector Position { get; set; }
            public Vector Direction { get; set; }

            public float MinSpeed { get; set; }

            public float MaxSpeed { get; set; }

            public Flags RequiredFlags {get;set;}
            public Flags ForbiddenFlags { get; set; }

            //MultiEffect params
            public List<SubEffect> SubEffects { get; set; }
            public float Duration;

            public Effect()
            {
                // Set some default values
                MinSpeed = 0;
                MaxSpeed = 10000;

                ForbiddenFlags = 0;
                RequiredFlags = Flags.Created;
            }

            // Exact copy of Vehicle.Flags needed due to namespace problems with Vehicle and Vehicle...
            [Flags]
            public enum Flags
            {
                LeftHandDrive = int.MinValue,
                All = -1,
                Created = 1,
                Deleted = 2,
                Spawned = 4,
                Inverted = 8,
                TransferToTarget = 16,
                TransferToSource = 32,
                Emergency1 = 64,
                Emergency2 = 128,
                WaitingPath = 256,
                Stopped = 512,
                Leaving = 1024,
                Arriving = 2048,
                Reversed = 4096,
                TakingOff = 8192,
                Flying = 16384,
                Landing = 32768,
                WaitingSpace = 65536,
                WaitingCargo = 131072,
                GoingBack = 262144,
                WaitingTarget = 524288,
                Importing = 1048576,
                Exporting = 2097152,
                Parking = 4194304,
                CustomName = 8388608,
                OnGravel = 16777216,
                WaitingLoading = 33554432,
                Congestion = 67108864,
                DummyTraffic = 134217728,
                Underground = 268435456,
                Transition = 536870912,
                InsideBuilding = 1073741824
            }
        }

        public class SubEffect
        {
            public Effect Effect { get; set; }

            public float StartTime { get; set; }
            public float EndTime { get; set; }
            public float Probability { get; set; }

            public SubEffect()
            {
                Probability = 1.0f;
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
