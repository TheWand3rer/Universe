#region

using System;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics.Manoeuvres
{
    public readonly struct TransferData
    {
        public DateTime Arrival { get; }
        public DateTime Launch { get; }

        public Manoeuvre Manoeuvre { get; }
        public OrbitState Final { get; }

        public OrbitState Initial { get; }

        public TransferData(DateTime launch, DateTime arrival, Manoeuvre manoeuvre, OrbitState initialState, OrbitState finalState)
        {
            Launch    = launch;
            Arrival   = arrival;
            Manoeuvre = manoeuvre;
            Initial   = initialState;
            Final     = finalState;
        }

        public OrbitState TransferOrbit(int impulses = 0)
        {
            return Initial.ApplyManoeuvre(Manoeuvre, impulses);
        }
    }
}