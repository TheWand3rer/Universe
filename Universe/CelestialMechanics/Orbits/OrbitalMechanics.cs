// Terminalizer © 2025 Vindemiatrix Collective
// Website and Documentation - https://dev.vindemiatrixcollective.com

#region

using System;
using UnitsNet;
using Unity.Mathematics;
using VindemiatrixCollective.Universe.Model;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics.Orbits
{
    /// <summary>
    ///     References:
    ///     [1] Braeunig http://www.braeunig.us/space/interpl.htm
    ///     [2] Vallado Fundamentals of Astrodynamics
    /// </summary>
    public static class OrbitalMechanics
    {
        public delegate double NewtonFunction(double p0, double M, double e);

        /// <summary>
        ///     Converts classical orbital elements to position and velocity vectors.
        /// </summary>
        /// <param name="mu">Gravitational parameter (m3/s2)</param>
        /// <param name="p">Semi latus rectum (m)</param>
        /// <param name="e">Eccentricity</param>
        /// <param name="nu">True anomaly (rad)</param>
        /// <returns>Position (r) and velocity (v) vectors</returns>
        public static (Vector3d r, Vector3d v) RVinPerifocalFrame(double mu, double p, double e, double nu)
        {
            Vector3d r = new Vector3d(Math.Cos(nu), Math.Sin(nu), 0) * p / (1 + e * Math.Cos(nu));
            Vector3d v = new Vector3d(-Math.Sin(nu), e + Math.Cos(nu), 0) * Math.Sqrt(mu / p);

            return (r, v);
        }

        /// <summary>
        ///     From Vallado [3] p.48
        /// </summary>
        /// <param name="eccentricAnomaly">The eccentric anomaly E</param>
        /// <param name="eccentricity">The eccentricity of the orbit e</param>
        /// <returns>The true anomaly nu</returns>
        public static Angle EccentricToTrueAnomaly(Angle eccentricAnomaly, Ratio eccentricity)
        {
            double nu = EccentricToTrueAnomaly(eccentricAnomaly.Radians, eccentricity.Value);
            return Angle.FromRadians(nu);
        }

        public static double AuYToMY(double value)
        {
            double distanceScale = Length.FromAstronomicalUnits(1).Meters;
            double timeScale     = Duration.FromYears365(1).Seconds;
            timeScale = Math.Pow(timeScale, -1);
            return value * distanceScale * timeScale;
        }

        /// <summary>
        ///     Converts Eccentric to Mean anomaly using the classic Kepler equation.
        /// </summary>
        /// <param name="E">Eccentric anomaly (rad)</param>
        /// <param name="eccentricity">Eccentricity</param>
        /// <returns>Mean anomaly (rad)</returns>
        public static double EccentricToMeanAnomaly(double E, double eccentricity) => E - eccentricity * Math.Sin(E);

        /// <summary>
        ///     Converts the eccentric to mean anomaly.
        /// </summary>
        /// <param name="eccentricAnomaly">The eccentric anomaly.</param>
        /// <param name="eccentricity">The eccentricity.</param>
        /// <returns>Mean anomaly in radians.</returns>
        public static Angle EccentricToMeanAnomaly(Angle eccentricAnomaly, Ratio eccentricity)
        {
            double m = EccentricToMeanAnomaly(eccentricAnomaly.Radians, eccentricity.DecimalFractions);
            return Angle.FromRadians(m);
        }

        public static double EccentricToTrueAnomaly(double eAnomalyRad, double eccentricity) =>
            2 * Math.Atan(Math.Sqrt((1 + eccentricity) / (1 - eccentricity)) * Math.Tan(eAnomalyRad / 2));

        public static double HyperbolicAnomalyToTrueAnomaly(double hAnomalyRad, double eccentricity) =>
            2 * Math.Atan(Math.Sqrt((eccentricity + 1) / (eccentricity - 1)) * Math.Tanh(hAnomalyRad / 2));

        public static double M3S2ToAu3S2(double value) => value / Math.Pow(UniversalConstants.Celestial.MetresPerAu, 3);

        public static double M3S2ToAu3Y2(double value)
        {
            // could become constants
            double distanceScale = Length.FromMeters(1).AstronomicalUnits;
            distanceScale = Math.Pow(distanceScale, 3);
            double timeScale = Duration.FromSeconds(1).Years365;
            timeScale = Math.Pow(timeScale, -2);
            return value * distanceScale * timeScale;
        }

        public static Angle MeanToEccentricAnomaly(Angle meanAnomaly, Ratio eccentricity)
        {
            double E = MeanToEccentricAnomaly(meanAnomaly.Radians, eccentricity.DecimalFractions);
            return Angle.FromRadians(E);
        }

        public static double MeanToEccentricAnomaly(double M, double e)
        {
            const double pi = UniversalConstants.Tri.Pi;
            double       E0;
            if (M is >= -pi and < 0 || M > pi)
            {
                E0 = M - e;
            }
            else
            {
                E0 = M + e;
            }

            double E = NewtonElliptic(E0, M, e);
            return E;
        }

        public static double NewtonElliptic(double x0, double M, double e, int maxIterations = 50, double tol = 1.48e-08) =>
            NewtonMethod(KeplerEquation, KeplerEquationPrime, x0, M, e, maxIterations, tol);

        /// <summary>
        ///     Calculates the semi latus rectum.
        /// </summary>
        /// <param name="a">Semi-major axis (m)</param>
        /// <param name="e">Eccentricity</param>
        /// <returns></returns>
        public static double SemiLatusRectum(double a, double e) => a * (1 - e * e);

        public static double TrueToEccentricAnomaly(double nu, double eccentricity) =>
            2 * Math.Atan(Math.Sqrt((1 - eccentricity) / (1 + eccentricity)) * Math.Tan(nu / 2));

        public static Angle TrueToEccentricAnomaly(Angle nu, Ratio eccentricity)
        {
            double E = TrueToEccentricAnomaly(nu.Radians, eccentricity.DecimalFractions);
            return Angle.FromRadians(E);
        }

        public static double VisViva(double centreGM, Vector3d position, Length a)
        {
            double gmAU = M3S2ToAu3Y2(centreGM);
            double vMag = Math.Sqrt(gmAU * (2 / position.magnitude - 1 / a.AstronomicalUnits));

            return vMag;
        }

        public static double3x3 RotationMatrix(double angle, int axis)
        {
            double    c  = Math.Cos(angle);
            double    s  = Math.Sin(angle);
            int       a1 = (axis + 1) % 3;
            int       a2 = (axis + 2) % 3;
            double3x3 r  = double3x3.zero;

            r[axis][axis] = 1.0;
            r[a1][a1]     = c;
            r[a2][a1]     = -s;
            r[a1][a2]     = s;
            r[a2][a2]     = c;

            return r;
        }

        public static Duration CalculatePeriod(Length semiMajorAxis, CelestialBody attractor)
        {
            double period = Math.Pow(4 * Math.Pow(Math.PI, 2) / attractor.Mu.Au3Y2 * Math.Pow(semiMajorAxis.AstronomicalUnits, 3), 0.5d);
            return Duration.FromYears365(period);
        }

        public static Duration CalculateTwoBodyPeriod(Length semiMajorAxis, Mass primary, Mass companion)
        {
            double a  = semiMajorAxis.Meters;
            double m1 = primary.Kilograms;
            double m2 = companion.Kilograms;
            double G  = UniversalConstants.Celestial.GravitationalConstant;

            double T = UniversalConstants.Tri.Pi2 * Math.Sqrt(Math.Pow(a, 3) / (G * (m1 + m2)));
            return Duration.FromSeconds(T);
        }

        public static Mass TwoBodyMass(Length semiMajorAxis, Duration period)
        {
            double a = semiMajorAxis.Meters;
            double T = period.Seconds;
            double G = UniversalConstants.Celestial.GravitationalConstant;

            double M = 4 * Math.PI * Math.PI * Math.Pow(a, 3) / (G * T * T);
            return Mass.FromKilograms(M);
        }

        public static Duration CalculateSynodicPeriod(Duration p1, Duration p2)
        {
            double sp = 1 / Math.Abs(1 / p1.Years365 - 1 / p2.Years365);
            return Duration.FromYears365(sp);
        }

        public static Length CalculateSOI(CelestialBody attracted, CelestialBody attractor) => attracted.OrbitalData.SemiMajorAxis
          * Math.Pow(attracted.Mu.M3S2 / (3 * attractor.Mu.M3S2), 1 / 3d);

        // From http://www.braeunig.us/space/orbmech.htm (4.78)
        public static Speed CalculateEscapeVelocity(CelestialBody body, Length orbitHeight)
        {
            Length rTotal         = body.PhysicalData.Radius + orbitHeight;
            double escapeVelocity = Math.Pow(2 * body.Mu.M3S2 / rTotal.Meters, 0.5);
            return Speed.FromMetersPerSecond(escapeVelocity);
        }


        public static Speed CalculateHyperbolicExcessVelocity(Speed velocity, CelestialBody body, Length orbitHeight)
        {
            Length rTotal  = body.PhysicalData.Radius + orbitHeight;
            double vExcess = Math.Sqrt(Math.Pow(velocity.MetersPerSecond, 2) - 2 * body.Mu.M3S2 / rTotal.Meters);
            return Speed.FromMetersPerSecond(vExcess);
        }

        public static Speed CalculateHyperbolicExcessVelocity(Speed velocity, Speed vEscape)
        {
            double vExcess = Math.Sqrt(Math.Pow(velocity.MetersPerSecond, 2) - Math.Pow(vEscape.MetersPerSecond, 2));
            return Speed.FromMetersPerSecond(vExcess);
        }

        public static Speed CalculateOrbitalVelocity(CelestialBody body, Length orbitHeight)
        {
            double vOrbital = Math.Sqrt(body.Mu.M3S2 / (body.PhysicalData.Radius + orbitHeight).Meters);
            return Speed.FromMetersPerSecond(vOrbital);
        }

        public static Vector3d AuToMetres(Vector3d from) => from * UniversalConstants.Celestial.MetresPerAu;

        public static Vector3d AuYToMY(Vector3d from) => from * AuYToMY(1);

        public static Vector3d MetresToAu(Vector3d position, float scale = 1) => position * UniversalConstants.Celestial.AuPerMetre * scale;


        private static double KeplerEquation(double E, double M, double e) => EccentricToMeanAnomaly(E, e) - M;

        private static double KeplerEquationPrime(double E, double M, double e) => 1 - e * Math.Cos(E);

        private static double NewtonMethod(
            NewtonFunction function, NewtonFunction prime, double x0, double M, double e, int maxIterations, double tol)
        {
            double p0   = x0;
            double step = 0;
            for (int i = 0; i < maxIterations; i++)
            {
                double fValue = function(p0, M, e);
                double fDer   = prime(p0, M, e);
                step = fValue / fDer;
                double p = p0 - step;
                if (Math.Abs(p - p0) < tol)
                {
                    return p;
                }

                p0 = p;
            }

            return double.NaN;
        }

        /// <summary>
        ///     Creates a range of spaced dates from start to end.
        /// </summary>
        /// <param name="start">Starting date.</param>
        /// <param name="end">End date.</param>
        /// <param name="numValues">Number of dates to calculate (inclusive of start and end).</param>
        /// <returns></returns>
        public static DateTime[] TimeRange(DateTime start, DateTime end, int numValues = 50)
        {
            DateTime[] dates = new DateTime[numValues];

            int step = (int)math.floor((end - start).TotalSeconds / numValues);

            for (int i = 1; i < numValues - 1; i++)
            {
                dates[i] = start.AddSeconds(step * i);
            }

            dates[0]  = start;
            dates[^1] = end;

            return dates;
        }


        /// <summary>
        /// Converts latitude and longitude to xyz coordinates.
        /// </summary>
        /// <param name="latitude">Positive values indicate N, negative S.</param>
        /// <param name="longitude">Positive values indicate E, negative W.</param>
        /// <param name="radius">Radius of the planet in metres.</param>
        /// <returns></returns>
        public static Vector3d GeoToCartesian(double latitude, double longitude, double radius)
        {
            double latRad = latitude.ToRadians();
            double lonRad = longitude.ToRadians();

            // Assuming y as up
            double x = radius * Math.Cos(latRad) * Math.Sin(lonRad);
            double z = radius * Math.Cos(latRad) * Math.Cos(lonRad);
            double y = radius * Math.Sin(latRad);

            return new Vector3d(x, y, z);
        }
    }
}