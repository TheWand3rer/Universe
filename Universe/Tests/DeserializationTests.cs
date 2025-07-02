using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Tests
{
    public class DeserializationTests
    {
        private readonly DeserializationHelper dataHelper = new();


        [Test]
        public void DeserializePlanet()
        {
            string earthJson = @"{
            ""Earth"": {
              ""Attributes"": {
                ""Type"": ""Planet"",
                ""Class"": ""Terrestrial"",
                ""Size"": ""Average"",
                ""SurfaceMaterial"": ""System/Planets/Earth"",
                ""AtmosphereType"": ""Oxygen""
              },
              ""Orbiters"": {},
              ""PhysicalData"": {
                ""Mass"": 5.97219e+24,
                ""Density"": 5.51,
                ""Radius"": 6378.137,
                ""Gravity"": 9.7803267715,
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
            }
          }";
            Dictionary<string, Planet> kvp =
                dataHelper.DeserializeObject<Dictionary<string, Planet>>(earthJson, DeserializationHelper.Converters);
            Assert.IsNotNull(kvp);

            Planet earth = kvp["Earth"];
            Debug.Log($"Deserialized Planet <{earth.Name}>");

            ComparePlanet(Common.Earth, earth);
        }

        [Test]
        public void DeserializeStar()
        {
            string proximaJson = @"{
            ""C"": {
              ""Id"": ""HIP 70890"",
              ""Name"": ""Proxima"",
              ""SC"": ""M5Ve"",
              ""PhysicalData"": {
                ""l"": 0.0017,
                ""m"": 0.12,
                ""t"": 3306.0,
                ""g"": 112000.0,
                ""age"": 4.85
              },
              ""OrbitalData"": {
                ""a"": 14666.424758,
                ""P"": 547000.0,
                ""e"": 0.5,
                ""i"": 107.6,
                ""lan"": 126.0,
                ""argp"": 72.3
              }
            }
          }";

            Dictionary<string, Star> kvp =
                dataHelper.DeserializeObject<Dictionary<string, Star>>(proximaJson, DeserializationHelper.Converters);

            Assert.IsNotNull(kvp);

            Star proxima = kvp["C"];

            Assert.IsNotNull(proxima, nameof(Star));
            Assert.AreEqual("Proxima", proxima.Name, nameof(Star.Name));
            Assert.AreEqual("M5V", proxima.SpectralClass.Signature, nameof(Star.SpectralClass));
            Assert.AreEqual(0.0017, proxima.StellarData.Luminosity.SolarLuminosities, 1e-4, nameof(StellarData.Luminosity));
            Assert.AreEqual(0.12, proxima.StellarData.Mass.SolarMasses, 1e-3, nameof(StellarData.Mass));
            Assert.AreEqual(3306, proxima.StellarData.Temperature.Kelvins, 1e-1, nameof(StellarData.Temperature));
            Assert.AreEqual(112000, proxima.StellarData.Gravity.CentimetersPerSecondSquared, 1e-1, nameof(StellarData.Gravity));
        }

        [Test]
        public void GalaxyFindBody()
        {
            Galaxy galaxy = dataHelper.LoadSol();
            Planet earth  = galaxy.GetBody<Planet>("Sol/Sun/Earth");

            Assert.IsNotNull(earth);
            Assert.AreEqual(earth.Name, "Earth");
        }

        [Test]
        public void PlanetAttributes()
        {
            Galaxy     galaxy = dataHelper.LoadSol();
            StarSystem sol    = galaxy["Sol"];
            Star       sun    = sol[0];
            Planet     venus  = sun.GetPlanet("Venus");

            Assert.IsNotNull(venus);
            Assert.AreEqual(nameof(CelestialBodyType.Planet), venus.Type.ToString(), nameof(CelestialBodyType));
        }

        [Test]
        public void PlanetData()
        {
            Galaxy galaxy = dataHelper.LoadSol();

            Planet mars    = (Planet)galaxy["Sol"][0]["Mars"];
            Planet marsExp = Common.Mars;

            Assert.IsFalse(mars.IsSatellite, nameof(Planet.IsSatellite));
            ComparePlanet(marsExp, mars);
        }


        [Test]
        public void SatelliteData()
        {
            Galaxy galaxy = dataHelper.LoadSol();

            Planet io    = (Planet)galaxy["Sol"][0]["Jupiter"]["Io"];
            Planet ioExp = Common.Io;

            Assert.IsTrue(io.IsSatellite, nameof(Planet.IsSatellite));
            ComparePlanet(ioExp, io);
        }

        [Test]
        public void SystemTree()
        {
            Galaxy     galaxy = dataHelper.LoadSol();
            StarSystem sol    = galaxy["Sol"];
            Star       sun    = sol[0];
            Planet     earth  = (Planet)sun["Earth"];
            Planet     moon   = (Planet)earth[0];

            Debug.Log(sol.SystemTree());

            Assert.AreEqual(galaxy, sol.Galaxy, nameof(Galaxy));
            Assert.AreEqual(sun, earth.ParentBody, nameof(CelestialBody.ParentBody));
            Assert.AreEqual(sun, earth.ParentStar, nameof(CelestialBody.ParentStar));
            Assert.AreEqual(earth.ParentBody, earth.ParentStar, nameof(CelestialBody.ParentBody));
            Assert.AreEqual(earth, moon.ParentBody, nameof(CelestialBody.ParentBody));
            Assert.AreEqual(sun, moon.ParentStar, nameof(CelestialBody.ParentStar));
            Assert.AreEqual(sol, moon.StarSystem, nameof(CelestialBody.ParentStar));

            Assert.AreEqual("Milky Way", galaxy.Name, nameof(Galaxy.Name));
            Assert.AreEqual("Moon", moon.Name, nameof(CelestialBody.Name));
            Assert.AreEqual("Sun", earth.ParentStar.FullName, nameof(CelestialBody.ParentStar));
            Assert.AreEqual("Sol", moon.StarSystem.Name, nameof(StarSystem));
            Assert.AreEqual("Sol", earth.StarSystem.Name, nameof(StarSystem));
            Assert.AreEqual("Sol", earth.ParentBody.StarSystem.Name, nameof(StarSystem));

            Assert.AreEqual(sun, earth.OrbitState.Attractor, nameof(OrbitState.Attractor));
            Assert.AreEqual(earth, moon.OrbitState.Attractor, nameof(OrbitState.Attractor));

            Assert.IsTrue(moon.IsSatellite, nameof(Planet.IsSatellite));
        }

        [Test]
        public void VisitLevelOrder()
        {
            Galaxy     galaxy = dataHelper.LoadSol();
            StarSystem solar  = galaxy["Sol"];

            string levelOrderVisit = "";
            solar.VisitHierarchy<CelestialBody>(o => levelOrderVisit += $"{o.Name}, ", Tree.LevelOrderVisit);
            CelestialBody[] systemBodies = solar.Hierarchy.ToArray();

            // Sol + 8 planets + Moon, Phobos, Deimos, Io, Europa, Pluto = 15
            Assert.AreEqual(15, systemBodies.Length);
            Assert.AreEqual("Sun, Mercury, Venus, Earth, Mars, Jupiter, Saturn, Uranus, Neptune, Pluto, Moon, Phobos, Deimos, Io, Europa, ",
                            levelOrderVisit, nameof(levelOrderVisit));
        }

        [Test]
        public void VisitPreOrder()
        {
            Galaxy     galaxy = dataHelper.LoadSol();
            StarSystem solar  = galaxy["Sol"];

            string preOrderVisit = "";
            solar.VisitHierarchy<CelestialBody>(o => preOrderVisit += $"{o.Name}, ");
            CelestialBody[] systemBodies = solar.Hierarchy.ToArray();

            // Sol + 8 planets + Moon, Deimos, Phobos, Io, Europa, Pluto = 15
            Assert.AreEqual(15, systemBodies.Length);
            Assert.AreEqual("Sun, Mercury, Venus, Earth, Moon, Mars, Phobos, Deimos, Jupiter, Io, Europa, Saturn, Uranus, Neptune, Pluto, ",
                            preOrderVisit, nameof(preOrderVisit));
        }

        private void ComparePlanet(Planet expected, Planet actual)
        {
            PhysicalData physicalEx     = expected.PhysicalData;
            PhysicalData physicalActual = actual.PhysicalData;
            OrbitalData  orbitalEx      = expected.OrbitalData;
            OrbitalData  orbitalActual  = actual.OrbitalData;

            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(physicalEx.Mass.EarthMasses, physicalActual.Mass.EarthMasses, 1e-2, nameof(PhysicalData.Mass));
            Assert.AreEqual(physicalEx.Radius.Kilometers / UniversalConstants.Physical.EarthRadiusKm,
                            physicalActual.Radius.Kilometers / UniversalConstants.Physical.EarthRadiusKm, 1e-3,
                            nameof(PhysicalData.Radius));
            Assert.AreEqual(physicalEx.Gravity.StandardGravity, physicalActual.Gravity.StandardGravity, 1e-2,
                            nameof(PhysicalData.Gravity));
            Assert.AreEqual(physicalEx.Density.GramsPerCubicCentimeter / UniversalConstants.Physical.EarthDensityGCm3,
                            physicalActual.Density.GramsPerCubicCentimeter / UniversalConstants.Physical.EarthDensityGCm3, 1e-3,
                            nameof(PhysicalData.Density));

            Assert.AreEqual(orbitalEx.SemiMajorAxis.AstronomicalUnits, orbitalActual.SemiMajorAxis.AstronomicalUnits, 1e-2,
                            nameof(OrbitalData.SemiMajorAxis));
            Assert.AreEqual(orbitalEx.Eccentricity.Value, orbitalActual.Eccentricity.Value, 1e-2, nameof(OrbitalData.Eccentricity));
            Assert.AreEqual(orbitalEx.Period.Days, orbitalActual.Period.Days, 1e-2, nameof(OrbitalData.Period));
            Assert.AreEqual(orbitalEx.SiderealRotationPeriod.Days, orbitalActual.SiderealRotationPeriod.Days, 1e-2,
                            nameof(OrbitalData.SiderealRotationPeriod));
            Assert.AreEqual(orbitalEx.AxialTilt.Degrees, orbitalActual.AxialTilt.Degrees, 1e-2, nameof(OrbitalData.AxialTilt));
            Assert.AreEqual(orbitalEx.LongitudeAscendingNode.Degrees, orbitalActual.LongitudeAscendingNode.Degrees, 1e-2,
                            nameof(OrbitalData.LongitudeAscendingNode));
            Assert.AreEqual(orbitalEx.ArgumentPeriapsis.Degrees, orbitalActual.ArgumentPeriapsis.Degrees, 1e-2,
                            nameof(OrbitalData.ArgumentPeriapsis));
            Assert.AreEqual(orbitalEx.Inclination.Degrees, orbitalActual.Inclination.Degrees, 1e-2, nameof(OrbitalData.Inclination));
            Assert.AreEqual(orbitalEx.TrueAnomalyAtEpoch.Degrees, orbitalActual.TrueAnomalyAtEpoch.Degrees, 1e-2,
                            nameof(OrbitalData.TrueAnomalyAtEpoch));
        }
    }
}