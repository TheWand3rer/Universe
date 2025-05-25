#region

using UnitsNet;
using VindemiatrixCollective.Universe.CelestialMechanics;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    public class Barycentre : IAttractor
    {
        public GravitationalParameter Mu => GravitationalParameter.FromMass(Mass);
        public Mass Mass => StarSystem.Mass;
        public StarSystem StarSystem { get; }
        public string Name => $"{nameof(Barycentre)} {StarSystem.Name}";

        public Barycentre(StarSystem system)
        {
            StarSystem = system;
        }

        public (Length a1, Length a2) CalculateSemiMajorAxes(Star primary, Star companion, Length a)
        {
            Mass   m1 = primary.PhysicalData.Mass;
            Mass   m2 = companion.PhysicalData.Mass;
            Length a1 = m2 / (m1 + m2) * a;
            Length a2 = m1 / (m1 + m2) * a;
            return (a1, a2);
        }
    }
}