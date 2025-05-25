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
        public void BuildSolarSystem()
        {
            Galaxy     galaxy = new("Milky Way");
            StarSystem sol    = new("Sol");
            Star       sun    = Star.Sun;
            Planet     earth  = Planet.Earth;

            sun.AddOrbiter(earth);
            sol.AddOrbiter(sun);
            galaxy.AddSystem(sol);

            Assert.IsTrue(earth.ParentBody == sun, nameof(CelestialBody.ParentBody));
            Assert.IsTrue(earth.ParentStar == sun, nameof(CelestialBody.ParentStar));
            Assert.IsTrue(earth.StarSystem == sol, nameof(CelestialBody.StarSystem));
        }

        [Test]
        public void LevelOrderVisit()
        {
            Galaxy     galaxy = dataHelper.LoadSol();
            StarSystem solar  = galaxy["Sol"];

            string levelOrderVisit = "";
            solar.VisitHierarchy<CelestialBody>(o => levelOrderVisit += $"{o.Name}, ", CelestialBody.LevelOrderVisit);
            CelestialBody[] systemBodies = solar.Hierarchy.ToArray();

            // Sol + 9 planets + Moon, Io, Europa = 13
            Assert.AreEqual(13, systemBodies.Length);
            Assert.AreEqual("Sol, Mercury, Venus, Earth, Mars, Jupiter, Saturn, Uranus, Neptune, Pluto, Moon, Io, Europa, ", levelOrderVisit, nameof(levelOrderVisit));
        }

        [Test]
        public void PlanetAttributes()
        {
            Galaxy     galaxy = dataHelper.LoadSol();
            StarSystem solar  = galaxy["Sol"];
            Star       sol    = solar[0];
            Planet     venus  = sol["Venus"];

            Assert.IsNotNull(venus);
            Assert.AreEqual(nameof(CelestialBodyType.Planet), venus.Type.ToString(), nameof(CelestialBodyType));
        }

        [Test]
        public void PlanetData()
        {
            Galaxy galaxy = dataHelper.LoadSol();

            Planet mars    = galaxy["Sol"][0]["Mars"];
            Planet marsExp = Common.Mars;

            Assert.IsFalse(mars.IsSatellite, nameof(Planet.IsSatellite));
            ComparePlanet(marsExp, mars);
        }

        [Test]
        public void PreOrderVisit()
        {
            Galaxy     galaxy = dataHelper.LoadSol();
            StarSystem solar  = galaxy["Sol"];

            string preOrderVisit = "";
            solar.VisitHierarchy<CelestialBody>(o => preOrderVisit += $"{o.Name}, ");
            CelestialBody[] systemBodies = solar.Hierarchy.ToArray();

            // Sol + 9 planets + Moon, Io, Europa = 13
            Assert.AreEqual(13, systemBodies.Length);
            Assert.AreEqual("Sol, Mercury, Venus, Earth, Moon, Mars, Jupiter, Io, Europa, Saturn, Uranus, Neptune, Pluto, ", preOrderVisit, nameof(preOrderVisit));
        }


        [Test]
        public void SatelliteData()
        {
            Galaxy galaxy = dataHelper.LoadSol();

            Planet io    = galaxy["Sol"][0]["Jupiter"]["Io"];
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
            Planet     earth  = sun["Earth"];
            Planet     moon   = earth[0];

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
            Assert.AreEqual("Sol", earth.ParentStar.FullName, nameof(CelestialBody.ParentStar));
            Assert.AreEqual("Sol", moon.StarSystem.Name, nameof(StarSystem));
            Assert.AreEqual("Sol", earth.StarSystem.Name, nameof(StarSystem));
            Assert.AreEqual("Sol", earth.ParentBody.StarSystem.Name, nameof(StarSystem));

            Assert.AreEqual(sun, earth.OrbitState.Attractor, nameof(OrbitState.Attractor));
            Assert.AreEqual(earth, moon.OrbitState.Attractor, nameof(OrbitState.Attractor));

            Assert.IsTrue(moon.IsSatellite, nameof(Planet.IsSatellite));
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
                            physicalActual.Radius.Kilometers / UniversalConstants.Physical.EarthRadiusKm, 1e-3, nameof(PhysicalData.Radius));
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