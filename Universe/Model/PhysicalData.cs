// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective

#region using

using System;
using UnitsNet;
using VindemiatrixCollective.Universe.CelestialMechanics;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    public class PhysicalData
    {
        public Acceleration Gravity { get; private set; }
        public Density Density { get; private set; }
        public Length Radius { get; private set; }
        public Mass Mass { get; private set; }
        public Temperature Temperature { get; private set; }

        public PhysicalData(Mass mass, Length radius, Acceleration gravity, Density density = default, Temperature temperature = default)
        {
            Mass        = mass;
            Radius      = radius;
            Gravity     = gravity;
            Density     = density;
            Temperature = temperature;
        }

        public PhysicalData(Density density, Length radius, GravitationalParameter gm, Temperature temperature = default) : this(
            Mass.FromKilograms(4 / 3d * UniversalConstants.Tri.Pi * Math.Pow(radius.Meters, 3) * density.KilogramsPerCubicMeter), radius,
            Acceleration.FromMetersPerSecondSquared(gm.M3S2 / Math.Pow(radius.Meters, 2)), density, temperature) { }

        internal static PhysicalData Null => new(Mass.Zero, Length.Zero, Acceleration.Zero);
    }
}