using System;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

namespace VindemiatrixCollective.Universe.CelestialMechanics.Manoeuvres
{
    public readonly struct TransferData
    {
        public DateTime Launch { get; }
        public DateTime Arrival { get; }

        public OrbitState TransferOrbit(int impulses = 0)
        {
            return Initial.ApplyManoeuvre(Manoeuvre, impulses);
        }

        public Manoeuvre Manoeuvre { get; }

        public OrbitState Initial { get; }
        public OrbitState Final { get; }

        public TransferData(DateTime launch, DateTime arrival, Manoeuvre manoeuvre, OrbitState initialState, OrbitState finalState)
        {
            Launch = launch;
            Arrival = arrival;
            Manoeuvre = manoeuvre;
            Initial = initialState;
            Final = finalState;
        }
    }
}