using System;
using UnitsNet;
using VindemiatrixCollective.Universe.CelestialMechanics;

namespace VindemiatrixCollective.Universe.Tests
{
    public static class TextbookMethods
    {
        public static (double f, double g, double fDot) CalculateTransferParameters(Length r1Mag, Length r2Mag, Length p, Angle deltaV, double gmM3S2)
        {
            // (5.5)  f = 1 - r2 / p × (1 - cos )
            double f = 1 - r2Mag.Meters / p.Meters * (1 - Math.Cos(deltaV.Radians));

            // (5.6)  g = r1 × r2 × sin  / SQRT[ GM × p ]
            double g = r1Mag.Meters * r2Mag.Meters * Math.Sin(deltaV.Radians) / Math.Sqrt(gmM3S2 * p.Meters);

            // (5.7)  fdot = SQRT[ GM / p ] × tan(/2) × [(1 - cos ) / p - 1/r1 - 1/r2 ]
            double fDot = Math.Sqrt(gmM3S2 / p.Meters) * Math.Tan(deltaV.Radians / 2) *
                          ((1 - Math.Cos(deltaV.Radians)) / p.Meters - 1 / r1Mag.Meters - 1 / r2Mag.Meters);

            return (f, g, fDot);
        }

        public static Length CalculateSemiMajorAxisFromSemiLatusRectum(Length p, Length k, Length l, Length m)
        {
            double mAU = m.AstronomicalUnits;
            double kAU = k.AstronomicalUnits;
            double pAU = p.AstronomicalUnits;
            double lAU = l.AstronomicalUnits;

            // (5.12) a = m × k × p / [(2 × m - l2) × p2 + 2 × k × l × p - k2]
            double aAU = mAU * kAU * pAU / ((2 * mAU - lAU * lAU) * pAU * pAU + 2 * kAU * lAU * pAU - kAU * kAU);
            return Length.FromAstronomicalUnits(aAU);
        }


        public static Duration CalculateTransferTime(
            Length p, Length r1Mag, Length r2Mag, Length k, Length l, Length m, Angle deltaV,
            GravitationalParameter gm)
        {
            (double f, double g, double fDot) = CalculateTransferParameters(r1Mag, r2Mag, p, deltaV, gm.M3S2);
            double a = CalculateSemiMajorAxisFromSemiLatusRectum(p, k, l, m).AstronomicalUnits;

            // deltaE is Eccentric Anomaly
            double deltaE    = Math.Acos(1 - r1Mag.AstronomicalUnits / a * (1 - f));
            double sinDeltaE = -r1Mag.AstronomicalUnits * r2Mag.AstronomicalUnits * fDot / Math.Sqrt(gm.Au3S2 * a);
            if (Math.Asin(sinDeltaE) < 0)
            {
                deltaE = 2 * Math.PI - deltaE;
            }

            double t = g + Math.Sqrt(Math.Pow(a, 3) / gm.Au3S2) * (deltaE - Math.Sin(deltaE));

            // Hyperbolic case
            if (a < 0)
            {
                double deltaF = Math.Acosh(1 - r1Mag.Meters / a * (1 - f));
                if (deltaF < 0)
                {
                    deltaF = 2 * Math.PI + deltaF;
                }

                t = g + Math.Sqrt(Math.Pow(-a, 3) / gm.Au3S2) * (Math.Sinh(deltaF) - deltaF);
            }

            return double.IsNaN(t) ? Duration.Zero : Duration.FromSeconds(t);
        }
    }
}