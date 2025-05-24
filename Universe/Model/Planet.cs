#region

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnitsNet;
using UnityEngine;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    [DebuggerDisplay("Planet {Name}")]
    public class Planet : CelestialBody, IEnumerable<Planet>
    {
        public bool HasRings => !string.IsNullOrEmpty(Attributes.TryGet("Rings"));

        public bool IsSatellite => ParentBody is { Type: CelestialBodyType.Planet };

        /// <summary>
        ///     Returns a sequence of Planet (satellite) objects order by distance from this Planet.
        /// </summary>
        public IEnumerable<Planet> Satellites => _Orbiters.Values.OfType<Planet>().OrderBy(o => o.OrbitalData.SemiMajorAxis);

        public int SatelliteCount => Satellites.Count();
        public Planet ClosestMoon => Satellites.Aggregate((m1, m2) => m2.OrbitalData.SemiMajorAxis < m1.OrbitalData.SemiMajorAxis ? m2 : m1);

        public Planet FarthestMoon => Satellites.Aggregate((m1, m2) => m2.OrbitalData.SemiMajorAxis > m1.OrbitalData.SemiMajorAxis ? m2 : m1);
        public Planet this[string key] => Satellites.FirstOrDefault(p => p.Name == key);
        public Planet this[int index] => Satellites.ElementAt(index);
        public override string FullName => $"{Name}";


        public Planet() : base(nameof(Planet), CelestialBodyType.Planet) { }

        public Planet(string name) : base(name, CelestialBodyType.Planet) { }

        public Planet(string name, PhysicalData physical, OrbitalData orbital, CelestialBody attractor = null) : this(name)
        {
            PhysicalData = physical;
            OrbitalData  = orbital;
#if UNITY_EDITOR
            CopyValues();
#endif
        }

        public new IEnumerator<Planet> GetEnumerator()
        {
            return Satellites?.GetEnumerator() ?? Enumerable.Empty<Planet>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Creates a new planet with the physical and orbital characteristics of the Earth at J2000.
        ///     It returns the planet only, it must then be parented to an attractor, like the Sun.
        /// </summary>
        public static Planet Earth
        {
            get
            {
                OrbitalData orbital = new(Length.FromAstronomicalUnits(1.000448828934185),
                                          Ratio.FromDecimalFractions(0.01711862906746885),
                                          Angle.FromDegrees(7.251513445651153),
                                          Angle.FromDegrees(241.097743921078),
                                          Angle.FromDegrees(206.0459434316863),
                                          Duration.FromDays(365.5022838235192),
                                          Duration.FromHours(23.9344695944),
                                          Angle.FromDegrees(23.4392911),
                                          Angle.FromDegrees(358.5688856532555),
                                          Angle.FromDegrees(358.6172562416435));
                PhysicalData physical = new(Mass.FromEarthMasses(1),
                                            Length.FromKilometers(UniversalConstants.Physical.EarthRadiusKm),
                                            Acceleration.FromStandardGravity(1),
                                            Density.FromGramsPerCubicCentimeter(5.51));

                return new Planet(nameof(Earth), physical, orbital);
            }
        }

        public static Planet Luna
        {
            get
            {
                OrbitalData orbital = new(Length.FromKilometers(384400.0),
                                          Ratio.FromDecimalFractions(0.0549),
                                          Angle.FromDegrees(5.145),
                                          Angle.FromDegrees(241.2713606974586),
                                          Angle.FromDegrees(328.980187284116),
                                          Duration.FromDays(27.321582),
                                          Duration.FromDays(708.7344),
                                          Angle.FromDegrees(6.67),
                                          Angle.FromDegrees(235.5936224066131),
                                          Angle.FromDegrees(238.5779166596645));
                PhysicalData physical = new(Mass.FromKilograms(7.349e22),
                                            Length.FromKilometers(1738.0),
                                            Acceleration.FromMetersPerSecondSquared(1.62),
                                            Density.FromGramsPerCubicCentimeter(3.3437));

                return new Planet(nameof(Luna), physical, orbital);
            }
        }

        /// <summary>
        ///     Creates a new planet with the physical and orbital characteristics of the Earth at J2000.
        ///     It returns the planet only, it must then be parented to an attractor, like the Sun.
        /// </summary>
        public static Planet Mars
        {
            get
            {
                OrbitalData orbital = new(Length.FromAstronomicalUnits(1.523679),
                                          Ratio.FromDecimalFractions(0.093315f),
                                          Angle.FromDegrees(5.65),
                                          Angle.FromDegrees(249.4238472638976),
                                          Angle.FromDegrees(72.062034606933594d),
                                          Duration.FromSeconds(5.935431800266414e07),
                                          Duration.FromHours(24.622962),
                                          Angle.FromDegrees(25.19),
                                          Angle.FromDegrees(23.33),
                                          Angle.FromDegrees(19.35648274725784));
                PhysicalData physical = new(Mass.FromKilograms(0.64169e24),
                                            Length.FromKilometers(3396.2),
                                            Acceleration.FromMetersPerSecondSquared(3.73),
                                            Density.FromGramsPerCubicCentimeter(3.934));

                return new Planet(nameof(Mars), physical, orbital);
            }
        }

        public static Planet Moon => Luna;

        #region Unity_Editor

#if UNITY_EDITOR
        [SerializeField] private string AxialTiltDeg;

        [SerializeField] private string EccentricityValue;

        [SerializeField] private string InclinationDeg;

        [SerializeField] private string MassEM;

        [SerializeField] private string OrbitalPeriodYears;

        [SerializeField] private string RadiusER;

        [SerializeField] private string SemiMajorAxisAU;

        [SerializeField] private string SiderealRotationDays;

        protected override void CopyValues()
        {
            base.CopyValues();
            MassEM               = PhysicalData.Mass.EarthMasses.ToString("0.00");
            RadiusER             = (PhysicalData.Radius.Kilometers / UniversalConstants.Physical.EarthRadiusKm).ToString("0.00");
            SemiMajorAxisAU      = OrbitalData.SemiMajorAxis.AstronomicalUnits.ToString("0.00");
            EccentricityValue    = OrbitalData.Eccentricity.Value.ToString("0.00");
            OrbitalPeriodYears   = OrbitalData.Period.Years365.ToString("0.00");
            SiderealRotationDays = OrbitalData.SiderealRotationPeriod.Days.ToString("0.00");
            InclinationDeg       = OrbitalData.Inclination.Degrees.ToString("0.00 °");
            AxialTiltDeg         = OrbitalData.AxialTilt.Degrees.ToString("0.00 °");
        }
#endif

        #endregion
    }
}