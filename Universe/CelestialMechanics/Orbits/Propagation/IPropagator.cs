#region

using UnitsNet;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics.Orbits.Propagation
{
    public interface IPropagator
    {
        (Angle nu, Angle E, Angle M) PropagateOrbit(OrbitState state, Duration tof);
    }
}