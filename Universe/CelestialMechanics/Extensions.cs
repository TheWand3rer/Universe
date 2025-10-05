// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics
{
    public static class Extensions
    {
        public static double ToDegrees(this double d) => d * (180 / Math.PI);

        public static double ToRadians(this double d) => d * (Math.PI / 180);
    }
}