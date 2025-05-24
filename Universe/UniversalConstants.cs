#region

using System;

#endregion

namespace VindemiatrixCollective.Universe
{
    public static class UniversalConstants
    {
        public struct Physical
        {
            public const double EarthDensityGCm3 = 5.515;

            /// <summary>
            ///     Earth equatorial radius from https://arxiv.org/abs/1510.07674
            /// </summary>
            public const double EarthRadiusKm = 6378.1;
        }

        public struct Tri
        {
            public const double Pi = 3.141592653589793d;
            public const double Pi2 = 2 * Pi;
            public const double RadToDegree = 180 / Pi;
            public const double DegreeToRad = 0.017453292519943295;
        }

        public struct Time
        {
            public const int SecondsPerDay = SecondsPerHour * 24;
            public const int SecondsPerHour = 60 * 60;
            public const int SecondsPerJulianYear = 31557600;
            public static DateTime J2000 = new(2000, 1, 1, 11, 58, 55, 816, DateTimeKind.Utc);
        }

        public struct Celestial
        {
            public const double AuPerMetre = 6.6845871222684454959959533702106e-12;
            public const double GravitationalConstant = 6.67429E-11;
            public const double KmPerAu = 149597870.700;
            public const double MetresPerAu = 149597870700;
            public const double LightSpeedKilometresPerSecond = 299792.458;
            public const double LightSpeedMetresPerSecond = 299792458;
        }

        public struct Energy
        {
            public const double SolarConstantWm2 = 1361;
        }
    }
}