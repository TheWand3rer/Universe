// VindemiatrixCollective.Universe.Tests © 2025-2026 Vindemiatrix Collective

#region using

using System;
using System.Text;
using NUnit.Framework;
using UnitsNet;
using UnityEngine;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Manoeuvres;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

#endregion

namespace VindemiatrixCollective.Universe.Tests
{
    public class LambertTest
    {
        private Planet mars;
        private Planet earth;
        private Planet moon;
        private Star sun;

        [OneTimeSetUp]
        public void Setup()
        {
            sun   = Star.Sun;
            earth = Planet.Earth;
            mars  = Planet.Mars;
            moon  = Planet.Moon;

            earth.SetParentBody(sun);
            mars.SetParentBody(sun);
            moon.SetParentBody(earth);
        }

        [Test]
        public void LambertMultiple()
        {
            Common.timer.Start();
            // From: https://BoInOr.readthedocs.io/en/latest/examples/revisiting-lamberts-problem-in-python.html#Part-4:-Run-some-examples
            // Multiple revolutions
            GravitationalParameter mu = GravitationalParameter.Earth;

            Vector3d r0  = new(22592.145603, -1599.915239, -19783.950506); // km
            Vector3d r   = new(1922.067697, 4054.157051, -8925.727465);    // km
            Duration tof = Duration.FromHours(10);

            Vector3d exp_va = new(2.000652697, 0.387688615, -2.666947760); // km/s
            Vector3d exp_vb = new(-3.79246619, -1.77707641, 6.856814395);  // km/s

            IzzoLambertSolver izzo = new();
            (Vector3d v0, Vector3d v) = izzo.Lambert(mu, r0, r, tof);
            Debug.Log(
                $"{nameof(LambertSingle)} M:{izzo.Revolutions} completed in {Common.timer.ElapsedMilliseconds} ms for {izzo.MaxIterations} iterations");
            Common.timer.Restart();

            Common.VectorsAreEqual(exp_va, v0, 1e-4, nameof(exp_va));
            Common.VectorsAreEqual(exp_vb, v, 1e-4, nameof(exp_vb));

            Vector3d exp_va_l = new(0.50335770, 0.61869408, -1.57176904);
            Vector3d exp_vb_l = new(-4.18334626, -1.13262727, 6.13307091);
            Vector3d exp_va_r = new(-2.45759553, 1.16945801, 0.43161258);
            Vector3d exp_vb_r = new(-5.53841370, 0.01822220, 5.49641054);

            izzo.Revolutions               = 1;
            izzo.LowPath                   = true;
            (Vector3d va_r, Vector3d vb_r) = izzo.Lambert(mu, r0, r, tof);
            Debug.Log(
                $"{nameof(LambertSingle)} M:{izzo.Revolutions} Lowpath: {izzo.LowPath} completed in {Common.timer.ElapsedMilliseconds} ms for {izzo.MaxIterations} iterations");
            Common.timer.Restart();

            Common.VectorsAreEqual(exp_va_r, va_r, 1e-4, nameof(exp_va));
            Common.VectorsAreEqual(exp_vb_r, vb_r, 1e-4, nameof(exp_vb));

            izzo.LowPath                   = false;
            (Vector3d va_l, Vector3d vb_l) = izzo.Lambert(mu, r0, r, tof);
            Debug.Log(
                $"{nameof(LambertSingle)} M:{izzo.Revolutions} Lowpath: {izzo.LowPath} completed in {Common.timer.ElapsedMilliseconds} ms for {izzo.MaxIterations} iterations");

            Common.VectorsAreEqual(exp_va_l, va_l, 1e-4, nameof(exp_va));
            Common.VectorsAreEqual(exp_vb_l, vb_l, 1e-4, nameof(exp_vb));
        }

        [Test]
        public void LambertSingle()
        {
            Common.timer.Start();

            // From: https://BoInOr.readthedocs.io/en/latest/examples/revisiting-lamberts-problem-in-python.html#Part-4:-Run-some-examples
            GravitationalParameter mu = GravitationalParameter.Earth;

            Vector3d r0  = new(15945.34, 0.0, 0.0);            // km
            Vector3d r   = new(12214.83399, 10249.46731, 0.0); // km
            Duration tof = Duration.FromMinutes(76);

            Vector3d exp_va = new(2.058925, 2.915956, 0.0);  // km/s
            Vector3d exp_vb = new(-3.451569, 0.910301, 0.0); // km/s

            IzzoLambertSolver izzo = new();

            (Vector3d va, Vector3d vb) = izzo.Lambert(mu, r0, r, tof);

            Common.VectorsAreEqual(exp_va, va, 1e-4);
            Common.VectorsAreEqual(exp_vb, vb, 1e-4);

            Common.timer.Stop();

            Debug.Log($"{nameof(LambertSingle)} completed in {Common.timer.ElapsedMilliseconds} ms for {izzo.MaxIterations} iterations");
        }

