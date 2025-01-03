using UnitsNet;
using Unity.Properties;

namespace VindemiatrixCollective.Universe.CelestialMechanics.Orbits
{
    public class OrbitalData
    {
        public Angle ArgumentPeriapsis => Angle.FromDegrees(argumentPeriapsis);
        public Angle AxialTilt => Angle.FromDegrees(axialTilt);
        public Angle Inclination => Angle.FromDegrees(inclination);
        public Angle LongitudeAscendingNode => Angle.FromDegrees(longitudeAscendingNode);
        public Angle MeanAnomalyAtEpoch => Angle.FromDegrees(meanAnomalyAtEpoch);
        public Angle TrueAnomalyAtEpoch => Angle.FromDegrees(trueAnomalyAtEpoch);

        [CreateProperty]
        public Duration Period => Duration.FromSeconds(periodS);

        public Duration SiderealRotationPeriod => Duration.FromSeconds(siderealRotationPeriodS);

        /// <summary>
        /// Semi-major axis of the orbit.
        /// </summary>
        [CreateProperty]
        public Length SemiMajorAxis => Length.FromMeters(semiMajorAxisM);

        public Ratio Eccentricity => Ratio.FromDecimalFractions(eccentricity);

        public bool Retrograde;

        private double argumentPeriapsis;
        private double axialTilt;
        private double eccentricity;
        private double inclination;
        private double longitudeAscendingNode;
        private double meanAnomalyAtEpoch;
        private double periodS;
        private double semiMajorAxisM;
        private double siderealRotationPeriodS;
        private double trueAnomalyAtEpoch;

        public static OrbitalData From(float semiMajorAxis, float orbitalPeriodDays,
                                       float eccentricity, float siderealRotationHours, float orbitalInclination, float? axialTilt, float loan, float aoPe,
                                       float? meanAnomaly, float? trueAnomaly)
        {
            OrbitalData orbitalData = new()
            {
                semiMajorAxisM = semiMajorAxis,
                periodS = orbitalPeriodDays * UniversalConstants.Time.SecondsPerDay,
                eccentricity = eccentricity,
                siderealRotationPeriodS = siderealRotationHours * UniversalConstants.Time.SecondsPerHour,
                inclination = orbitalInclination,
                axialTilt = axialTilt ?? 0,
                longitudeAscendingNode = loan,
                argumentPeriapsis = aoPe,
                meanAnomalyAtEpoch = meanAnomaly ?? 0,
                trueAnomalyAtEpoch = trueAnomaly ?? 0
            };

            if (orbitalData.inclination > 180)
            {
                orbitalData.inclination -= 180;
            }
            return orbitalData;
        }

        /// <summary>
        /// Creates an OrbitalData object.
        /// </summary>
        /// <param name="a">Semi-major axis (m)</param>
        /// <param name="eccentricity">Eccentricity</param>
        /// <param name="inclination">Inclination (deg)</param>
        /// <param name="longitudeAscendingNode">Longitude of the Ascending Node (deg)</param>
        /// <param name="argumentPeriapsis">Argument of Periapsis (deg)</param>
        /// <param name="trueAnomaly">True Anomaly (deg)</param>
        /// <param name="orbitalPeriod">Orbital period (s)</param>
        /// <param name="siderealPeriod"></param>
        /// <param name="axialTilt">Inclination to orbit (deg)</param>
        /// <returns></returns>
        public static OrbitalData From(Length semiMajorAxis, Ratio eccentricity, Angle inclination, Angle longitudeAscendingNode,
                                       Angle argumentPeriapsis, Angle trueAnomaly, Duration orbitalPeriod, Duration siderealPeriod, Angle axialTilt)
        {
            OrbitalData orbitalData = new()
            {
                semiMajorAxisM = semiMajorAxis.Meters,
                eccentricity = eccentricity.Value,
                inclination = inclination.Degrees,
                longitudeAscendingNode = longitudeAscendingNode.Degrees,
                argumentPeriapsis = argumentPeriapsis.Degrees,
                trueAnomalyAtEpoch = trueAnomaly.Degrees,
                periodS = orbitalPeriod.Seconds,
                siderealRotationPeriodS = siderealPeriod.Seconds,
                axialTilt = axialTilt.Degrees
            };
            return orbitalData;
        }

        /// <summary>
        /// Creates an OrbitalData object.
        /// </summary>
        /// <param name="a">Semi-major axis (m)</param>
        /// <param name="e">Eccentricity</param>
        /// <param name="i">Inclination (deg)</param>
        /// <param name="loAn">Longitude of the Ascending Node (deg)</param>
        /// <param name="argP">Argument of Periapsis (deg)</param>
        /// <param name="nu">True Anomaly (deg)</param>
        /// <param name="period">Orbital period (s)</param>
        /// <param name="siderealPeriod"></param>
        /// <param name="axialTilt">Inclination to orbit (deg)</param>
        ///  <returns></returns>
        public static OrbitalData FromClassicElements(float a, float e, float i, float loAn, float argP, float nu, float period = 0,
                                                      float siderealPeriod = 0, float axialTilt = 0)
        {
            OrbitalData orbitalData = new()
            {
                semiMajorAxisM = a,
                eccentricity = e,
                inclination = i,
                longitudeAscendingNode = loAn,
                argumentPeriapsis = argP,
                trueAnomalyAtEpoch = nu,
                periodS = period,
                siderealRotationPeriodS = siderealPeriod,
                axialTilt = axialTilt
            };
            return orbitalData;
        }
    }
}