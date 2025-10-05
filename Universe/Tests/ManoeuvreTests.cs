// VindemiatrixCollective.Universe.Tests © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using NUnit.Framework;
using UnitsNet;
using UnityEngine;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Manoeuvres;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using Impulse = VindemiatrixCollective.Universe.CelestialMechanics.Manoeuvres.Impulse;

#endregion

namespace VindemiatrixCollective.Universe.Tests
{
    public class ManoeuvreTests
    {
        [Test]
        public void EarthToMarsManoeuvre()
        {
            // Values from Hapsira for the Mars Science Laboratory
            DateTime epochDeparture = new(2011, 11, 26, 15, 2, 0, DateTimeKind.Utc);

            Vector3d r = new(6.46006458e+10, 1.21424867e+11, 5.26400459e+10); // m
            Vector3d v = new(-27227.01764589, 11944.59987887, 5176.81664313); // m/s

            OrbitState state = OrbitState.FromVectors(r, v, Common.Sun, epochDeparture);


            Manoeuvre m = new(new[]
            {
                new Impulse(Duration.Zero, new Vector3d(-2064.20560746, 2587.9683653, 239.11542941)),                  // m/s
                new Impulse(Duration.FromSeconds(21910501), new Vector3d(3331.39946578, 682.12917585, -1089.77932679)) // m/s
            });

            double deltaV   = m.ComputeTotalCost().MetersPerSecond;
            double duration = m.ComputeTotalDuration().Days;

            Debug.Log($"DeltaV cost: {deltaV:f2} m/s\nDuration: {duration:f2} d");

            Assert.AreEqual(6889.865, deltaV, 1e-3);
            Assert.AreEqual(253.55, duration, 1e-1);

            OrbitState newState = state.ApplyManoeuvre(m);

            Debug.Log(state.ToString());
            Debug.Log(newState.ToString());

            // TODO: see why there is a non-negligible difference in these results
            Assert.AreEqual(1.5307983220292651, newState.SemiMajorAxis.AstronomicalUnits, 0.01);
            Assert.AreEqual(691.791767927197, newState.Period.Days, 1);
        }
    }
}