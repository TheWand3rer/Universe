// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine.Assertions;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    public readonly struct GeoCoordinates : IEquatable<GeoCoordinates>
    {
        public static GeoCoordinates North = new(90, 0);
        public static GeoCoordinates South = new(-90, 0);

        public double Latitude { get; }

        public double Longitude { get; }

        public GeoCoordinates(double latitude, double longitude)
        {
            Assert.IsFalse(latitude is < -90 or > 90, $"{nameof(latitude)} not in range: {latitude}");
            Assert.IsFalse(longitude is < -180 or > 180, $"{nameof(longitude)} not in range: {longitude}");
            Latitude  = latitude;
            Longitude = longitude;
        }

        public GeoCoordinates(string coordinates)
        {
            const string pattern = @"^(?<latNum>-?\d+[.0-9]*)(?<latDir>[NS]*),\s*(?<lonNum>-?\d+[.0-9]*)(?<lonDir>[EW]*)$";
            Regex        re      = new(pattern, RegexOptions.Compiled);
            Match        match   = re.Match(coordinates);
            if (match.Success)
            {
                double latitude = double.Parse(match.Groups["latNum"].Value, CultureInfo.InvariantCulture);
                char   latDir   = 'N';
                if (match.Groups["latDir"].Success && !string.IsNullOrEmpty(match.Groups["latDir"].Value))
                {
                    latDir = char.ToUpper(match.Groups["latDir"].Value[0]);
                }

                char lonDir = 'E';
                if (match.Groups["lonDir"].Success && !string.IsNullOrEmpty(match.Groups["lonDir"].Value))
                {
                    lonDir = char.ToUpper(match.Groups["lonDir"].Value[0]);
                }

                double longitude = double.Parse(match.Groups["lonNum"].Value, CultureInfo.InvariantCulture);

                // Apply the sign based on direction
                if (char.ToUpper(latDir) == 'S' && latitude > 0)
                {
                    latitude *= -1;
                }

                if (char.ToUpper(lonDir) == 'W' && longitude > 0)
                {
                    longitude *= -1;
                }

                this = new GeoCoordinates(latitude, longitude);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(GeoCoordinates)}: cannot parse <{coordinates}>");
            }
        }


        // Equality members
        public bool Equals(GeoCoordinates other)
        {
            const double epsilon = 1e-5;
            return Math.Abs(Latitude - other.Latitude) < epsilon && Math.Abs(Longitude - other.Longitude) < epsilon;
        }

        public override bool Equals(object? obj) => obj is GeoCoordinates other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Latitude.GetHashCode(), Longitude.GetHashCode());

        public override string ToString()
        {
            char latDir = Latitude >= 0 ? 'N' : 'S';
            char lonDir = Longitude >= 0 ? 'E' : 'W';
            return $"{Math.Abs(Latitude):F6}{latDir}, {Math.Abs(Longitude):F6}{lonDir}";
        }

        public Vector3d ToCartesian(double radius) => OrbitalMechanics.GeoToCartesian(Latitude, Longitude, radius);

        public static GeoCoordinates East => new(0, 0);
        public static GeoCoordinates West => new(0, 180);


        public static bool operator ==(GeoCoordinates left, GeoCoordinates right) => left.Equals(right);

        public static bool operator !=(GeoCoordinates left, GeoCoordinates right) => !(left == right);
    }
}