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
        [CreateProperty] public Temperature Temperature { get; private set; }

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