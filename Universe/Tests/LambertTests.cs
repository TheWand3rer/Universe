// VindemiatrixCollective.Universe.Tests © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

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
        [Test]
        public void LambertMultiple()
        {
            Common.timer.Start();
            // From: https://hapsira.readthedocs.io/en/latest/examples/revisiting-lamberts-problem-in-python.html#Part-4:-Run-some-examples
            // Multiple revolutions
            GravitationalParameter mu = GravitationalParameter.Earth;

            Vector3d r0  = new(22592.145603, -1599.915239, -19783.950506); // km
            Vector3d r   = new(1922.067697, 4054.157051, -8925.727465);    // km
            Duration tof = Duration.FromHours(10);

            Vector3d exp_va = new(2.000652697, 0.387688615, -2.666947760); // km/s
            Vector3d exp_vb = new(-3.79246619, -1.77707641, 6.856814395);  // km/s

            IzzoLambertSolver izzo = new();
            (Vector3d v0, Vector3d v) = izzo.Lambert(mu, r0, r, tof);
            Debug.Log($"{nameof(LambertSingle)} M:{izzo.Revolutions} completed in {Common.timer.ElapsedMilliseconds} ms for {izzo.MaxIterations} iterations");
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
            Debug.Log($"{nameof(LambertSingle)} M:{izzo.Revolutions} Lowpath: {izzo.LowPath} completed in {Common.timer.ElapsedMilliseconds} ms for {izzo.MaxIterations} iterations");
            Common.timer.Restart();

            Common.VectorsAreEqual(exp_va_r, va_r, 1e-4, nameof(exp_va));
            Common.VectorsAreEqual(exp_vb_r, vb_r, 1e-4, nameof(exp_vb));

            izzo.LowPath                   = false;
            (Vector3d va_l, Vector3d vb_l) = izzo.Lambert(mu, r0, r, tof);
            Debug.Log($"{nameof(LambertSingle)} M:{izzo.Revolutions} Lowpath: {izzo.LowPath} completed in {Common.timer.ElapsedMilliseconds} ms for {izzo.MaxIterations} iterations");

            Common.VectorsAreEqual(exp_va_l, va_l, 1e-4, nameof(exp_va));
            Common.VectorsAreEqual(exp_vb_l, vb_l, 1e-4, nameof(exp_vb));
        }

        [Test]
        public void LambertSingle()
        {
            Common.timer.Start();

            // From: https://hapsira.readthedocs.io/en/latest/examples/revisiting-lamberts-problem-in-python.html#Part-4:-Run-some-examples
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
        public void LambertTransfer()
        {
            DateTime epochDeparture = new DateTime(2018, 12, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime epochArrival   = epochDeparture.AddYears(2);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(epochDeparture.ToLongDateString());
            sb.AppendLine(epochArrival.ToLongDateString());

            CelestialBody earth = Common.Earth;
            CelestialBody mars  = Common.Mars;

            earth.OrbitState.SetAttractor(Common.Sun);
            mars.OrbitState.SetAttractor(Common.Sun);

            earth.OrbitState.Propagate(epochDeparture);
            mars.OrbitState.Propagate(epochArrival);

            IzzoLambertSolver solver = new IzzoLambertSolver();
            Manoeuvre         m      = Manoeuvre.Lambert(earth.OrbitState, mars.OrbitState, solver);

            sb.AppendLine(m.ToString());
            sb.AppendLine($"Total duration: {m.ComputeTotalDuration().Days} d");
            sb.AppendLine($"Total cost: {m.ComputeTotalCost().KilometersPerSecond:F3} km/s");
            sb.AppendLine("-----\n");
            sb.AppendLine("Initial state:");
            sb.AppendLine(earth.OrbitState.ToString());

            OrbitState afterManoeuvre = earth.OrbitState.ApplyManoeuvre(m);
            sb.AppendLine("\nAfter manoeuvre:");
            sb.AppendLine(afterManoeuvre.ToString());
            Debug.Log(sb);
            Assert.AreEqual(1.5247995030657975, afterManoeuvre.SemiMajorAxis.AstronomicalUnits, 1.5e-3, nameof(OrbitState.SemiMajorAxis));
        }
    }
}