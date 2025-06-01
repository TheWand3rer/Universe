using NUnit.Framework;
using UnityEngine;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Tests
{
    public class RuntimeSystemTests : ScriptableObject
    {
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
    }
}