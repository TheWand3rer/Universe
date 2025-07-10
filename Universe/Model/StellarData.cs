#region

using UnitsNet;
using Unity.Properties;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    public class StellarData : PhysicalData
    {
        [CreateProperty] public Duration Age { get; private set; }
        [CreateProperty] public Luminosity Luminosity { get; private set; }

        public StellarData(
            Luminosity luminosity, Mass mass, Length radius = default, Acceleration gravity = default, Temperature temperature = default,
            Density density = default, Duration age = default) : base(mass, radius, gravity, density, temperature)
        {
            Luminosity = luminosity;
            Age = age;
        }

        internal static StellarData Null => new(Luminosity.Zero, Mass.Zero);

        private static Acceleration FromMassRadius(Mass m, Length r)
        {
            double G = UniversalConstants.Celestial.GravitationalConstant;
            double g = G * m.Kilograms / (r.Meters * r.Meters);

            return Acceleration.FromMetersPerSecondSquared(g);
        }
    }
}