using System;
using System.Linq;
using NUnit.Framework;
using UnitsNet;
using UnityEngine;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Tests
{
    public class OrbitStateTests
    {
        private readonly DeserializationHelper dataHelper = new();

        [Test]
        public void Clone()
        {
            OrbitState state  = OrbitState.FromOrbitalElements(Common.MarsElements, Common.Sun);
            OrbitState state1 = state.Clone();

            Duration week = Duration.FromDays(7);

            state.Propagate(week);
            state1.Propagate(week);

            Common.ArrayAreEqual(state.ToArrayElements(), state1.ToArrayElements(), 1e-3, "Elements");
        }

        [Test]
        public void CoeToRV()
        {
            GravitationalParameter gm = GravitationalParameter.Earth;
            double                 h  = 60000e6;
            double                 e  = 0.3;
            double                 nu = UniversalConstants.Tri.DegreeToRad * 120;
            double                 p  = h * h / gm.M3S2;
            (Vector3d r, Vector3d v) = OrbitalMechanics.RVinPerifocalFrame(gm.M3S2, p, e, nu);

            Common.VectorsAreEqual(new Vector3d(-5312706.25105345, 9201877.15251336, 0), r, 1e-2, nameof(OrbitState.Position));
            Common.VectorsAreEqual(new Vector3d(-5753.30180931, -1328.66813933, 0), v, 1e-2, nameof(OrbitState.Velocity));
        }

        [Test]
        public void CoeToRvTransitive()
        {
            Star        sun           = Common.Sun;
            OrbitalData marsElements  = OrbitalData.FromClassicElements(1.523679f, 0.093315f, 1.85f, 49.562f, 286.537f, 23.33f);
            OrbitState  stateExpected = OrbitState.FromOrbitalElements(marsElements, sun);

            (Vector3d r, Vector3d v) = stateExpected.ToVectors();

            OrbitState stateResult = OrbitState.FromVectors(r, v, sun, stateExpected.Epoch);

            Assert.AreEqual(stateExpected.SemiMajorAxis.AstronomicalUnits, stateResult.SemiMajorAxis.AstronomicalUnits, 1e-3,
                            nameof(OrbitState.SemiMajorAxis));
            Assert.AreEqual(stateExpected.Eccentricity.Value, stateResult.Eccentricity.Value, 1e-3, nameof(OrbitState.Eccentricity));
            Assert.AreEqual(stateExpected.SemiLatusRectum.AstronomicalUnits, stateResult.SemiLatusRectum.AstronomicalUnits, 1e-3,
                            nameof(OrbitState.SemiLatusRectum));

            Assert.AreEqual(stateExpected.TrueAnomaly.Degrees, stateResult.TrueAnomaly.Degrees, 0.5, nameof(OrbitState.TrueAnomaly));
            Assert.AreEqual(stateExpected.LongitudeAscendingNode.Degrees, stateResult.LongitudeAscendingNode.Degrees, 1e-3,
                            nameof(OrbitState.LongitudeAscendingNode));
            Assert.AreEqual(stateExpected.ArgumentPeriapsis.Degrees, stateResult.ArgumentPeriapsis.Degrees, 1, nameof(OrbitState.ArgumentPeriapsis));
            Assert.AreEqual(stateExpected.Inclination.Degrees, stateResult.Inclination.Degrees, 1e-3, nameof(OrbitState.Inclination));
        }

        [Test]
        public void FromClassicElements()
        {
            double tol = 1e-6;
            Star   sun = Common.Sun;

            OrbitState state = OrbitState.FromOrbitalElements(Common.MarsElements, sun);

            Assert.AreEqual(686.9713834767824, state.Period.Days, 1);
            Assert.AreEqual(1.6658611058850001, state.ApoapsisDistance.AstronomicalUnits, 1e-3);
            Assert.AreEqual(1.3814968941149999, state.PeriapsisDistance.AstronomicalUnits, 1e-3);
        }

        [Test]
        public void FromVectorsMars()
        {
            Star sun = Common.Sun;

            Vector3d   r     = new(2.08047627e+11, -2.02006193e+09, -5.15689300e+09);
            Vector3d   v     = new(1164.20212, 26296.03633, 522.29379);
            OrbitState state = OrbitState.FromVectors(r, v, sun, Common.J2000);

            Vector3d hExpected = new(1.34550780e14, -1.14665650e14, 5.47317972e15);
            Common.VectorsAreEqual(hExpected, state.AngularMomentum, 1e7);

            Vector3d EExpected = new(0.08527746, -0.03777703, -0.00288788);
            Common.VectorsAreEqual(EExpected, state.EccentricityVector, 1e-3);

            Assert.AreEqual(1.523679, state.SemiMajorAxis.AstronomicalUnits, 1e-3, nameof(OrbitState.SemiMajorAxis));
            Assert.AreEqual(0.093315, state.Eccentricity.Value, 1e-3, nameof(OrbitState.Eccentricity));
            Assert.AreEqual(1.85, state.Inclination.Degrees, 1e-3, nameof(OrbitState.Inclination));
            Assert.AreEqual(1.5104112767893412, state.SemiLatusRectum.AstronomicalUnits, 1e-3, nameof(OrbitState.SemiLatusRectum));
            Assert.AreEqual(23.33, state.TrueAnomaly.Degrees, 0.5, nameof(OrbitState.TrueAnomaly));
            Assert.AreEqual(49.562, state.LongitudeAscendingNode.Degrees, 1e-3, nameof(OrbitState.LongitudeAscendingNode));
            Assert.AreEqual(286.537, state.ArgumentPeriapsis.Degrees, 1, nameof(OrbitState.ArgumentPeriapsis));
        }

        [Test]
        public void LunarPeriod()
        {
            Mass   eMass   = Mass.FromEarthMasses(1);
            Length eRadius = Length.FromKilometers(6371.0);

            double volume = 4 / 3d * Math.PI * Math.Pow(eRadius.Meters, 3);

            Planet earth = new()
            {
                Name = "Earth",
                PhysicalData = new PhysicalData(Density.FromKilogramsPerCubicMeter(eMass.Kilograms / volume),
                                                eRadius,
                                                GravitationalParameter.FromMass(eMass)),
                OrbitalData = Planet.Earth.OrbitalData
            };

            Planet moon = new()
            {
                Name = "Luna",
                PhysicalData = new PhysicalData(Mass.FromKilograms(7.346e22),
                                                Length.FromKilometers(1737.4),
                                                Acceleration.FromMetersPerSecondSquared(1.622),
                                                Density.FromGramsPerCubicCentimeter(3.34)),
                OrbitalData = Planet.Moon.OrbitalData
            };

            earth.AddOrbiters(new[] { moon });
            moon.OrbitState.Propagate(Duration.FromDays(1));

            // Half a day of tolerance
            Assert.AreEqual(27, moon.OrbitState.Period.Days, 5e-1, nameof(OrbitState.Period));
            Debug.Log(moon.OrbitState.Period.Days);
        }


        [Test]
        public void PropagatedFromToVectors()
        {
            Planet earth    = Planet.Earth;
            int    altitude = 7000000;

            // this method uses Mu.M3S2 / (body.Radius + orbitHeight)
            Speed orbitSpeed1 = OrbitalMechanics.CalculateOrbitalVelocity(Planet.Earth, Length.FromMeters(7000000 - earth.PhysicalData.Radius.Meters));

            Speed orbitSpeed =
                Speed.FromMetersPerSecond(Math.Sqrt(UniversalConstants.Celestial.GravitationalConstant * earth.PhysicalData.Mass.Kilograms / altitude));

            Assert.AreEqual(orbitSpeed1.MetersPerSecond, orbitSpeed.MetersPerSecond, 1e-2, "Orbital Speed");


            DateTime date = DateTime.Now;

            OrbitState orbit = OrbitState.FromVectors(new Vector3d(altitude, 0, 0), new Vector3d(0, orbitSpeed.MetersPerSecond, 0),
                                                      earth,
                                                      date);

            Assert.AreEqual(altitude, orbit.PeriapsisDistance.Meters, 5, "PeriapsisDistance Before");
            Assert.AreEqual(altitude, orbit.ApoapsisDistance.Meters, 5, "ApoapsisDistance Before");
            Assert.AreEqual(altitude, orbit.SemiMajorAxis.Meters, 5, "Semi Major Axis Before");
            Assert.AreEqual(0, orbit.TrueAnomaly.Degrees, 0, "TrueAnomaly Before");


            for (int i = 0; i < 1000; i++)
            {
                date += Duration.FromDays(0.016);
                orbit.Propagate(date);
            }

            Vector3d r, v;

            (r, v) = orbit.ToVectors();

            OrbitState newOrbit = OrbitState.FromVectors(r, v, earth, date);

            Assert.AreEqual(altitude, newOrbit.PeriapsisDistance.Meters, 5, "PeriapsisDistance After");
            Assert.AreEqual(altitude, newOrbit.ApoapsisDistance.Meters, 5, "ApoapsisDistance After");
            Assert.AreEqual(altitude, newOrbit.SemiMajorAxis.Meters, 5, "Semi Major Axis After");
            //Assert.AreEqual(0, newOrbit.TrueAnomaly.Degrees, 0, "TrueAnomaly After");
        }

        [Test]
        public void PropagationElements()
        {
            Star sun = Common.Sun;

            OrbitState halley = OrbitState.FromVectors(new Vector3d(-9018878635.69932, -94116054798.39276, 22619058699.43215),
                                                       new Vector3d(-49950.92305, -12948.43055, -4292.51577), sun, Common.J2000);

            double[] elementsExpected = halley.ToArrayElements().SkipLast(1).ToArray();
            halley.Propagate(Duration.FromDays(1));
            double[] elements = halley.ToArrayElements().SkipLast(1).ToArray();

            Common.ArrayAreEqual(elementsExpected, elements, 1e-3, "Elements");
        }

        [Test]
        public void PropagationKnownValues()
        {
            Galaxy galaxy = dataHelper.LoadSol();
            Planet earth  = galaxy["Sol"][0]["Earth"];

            // Data from Vallado, example 2.4
            Vector3d r0   = new(1131340, -2282343, 6672423);        // m
            Vector3d v0   = new(-5643.05, 4303.33, 2428.79);        // m / s
            Vector3d rExp = new(-4219752.7, 4363029.2, -3958766.6); // m
            Vector3d vExp = new(3689.866, -1916.735, -6112.511);    // m / s

            OrbitState state0 = OrbitState.FromVectors(r0, v0, earth, earth.OrbitState.Epoch);
            Duration   tof    = Duration.FromMinutes(40);
            state0.Propagate(tof);

            (Vector3d r1, Vector3d v1) = state0.ToVectors();

            Common.VectorsAreEqual(rExp, r1, 1e2, nameof(OrbitState.Position));  // 100 m precision
            Common.VectorsAreEqual(vExp, v1, 1e-1, nameof(OrbitState.Velocity)); // 10 cm / s
        }
    }
}