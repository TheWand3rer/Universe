// com.vindemiatrixcollective.universe.tests © 2025 Vindemiatrix Collective
// Website and Documentation: https://dev.vindemiatrixcollective.com

#region

using NUnit.Framework;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Data;
using VindemiatrixCollective.Universe.Model;

#endregion

namespace VindemiatrixCollective.Universe.Tests
{
    public class CelestialBodyTests
    {
        private readonly DeserializationHelper dataHelper = new();

        [TestCase(@"{
            ""Name"": ""Earth"",
            ""Attributes"": {
                ""Type"": ""Planet""
                },
            ""PhysicalData"": {
                ""Mass"": 5.97219e+24,
                ""Density"": 5.51,
                ""Radius"": 6378.1,
                ""Gravity"": 9.80665,
                ""Temperature"": 287.6,
                ""HillSphereRadius"": 234.9,
                ""GravitationalParameter"": 398600.435436
            },
            ""OrbitalData"": {
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
            }
        }", 1e-5, "Earth")]
        [TestCase(@"{
                  ""Name"": ""Moon"",
                  ""Attributes"": {
                    ""Type"": ""Planet""
                  },
                  ""PhysicalData"": {
                    ""Mass"": 7.349e+22,
                    ""Density"": 3.3437,
                    ""Radius"": 1738.0,
                    ""Gravity"": 1.62,
                    ""GravitationalParameter"": 4902.800066
                  },
                  ""OrbitalData"": {
                    ""SemiMajorAxis"": {
                      ""v"": 384400.0,
                      ""u"": ""km""
                    },
                    ""Eccentricity"": 0.0549,
                    ""Period"": 27.321582,
                    ""SiderealRotationPeriod"": 655.719864,
                    ""Inclination"": 5.145,
                    ""LongitudeAscendingNode"": 241.2713606974586,
                    ""ArgumentPeriapsis"": 328.980187284116,
                    ""MeanAnomaly"": 238.5779166596645,
                    ""TrueAnomaly"": 235.5936224066131,
                    ""AxialTilt"": 6.67
                  }
                }", 1e-5, "Moon")]
        [TestCase(@"{
              ""Name"": ""Mars"",
              ""Attributes"": {
                ""Type"": ""Planet""
              },
              ""PhysicalData"": {
                ""Mass"": 6.4171e+23,
                ""Density"": 3.933,
                ""Radius"": 3396.19,
                ""Gravity"": 3.71,
                ""Temperature"": 210.0,
                ""HillSphereRadius"": 319.8,
                ""GravitationalParameter"": 42828.375214
              },
              ""OrbitalData"": {
                ""SemiMajorAxis"": 1.523678992938855,
                ""Eccentricity"": 0.09331510145826083,
                ""Period"": 686.9712731789837,
                ""Inclination"": 5.650993606269039,
                ""LongitudeAscendingNode"": 249.4238472638976,
                ""ArgumentPeriapsis"": 72.0620330470033,
                ""MeanAnomaly"": 19.35648274725784,
                ""TrueAnomaly"": 23.33319664407214,
                ""AxialTilt"": 25.19,
                ""SiderealRotationPeriod"": 24.622962
              }
            }", 1e-5, "Mars")]
        public void DeserializePlanet(string input, double tol, string name)
        {
            Planet planet = dataHelper.DeserializeObjectNew<Planet>(input, new PlanetConverter(), new PhysicalDataConverter(),
                                                                    new OrbitalDataConverter());
            Planet expected = name switch
            {
                nameof(Planet.Mars) => Planet.Mars,
                nameof(Planet.Moon) => Planet.Moon,
                _                   => Planet.Earth
            };

            Assert.AreEqual(expected.Name, planet.Name, nameof(CelestialBody.Name));
            PhysicalDataTests.ComparePhysicalData(expected.PhysicalData, planet.PhysicalData, tol);
            OrbitalDataTests.CompareOrbitalData(expected.OrbitalData, planet.OrbitalData, tol);
            Assert.That(expected.Attributes, Is.EquivalentTo(planet.Attributes));
        }

        [TestCase(@"{
          ""Name"": ""Sun"",
          ""SpectralClass"": ""G25V"",
          ""PhysicalData"": {
            ""Mass"": 1.0,
            ""Luminosity"": 1.0,
            ""Age"": 4.6,
            ""Temperature"": 5770.0,
            ""Radius"": 1.0,
            ""Density"": 1.410531,
            ""Gravity"": 274.345891
          }
        }", 1e-6, "Sun")]
        [TestCase(@"{
            ""Id"": ""HIP 70890"",
            ""Name"": ""Proxima"",
            ""SC"": ""M5Ve"",
            ""PhysicalData"": {
              ""l"": 0.0017,
              ""m"": 0.12,
              ""r"": 0.1542,
              ""t"": 3306.0,
              ""g"": 112000.0,
              ""age"": 4.85
            },
            ""OrbitalData"": {
              ""a"": 14666.424758,
              ""p"": { ""v"":547000.0, ""u"": ""yr"" },
              ""e"": 0.5,
              ""i"": 107.6,
              ""lan"": 126.0,
              ""argp"": 72.3
            }
          }", 1e-6, "Proxima")]
        public void DeserializeStar(string input, double tol, string name)
        {
            Star star = dataHelper.DeserializeObjectNew<Star>(input, new StarConverter(), new StellarDataConverter(),
                                                              new OrbitalDataConverter());
            Star expected = name switch
            {
                nameof(Common.Proxima) => Common.Proxima,
                _                      => Star.Sun
            };

            Assert.AreEqual(expected.Name, star.Name, nameof(CelestialBody.Name));
            PhysicalDataTests.ComparePhysicalData(expected.StellarData, star.StellarData, tol);
            if (star.OrbitalData != null)
            {
                OrbitalDataTests.CompareOrbitalData(expected.OrbitalData, star.OrbitalData, tol);
            }

            Assert.That(expected.Attributes, Is.EquivalentTo(star.Attributes));
        }


        public static void ComparePlanet(Planet expected, Planet actual)
        {
            PhysicalData physicalEx     = expected.PhysicalData;
            PhysicalData physicalActual = actual.PhysicalData;
            OrbitalData  orbitalEx      = expected.OrbitalData;
            OrbitalData  orbitalActual  = actual.OrbitalData;

            Assert.AreEqual(expected.Name, actual.Name);
            PhysicalDataTests.ComparePhysicalData(physicalEx, physicalActual, 1e-3);
            OrbitalDataTests.CompareOrbitalData(orbitalEx, orbitalActual, 1e-3);
        }
    }
}