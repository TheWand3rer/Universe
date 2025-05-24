#region

using UnitsNet;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    public class StellarData : PhysicalData
    {
        public Duration Age { get; private set; }
        public Luminosity Luminosity { get; private set; }
        public Temperature Temperature { get; private set; }

        public StellarData(
            Luminosity luminosity, Mass mass = default, Acceleration gravity = default, Length radius = default, Temperature temperature = default,
            Duration age = default, Density density = default) : base(mass, radius, gravity, density)
        {
            Luminosity  = luminosity;
            Temperature = temperature;
            Age         = age;
        }
    }
}