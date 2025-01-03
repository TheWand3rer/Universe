using System;
using UnitsNet;

namespace VindemiatrixCollective.Universe.CelestialMechanics
{
    public readonly struct GravitationalParameter
    {
        public const double GravitationalConstant = UniversalConstants.Celestial.GravitationalConstant;

        private readonly double value;

        public GravitationalParameter(double value)
        {
            this.value = value;
        }

        public double M3S2 => value;

        public double Km3S2 => value / Math.Pow(1000, 3);

        public double Au3S2 => value / Math.Pow(UniversalConstants.Celestial.MetresPerAu, 3);

        public double Au3Y2
        {
            get
            {
                double timeScale     = Math.Pow(UniversalConstants.Time.SecondsPerJulianYear, 2);
                double distanceScale = Math.Pow(UniversalConstants.Celestial.MetresPerAu, 3);

                return value * (timeScale / distanceScale);
            }
        }

        public static GravitationalParameter Sun => new GravitationalParameter(1.32712440041279419e20);
        public static GravitationalParameter Earth => new GravitationalParameter(3.9860044188e14);
        public static GravitationalParameter Mars => new GravitationalParameter(4.2828372e13);

        public static GravitationalParameter FromMass(Mass mass)
        {
            return new GravitationalParameter(GravitationalConstant * mass.Kilograms);
        }

        public static GravitationalParameter FromKm3S2(double value)
        {
            return new GravitationalParameter(value * Math.Pow(1000, 3));
        }
    }
}