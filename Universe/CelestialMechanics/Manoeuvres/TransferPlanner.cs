using System;
using System.Collections.Generic;
using UnitsNet;
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

    public class TransferPlanner
    {
        private static readonly DateTime J2000 = UniversalConstants.Time.J2000;
        private readonly IzzoLambertSolver solver;

        private List<TransferData> transfers;

        public TransferPlanner(ICelestialBody departureBody, ICelestialBody targetBody)
        {
            DepartureBody = departureBody;
            TargetBody = targetBody;
            solver = new IzzoLambertSolver();
        }

        public ICelestialBody DepartureBody { get; }
        public ICelestialBody TargetBody { get; }

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

        public IEnumerable<TransferData> BestByDeltaV()
        {
            transfers.Sort((m1, m2) => m2.Manoeuvre.ComputeTotalCost().CompareTo(m1.Manoeuvre.ComputeTotalCost()));
            return transfers;
        }

        public IEnumerable<TransferData> BestByTransferTime()
        {
            transfers.Sort((m1, m2) => m1.Manoeuvre.ComputeTotalCost().CompareTo(m2.Manoeuvre.ComputeTotalCost()));
            return transfers;
        }

        private void CalculateTransfer(DateTime launch, DateTime arrival)
        {
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
            catch (Exception ex)
            {
                //Debug.Log($"{launch.ToString(CultureInfo.CurrentCulture)} / {arrival.ToString(CultureInfo.CurrentCulture)}");
            }
        }
    }
}