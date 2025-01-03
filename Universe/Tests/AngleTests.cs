using NUnit.Framework;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

namespace VindemiatrixCollective.Universe.Tests
{
    public class AngleTests
    {
        // Data from Schlesinger & Udick, 1912
        private readonly double[][] ellipticAngles =
        {
            // ecc,E (deg), nu(deg)
            new[] { 0.0, 0.0, 0.0 },
            new[] { 0.05, 10.0, 11.06 },
            new[] { 0.06, 30.0, 33.67 },
            new[] { 0.04, 120.0, 123.87 },
            new[] { 0.14, 65.0, 80.50 },
            new[] { 0.19, 21.0, 30.94 },
            new[] { 0.35, 65.0, 105.71 },
            new[] { 0.48, 180.0, 180.0 },
            new[] { 0.75, 125.0, 167.57 },
        };

        [Test]
        public void MeanToTrueAnomaly()
        {
            foreach (double[] row in ellipticAngles)
            {
                double e          = row[0];
                double M          = row[1] * UniversalConstants.Tri.DegreeToRad;
                double expectedNu = row[2];

                double nu = OrbitalMechanics.EccentricToTrueAnomaly(OrbitalMechanics.MeanToEccentricAnomaly(M, e), e);
                Assert.AreEqual(expectedNu, nu * UniversalConstants.Tri.RadToDegree, 1e-2, nameof(OrbitState.TrueAnomaly));
            }
        }

        [Test]
        public void TrueToEccentric()
        {
            double[][] data =
            {
                //# ecc,E (deg), nu (deg)
                new[] { 0.0, 0.0, 0.0 },
                new[] { 0.05, 10.52321, 11.05994 },
                new[] { 0.10, 54.67466, 59.49810 },
                new[] { 0.35, 142.27123, 153.32411 },
                new[] { 0.61, 161.87359, 171.02189 },
            };

            foreach (double[] row in data)
            {
                double e = row[0];
                double expectedE = row[1];
                double nu = row[2] * UniversalConstants.Tri.DegreeToRad;

                double E = OrbitalMechanics.TrueToEccentricAnomaly(nu, e);
                Assert.AreEqual(expectedE, E * UniversalConstants.Tri.RadToDegree, 1e-2, nameof(OrbitState.EccentricAnomaly));
            }
        }

        [Test]
        public void TrueToMeanAnomaly()
        {
            foreach (double[] row in ellipticAngles)
            {
                double e = row[0];
                double expectedM = row[1];
                double nu = row[2] * UniversalConstants.Tri.DegreeToRad;

                double M = OrbitalMechanics.EccentricToMeanAnomaly(OrbitalMechanics.TrueToEccentricAnomaly(nu, e), e);
                Assert.AreEqual(expectedM, M * UniversalConstants.Tri.RadToDegree, 1e-2, "MeanAnomaly");
            }
        }
    }
}