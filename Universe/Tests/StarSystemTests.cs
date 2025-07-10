using System.Linq;
using NUnit.Framework;
using UnityEngine;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Tests
{
    public class StarSystemTests
    {
        private readonly DeserializationHelper dataHelper = new();

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
            CelestialBodyTests.ComparePlanet(marsExp, mars);
        }


        [Test]
        public void SatelliteData()
        {
            Galaxy galaxy = dataHelper.LoadSol();

            Planet io    = (Planet)galaxy["Sol"][0]["Jupiter"]["Io"];
            Planet ioExp = Common.Io;

            Assert.IsTrue(io.IsSatellite, nameof(Planet.IsSatellite));
            CelestialBodyTests.ComparePlanet(ioExp, io);
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

            // Sol + 8 planets + Moon, Phobos, Deimos, Io, Europa, Triton, Naiad, Pluto = 17
            Assert.AreEqual(17, systemBodies.Length);
            Assert.AreEqual("Sun, Mercury, Venus, Earth, Mars, Jupiter, Saturn, Uranus, Neptune, Pluto, Moon, Phobos, Deimos, Io, Europa, Triton, Naiad, ",
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

            // Sol + 8 planets + Moon, Deimos, Phobos, Io, Europa, Triton, Naiad, Pluto, = 17
            Assert.AreEqual(17, systemBodies.Length);
            Assert.AreEqual("Sun, Mercury, Venus, Earth, Moon, Mars, Phobos, Deimos, Jupiter, Io, Europa, Saturn, Uranus, Neptune, Triton, Naiad, Pluto, ",
                            preOrderVisit, nameof(preOrderVisit));
        }
    }
}