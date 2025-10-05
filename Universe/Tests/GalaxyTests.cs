// VindemiatrixCollective.Universe.Tests © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using NUnit.Framework;
using VindemiatrixCollective.Universe.Model;

#endregion

namespace VindemiatrixCollective.Universe.Tests
{
    public class GalaxyTests
    {
        private readonly DeserializationHelper dataHelper = new();

        [Test]
        public void DeserializeGalaxy()
        {
            Galaxy galaxy = dataHelper.LoadGalaxy();
            Assert.AreEqual(76, galaxy.SystemCount, nameof(Galaxy.SystemCount));
        }

        [Test]
        public void GalaxyFindBody()
        {
            Galaxy galaxy = dataHelper.LoadSol();
            Planet earth  = galaxy.GetBody<Planet>("Sol/Sun/Earth");

            Assert.IsNotNull(earth);
            Assert.AreEqual(earth.Name, "Earth");
        }
    }
}