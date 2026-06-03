// VindemiatrixCollective.Universe.Tests © 2025-2026 Vindemiatrix Collective

#region using

using NUnit.Framework;
using UnitsNet;
using UnityEngine;
using VindemiatrixCollective.Universe.Physics;

#endregion

namespace VindemiatrixCollective.Universe.Tests
{
    public class RelativisticRocket
    {
        [Test]
        public void DaedalusToBarnardsStar()
        {
            Length       distanceAlphaCentauri = Length.FromLightYears(5.9);
            Speed        maxSpeed              = Relativity.SpeedFromFractionOfC(0.12f);
            Acceleration acceleration          = Acceleration.FromMetersPerSecondSquared(0.14);

            RelativisticTravelData result = Relativity.CalculateTravel(distanceAlphaCentauri, maxSpeed, acceleration, false);

            Debug.Log(result);
            Assert.AreEqual(44, result.ShipTimeCruise.Years365, 1);
        }

        [Test]
        // Values from http://large.stanford.edu/courses/2012/ph241/klein2/docs/19890007533_1989007533.pdf
        // Project Longshot to Alpha Centauri Orbit
        public void LongshotToAlphaCentauriOrbit()
        {
            Length distanceAlphaCentauri = Length.FromLightYears(4.344);
            // To match the planned time of 100 years
            Speed        deltaV       = Speed.FromKilometersPerSecond(26200);
            Acceleration acceleration = Acceleration.FromMetersPerSecondSquared(0.429); // page 68

            RelativisticTravelData result = Relativity.CalculateTravel(distanceAlphaCentauri, deltaV, acceleration);

            Debug.Log(result);

            Assert.AreEqual(100, result.TotalObserverTime.Years365, 2);
        }

        [Test]
        public void TAU()
        {
            // "TAU - A mission to a Thousand Astronomical Units"
            // was a proposal for an uncrewed interstellar probe.
            // 
            // https://en.wikipedia.org/wiki/TAU_(spacecraft)

            // From: https://doi.org/10.2514/6.1987-1049
            // Mass: 5000 kg spacecraft
            // 10 mt Propulsion system
            // I_sp: 12.500 s (Specific Impulse)
            // v_e: 250.000 km/s (Exhaust Velocity) Xenon Ion engine
            // 10 year burn time
            // 1 MW Nuclear Reactor
            // Max speed: 106 km/s (from figure 2)
            // Achieve 1000 AU in 50 years (from Table 2)

            Length                 distance     = Length.FromAstronomicalUnits(1000);
            Speed                  deltaV       = Speed.FromKilometersPerSecond(106);
            Acceleration           acceleration = deltaV / Duration.FromYears365(10);
            RelativisticTravelData result       = Relativity.CalculateTravel(distance, deltaV, acceleration, false);

            Debug.Log(result);

            Assert.AreEqual(50, result.TotalShipTime.Years365, 0.5d, nameof(result.TotalShipTime));
        }
    }
}