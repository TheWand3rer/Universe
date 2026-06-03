// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using System;
using System.Runtime.CompilerServices;

#endregion

[assembly: InternalsVisibleTo("VindemiatrixCollective.Universe.Data")]

namespace VindemiatrixCollective.Universe
{
    public static class UniversalConstants
    {
        public struct Physical
        {
            /// <summary>
            ///     Earth equatorial radius from https://arxiv.org/abs/1510.07674
            /// </summary>
            public const double EarthRadiusKm = 6378.1;
        }

        public struct Tri
        {
            public const double DegreeToRad = 0.017453292519943295;
            public const double Pi = 3.141592653589793d;
            public const double Pi2 = 2 * Pi;
            public const double RadToDegree = 180 / Pi;
        }

        public struct Time
        {
            /// <summary>
            /// Represents the number of seconds in a 24-hour day.
            /// </summary>
            public const int SecondsPerDay = SecondsPerHour * 24;

            /// <summary>
            /// Represents the number of seconds in one hour.
            /// </summary>
            public const int SecondsPerHour = 60 * 60;

            /// <summary>
            /// Represents the number of seconds in 1/12 of an astronomic Julian year, defined as 365,25.
            /// </summary>
            /// <remarks>A Julian month is calculated as exactly 1/12 of a Julian year (365,25 days).</remarks>
            public const int SecondsPerJulianMonth = SecondsPerJulianYear / 12;

            /// <summary>
            /// Represents the number of seconds in an astronomic Julian year (365,25 days).
            /// </summary>
            /// <remarks>A Julian year is defined as exactly 365,25 days, or 31.557.600,00 seconds. This
            /// constant is commonly used in astronomical calculations and time conversions.</remarks>
            public const int SecondsPerJulianYear = 31557600;

            /// <summary>
            /// Represents the number of seconds in a non-leap year (365 days).
            /// </summary>
            public const int SecondsPerYear365 = 365 * SecondsPerDay;

            /// <summary>
            /// Represents the J2000 epoch as a UTC date and time.
            /// </summary>
            /// <remarks>The J2000 epoch is commonly used as a reference point in astronomical
            /// calculations. It corresponds to January 1, 2000, at 11:58:55.816 UTC.</remarks>
            public static DateTime J2000 = new(2000, 1, 1, 11, 58, 55, 816, DateTimeKind.Utc);
        }

        public struct Celestial
        {
            public const double AuPerMetre = 6.6845871222684454959959533702106e-12;
            public const double GravitationalConstant = 6.67429E-11; // N⋅m²/kg²
            public const double KmPerAu = 149597870.700;
            public const double KmPerPc = 3.08567758129e13;
            public const double LightSpeedKilometresPerSecond = 299792.458;
            public const double LightSpeedMetresPerSecond = 299792458;
            public const double MetresPerAu = 149597870700;
            public const double SquareAuPerSquareKm = 4.4683704831421e-17;
        }

        public struct Gas
        {
            public const double AvogadroConstant = 6.02214076e23; // mol -1
            public const double BoltzmannConstant = 1.380649e-23; // J/K
        }

        public struct Chemistry
        {
            public const double H = 1.007825;
        }

        public struct Energy
        {
            public const double SolarConstantWm2 = 1361;
        }
    }
}