#region

using UnitsNet;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.Model;

#endregion

namespace VindemiatrixCollective.Universe
{
    public interface IAttractor
    {
        GravitationalParameter Mu { get; }
        Mass Mass { get; }


        StarSystem StarSystem { get; }


        string Name { get; }
    }
}