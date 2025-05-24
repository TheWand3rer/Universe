#region

using System;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics
{
    public static class Distance
    {
        public static double AngularSize(double distance, double radius)
        {
            return 2 * Math.Asin(radius / distance);
        }
    }
}