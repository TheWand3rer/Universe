#region

using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

#endregion

namespace VindemiatrixCollective.Universe
{
    public interface ICelestialBody : IAttractor
    {
        OrbitState OrbitState { get; }
        PhysicalData PhysicalData { get; }
        string FullName { get; }
    }
}