// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using System.Collections.Generic;
using UnitsNet;
using UnityEngine.Assertions;
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

        public static (Length a1, Length a2) CalculateSemiMajorAxes(Mass m1, Mass m2, Length a)
        {
            Assert.IsTrue(m1.SolarMasses > 0 && m2.SolarMasses > 0, $"Invalid m1: {m1.SolarMasses:f2}, m2:{m2.SolarMasses:f2}");

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