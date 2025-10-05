// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using UnitsNet;
using VindemiatrixCollective.Universe.Model;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics.Orbits
{
    public interface IAttractor : IOrbiter
    {
        GravitationalParameter Mu { get; }
        Mass Mass { get; }

        StarSystem StarSystem { get; }
    }
}