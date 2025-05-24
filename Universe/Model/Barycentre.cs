#region

using UnitsNet;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    public class Barycentre : IAttractor
    {
        public GravitationalParameter Mu => GravitationalParameter.FromMass(Mass);
        public Mass Mass { get; }
        public OrbitalData OrbitalData { get; }
        public OrbitState OrbitState { get; }
        public string Name { get; }

        public Barycentre(string name, Mass systemMass, OrbitalData orbitalData)
        {
            Name        = name;
            Mass        = systemMass;
            OrbitalData = orbitalData;
            OrbitState  = OrbitState.FromOrbitalElements(OrbitalData);
        }

        public (Length a1, Length a2) CalculateSemiMajorAxes(Star primary, Star companion)
        {
            Mass   m1 = primary.PhysicalData.Mass;
            Mass   m2 = companion.PhysicalData.Mass;
            Length a  = OrbitalData.SemiMajorAxis;
            Length a1 = m2 / (m1 + m2) * a;
            Length a2 = m1 / (m1 + m2) * a;
            return (a1, a2);
        }
    }
}