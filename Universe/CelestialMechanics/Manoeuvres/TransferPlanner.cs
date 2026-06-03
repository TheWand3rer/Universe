// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using System;
using System.Collections.Generic;
using UnitsNet;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics.Manoeuvres
{
    public class TransferPlanner
    {
        private readonly IzzoLambertSolver solver;

        private List<TransferData> transfers;
        private readonly IAttractor attractor;
        private readonly GravitationalParameter mu;
        private OrbitState initial;
        private readonly OrbitState final;
        public ICelestialBody DepartureBody { get; }
        public ICelestialBody TargetBody { get; }
        public IEnumerable<TransferData> Transfers => transfers;

        public TransferPlanner(ICelestialBody departureBody, ICelestialBody targetBody)
        {
            DepartureBody = departureBody;
            TargetBody    = targetBody;
            attractor     = FindCommonAttractor(departureBody, targetBody);
            mu            = attractor.Mu;
            solver        = new IzzoLambertSolver();

            initial = DepartureBody.OrbitState.Clone();
            final   = TargetBody.OrbitState.Clone();

            DetermineReferenceFrame();
        }

        private void DetermineReferenceFrame()
        {
            if (DepartureBody.OrbitState.Attractor != attractor)
            {
                if (DepartureBody == attractor)
                {
                    initial = OrbitState.Circular(DepartureBody, Length.FromKilometers(100));
                }
                else
                {
                    initial.SetAttractor(attractor);
                }
            }

            if (TargetBody.OrbitState.Attractor != attractor)
            {
                final.SetAttractor(attractor);
            }
        }

        public IEnumerable<TransferData> OrderByDeltaV()
        {
            transfers.Sort((m1, m2) => m1.Manoeuvre.ComputeTotalCost().CompareTo(m2.Manoeuvre.ComputeTotalCost()));
            return transfers;
        }

        public IEnumerable<TransferData> OrderByTransferTime()
        {
            transfers.Sort((m1, m2) => m1.Manoeuvre.ComputeTotalDuration().CompareTo(m2.Manoeuvre.ComputeTotalDuration()));
            return transfers;
        }

        /// <summary>
        /// Calculate a set of transfer windows between two celestial bodies.
        /// </summary>
        /// <param name="start">The date at which the calculation should start.</param>
        /// <param name="timeWindow">The time window to use in the calculation.</param>
        /// <param name="number">The number of transfer windows to return (n*n).</param>
        public void CalculateTransferWindows(DateTime start, Duration timeWindow, int number = 20)
        {
            DateTime   launchSpanEnd = start.AddSeconds(timeWindow.Seconds);
            DateTime[] launchSpan    = OrbitalMechanics.TimeRange(start, launchSpanEnd);
            DateTime[] arrivalSpan   = OrbitalMechanics.TimeRange(launchSpanEnd, launchSpanEnd.AddSeconds(timeWindow.Seconds));

            transfers = new List<TransferData>(number);

            for (int i = 0; i < number; i++)
            for (int j = 0; j < number; j++)
            {
                DateTime launch  = launchSpan[i];
                DateTime arrival = arrivalSpan[j];
                CalculateTransfer(launch, arrival);
            }
        }

        /// <summary>
        /// Returns the minimum transfer time between the two celestial bodies.
        /// </summary>
        /// <returns>The duration of the transfer time.</returns>
        public Duration EstimateParameters()
        {
            Length a1 = initial.SemiMajorAxis;
            Length a2 = final.SemiMajorAxis;

            Length   a = (a1 + a2) / 2;
            Duration T = OrbitalMechanics.CalculatePeriod(a, attractor.Mu);

            return T / 2;
        }

        private void CalculateTransfer(DateTime launch, DateTime arrival)
        {
            if (DepartureBody == null)
            {
                throw new InvalidOperationException($"{nameof(DepartureBody)} cannot be null");
            }

            if (TargetBody == null)
            {
                throw new InvalidOperationException($"{nameof(TargetBody)} cannot be null");
            }

            if (launch > arrival)
            {
                throw new ArgumentException("Launch date cannot be after arrival date");
            }

            OrbitState orbitDeparture = initial.PropagateAsNew(launch);
            OrbitState orbitArrival   = final.PropagateAsNew(arrival);

            Duration tof = Duration.FromSeconds((orbitArrival.Epoch - orbitDeparture.Epoch).TotalSeconds);
            if (tof.Seconds <= 0)
            {
                return;
            }

            try
            {
                Manoeuvre    m     = Manoeuvre.Lambert(orbitDeparture, orbitArrival, solver, mu);
                TransferData tData = new(launch, arrival, m, orbitDeparture, orbitArrival);

                transfers.Add(tData);
            }
            catch (Exception ex) { }
        }

        private static IAttractor FindCommonAttractor(ICelestialBody origin, ICelestialBody destination)
        {
            if (origin.OrbitState.Attractor == destination.OrbitState.Attractor)
                return origin.OrbitState.Attractor;

            return (IAttractor)Tree.FindCommonAncestor(origin, destination);
        }
    }
}