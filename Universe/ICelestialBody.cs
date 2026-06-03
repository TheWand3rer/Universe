// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using System.Collections.Generic;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

#endregion

namespace VindemiatrixCollective.Universe
{
    public interface ICelestialBody : IOrbiter
    {
        IEnumerable<ICelestialBody> Orbiters { get; }
        int OrbiterCount { get; }
        PhysicalData PhysicalData { get; }
        GravitationalParameter Mu { get; }
    }

    public interface IOrbiter : ITreeNode
    {
        OrbitalData OrbitalData { get; }
        OrbitState OrbitState { get; }
    }

    public interface IName
    {
        string Name { get; }
    }
}