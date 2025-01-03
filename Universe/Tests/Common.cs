﻿using System;
using System.Diagnostics;
using NUnit.Framework;
using UnitsNet;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;
using Debug = UnityEngine.Debug;

namespace VindemiatrixCollective.Universe.Tests
{
    public static class Common
    {
        public static Stopwatch timer = new();
        public static Galaxy Galaxy;
        public static readonly DateTime J2000 = new(2000, 1, 1, 11, 58, 55, 816, DateTimeKind.Utc);

        internal static OrbitalData MarsElements =>
            OrbitalData.From(Length.FromAstronomicalUnits(1.523679),
                             Ratio.FromDecimalFractions(0.093315f),
                             Angle.FromDegrees(5.65),
                             Angle.FromDegrees(249.4238472638976),
                             Angle.FromDegrees(72.062034606933594d),
                             Angle.FromDegrees(23.33),
                             Duration.FromSeconds(5.935431800266414e07),
                             Duration.FromHours(24.622962),
                             Angle.FromDegrees(25.19));

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
                OrbitalData orbital = OrbitalData.From(Length.FromKilometers(421700), Ratio.FromDecimalFractions(0.0041), Angle.FromDegrees(0.0375),
                                                       Angle.FromDegrees(241.1210503807339), Angle.FromDegrees(127.39925384521484),
                                                       Angle.FromDegrees(13.08436484643558f),
                                                       Duration.FromSeconds(152853.5047), Duration.FromDays(1.77f), Angle.Zero);

                PhysicalData physical = PhysicalData.Create(Density.FromGramsPerCubicCentimeter(3.528), Length.FromKilometers(1821.6),
                                                            GravitationalParameter.FromMass(Mass.FromKilograms(8.931938e22)));
                return Planet.CreateFromPhysicalData("Io", physical, orbital);
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

        internal static Star Sun => Star.Sun;

        public static void ArrayAreEqual(double[] expected, double[] actual, double tolerance, string name)
        {
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], tolerance, name);
            }
        }

        public static void LoadData()
        {
            Debug.Log($"Loaded {Galaxy.Systems.Count} systems");
        }

        public static void VectorsAreEqual(Vector3d expected, Vector3d actual, double tolerance, string name = null)
        {
            Assert.AreEqual(expected.x, actual.x, tolerance, $"{name}.x" ?? string.Empty);
            Assert.AreEqual(expected.y, actual.y, tolerance, $"{name}.y" ?? string.Empty);
            Assert.AreEqual(expected.z, actual.z, tolerance, $"{name}.z" ?? string.Empty);
        }
    }
}