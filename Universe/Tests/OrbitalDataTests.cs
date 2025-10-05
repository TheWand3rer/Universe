// VindemiatrixCollective.Universe.Tests © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using NUnit.Framework;
using UnitsNet;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Data;

#endregion

namespace VindemiatrixCollective.Universe.Tests
{
    public class OrbitalDataTests
    {
        private readonly DeserializationHelper dataHelper = new();

        [TestCase(@"{
                ""SemiMajorAxis"": 1.000448828934185,
                ""Eccentricity"": 0.01711862906746885,
                ""Period"": 365.5022838235192,
                ""Inclination"": 7.251513445651153,
                ""LongitudeAscendingNode"": 241.097743921078,
                ""ArgumentPeriapsis"": 206.0459434316863,
                ""MeanAnomaly"": 358.6172562416435,
                ""TrueAnomaly"": 358.5688856532555,
                ""AxialTilt"": 23.4392911,
                ""SiderealRotationPeriod"": 23.9344695944
              }", 1e-6, 1.000448828934185, 0.01711862906746885, 365.5022838235192, 7.251513445651153, 241.097743921078, 206.0459434316863,
                  358.6172562416435, 358.5688856532555, 23.4392911, 23.9344695944, "Earth")]
        [TestCase(@"{
                ""SemiMajorAxis"": {""v"": 1.000448828934185, ""u"": ""au""},
                ""Eccentricity"": {""v"": 0.01711862906746885},
                ""Period"": {""v"": 365.5022838235192, ""u"": ""d""},
                ""Inclination"": {""v"": 7.251513445651153, ""u"": ""deg""},
                ""LongitudeAscendingNode"": {""v"": 241.097743921078, ""u"": ""deg""},
                ""ArgumentPeriapsis"": {""v"": 206.0459434316863, ""u"": ""deg""},
                ""MeanAnomaly"": {""v"": 358.6172562416435, ""u"": ""deg""},
                ""TrueAnomaly"": {""v"": 358.5688856532555, ""u"": ""deg""},
                ""AxialTilt"": {""v"": 23.4392911, ""u"": ""deg""},
                ""SiderealRotationPeriod"": {""v"": 23.9344695944, ""u"": ""h""}
            }", 1e-6, 1.000448828934185, 0.01711862906746885, 365.5022838235192, 7.251513445651153, 241.097743921078, 206.0459434316863,
                  358.6172562416435, 358.5688856532555, 23.4392911, 23.9344695944, "Earth")]
        [TestCase(@"{
            ""a"": 22.160317,
            ""p"": {""v"":79.91, ""u"": ""yr""},
            ""e"": 0.524,
            ""i"": 79.32,
            ""lan"": 204.75,
            ""argp"": 232.3
          }", 1e-6, 22.160317, 0.524, 29167.15, 79.32, 204.75, 232.3, 0,
                  0, 0, 0, "Toliman")]
        public void DeserializeOrbitalData(
            string input, double tol, double a, double e, double p, double i, double lan, double argp, double ma, double ta, double tilt,
            double srp, string body)
        {
            OrbitalData actual = dataHelper.DeserializeObject<OrbitalData>(input, new OrbitalDataConverter());
            OrbitalData expected = new(Length.FromAstronomicalUnits(a), Ratio.FromDecimalFractions(e), Angle.FromDegrees(i),
                                       Angle.FromDegrees(lan), Angle.FromDegrees(argp), Angle.FromDegrees(ta), Duration.FromDays(p),
                                       Duration.FromHours(srp), Angle.FromDegrees(tilt), Angle.FromDegrees(ma));

            CompareOrbitalData(expected, actual, tol, body);
        }

        public static void CompareOrbitalData(OrbitalData expected, OrbitalData actual, double tol, string body)
        {
            Assert.AreEqual(expected.SemiMajorAxis.AstronomicalUnits, actual.SemiMajorAxis.AstronomicalUnits, tol,
                            $"{body}.{actual.SemiMajorAxis}");
            Assert.AreEqual(expected.Eccentricity.DecimalFractions, actual.Eccentricity.DecimalFractions, tol,
                            $"{body}.{actual.Eccentricity}");
            Assert.AreEqual(expected.Period.Days, actual.Period.Days, tol, $"{body}.{actual.Period}");
            Assert.AreEqual(expected.Inclination.Degrees, actual.Inclination.Degrees, tol, $"{body}.{actual.Inclination}");
            Assert.AreEqual(expected.LongitudeAscendingNode.Degrees, actual.LongitudeAscendingNode.Degrees, tol,
                            $"{body}.{actual.LongitudeAscendingNode}");
            Assert.AreEqual(expected.ArgumentPeriapsis.Degrees, actual.ArgumentPeriapsis.Degrees, tol,
                            $"{body}.{actual.ArgumentPeriapsis}");
            Assert.AreEqual(expected.MeanAnomaly.Degrees, actual.MeanAnomaly.Degrees, tol, $"{body}.{actual.MeanAnomaly}");
            Assert.AreEqual(expected.TrueAnomaly.Degrees, actual.TrueAnomaly.Degrees, tol, $"{body}.{actual.TrueAnomaly}");
            Assert.AreEqual(expected.AxialTilt.Degrees, actual.AxialTilt.Degrees, tol, $"{body}.{actual.AxialTilt}");
            Assert.AreEqual(expected.SiderealRotationPeriod.Hours, actual.SiderealRotationPeriod.Hours, tol,
                            $"{body}.{actual.SiderealRotationPeriod}");
        }
    }
}