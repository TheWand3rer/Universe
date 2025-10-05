// VindemiatrixCollective.Universe.Tests © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using System.Diagnostics;
using NUnit.Framework;
using UnitsNet;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;
using Debug = UnityEngine.Debug;

#endregion

namespace VindemiatrixCollective.Universe.Tests
{
    public static class Common
    {
        public static Galaxy Galaxy;
        public static readonly DateTime J2000 = new(2000, 1, 1, 11, 58, 55, 816, DateTimeKind.Utc);
        public static Stopwatch timer = new();

        internal static Planet Earth
        {
            get
            {
                Planet earth = Planet.Earth;
                earth.SetParentBody(Sun);
                return earth;
            }
        }

        internal static Planet Io
        {
            get
            {
                OrbitalData orbital = new(Length.FromKilometers(421700), Ratio.FromDecimalFractions(0.0041), Angle.FromDegrees(0.0375),
                                          Angle.FromDegrees(241.1210503807339), Angle.FromDegrees(127.39925384521484),
                                          Angle.FromDegrees(13.08436484643558f), Duration.FromSeconds(152853.5047),
                                          Duration.FromDays(1.77f), Angle.Zero, Angle.FromDegrees(33.54986953430662));

                PhysicalData physical = new(Density.FromGramsPerCubicCentimeter(3.528), Length.FromKilometers(1821.49),
                                            GravitationalParameter.FromMass(Mass.FromKilograms(8.931938e22)));
                return new Planet("Io", physical, orbital);
            }
        }

        internal static Planet Mars
        {
            get
            {
                Planet mars = Planet.Mars;
                mars.SetParentBody(Sun);
                return mars;
            }
        }

        internal static OrbitalData MarsElements => Planet.Mars.OrbitalData;

        internal static Star Proxima
        {
            get
            {
                Luminosity   luminosity  = Luminosity.FromSolarLuminosities(0.0017);
                Mass         mass        = Mass.FromSolarMasses(0.12);
                Length       radius      = Length.FromSolarRadiuses(0.1542);
                Temperature  temperature = Temperature.FromKelvins(3306);
                Acceleration gravity     = Acceleration.FromMetersPerSecondSquared(112000);
                Duration     age         = Duration.FromYears365(4.85 * 1E9);

                Length   sma          = Length.FromAstronomicalUnits(14666.424758);
                Duration period       = Duration.FromYears365(547000.0);
                Ratio    eccentricity = Ratio.FromDecimalFractions(0.5);
                Angle    inclination  = Angle.FromDegrees(107.6);
                Angle    lan          = Angle.FromDegrees(126.0);
                Angle    argp         = Angle.FromDegrees(72.3);

                OrbitalData orbitalData = new(sma, eccentricity, inclination, lan, argp, Angle.Zero, period);
                Star        proxima     = new("Proxima", new StellarData(luminosity, mass, radius, gravity, temperature, age: age));
                proxima.OrbitalData = orbitalData;
                return proxima;
            }
        }

        internal static Star Sun => Star.Sun;

        public static void ArrayAreEqual(double[] expected, double[] actual, double tolerance, string name)
        {
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], tolerance, name);
            }
        }

        public static void CompareVector3d(Vector3d expected, Vector3d actual, double tolerance, string name)
        {
            ArrayAreEqual(expected.ToArray(), actual.ToArray(), tolerance, name);
        }

        public static void LoadData()
        {
            Debug.Log($"Loaded {Galaxy.SystemCount} systems");
        }

        public static void VectorsAreEqual(Vector3d expected, Vector3d actual, double tolerance, string name = null)
        {
            Assert.AreEqual(expected.x, actual.x, tolerance, $"{name}.x" ?? string.Empty);
            Assert.AreEqual(expected.y, actual.y, tolerance, $"{name}.y" ?? string.Empty);
            Assert.AreEqual(expected.z, actual.z, tolerance, $"{name}.z" ?? string.Empty);
        }
    }
}