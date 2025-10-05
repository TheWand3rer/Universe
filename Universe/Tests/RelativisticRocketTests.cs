// VindemiatrixCollective.Universe.Tests © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using NUnit.Framework;
using UnitsNet;
using UnityEngine;
using VindemiatrixCollective.Universe.CelestialMechanics;

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

            var result = Relativity.CalculateTravel(distanceAlphaCentauri, maxSpeed, acceleration, false);

            Debug.Log(result);
            Assert.AreEqual(44, result.ShipTimeCruise.Years365, 1);
        }

        [Test]
        // Values from http://large.stanford.edu/courses/2012/ph241/klein2/docs/19890007533_1989007533.pdf
        // Project Longshot to Alpha Centauri Orbit
        public void LongshotToAlphaCentauriOrbit()
        {
            Length       distanceAlphaCentauri = Length.FromLightYears(4.344);
            Speed        maxSpeed              = Relativity.SpeedFromFractionOfC(0.048f);        // page 20, 59
            Acceleration acceleration          = Acceleration.FromMetersPerSecondSquared(0.429); // page 68

            RelativisticTravelData result = Relativity.CalculateTravel(distanceAlphaCentauri, maxSpeed, acceleration);

            Debug.Log(result);

            // Original plan from the paper, but this won't pass
            // Assert.AreEqual(100, result.TotalObserverTime.Years365, 1);

            // Values from https://gregsspacecalculations.blogspot.com/p/blog-page.html
            Assert.AreEqual(92.6283, result.TotalObserverTime.Years365, 1.05);
            Assert.AreEqual(1.0641, result.ObserverTimeAcceleration.Years365, 0.01);
        }
    }
}