#region

using UnitsNet;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics.Orbits.Propagation
{
    public interface IPropagator
    {
        Angle PropagateOrbit(OrbitState state, Duration tof);
    }
}