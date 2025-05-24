#region

using UnitsNet;
using Unity.Properties;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics.Orbits
{
    public class OrbitalData
    {
        public bool Retrograde;

        private readonly double argumentPeriapsis;
        private readonly double axialTilt;
        private readonly double eccentricity;
        private readonly double inclination;
        private readonly double longitudeAscendingNode;
        private readonly double meanAnomalyAtEpoch;
        private readonly double periodS;
        private readonly double semiMajorAxisM;
        private readonly double siderealRotationPeriodS;
        private readonly double trueAnomalyAtEpoch;
        public Angle ArgumentPeriapsis => Angle.FromDegrees(argumentPeriapsis);
        public Angle AxialTilt => Angle.FromDegrees(axialTilt);
        public Angle Inclination => Angle.FromDegrees(inclination);
        public Angle LongitudeAscendingNode => Angle.FromDegrees(longitudeAscendingNode);
        public Angle MeanAnomalyAtEpoch => Angle.FromDegrees(meanAnomalyAtEpoch);
        public Angle TrueAnomalyAtEpoch => Angle.FromDegrees(trueAnomalyAtEpoch);

        [CreateProperty] public Duration Period => Duration.FromSeconds(periodS);

        public Duration SiderealRotationPeriod => Duration.FromSeconds(siderealRotationPeriodS);

        /// <summary>
        ///     Semi-major axis of the orbit.
        /// </summary>
        [CreateProperty]
        public Length SemiMajorAxis => Length.FromMeters(semiMajorAxisM);

        public Ratio Eccentricity => Ratio.FromDecimalFractions(eccentricity);

        internal OrbitalData(
            double semiMajorAxisMetres, double eccentricity, double orbitalInclination, double lan, double argp, double orbitalPeriodDays,
            double? siderealRotationHours = null, double? axialTilt = null, double? meanAnomaly = null, double? trueAnomaly = null)
        {
            semiMajorAxisM          = semiMajorAxisMetres;
            this.eccentricity       = eccentricity;
            siderealRotationPeriodS = (siderealRotationHours ?? 0) * UniversalConstants.Time.SecondsPerHour;
            inclination             = orbitalInclination;
            longitudeAscendingNode  = lan;
            argumentPeriapsis       = argp;
            periodS                 = orbitalPeriodDays * UniversalConstants.Time.SecondsPerDay;

            this.axialTilt     = axialTilt ?? 0;
            meanAnomalyAtEpoch = meanAnomaly ?? 0;
            trueAnomalyAtEpoch = trueAnomaly ?? 0;

            if (inclination > 180)
            {
                inclination -= 180;
            }
        }

        public OrbitalData(
            Length semiMajorAxis, Ratio eccentricity, Angle inclination, Angle longitudeAscendingNode, Angle argumentPeriapsis, Duration orbitalPeriod,
            Duration siderealPeriod, Angle axialTilt, Angle trueAnomaly)
        {
            semiMajorAxisM              = semiMajorAxis.Meters;
            this.eccentricity           = eccentricity.Value;
            this.inclination            = inclination.Degrees;
            this.longitudeAscendingNode = longitudeAscendingNode.Degrees;
            this.argumentPeriapsis      = argumentPeriapsis.Degrees;
            periodS                     = orbitalPeriod.Seconds;

            trueAnomalyAtEpoch      = trueAnomaly.Degrees;
            siderealRotationPeriodS = siderealPeriod.Seconds;
            this.axialTilt          = axialTilt.Degrees;
        }


        /// <summary>
        ///     Creates an OrbitalData object.
        /// </summary>
        /// <param name="a">Semi-major axis (m)</param>
        /// <param name="e">Eccentricity</param>
        /// <param name="i">Inclination (deg)</param>
        /// <param name="lan">Longitude of the Ascending Node (deg)</param>
        /// <param name="argP">Argument of Periapsis (deg)</param>
        /// <param name="nu">True Anomaly (deg)</param>
        /// <param name="period">Orbital period (s)</param>
        /// <param name="siderealPeriod">Sidereal period (s)</param>
        /// <param name="axialTilt">Inclination to orbit (deg)</param>
        /// <returns></returns>
        public static OrbitalData FromClassicElements(
            float a, float e, float i, float lan, float argP, float nu, float period = 0,
            float siderealPeriod = 0, float axialTilt = 0)
        {
            return new OrbitalData(a, e, i, lan, argP, period,
                                   siderealPeriod, axialTilt, 0, nu);
        }
    }
}