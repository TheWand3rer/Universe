// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System.Collections.Generic;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

#endregion

namespace VindemiatrixCollective.Universe
{
    public interface ICelestialBody : IOrbiter
    {
        IEnumerable<CelestialBody> Orbiters { get; }
        int OrbiterCount { get; }
        PhysicalData PhysicalData { get; }
    }

    public interface IOrbiter : ITreeNode
    {
        OrbitalData OrbitalData { get; }
        OrbitState OrbitState { get; }
    }
}