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