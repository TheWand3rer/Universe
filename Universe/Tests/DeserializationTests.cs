using NUnit.Framework;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Tests
{
    public class DeserializationTests
    {
        private readonly DeserializationHelper dataHelper = new();


        [Test]
        public void PlanetAttributes()
        {
            Galaxy galaxy = dataHelper.LoadSol();

            Planet venus  = galaxy["Sol"][0]["Venus"];

            Assert.AreEqual(CelestialBodyType.Planet.ToString(), venus.Type.ToString(), nameof(CelestialBodyType));
        }

        [Test]
        public void PlanetData()
        {
            Galaxy galaxy = dataHelper.LoadSol();
            
            Planet mars    = galaxy["Sol"][0]["Mars"];
            Planet marsExp = Common.Mars;

            ComparePlanet(marsExp, mars);
        }

        [Test]
        public void SatelliteData()
        {
            Galaxy galaxy = dataHelper.LoadSol();

            Planet io     = galaxy["Sol"][0]["Jupiter"]["Io"];
            Planet ioExp  = Common.Io;

            ComparePlanet(ioExp, io);
        }

        [Test]
        public void SystemTree()
        {
            Galaxy galaxy = dataHelper.LoadSol();

            Planet earth  = galaxy["Sol"][0]["Earth"];

            Assert.AreEqual("Sol A", earth.ParentStar.FullName, nameof(CelestialBody.ParentStar));
            Assert.AreEqual(earth.ParentBody, earth.ParentStar, nameof(CelestialBody.ParentBody));
            Planet moon = earth[0];
            Assert.AreEqual("Moon", moon.Name);
            Assert.AreEqual("Sol", moon.StarSystem.Name, nameof(StarSystem));
            Assert.AreEqual("Sol", earth.StarSystem.Name, nameof(StarSystem));
            Assert.AreEqual("Sol", earth.ParentBody.StarSystem.Name, nameof(StarSystem));
        }

        public void X()
        {
            Galaxy     galaxy = new Galaxy("Milky Way");
            StarSystem sol    = new StarSystem("Sol");

            // Star.Sol and Planet.Earth are included as examples with hardcoded values at J2000.
            // You should create stars and planets with the appropriate constructors or factory methods
            Star   sun   = Star.Sun;
            Planet earth = Planet.Earth;

            galaxy.AddSystem(sol);
            sun.AddPlanet(earth);
            sol.AddStar(sun);
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