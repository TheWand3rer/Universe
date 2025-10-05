// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

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