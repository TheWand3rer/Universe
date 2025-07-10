using NUnit.Framework;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Tests
{
    public class DeserializationTests
    {
        private readonly DeserializationHelper dataHelper = new();

        [TestCase("40.7128N,74.0060W", 40.7128, -74.0060)]
        [TestCase("40N, -74W", 40, -74)]
        [TestCase("0, -180", 0, -180)] // Test the (0, -180) coordinate
        [TestCase("0, 180", 0, 180)]   // Test the (0, 180) coordinate
        public void DeserializeCoordinateString(string input, double expectedLatitude, double expectedLongitude)
        {
            GeoCoordinates coords = new(input);
            Assert.AreEqual(expectedLatitude, coords.Latitude, 1e-6, nameof(GeoCoordinates.Latitude));
            Assert.AreEqual(expectedLongitude, coords.Longitude, 1e-6, nameof(GeoCoordinates.Longitude));
        }
    }
}