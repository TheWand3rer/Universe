using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnitsNet;
using UnityEngine;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Manoeuvres;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Tests
{
    public class TransferPlannerTest
    {
        private CelestialBody star;
        private Galaxy galaxy;

        [Test]
        // Problem 5.4 & 5.5
        // http://www.braeunig.us/space/problem.htm#5.3
        public void CalculateOrbitalElements()
        {
            Vector3d r1      = new(0.473265, 0, -0.899215);
            Vector3d r2      = new(0.066842, 0.030948, 1.561256);
            Length   rOneMag = Length.FromAstronomicalUnits(r1.magnitude);
            Length   rTwoMag = Length.FromAstronomicalUnits(r2.magnitude);
            Length   p       = Length.FromAstronomicalUnits(1.250633);
            Length   a       = Length.FromAstronomicalUnits(1.320971);
            Angle    deltaV  = Angle.FromDegrees(149.770967);
            
            (double f, double g, double fDot) = OrbitalMechanics.CalculateTransferParameters(rOneMag, rTwoMag, p, deltaV, GravitationalParameter.Sun.M3S2);

            //double f = 1 - ((rTwoMag.AstronomicalUnits / p.AstronomicalUnits) * (1 - Math.Cos(deltaV.Radians)));
            //double g = (rOneMag.AstronomicalUnits * rTwoMag.AstronomicalUnits * Math.Sin(deltaV.Radians)) / Math.Sqrt(gmAu * p.AstronomicalUnits);
            //double fDot = Math.Sqrt(gmAu / p.AstronomicalUnits) * Math.Tan(deltaV.Radians / 2) *
            //              (((1 - Math.Cos(deltaV.Radians)) / p.AstronomicalUnits) - (1 / rOneMag.AstronomicalUnits) - (1 / rTwoMag.AstronomicalUnits));
            double gDot = 1 - ((rOneMag.AstronomicalUnits / p.AstronomicalUnits) * (1 - Math.Cos(deltaV.Radians)));

            Vector3d v1 = (r2 - (f * r1)) / g;
            Vector3d v2 = (fDot * r1) + (gDot * v1);

            v1 = OrbitalMechanics.AuToMetres(v1);
            v2 = OrbitalMechanics.AuToMetres(v2);

            Assert.AreEqual(28996.2, v1.x, 0.1);
            Assert.AreEqual(15232.7, v1.z, 0.1);
            Assert.AreEqual(1289.2, v1.y, 0.1);

            Assert.AreEqual(-21147.0, v2.x, 0.1);
            Assert.AreEqual(3994.5, v2.z, 0.1);
            Assert.AreEqual(-663.3, v2.y, 0.1);

            Vector3d r1Metres = OrbitalMechanics.AuToMetres(r1);
            double   r1Mag    = r1Metres.magnitude;
            Vector3d h        = -Vector3d.Cross(r1Metres, v1);
            Assert.AreEqual(-1.73424e14, h.x, 1e10);
            Assert.AreEqual(-9.12746e13, h.z, 1e10);
            Assert.AreEqual(4.97905e15, h.y, 1e10);

            double   vMag  = v1.magnitude;
            double   v2Gmr = Math.Pow(vMag, 2) - (GravitationalParameter.Sun.M3S2 / r1Mag);
            double   rDotV = Vector3d.Dot(r1Metres, v1);
            Vector3d e     = ((v2Gmr * r1Metres) - (rDotV * v1)) / GravitationalParameter.Sun.M3S2;
            Assert.AreEqual(0.230751, e.magnitude, 0.01);

            Angle trueAnomaly = Angle.FromRadians(Math.Acos(Vector3d.Dot(e, r1Metres) / (e.magnitude * r1Mag)));
            Assert.AreEqual(0.226, trueAnomaly.Degrees, 0.1);

            Angle inclination = Angle.FromRadians(Math.Acos(h.y / h.magnitude));
            Assert.AreEqual(2.255, inclination.Degrees, 0.1);
        }

        [Test]
        // Problem 5.3
        // http://www.braeunig.us/space/problem.htm#5.3
        public void CalculateTransferOrbit()
        {
            //2020-7-20 00.00 UTC
            double                 deltaAu    = 1e-3;
            Duration               t          = Duration.FromDays(207);
            Vector3d               r1         = new(0.473265, 0, -0.899215);
            Vector3d               r2         = new(0.066842, 0.030948, 1.561256);
            Length                 rOneMag    = Length.FromAstronomicalUnits(r1.magnitude);
            Length                 rTwoMag    = Length.FromAstronomicalUnits(r2.magnitude);
            GravitationalParameter gmSun      = GravitationalParameter.Sun;
            double                 gmSunAu3S2 = gmSun.Au3S2;

            Assert.AreEqual(3.964016e-14, gmSunAu3S2, 0.1e-14);

            Angle deltaV = Angle.FromDegrees(Vector3d.Angle(r1, r2));
            Assert.AreEqual(149.770967, deltaV.Degrees, deltaAu);

            Length k   = Length.FromAstronomicalUnits(rOneMag.AstronomicalUnits * rTwoMag.AstronomicalUnits * (1 - Math.Cos(deltaV.Radians)));
            Length m   = Length.FromAstronomicalUnits(rOneMag.AstronomicalUnits * rTwoMag.AstronomicalUnits * (1 + Math.Cos(deltaV.Radians)));
            Length l   = rOneMag + rTwoMag;
            Length pi  = Length.FromAstronomicalUnits(k.AstronomicalUnits / (l.AstronomicalUnits + Math.Pow(2 * m.AstronomicalUnits, 0.5d)));
            Length pii = Length.FromAstronomicalUnits(k.AstronomicalUnits / (l.AstronomicalUnits - Math.Pow(2 * m.AstronomicalUnits, 0.5d)));

            Assert.AreEqual(2.960511, k.AstronomicalUnits, deltaAu);
            Assert.AreEqual(2.579146, l.AstronomicalUnits, deltaAu);
            Assert.AreEqual(0.215969, m.AstronomicalUnits, deltaAu);
            Assert.AreEqual(0.914764, pi.AstronomicalUnits, deltaAu);
            Assert.AreEqual(1.540388, pii.AstronomicalUnits, deltaAu);

            Length p = Length.FromAstronomicalUnits(1.2);

            double pAu = p.AstronomicalUnits;
            double kAu = k.AstronomicalUnits;
            double lAu = l.AstronomicalUnits;
            double mAu = m.AstronomicalUnits;
            double aAu = (mAu * kAu * pAu) / (((((2 * mAu) - Math.Pow(lAu, 2)) * Math.Pow(pAu, 2)) + (2 * kAu * lAu * pAu)) - Math.Pow(kAu, 2));

            Length a = Length.FromAstronomicalUnits(aAu);
            Assert.AreEqual(1.270478, a.AstronomicalUnits, deltaAu);

            double f = 1 - ((rTwoMag.AstronomicalUnits / p.AstronomicalUnits) * (1 - Math.Cos(deltaV.Radians)));
            double g = (rOneMag.AstronomicalUnits * rTwoMag.AstronomicalUnits * Math.Sin(deltaV.Radians)) /
                       Math.Pow(gmSunAu3S2 * p.AstronomicalUnits, 0.5);
            double fDot = Math.Pow(gmSunAu3S2 / p.AstronomicalUnits, 0.5) * Math.Tan(deltaV.Radians / 2) *
                          (((1 - Math.Cos(deltaV.Radians)) / p.AstronomicalUnits) - (1 / rOneMag.AstronomicalUnits) - (1 / rTwoMag.AstronomicalUnits));
            double gDot = 1 - ((rOneMag.AstronomicalUnits / p.AstronomicalUnits) * (1 - Math.Cos(deltaV.Radians)));

            Length r1Mag = Length.FromAstronomicalUnits(r1.magnitude);
            Length r2Mag = Length.FromAstronomicalUnits(r2.magnitude);
            Assert.AreEqual(-1.427875, f, 0.1);
            Assert.AreEqual(3666240, g, 1);
            Assert.AreEqual(-4.74601e-8, fDot, 1e-9);

            (double f1, double g1, double fDot1) = OrbitalMechanics.CalculateTransferParameters(r1Mag, r2Mag, p, deltaV, gmSun.M3S2);
            double a1 = OrbitalMechanics.CalculateSemiMajorAxisFromSemiLatusRectum(p, k, l, m).AstronomicalUnits;

            Assert.AreEqual(a.AstronomicalUnits, a1, deltaAu);
            Assert.AreEqual(f, f1, 0.1);
            Assert.AreEqual(g, g1, 1);
            Assert.AreEqual(fDot, fDot1, 1e-9);

            Length   pnMinus1 = p;
            Length   pn       = Length.FromAstronomicalUnits(1.3);
            Duration tnMinus1 = OrbitalMechanics.CalculateTransferTime(pnMinus1, rOneMag, rTwoMag, k, l, m, deltaV, gmSun);
            Duration tn       = OrbitalMechanics.CalculateTransferTime(Length.FromAstronomicalUnits(1.3), rOneMag, rTwoMag, k, l, m, deltaV, gmSun);
            Length   pn1      = pn + (((t - tn).Days * (pn - pnMinus1)) / (tn - tnMinus1).Days);

            Assert.AreEqual(247.4647, tnMinus1.Days, deltaAu);
            Assert.AreEqual(1.259067, pn1.AstronomicalUnits, deltaAu);

            Duration tpn1 = OrbitalMechanics.CalculateTransferTime(pn1, rOneMag, rTwoMag, k, l, m, deltaV, gmSun);
            Assert.AreEqual(201.5624, tpn1.Days, 0.1);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Duration: {tpn1.Days:F2}");
            Debug.Log(sb.ToString());
        }

        [Test]
        public void CalculateVelocities()
        {
            Planet earth    = Common.Earth;
            Length radius   = Length.FromKilometers(6378.14);
            Length altitude = Length.FromKilometers(200);

            Length rTotal = radius + altitude;

            double vEsc    = Math.Sqrt((2 * earth.Mu.M3S2) / rTotal.Meters);
            double vb      = 11500;
            double vExcess = Math.Sqrt((vb * vb) - (vEsc * vEsc));

            Assert.AreEqual(11009, vEsc, 1);
            Assert.AreEqual(3325, vExcess, 1);

            Speed burnoutVelocity          = Speed.FromMetersPerSecond(vb);
            Speed escapeVelocity           = OrbitalMechanics.CalculateEscapeVelocity(earth, altitude);
            Speed hyperbolicExcessVelocity = OrbitalMechanics.CalculateHyperbolicExcessVelocity(burnoutVelocity, earth, altitude);


            Assert.AreEqual(11009, escapeVelocity.MetersPerSecond, 1);
            Assert.AreEqual(3325, hyperbolicExcessVelocity.MetersPerSecond, 1, "Hyperbolic Excess Velocity");
            Assert.AreEqual(3325,
                            OrbitalMechanics.CalculateHyperbolicExcessVelocity(burnoutVelocity, escapeVelocity)
                                            .MetersPerSecond, 1, "Hyperbolic Excess Velocity from Escape Velocity");

            Vector3d vp   = new Vector3d(25876.6, 13759.5, 0);
            Vector3d vs   = new Vector3d(28996.6, 15232.7, 1289.2);
            Vector3d vsp  = vs - vp;
            double   vinf = vsp.magnitude;
            Assert.AreEqual(3683, vinf, 1);

            double v0 = Math.Sqrt((vinf * vinf) + ((2 * earth.Mu.M3S2) / rTotal.Meters));
            double dv = v0 - Math.Sqrt(earth.Mu.M3S2 / rTotal.Meters);

            Assert.AreEqual(11608.4, v0, 1, "v0");
            Assert.AreEqual(3824.1, dv, 1, "Delta V");
        }

        [Test]
        public void CompareToOrbitState()
        {
            Star     sun   = Common.Sun;
            Planet   earth = Common.Earth;
            Vector3d r     = new(0.473265, -0.899215, 0);
            Vector3d v     = new Vector3d(0.000000193828, 0.000000101824, 0.00000000861759);
            v = OrbitalMechanics.AuToMetres(v);
            r = OrbitalMechanics.AuToMetres(r);

            OrbitState orbitFromVectors = OrbitState.FromVectors(r, v, sun, earth.OrbitState.Epoch);

            Assert.AreEqual(0.230751, orbitFromVectors.Eccentricity.Value, 0.01, nameof(OrbitState.Eccentricity));
            Assert.AreEqual(1.97614e11, orbitFromVectors.SemiMajorAxis.Meters, 1e9, nameof(OrbitState.SemiMajorAxis));
            Assert.AreEqual(0.226, orbitFromVectors.TrueAnomaly.Degrees, 0.01, nameof(OrbitState.TrueAnomaly));

            Vector3d h    = Vector3d.Cross(r, v);
            Vector3d n    = Vector3d.Cross(Vector3d.right, h);
            double   loAn = ((Math.PI * 2) + Math.Acos(n.x / n.magnitude)) % Math.PI;
            if (n.y < 0)
            {
                loAn = (2 * Math.PI) - loAn;
            }

            loAn = (loAn % 2) * Math.PI;
            
            double   gm  = sun.Mu.M3S2;
            Vector3d ecc = (1 / gm) * (((v.sqrMagnitude - (gm / r.magnitude)) * r) - (Vector3d.Dot(r, v) * v));
            Ratio    e   = Ratio.FromDecimalFractions(ecc.magnitude);

            // Eccentricity
            Assert.AreEqual(0.230751, e.Value, 0.01);
            // AoPe
            double aoPe = Math.Acos(Vector3d.Dot(n, ecc) / (n.magnitude * ecc.magnitude));

            // True Anomaly
            double cosTa = Vector3d.Dot(ecc, r) / (ecc.magnitude * r.magnitude);
            double ta    = Math.Acos(cosTa);

            if (Vector3d.Dot(r, v) < 0)
            {
                ta = (2 * Math.PI) - ta;
            }

            double cosE = (e.Value + cosTa) / (1 + (e.Value * cosTa));
            Angle  E    = Angle.FromRadians(Math.Acos(cosE)); // Eccentric Anomaly
            Angle  nu   = OrbitalMechanics.EccentricToTrueAnomaly(E, e);

            Assert.AreEqual(0.226, ta * 57.295779513, 0.01, nameof(OrbitState.TrueAnomaly));
            Assert.AreEqual(0.226, nu.Degrees, 0.01, nameof(OrbitState.TrueAnomaly));
        }

        
        [Test]
        // Problem 5.1
        // http://www.braeunig.us/space/problem.htm#5.1
        public void TrueAnomalyChange()
        {
            Length ra  = Length.FromAstronomicalUnits(1);
            Length rb  = Length.FromAstronomicalUnits(1.524);
            Length atx = Length.FromAstronomicalUnits(1.3);
            double e   = 1 - (ra / atx);
            Assert.AreEqual(0.230769d, e, 0.01d);

            Angle trueAnomaly = Angle.FromRadians(Math.Acos((((atx * (1 - (e * e))) / rb) - 1) / e));
            Assert.AreEqual(146.488d, trueAnomaly.Degrees, 0.1);

            Angle eccentricAnomaly = Angle.FromRadians(Math.Acos((e + Math.Cos(trueAnomaly.Radians)) / (1 + (e * Math.Cos(trueAnomaly.Radians)))));
            Assert.AreEqual(2.41383d, eccentricAnomaly.Radians, 0.1);

            double gmSun = 1.327124e20;
            Duration tof = Duration.FromSeconds((eccentricAnomaly.Radians - (e * Math.Sin(eccentricAnomaly.Radians))) * Math.Sqrt(
                                                     Math.Pow(atx.Meters, 3) / gmSun));

            Assert.AreEqual(194.77, tof.Days, 0.1);

            Angle meanAnomaly       = Angle.FromRadians(eccentricAnomaly.Radians - (e * Math.Sin(eccentricAnomaly.Radians)));
            Angle eccentricAnomaly1 = OrbitalMechanics.MeanToEccentricAnomaly(meanAnomaly, Ratio.FromDecimalFractions(e));
            Assert.AreEqual(eccentricAnomaly.Radians, eccentricAnomaly1.Radians, 0.1);

            Angle trueAnomaly2 = OrbitalMechanics.EccentricToTrueAnomaly(eccentricAnomaly, Ratio.FromDecimalFractions(e));
            Assert.AreEqual(trueAnomaly.Radians, trueAnomaly2.Radians, 0.1);
        }

        [Test]
        public void PorkchopTest()
        {
            Common.timer.Start();
            Planet earth = Common.Earth;
            Planet mars  = Common.Mars;

            earth.OrbitState.SetAttractor(Common.Sun);
            mars.OrbitState.SetAttractor(Common.Sun);

            var      tp        = new CelestialMechanics.Manoeuvres.TransferPlanner(earth, mars);
            DateTime startDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            tp.CalculateTransferWindows(startDate, 200, 50);
            Common.timer.Stop();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Calculated 200 transfers in {Common.timer.ElapsedMilliseconds} ms");

            var tWindows = tp.BestByDeltaV().Take(5).ToArray();

            sb.AppendLine($"Calculated {tWindows.Length} transfers");
            //sb.AppendLine($"Fastest transfers:");
            //foreach (Manoeuvre m in tWindows)
            //{
            //    sb.AppendLine($"dt: {m.ComputeTotalDuration().Days} d | dv: {m.ComputeTotalCost().KilometersPerSecond:F3} km/s");
            //}

            tWindows = tp.BestByTransferTime().Take(5).ToArray();
            //sb.AppendLine($"Cheapest transfers:");
            //foreach (Manoeuvre m in tWindows)
            //{
            //    sb.AppendLine($"dt: {m.ComputeTotalDuration().Days} d | dv: {m.ComputeTotalCost().KilometersPerSecond:F3} km/s");
            //}

            TransferData t      = tWindows[0];
            var          tOrbit = t.TransferOrbit();
            sb.AppendLine(tOrbit.ToString());
            
            Debug.Log(sb);
        }
    }
}