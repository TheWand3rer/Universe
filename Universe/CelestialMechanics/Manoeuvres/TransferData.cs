// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using System;
using UnitsNet;
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

        public Duration Duration => Duration.FromSeconds((Arrival - Launch).TotalSeconds);

        public OrbitState TransferOrbit(int impulses = 0) => Initial.ApplyManoeuvre(Manoeuvre, impulses);
    }
}