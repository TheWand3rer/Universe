using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnitsNet;
using UnityEngine;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

namespace VindemiatrixCollective.Universe.Model
{
    [DebuggerDisplay("Planet {Name}")]
    public class Planet : CelestialBody, IEnumerable<Planet>
    {
        protected List<Planet> Satellites => Orbiters.Values.Cast<Planet>().ToList();
        /// <summary>
        /// Returns a Tuple <starindex, planetIndex, moonIndex>
        /// </summary>
        public (int starIndex, int planetIndex, int? moonIndex) Trace => Type == CelestialBodyType.Planet
            ? (ParentStar.Index, Index, null)
            : (ParentStar.Index, ParentBody.Index, Index);

        public bool HasRings => !string.IsNullOrEmpty(Attributes.TryGet("Rings"));

        public int SatelliteCount => Orbiters.Count;
        public Planet ClosestMoon => (Planet)Orbiters.Values.Aggregate((m1, m2) => m2.OrbitalData.SemiMajorAxis < m1.OrbitalData.SemiMajorAxis ? m2 : m1);

        public Planet FarthestMoon => (Planet)Orbiters.Values.Aggregate((m1, m2) => m2.OrbitalData.SemiMajorAxis > m1.OrbitalData.SemiMajorAxis ? m2 : m1);
        public Planet this[string key] => (Planet)Orbiters[key];
        public Planet this[int index] => (index >= 0) && (index < SatelliteCount) ? (Planet)Orbiters.ElementAt(index).Value : null;
        public override string FullName => $"{Name}";

        public override string Path => Type == CelestialBodyType.Planet
            ? $"{StarSystem.Name}/{ParentStar.Index}/{Name}"
            : $"{StarSystem.Name}/{ParentStar.Index}/{ParentBody.Name}/{Name}";

        public Planet() : base(nameof(Planet), CelestialBodyType.Planet)
        {
        }

        public Planet(string name) : base(name, CelestialBodyType.Planet)
        {
        }

        public void AddPlanets(IEnumerable<Planet> planets)
        {
            AddOrbiters(planets);
        }

        public static Planet CreateFromPhysicalData(string name, PhysicalData physical, OrbitalData orbital, CelestialBody attractor = null)
        {
            Planet newPlanet = new(name)
            {
                PhysicalData = physical,
                OrbitalData = orbital,
            };

#if UNITY_EDITOR
            newPlanet.CopyValues();
#endif
            return newPlanet;
        }
        
        #region Unity_Editor
#if UNITY_EDITOR
        [SerializeField]
        private string AxialTiltDeg;

        [SerializeField]
        private string EccentricityValue;

        [SerializeField]
        private string InclinationDeg;

        [SerializeField]
        private string MassEM;

        [SerializeField]
        private string OrbitalPeriodYears;

        [SerializeField]
        private string RadiusER;

        [SerializeField]
        private string SemiMajorAxisAU;

        [SerializeField]
        private string SiderealRotationDays;

        protected override void CopyValues()
        {
            base.CopyValues();
            MassEM = PhysicalData.Mass.EarthMasses.ToString("0.00");
            RadiusER = (PhysicalData.Radius.Kilometers / UniversalConstants.Physical.EarthRadiusKm).ToString("0.00");
            SemiMajorAxisAU = OrbitalData.SemiMajorAxis.AstronomicalUnits.ToString("0.00");
            EccentricityValue = OrbitalData.Eccentricity.Value.ToString("0.00");
            OrbitalPeriodYears = OrbitalData.Period.Years365.ToString("0.00");
            SiderealRotationDays = OrbitalData.SiderealRotationPeriod.Days.ToString("0.00");
            InclinationDeg = OrbitalData.Inclination.Degrees.ToString("0.00 °");
            AxialTiltDeg = OrbitalData.AxialTilt.Degrees.ToString("0.00 °");
        }
#endif
#endregion

        public IEnumerator<Planet> GetEnumerator()
        {
            return Satellites?.GetEnumerator() ?? Enumerable.Empty<Planet>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        /// Creates a new planet with the physical and orbital characteristics of the Earth at J2000.
        /// It returns the planet only, it must then be parented to an attractor, like the Sun.
        /// </summary>
        public static Planet Earth
        {
            get
            {
                OrbitalData orbital = OrbitalData.From(Length.FromAstronomicalUnits(1.000448828934185),
                    Ratio.FromDecimalFractions(0.01711862906746885),
                    Angle.FromDegrees(7.251513445651153),
                    Angle.FromDegrees(241.097743921078),
                    Angle.FromDegrees(206.0459434316863),
                    Angle.FromDegrees(358.5688856532555),
                    Duration.FromDays(365.5022838235192),
                    Duration.FromHours(23.9344695944), 
                    Angle.FromDegrees(23.4392911));
                PhysicalData physical = PhysicalData.Create(Mass.FromEarthMasses(1),
                    Length.FromKilometers(UniversalConstants.Physical.EarthRadiusKm),
                    Acceleration.FromStandardGravity(1), 
                    Density.FromGramsPerCubicCentimeter(5.51));

                return Planet.CreateFromPhysicalData(nameof(Earth), physical, orbital);
            }
        }

        /// <summary>
        /// Creates a new planet with the physical and orbital characteristics of the Earth at J2000.
        /// It returns the planet only, it must then be parented to an attractor, like the Sun.
        /// </summary>
        public static Planet Mars
        {
            get
            {
                OrbitalData orbital = OrbitalData.From(Length.FromAstronomicalUnits(1.523679),
                        Ratio.FromDecimalFractions(0.093315f),
                        Angle.FromDegrees(5.65),
                        Angle.FromDegrees(249.4238472638976),
                        Angle.FromDegrees(72.062034606933594d),
                        Angle.FromDegrees(23.33),
                        Duration.FromSeconds(5.935431800266414e07),
                        Duration.FromHours(24.622962),
                        Angle.FromDegrees(25.19));
                PhysicalData physical = PhysicalData.Create(Mass.FromKilograms(0.64169e24), 
                    Length.FromKilometers(3396.2),
                    Acceleration.FromMetersPerSecondSquared(3.73), 
                    Density.FromGramsPerCubicCentimeter(3.934));

                return Planet.CreateFromPhysicalData(nameof(Mars), physical, orbital);
            }
        }
    }
}