// com.vindemiatrixcollective.universe.tests © 2025 Vindemiatrix Collective
// Website and Documentation: https://dev.vindemiatrixcollective.com

#region

using NUnit.Framework;
using UnitsNet;
using VindemiatrixCollective.Universe.Data;
using VindemiatrixCollective.Universe.Model;

#endregion

namespace VindemiatrixCollective.Universe.Tests
{
    public class PhysicalDataTests
    {
        private readonly DeserializationHelper dataHelper = new();

        [TestCase(@"{
                ""Mass"": 3.302e+23, 
                ""Density"": 5.427,
                ""Radius"": 2440.53,
                ""Gravity"": 3.701,
                ""Temperature"": 440.0,
                ""HillSphereRadius"": 94.4,
                ""GravitationalParameter"": 22031.86855 
            }", 1e-6, 3.302e+23, 5.427, 2440.53, 3.701, 440.0, 94.4, 22031.86855)]
        [TestCase(@"{
                ""Mass"": 3.302e+23, 
                ""Radius"": 2440.53,
                ""Gravity"": 3.701
            }", 1e-6, 3.302e+23, 0, 2440.53, 3.701, 0, 0, 0)]
        [TestCase(@"{
                ""Density"": 5.427,
                ""Radius"": 2440.53,
                ""GravitationalParameter"": 22031.86855 
            }", 1e-2, 3.202e+23, 5.427, 2440.53, 3.701, 0, 0, 0)]
        [TestCase(@"{
                ""Mass"": {""v"": 3.302e+23, ""u"": ""kg""}, 
                ""Density"": {""v"": 5.427, ""u"": ""g/cm3""},
                ""Radius"": {""v"": 2440.53, ""u"": ""km""},
                ""Gravity"":{""v"": 3.701, ""u"": ""m/s2""},
                ""Temperature"": {""v"": 440.0, ""u"": ""K""},
                ""HillSphereRadius"": 94.4,
                ""GravitationalParameter"": 22031.86855 
            }", 1e-6, 3.302e+23, 5.427, 2440.53, 3.701, 440.0, 94.4, 22031.86855)]
        public void DeserializeCompleteData(
            string input, double tol, double mass, double density, double radius, double g, double temp, double hsr, double gm)
        {
            PhysicalData physicalData = dataHelper.DeserializeObjectNew<PhysicalData>(input, new PhysicalDataConverter());
            PhysicalData expected = new(Mass.FromKilograms(mass), Length.FromKilometers(radius), Acceleration.FromMetersPerSecondSquared(g),
                                        Density.FromGramsPerCubicCentimeter(density), Temperature.FromKelvins(temp));
            ComparePhysicalData(expected, physicalData, tol);
        }

        [TestCase(@"{
            ""Mass"": 1.0,
            ""Luminosity"": 1.0,
            ""Age"": 4.6,
            ""Temperature"": 5770.0,
            ""Radius"": 1.0
          }", 1e-6, 1, 1, 4.6, 5770, 1.0, 0)]
        [TestCase(@"{
              ""l"": 1.519,
              ""m"": 1.1,
              ""t"": 5271.0,
              ""g"": 10620.0
            }", 1e-6, 1.1, 1.519, 0.0, 5271, 0.0, 10620)]
        [TestCase(@"{
              ""l"": { ""v"": 1.519, ""u"": ""sl"" },
              ""m"": { ""v"": 1.1, ""u"": ""sm"" },
              ""t"": { ""v"": 5271.0, ""u"": ""K"" },
              ""g"": { ""v"": 10620.0, ""u"": ""m/s2"" }
            }", 1e-6, 1.1, 1.519, 0.0, 5271, 0.0, 10620)]
        public void DeserializeCompleteStellarData(
            string input, double tol, double mass, double lum, double age, double temp, double radius, double gravity)
        {
            StellarData stellarData = dataHelper.DeserializeObjectNew<StellarData>(input, new StellarDataConverter());
            StellarData expected = new(Luminosity.FromSolarLuminosities(lum), Mass.FromSolarMasses(mass), Length.FromSolarRadiuses(radius),
                                       Acceleration.FromMetersPerSecondSquared(gravity), Temperature.FromKelvins(temp), Density.Zero,
                                       Duration.FromYears365(age * 1E9));
            CompareStellarData(expected, stellarData, tol);
        }


        //[TestCase(@"{
        //        ""Mass"": 3.302e+23, 
        //        ""Gravity"": 3.701
        //    }")]
        //[TestCase(@"{
        //        ""Density"": 5.427, 
        //        ""Gravity"": 3.701
        //    }")]
        //public void DeserializeIncompleteData(string input)
        //{
        //    Assert.Throws<ArgumentException>(() => dataHelper.DeserializeObjectNew<PhysicalData>(input, new PhysicalDataConverter()));
        //}

        public static void CompareStellarData(StellarData expected, StellarData actual, double tol)
        {
            Assert.IsNotNull(actual, nameof(StellarData));
            Assert.AreEqual(expected.Luminosity.SolarLuminosities, actual.Luminosity.SolarLuminosities, tol, nameof(Luminosity));
            Assert.AreEqual(expected.Mass.SolarMasses, actual.Mass.SolarMasses, tol, nameof(Mass));
            Assert.AreEqual(expected.Temperature.Kelvins, actual.Temperature.Kelvins, tol, nameof(Temperature));
            Assert.AreEqual(expected.Age.Years365, actual.Age.Years365, tol, nameof(StellarData.Age));

            if (expected.Radius.Value > 0)
            {
                Assert.AreEqual(expected.Radius.SolarRadiuses, actual.Radius.SolarRadiuses, tol, nameof(PhysicalData.Radius));
            }

            if (expected.Gravity.Value > 0)
            {
                Assert.AreEqual(expected.Gravity.StandardGravity, actual.Gravity.StandardGravity, 0.05, nameof(StellarData.Gravity));
            }
        }

        public static void ComparePhysicalData(PhysicalData expected, PhysicalData actual, double tol)
        {
            Assert.IsNotNull(actual, nameof(PhysicalData));
            Assert.IsNotNull(expected, nameof(PhysicalData));
            Assert.AreEqual(expected.Mass.EarthMasses, actual.Mass.EarthMasses, tol, nameof(Mass));
            Assert.AreEqual(expected.Density.GramsPerCubicCentimeter, actual.Density.GramsPerCubicCentimeter, 1e-3, nameof(Density));
            Assert.AreEqual(expected.Radius.Kilometers, actual.Radius.Kilometers, tol, nameof(PhysicalData.Radius));
            Assert.AreEqual(expected.Gravity.MetersPerSecondSquared, actual.Gravity.MetersPerSecondSquared, tol,
                            nameof(PhysicalData.Gravity));
            Assert.AreEqual(expected.Temperature.Kelvins, actual.Temperature.Kelvins, tol, nameof(Temperature));
        }
    }
}