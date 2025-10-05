// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using UnitsNet;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics
{
    public readonly struct GravitationalParameter
    {
        public const double GravitationalConstant = UniversalConstants.Celestial.GravitationalConstant;

        public double Au3S2 => M3S2 / Math.Pow(UniversalConstants.Celestial.MetresPerAu, 3);

        public double Au3Y2
        {
            get
            {
                double timeScale     = Math.Pow(UniversalConstants.Time.SecondsPerJulianYear, 2);
                double distanceScale = Math.Pow(UniversalConstants.Celestial.MetresPerAu, 3);

                return M3S2 * (timeScale / distanceScale);
            }
        }

        public double Km3S2 => M3S2 / Math.Pow(1000, 3);

        public double M3S2 { get; }

        public GravitationalParameter(double value)
        {
            M3S2 = value;
        }

        public static GravitationalParameter Earth => new(3.9860044188e14);
        public static GravitationalParameter Mars => new(4.2828372e13);

        public static GravitationalParameter Sun => new(1.32712440041279419e20);

        public static GravitationalParameter FromMass(Mass mass) => new(GravitationalConstant * mass.Kilograms);

        public static GravitationalParameter FromKm3S2(double value) => new(value * Math.Pow(1000, 3));
    }
}