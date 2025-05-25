#region

using System.Collections.Generic;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

#endregion

namespace VindemiatrixCollective.Universe
{
    public interface ICelestialBody : IAttractor
    {
        IEnumerable<CelestialBody> Orbiters { get; }
        int OrbiterCount { get; }
        OrbitalData OrbitalData { get; }
        OrbitState OrbitState { get; }
        PhysicalData PhysicalData { get; }
    }
}