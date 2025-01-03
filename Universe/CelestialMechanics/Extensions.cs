using System;

namespace VindemiatrixCollective.Universe.CelestialMechanics
{
    public static class Extensions
    {
        public static double ToDegrees(this double d)
        {
            return d * (180 / Math.PI);
        }

        public static double ToRadians(this double d)
        {
            return d * (Math.PI / 180);
        }
    }
}