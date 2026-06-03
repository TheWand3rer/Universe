// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitsNet;
using UnitsNet.Units;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Extensions;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics.Manoeuvres
{
    public readonly struct Impulse
    {
        public Duration DeltaTime { get; }
        public Vector3d DeltaVelocity { get; }

        public Impulse(Duration deltaTime, Vector3d deltaVelocity)
        {
            DeltaTime     = deltaTime;
            DeltaVelocity = deltaVelocity;
        }

        public override string ToString() => $"dt: {DeltaTime.Days} d dV: {DeltaVelocity} m/s";
    }

    public class Manoeuvre : IEnumerable<Impulse>
    {
        private readonly List<Impulse> impulses;

        public Impulse[] Impulses => impulses.ToArray();

        public Manoeuvre(params Impulse[] impulses)
        {
            this.impulses = new List<Impulse>(impulses);
        }

        public Manoeuvre(IEnumerable<Impulse> impulses)
        {
            this.impulses = new List<Impulse>(impulses);
        }

        public Duration ComputeTotalDuration()
        {
            return impulses.Sum(i => i.DeltaTime, DurationUnit.Second);
        }

        public IEnumerator<Impulse> GetEnumerator() => impulses.GetEnumerator();

        public Speed ComputeTotalCost()
        {
            double dV = impulses.Sum(i => i.DeltaVelocity.magnitude);
            return Speed.FromMetersPerSecond(dV);
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            foreach (Impulse i in impulses)
            {
                sb.AppendLine(i.ToString());
            }

            return sb.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static Manoeuvre Lambert(OrbitState initialState, OrbitState finalState, ILambertSolver solver, GravitationalParameter mu)
        {
            // Get initial algorithm conditions
            Vector3d r0 = initialState.LocalPosition.FromMetresToKm();
            Vector3d r1 = finalState.LocalPosition.FromMetresToKm();

            Duration tof = Duration.FromSeconds((finalState.Epoch - initialState.Epoch).TotalSeconds);

            if (tof.Seconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialState),
                                                      "Epoch of initial orbit greater than epoch of final orbit, causing a negative time of flight");
            }

            (Vector3d deltaV_a, Vector3d deltaV_b) = solver.Lambert(mu, r0, r1, tof);

            return new Manoeuvre(new Impulse(Duration.Zero, deltaV_a.FromKmToMetres() - initialState.LocalVelocity),
                                 new Impulse(tof, finalState.LocalVelocity - deltaV_b.FromKmToMetres()));
        }
    }
}