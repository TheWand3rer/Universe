using NUnit.Framework;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Tests
{
    public class CoordinatesTests
    {
        [Test]
        public void CoordinatesEquality()
        {
            GeoCoordinates coords1 = new(34.56700001, -118.123000009);
            GeoCoordinates coords2 = new(34.56700000, -118.123000001);
            Assert.IsTrue(coords1.Equals(coords2), nameof(Equals));
            Assert.IsTrue(coords1 == coords2, "==");
        }

        [Test]
        public void CoordinatesFromDouble()
        {
            GeoCoordinates coords = new(34.5, -118.0);
            Assert.AreEqual(34.5, coords.Latitude, nameof(GeoCoordinates.Latitude));
            Assert.AreEqual(-118.0, coords.Longitude, nameof(GeoCoordinates.Longitude));
        }

        [TestCase("90N,0E", 0, 1, 0)]
        [TestCase("0N,0E", 0, 0, 1)]
        [TestCase("0N,90E", 1, 0, 0)]
        public void GeoToCartesian(string input, double exp_x, double exp_y, double exp_z)
        {
            GeoCoordinates coords    = new(input);
            Vector3d       cartesian = coords.ToCartesian(1);
            Vector3d       expected  = new(exp_x, exp_y, exp_z);

            Common.VectorsAreEqual(expected, cartesian, 1e-6, nameof(cartesian));
        }
    }
}