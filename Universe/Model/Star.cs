#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnitsNet;
using Unity.Properties;
using UnityEngine;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    [Serializable]
    [DebuggerDisplay("{Name}")]
    public class Star : CelestialBody, IEnumerable<Planet>
    {
        public Star() : base(nameof(Star), CelestialBodyType.Star) { }

        public Star(string name) : base(name, CelestialBodyType.Star) { }

        public Star(string name, PhysicalData data) : this(name)
        {
            PhysicalData = data;
#if UNITY_EDITOR
            CopyValues();
#endif
        }

        public bool HasPlanets => Planets.Any();
        public float Distance => (float)StarSystem.DistanceFromSol.LightYears;

        public int PlanetCount => Planets.Count();
        public Planet this[string key] => Planets.FirstOrDefault(p => p.Name == key);
        public Planet this[int index] => Planets.ElementAt(index);

        public override string FullName => Name.Length > 1 ? Name : $"{StarSystem.Name} {Name}";

        [CreateProperty] public Duration Age { get; set; }
        [CreateProperty] public Luminosity Luminosity { get; set; }
        [CreateProperty] public SpectralClass SpectralClass { get; set; }
        [CreateProperty] public Temperature Temperature { get; set; }


        /// <summary>
        ///     Returns a sequence of Planet objects ordered by distance from this star.
        /// </summary>
        public IEnumerable<Planet> Planets => _Orbiters.Values.OfType<Planet>().OrderBy(o => o.OrbitalData.SemiMajorAxis);

        /// <summary>
        ///     Creates a new star with the physical characteristics of the Sun.
        /// </summary>
        public static Star Sun
        {
            get
            {
                Mass    mass    = Mass.FromSolarMasses(1);
                Length  radius  = Length.FromSolarRadiuses(1);
                Density density = Density.FromGramsPerCubicCentimeter(3 * mass.Grams / (4 * UniversalConstants.Tri.Pi * Math.Pow(radius.Centimeters, 3)));
                Acceleration gravity = Acceleration.FromMetersPerSecondSquared(UniversalConstants.Celestial.GravitationalConstant * mass.Kilograms /
                                                                               Math.Pow(radius.Meters, 2));

                return new Star("Sol", new PhysicalData(mass, radius, gravity, density));
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

        public Mass CalculatePlanetaryMass()
        {
            Mass sum = Mass.Zero;
            foreach (CelestialBody orbiter in _Orbiters.Values)
            {
                sum += orbiter.PhysicalData.Mass;
            }

            return sum;
        }

        #region UnityEditor

#if UNITY_EDITOR
        protected override void CopyValues()
        {
            if (Planets?.Count() > 0)
            {
                PlanetArray = Planets.ToArray();
            }

            MassSM       = PhysicalData.Mass.SolarMasses.ToString("0.00");
            LuminositySL = Luminosity.SolarLuminosities.ToString("0.00");
            TemperatureK = Temperature.Kelvins.ToString("0");
            AgeGY        = (Age.Years365 / 1E9).ToString("0.00");
            RadiusSR     = PhysicalData.Radius.SolarRadiuses.ToString("0.00");
            //Class = SpectralClass.Signature;
            //DistanceFromSol = Length.FromParsecs(Coordinates.magnitude).LightYears.ToString("0.00 LY");
        }

        [SerializeField] internal string MassSM;

        [SerializeField] internal string LuminositySL;

        [SerializeField] internal string TemperatureK;

        [SerializeField] internal string AgeGY;

        [SerializeField] internal string RadiusSR;

        [SerializeField] private Planet[] PlanetArray;
#endif

        #endregion
    }
}