        [Test]
        public void LambertSingleMoon()
        {
            Common.timer.Start();

            GravitationalParameter mu = GravitationalParameter.Sun;

            Vector3d r0  = new(-1.46146663e+08, -3.13405127e+07, -1.35649988e+07); // km
            Vector3d r   = new(-1.42146411e+08, -4.53692929e+07, -1.96813566e+07); // km
            Duration tof = Duration.FromDays(6);

            Vector3d exp_va = new(6.24561887, -27.42747305, -11.9570001);  // km/s
            Vector3d exp_vb = new(9.16985408, -26.64959849, -11.61986648); // km/s

            IzzoLambertSolver izzo = new();

            (Vector3d va, Vector3d vb) = izzo.Lambert(mu, r0, r, tof);

            Common.VectorsAreEqual(exp_va, va, 1e-4);
            Common.VectorsAreEqual(exp_vb, vb, 1e-4);

            Common.timer.Stop();

            Debug.Log($"{nameof(LambertSingle)} completed in {Common.timer.ElapsedMilliseconds} ms for {izzo.MaxIterations} iterations");
        }

        [Test]
        public void LambertTransferEarthMars()
        {
            DateTime epochDeparture = new(2011, 11, 26, 15, 02, 0, DateTimeKind.Utc);
            DateTime epochArrival   = new(2012, 08, 06, 05, 17, 0, DateTimeKind.Utc);

            OrbitState afterManoeuvre = LambertTransfer(epochDeparture, epochArrival, earth, mars);

            Assert.AreEqual(1.5247995030657975, afterManoeuvre.SemiMajorAxis.AstronomicalUnits, 1.5e-3, nameof(OrbitState.SemiMajorAxis));
        }

        [Test]
        public void LambertTransferEarthMoon()
        {
            DateTime epochDeparture = new(2026, 4, 2, 23, 50, 0, DateTimeKind.Utc);
            DateTime epochArrival   = epochDeparture.AddDays(6);

            OrbitState afterManoeuvre = LambertTransferCircular(epochDeparture, epochArrival, earth, moon);

            Assert.AreEqual(0.002, afterManoeuvre.SemiMajorAxis.AstronomicalUnits, 1e-3, nameof(OrbitState.SemiMajorAxis));
        }

        private OrbitState LambertTransfer(DateTime epochDeparture, DateTime epochArrival, CelestialBody origin, CelestialBody destination)
        {
            StringBuilder sb = new();
            sb.AppendLine(epochDeparture.ToLongDateString());
            sb.AppendLine(epochArrival.ToLongDateString());

            GravitationalParameter mu      = origin.OrbitState.GravitationalParameter;
            OrbitState             initial = origin.OrbitState.Clone();
            OrbitState             final   = destination.OrbitState.Clone();

            initial.Propagate(epochDeparture);
            final.Propagate(epochArrival);

            IzzoLambertSolver solver   = new();
            Manoeuvre         m        = Manoeuvre.Lambert(initial, final, solver, mu);
            OrbitState        transfer = initial.ApplyManoeuvre(m);

            LogManoeuvre(m, initial, final, transfer, ref sb);

            return transfer;
        }

        private OrbitState LambertTransferCircular(DateTime epochDeparture, DateTime epochArrival, CelestialBody origin, CelestialBody destination)
        {
            StringBuilder sb = new();
            sb.AppendLine(epochDeparture.ToLongDateString());
            sb.AppendLine(epochArrival.ToLongDateString());

            GravitationalParameter mu      = origin.Mu;
            OrbitState             initial = OrbitState.Circular(origin, Length.FromKilometers(100));
            OrbitState             final   = destination.OrbitState.Clone();

            initial.Propagate(epochDeparture);
            final.Propagate(epochArrival);

            IzzoLambertSolver solver   = new();
            Manoeuvre         m        = Manoeuvre.Lambert(initial, final, solver, mu);
            OrbitState        transfer = initial.ApplyManoeuvre(m);

            LogManoeuvre(m, initial, final, transfer, ref sb);

            return transfer;
        }

        private void LogManoeuvre(Manoeuvre m, OrbitState initial, OrbitState final, OrbitState transfer, ref StringBuilder sb)
        {
            sb.AppendLine(m.ToString());
            sb.AppendLine($"Total duration: {m.ComputeTotalDuration().Days} d");
            sb.AppendLine($"Total cost: {m.ComputeTotalCost().KilometersPerSecond:F3} km/s");
            sb.AppendLine("-----\n");
            sb.AppendLine("Initial state:");
            sb.AppendLine(initial.ToString());

            sb.AppendLine("\nAfter manoeuvre:");
            sb.AppendLine(transfer.ToString());
            Debug.Log(sb);
        }
    }
}