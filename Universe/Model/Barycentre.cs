// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System.Collections.Generic;
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
        public StarSystem StarSystem { get; }
        public string Name => $"{nameof(Barycentre)} {StarSystem.Name}";

        public Barycentre(StarSystem system, Mass mass)
        {
            StarSystem  = system;
            Mass        = mass;
            OrbitalData = OrbitalData.Empty;
            OrbitState  = new OrbitState();
        }

        public Barycentre(StarSystem system) : this(system, system.Mass) { }

        public (Length a1, Length a2) CalculateSemiMajorAxes(Star primary, Star companion, Length a)
        {
            Mass   m1 = primary.PhysicalData.Mass;
            Mass   m2 = companion.PhysicalData.Mass;
            Length a1 = m2 / (m1 + m2) * a;
            Length a2 = m1 / (m1 + m2) * a;
            return (a1, a2);
        }

        #region IOrbiter

        public IEnumerable<ITreeNode> Children => StarSystem.Orbiters;

        ITreeNode ITreeNode.this[string name] => StarSystem[name];

        public ITreeNode Parent => null;

        #endregion
    }
}