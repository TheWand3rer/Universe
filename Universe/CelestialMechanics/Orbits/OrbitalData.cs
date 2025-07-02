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

        private OrbitalData(
            double semiMajorAxisMetres, double eccentricity, double orbitalInclination, double lan, double argp,
            double? orbitalPeriodSeconds, double? siderealRotationSeconds = null, double? axialTilt = null, double? meanAnomaly = null,
            double? trueAnomaly = null)
        {
            semiMajorAxisM          = semiMajorAxisMetres;
            this.eccentricity       = eccentricity;
            siderealRotationPeriodS = siderealRotationSeconds ?? 0;
            inclination             = orbitalInclination;
            longitudeAscendingNode  = lan;
            argumentPeriapsis       = argp;
            periodS                 = orbitalPeriodSeconds ?? 0;

            this.axialTilt     = axialTilt ?? 0;
            meanAnomalyAtEpoch = meanAnomaly ?? 0;
            trueAnomalyAtEpoch = trueAnomaly ?? 0;

            if (inclination > 180)
            {
                inclination -= 180;
            }
        }

        public OrbitalData(
            Length semiMajorAxis, Ratio eccentricity, Angle inclination, Angle longitudeAscendingNode, Angle argumentPeriapsis,
            Duration? orbitalPeriod, Duration? siderealPeriod, Angle? axialTilt, Angle? trueAnomaly,
            Angle? meanAnomaly = null) : this(semiMajorAxis.Meters,
                                              eccentricity.Value,
                                              inclination.Degrees,
                                              longitudeAscendingNode.Degrees, argumentPeriapsis.Degrees,
                                              orbitalPeriod?.Seconds,
                                              siderealPeriod?.Seconds,
                                              axialTilt?.Degrees,
                                              trueAnomaly?.Degrees,
                                              meanAnomaly?.Degrees) { }

        internal static OrbitalData Empty => new(0, 0, 0, 0, 0, 0);


        /// <summary>
        ///     Creates an OrbitalData object.
        /// </summary>
        /// <param name="a">Semi-major axis (m)</param>
        /// <param name="e">Eccentricity</param>
        /// <param name="i">Inclination (deg)</param>
        /// <param name="lan">Longitude of the Ascending Node (deg)</param>
        /// <param name="argP">Argument of Periapsis (deg)</param>
        /// <param name="nu">True Anomaly (deg)</param>
        /// <returns></returns>
        public static OrbitalData FromClassicElements(float a, float e, float i, float lan, float argP, float nu)
        {
            return new OrbitalData(a, e, i, lan, argP,
                                   0, 0, 0, 0, nu);
        }
    }
}