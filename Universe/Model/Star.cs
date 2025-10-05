// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;
using Unity.Properties;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    [Serializable]
    [DebuggerDisplay("{Name}")]
    public class Star : CelestialBody
    {
        public bool HasPlanets => Planets.Any();
        public float Distance => (float)StarSystem.DistanceFromSol.LightYears;


        /// <summary>
        ///     Returns a sequence of Planet objects ordered by distance from this star.
        /// </summary>
        public IEnumerable<Planet> Planets => _Orbiters.Values.OfType<Planet>().OrderBy(o => o.OrbitalData.SemiMajorAxis);

        public int PlanetCount => Planets.Count();

        public override PhysicalData PhysicalData
        {
            get => StellarData;
            set => StellarData = (StellarData)value;
        }

        [CreateProperty] public SpectralClass SpectralClass { get; set; }

        [CreateProperty] public StellarData StellarData { get; set; }
        public override string FullName => Name.Length > 2 ? Name : $"{StarSystem.Name} {Name}";
        public Star() : base(nameof(Star), CelestialBodyType.Star) { }

        public Star(string name) : base(name, CelestialBodyType.Star) { }

        public Star(string name, StellarData data) : this(name)
        {
            StellarData = data;
#if UNITY_EDITOR
            CopyPhysicalValues();
#endif
        }

        public Mass CalculatePlanetaryMass()
        {
            Mass sum = Mass.Zero;
            foreach (CelestialBody orbiter in _Orbiters.Values)
            {
                sum += orbiter.PhysicalData.Mass;
            }

            return sum;
        }

        public Planet GetPlanet(string key)
        {
            return Planets.FirstOrDefault(p => p.Name == key);
        }

        public Star Clone()
        {
            Star newStar = new(Name, StellarData) { SpectralClass = SpectralClass };
            if (OrbitalData != null)
            {
                newStar.OrbitalData = OrbitalData;
            }

            return newStar;
        }

        /// <summary>
        ///     Creates a new star with the physical characteristics of the Sun.
        /// </summary>
        public static Star Sun
        {
            get
            {
                Luminosity luminosity = Luminosity.FromSolarLuminosities(1);
                Mass       mass       = Mass.FromSolarMasses(1);
                Length     radius     = Length.FromSolarRadiuses(1);
                Density density =
                    Density.FromGramsPerCubicCentimeter(3 * mass.Grams / (4 * UniversalConstants.Tri.Pi * Math.Pow(radius.Centimeters, 3)));
                Temperature temperature = Temperature.FromKelvins(5770);
                Acceleration gravity =
                    Acceleration.FromMetersPerSecondSquared(UniversalConstants.Celestial.GravitationalConstant * mass.Kilograms
                                                          / Math.Pow(radius.Meters, 2));
                Duration age = Duration.FromYears365(4.6 * 1E9);

                return new Star("Sun", new StellarData(luminosity, mass, radius, gravity, temperature, density, age));
            }
        }

        #region UnityEditor

#if UNITY_EDITOR

        [Serializable]
        public struct StellarDataValues
        {
            public string Age;
            public string Luminosity;
            public string Mass;
            public string Radius;
            public string Temperature;
        }

        public StellarDataValues stellarDataValues;

        private void CopyPhysicalValues()
        {
            string massUnit   = Mass.GetAbbreviation(MassUnit.SolarMass);
            string radiusUnit = Length.GetAbbreviation(LengthUnit.SolarRadius);
            string lumUnit    = Luminosity.GetAbbreviation(LuminosityUnit.SolarLuminosity);

            stellarDataValues.Mass        = StellarData.Mass.EarthMasses.ToString($"0.00 {massUnit}");
            stellarDataValues.Radius      = StellarData.Radius.SolarRadiuses.ToString($"0.00 {radiusUnit}");
            stellarDataValues.Temperature = StellarData.Temperature.ToString("0.00 K");
            stellarDataValues.Luminosity  = StellarData.Luminosity.SolarLuminosities.ToString($"0.00 {lumUnit}");
            stellarDataValues.Age         = (StellarData.Age.Years365 / 1E9).ToString("0.00 Gy");
        }

#endif

        #endregion
    }
}