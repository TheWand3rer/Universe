using System;
using UnitsNet;
using Unity.Properties;
using VindemiatrixCollective.Universe.CelestialMechanics;

namespace VindemiatrixCollective.Universe.Model
{
    public class PhysicalData
    {
        [CreateProperty]
        public Acceleration Gravity { get; private set; }

        [CreateProperty]
        public Density Density { get; private set; }

        [CreateProperty]
        public Length Radius { get; private set; }

        [CreateProperty]
        public Mass Mass { get; private set; }


        public static PhysicalData Create(Mass mass, Length radius, Acceleration gravity, Density density)
        {
            PhysicalData data = new()
            {
                Mass = mass,
                Radius = radius,
                Gravity = gravity,
                Density = density
            };
            return data;
        }

        public static PhysicalData Create(Density density, Length radius, GravitationalParameter gm)
        {
            Mass         mass    = Mass.FromKilograms((4 / 3d) * UniversalConstants.Tri.Pi * Math.Pow(radius.Meters, 3) * density.GramsPerCubicMeter);
            Acceleration gravity = Acceleration.FromMetersPerSecondSquared(gm.M3S2 / Math.Pow(radius.Meters, 2));
            return Create(mass, radius, gravity, density);
        }
    }
}