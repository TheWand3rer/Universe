#region

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

        public ICelestialBody DepartureBody { get; }
        public ICelestialBody TargetBody { get; }

        public TransferPlanner(ICelestialBody departureBody, ICelestialBody targetBody)
        {
            DepartureBody = departureBody;
            TargetBody    = targetBody;
            solver        = new IzzoLambertSolver();
        }

        public void CalculateTransferWindows(DateTime start, int windowDays = 180, int number = 20)
        {
            DateTime launchSpanEnd = start.AddDays(180);
            var      launchSpan    = OrbitalMechanics.TimeRange(start, launchSpanEnd);
            var      arrivalSpan   = OrbitalMechanics.TimeRange(launchSpanEnd, launchSpanEnd.AddDays(180));

            transfers = new List<TransferData>(number);

            for (int i = 0; i < number; i++)
            for (int j = 0; j < number; j++)
            {
                DateTime launch  = launchSpan[i];
                DateTime arrival = arrivalSpan[j];
                CalculateTransfer(launch, arrival);
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

        private void CalculateTransfer(DateTime launch, DateTime arrival)
        {
            if (DepartureBody == null)
                throw new InvalidOperationException($"{nameof(DepartureBody)} cannot be null");
            if (TargetBody == null)
                throw new InvalidOperationException($"{nameof(TargetBody)} cannot be null");
            if (launch > arrival)
                throw new ArgumentException("Launch date cannot be after arrival date");


            OrbitState orbitDeparture = DepartureBody.OrbitState.PropagateAsNew(launch);
            OrbitState orbitArrival   = TargetBody.OrbitState.PropagateAsNew(arrival);

            Duration tof = Duration.FromSeconds((orbitArrival.Epoch - orbitDeparture.Epoch).TotalSeconds);
            if (tof.Seconds <= 0)
                return;
            try
            {
                Manoeuvre    m     = Manoeuvre.Lambert(orbitDeparture, orbitArrival, solver);
                TransferData tData = new(launch, arrival, m, orbitDeparture, orbitArrival);

                transfers.Add(tData);
            }
            catch (Exception ex) { }
        }
    }
}