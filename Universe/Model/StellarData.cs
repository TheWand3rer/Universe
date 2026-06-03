// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using UnitsNet;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    public class StellarData : PhysicalData
    {
        public Duration Age { get; private set; }
        public Luminosity Luminosity { get; }

        public StellarData(
            Luminosity luminosity, Mass mass, Length radius = default, Acceleration gravity = default, Temperature temperature = default,
            Density density = default, Duration age = default) : base(mass, radius, gravity, density, temperature)
        {
            Luminosity = luminosity;
            Age        = age;
        }

        public bool IsValid() => Luminosity.SolarLuminosities > 0 && Mass.SolarMasses > 0;

        internal new static StellarData Null => new(Luminosity.Zero, Mass.Zero);

        private static Acceleration FromMassRadius(Mass m, Length r)
        {
            double G = UniversalConstants.Celestial.GravitationalConstant;
            double g = G * m.Kilograms / (r.Meters * r.Meters);

            return Acceleration.FromMetersPerSecondSquared(g);
        }
    }
}