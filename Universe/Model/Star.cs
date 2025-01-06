using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnitsNet;
using UnityEngine;

namespace VindemiatrixCollective.Universe.Model
{
    [Serializable]
    [DebuggerDisplay("{Name}")]
    public class Star : CelestialBody, IEnumerable<Planet>
    {
        public bool HasPlanets => Orbiters.Count > 0;
        public float Distance => (float)StarSystem.DistanceFromSol.LightYears;

        protected List<Planet> Planets => Orbiters.Values.Cast<Planet>().ToList();

        public int PlanetCount => Orbiters.Count;
        public Planet this[string key] => (Planet)Orbiters[key];
        public Planet this[int index] => (index >= 0) && (index < Orbiters.Count) ? (Planet)Orbiters.ElementAt(index).Value : null;

        public override string FullName => $"{StarSystem.Name} {Name}";

        public override string Path => $"{StarSystem.Name}/{ParentStar.Index}";
        public Duration Age { get; set; }
        public Luminosity Luminosity { get; set; }
        public SpectralClass SpectralClass { get; set; }
        public Temperature Temperature { get; set; }

        public Mass CalculatePlanetaryMass()
        {
            Mass sum = Mass.Zero;
            foreach (CelestialBody orbiter in Orbiters.Values)
            {
                sum += orbiter.PhysicalData.Mass;
            }

            return sum;
        }

        public Star() : base(nameof(Star), CelestialBodyType.Star)
        {
        }

        public Star(string name) : base(name, CelestialBodyType.Star)
        {
        }

        public static Star Create(string name, PhysicalData data)
        {
            Star star = new(name)
            {
                PhysicalData = data
            };
#if UNITY_EDITOR
            star.CopyValues();
#endif
            return star;
        }

        public void AddPlanet(Planet planet)
        {
            AddOrbiter(planet);
            planet.ParentStar = this;
            foreach (Planet satellite in planet)
            {
                satellite.ParentStar = this;
            }
        }

        public void AddPlanets(IEnumerable<Planet> planets)
        {
            foreach (Planet planet in planets)
            {
                AddPlanet(planet);
            }
        }

        public IEnumerator<Planet> GetEnumerator()
        {
            return Planets?.GetEnumerator() ?? Enumerable.Empty<Planet>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Creates a new star with the physical characteristics of the Sun.
        /// </summary>
        public static Star Sun
        {
            get
            {
                Mass    mass    = Mass.FromSolarMasses(1);
                Length  radius  = Length.FromSolarRadiuses(1);
                Density density = Density.FromGramsPerCubicCentimeter((3 * mass.Grams) / (4 * UniversalConstants.Tri.Pi * Math.Pow(radius.Centimeters, 3)));
                Acceleration gravity = Acceleration.FromMetersPerSecondSquared(
                    (UniversalConstants.Celestial.GravitationalConstant * mass.Kilograms) / Math.Pow(radius.Meters, 2));

                return Star.Create("Sol", PhysicalData.Create(mass, radius, gravity, density));
            }
        }

        #region UnityEditor

#if UNITY_EDITOR
        protected override void CopyValues()
        {
            if (Planets?.Count > 0)
            {
                PlanetArray = Planets.ToArray();
            }

            MassSM = PhysicalData.Mass.SolarMasses.ToString("0.00");
            LuminositySL = Luminosity.SolarLuminosities.ToString("0.00");
            TemperatureK = Temperature.Kelvins.ToString("0");
            AgeGY = (Age.Years365 / 1E9).ToString("0.00");
            RadiusSR = PhysicalData.Radius.SolarRadiuses.ToString("0.00");
            //Class = SpectralClass.Signature;
            //DistanceFromSol = Length.FromParsecs(Coordinates.magnitude).LightYears.ToString("0.00 LY");
        }
#endif

#if UNITY_EDITOR
        [SerializeField]
        internal string MassSM;

        [SerializeField]
        internal string LuminositySL;

        [SerializeField]
        internal string TemperatureK;

        [SerializeField]
        internal string AgeGY;

        [SerializeField]
        internal string RadiusSR;

        [SerializeField]
        private Planet[] PlanetArray;
#endif

        #endregion
    }
}