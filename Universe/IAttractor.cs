#region

using UnitsNet;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

#endregion

namespace VindemiatrixCollective.Universe
{
    public interface IAttractor
    {
        GravitationalParameter Mu { get; }
        Mass Mass { get; }

        OrbitalData OrbitalData { get; }
        string Name { get; }
    }
}