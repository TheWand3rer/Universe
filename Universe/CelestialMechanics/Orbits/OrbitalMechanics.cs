// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using System;
using UnitsNet;
using Unity.Mathematics;
using VindemiatrixCollective.Universe.Extensions;
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
        /// <summary>
        ///     Converts classical orbital elements to position and velocity vectors.
        /// </summary>
        /// <param name="mu">Gravitational parameter (m³/s²)</param>
        /// <param name="p">Semi-latus rectum (m)</param>
        /// <param name="e">Eccentricity (dimensionless)</param>
        /// <param name="nu">True anomaly (rad)</param>
        /// <returns>Position (r) and velocity (v) vectors</returns>
        public static (Vector3d r, Vector3d v) RVinPerifocalFrame(double mu, double p, double e, double nu)
        {
            Vector3d r = new Vector3d(Math.Cos(nu), Math.Sin(nu), 0) * p / (1 + e * Math.Cos(nu));
            Vector3d v = new Vector3d(-Math.Sin(nu), e + Math.Cos(nu), 0) * Math.Sqrt(mu / p);

            return (r, v);
        }

        /// <summary>
        ///     Converts eccentric anomaly to true anomaly.
        ///     <para>ν = 2 · arctan(√((1 + e) / (1 − e)) · tan(E / 2))</para>
        /// </summary>
        /// <param name="eccentricAnomaly">The eccentric anomaly E</param>
        /// <param name="eccentricity">The eccentricity of the orbit e</param>
        /// <returns>The true anomaly nu</returns>
        public static Angle EccentricToTrueAnomaly(Angle eccentricAnomaly, Ratio eccentricity)
        {
            double nu = EccentricToTrueAnomaly(eccentricAnomaly.Radians, eccentricity.Value);
            return Angle.FromRadians(nu);
        }

        /// <summary>
        ///     Converts a velocity in AU/year to m/s.
        ///     <para>v [m/s] = v [AU/year] × MetersPerAu / SecondsPerJulianYear</para>
        /// </summary>
        /// <param name="value">Velocity in AU/year</param>
        /// <returns>Velocity in m/s</returns>
        public static double AuYToMY(double value)
        {
            double distanceScale = Length.FromAstronomicalUnits(1).Meters;
            double timeScale     = Duration.FromYears365(1).Seconds;
            timeScale = Math.Pow(timeScale, -1);
            return value * distanceScale * timeScale;
        }

        /// <summary>
        ///     Converts Eccentric to Mean anomaly using the classic Kepler equation.
        ///     <para>M = E − e · sin(E)</para>
        /// </summary>
        /// <param name="E">Eccentric anomaly (rad)</param>
        /// <param name="eccentricity">Eccentricity</param>
        /// <returns>Mean anomaly (rad)</returns>
        public static double EccentricToMeanAnomaly(double E, double eccentricity) => E - eccentricity * Math.Sin(E);

        /// <summary>
        ///     Converts the eccentric to mean anomaly.
        /// </summary>
        /// <param name="eccentricAnomaly">Eccentric anomaly</param>
        /// <param name="eccentricity">Eccentricity</param>
        /// <returns>Mean anomaly</returns>
        public static Angle EccentricToMeanAnomaly(Angle eccentricAnomaly, Ratio eccentricity)
        {
            double m = EccentricToMeanAnomaly(eccentricAnomaly.Radians, eccentricity.DecimalFractions);
            return Angle.FromRadians(m);
        }

        /// <summary>
        ///     Converts eccentric anomaly to true anomaly
        ///     <para>ν = 2 · arctan(√((1 + e) / (1 − e)) · tan(E / 2))</para>
        /// </summary>
        /// <param name="eAnomalyRad">Eccentric anomaly (rad)</param>
        /// <param name="eccentricity">Eccentricity</param>
        /// <returns>True anomaly ν (rad)</returns>
        public static double EccentricToTrueAnomaly(double eAnomalyRad, double eccentricity) =>
            2 * Math.Atan(Math.Sqrt((1 + eccentricity) / (1 - eccentricity)) * Math.Tan(eAnomalyRad / 2));

        /// <summary>
        ///     Converts hyperbolic anomaly to true anomaly for hyperbolic trajectories (e &gt; 1).
        ///     <para>ν = 2 · arctan(√((e + 1) / (e − 1)) · tanh(H / 2))</para>
        /// </summary>
        /// <param name="hAnomalyRad">Hyperbolic anomaly (rad)</param>
        /// <param name="eccentricity">Eccentricity (e &gt; 1)</param>
        /// <returns>True anomaly ν (rad)</returns>
        public static double HyperbolicAnomalyToTrueAnomaly(double hAnomalyRad, double eccentricity) =>
            2 * Math.Atan(Math.Sqrt((eccentricity + 1) / (eccentricity - 1)) * Math.Tanh(hAnomalyRad / 2));

        /// <summary>
        ///     Converts gravitational parameter from m³/s² to AU³/year².
        ///     <para>μ[AU³/yr²] = μ[m³/s²] × (MetersPerAu)³ / (SecondsPerYear)²</para>
        /// </summary>
        /// <param name="value">Gravitational parameter in m³/s²</param>
        /// <returns>Gravitational parameter in AU³/year²</returns>
        public static double M3S2ToAu3Y2(double value)
        {
            // could become constants
            double distanceScale = Length.FromMeters(1).AstronomicalUnits;
            distanceScale = Math.Pow(distanceScale, 3);
            double timeScale = Duration.FromSeconds(1).Years365;
            timeScale = Math.Pow(timeScale, -2);
            return value * distanceScale * timeScale;
        }

        /// <summary>
        ///     Solves Kepler's equation for the eccentric anomaly
        ///     <para>E₀ = M − e (if M &lt; 0) or E₀ = M + e (otherwise)</para>
        ///     <para>Eₙ₊₁ = Eₙ − (Eₙ − e·sin(Eₙ) − M) / (1 − e·cos(Eₙ))</para>
        /// </summary>
        /// <param name="meanAnomaly">Mean anomaly M</param>
        /// <param name="eccentricity">Eccentricity</param>
        /// <returns>Eccentric anomaly E (rad)</returns>
        public static Angle MeanToEccentricAnomaly(Angle meanAnomaly, Ratio eccentricity)
        {
            double E = MeanToEccentricAnomaly(meanAnomaly.Radians, eccentricity.DecimalFractions);
            return Angle.FromRadians(E);
        }

        /// <summary>
        ///     Solves Kepler's equation M = E − e·sin(E) for E
        /// </summary>
        /// <param name="M">Mean anomaly (rad)</param>
        /// <param name="e">Eccentricity</param>
        /// <returns>Eccentric anomaly E (rad). Returns NaN if convergence fails.</returns>
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

        /// <summary>
        ///     Newton-Raphson solver for Kepler's equation.
        /// </summary>
        /// <param name="x0">Initial guess for E</param>
        /// <param name="M">Mean anomaly</param>
        /// <param name="e">Eccentricity</param>
        /// <param name="maxIterations">Maximum iterations (default 50)</param>
        /// <param name="tol">Convergence tolerance (default 1.48e-8 rad ≈ 0.003 arcsec)</param>
        /// <returns>Eccentric anomaly E</returns>
        public static double NewtonElliptic(double x0, double M, double e, int maxIterations = 50, double tol = 1.48e-08) =>
            NewtonMethod(KeplerEquation, KeplerEquationPrime, x0, M, e, maxIterations, tol);

        /// <summary>
        ///     Computes the semi-latus rectum.
        ///     <para>p = a · (1 − e²)  for elliptical orbits (e &lt; 1)</para>
        ///     <para>p = a · (e² − 1)  for hyperbolic trajectories (e &gt; 1)</para>
        /// </summary>
        /// <param name="a">Semi-major axis (m).</param>
        /// <param name="e">Eccentricity</param>
        /// <returns>Semi-latus rectum (m)</returns>
        public static double SemiLatusRectum(double a, double e) => a * (1 - e * e);

        /// Converts true anomaly to eccentric anomaly.
        /// </summary>
        /// <param name="nu">True anomaly (rad)</param>
        /// <param name="eccentricity">Eccentricity</param>
        /// <returns>Eccentric anomaly (rad)</returns>
        public static double TrueToEccentricAnomaly(double nu, double eccentricity) =>
            2 * Math.Atan(Math.Sqrt((1 - eccentricity) / (1 + eccentricity)) * Math.Tan(nu / 2));

        /// <summary>
        ///     Converts true anomaly to eccentric anomaly.
        /// </summary>
        /// <param name="nu">True anomaly</param>
        /// <param name="eccentricity">Eccentricity</param>
        /// <returns>Eccentric anomaly</returns>
        public static Angle TrueToEccentricAnomaly(Angle nu, Ratio eccentricity)
        {
            double E = TrueToEccentricAnomaly(nu.Radians, eccentricity.DecimalFractions);
            return Angle.FromRadians(E);
        }

        /// <summary>
        ///     Computes orbital speed via the vis-viva equation.
        ///     <para>v = √(μ · (2/r − 1/a))</para>
        /// </summary>
        /// <param name="centreGM">Gravitational parameter of the central body (m³/s²)</param>
        /// <param name="position">Current position vector</param>
        /// <param name="a">Semi-major axis (m). Negative for hyperbolic trajectories.</param>
        /// <returns>Orbital speed (m/s)</returns>
        public static double VisViva(double centreGM, Vector3d position, Length a)
        {
            double gmAU = M3S2ToAu3Y2(centreGM);
            double rAU  = position.magnitude * UniversalConstants.Celestial.AuPerMetre;
            double aAU  = a.AstronomicalUnits;
            double vMag = Math.Sqrt(gmAU * (2 / rAU - 1 / aAU));

            return vMag;
        }

        /// <summary>
        ///     Constructs a rotation matrix about a principal axis (0 = X, 1 = Y, 2 = Z).
        ///     The matrix rotates vectors by <paramref name="angle" /> radians about the given axis.
        /// </summary>
        /// <param name="angle">Rotation angle (rad)</param>
        /// <param name="axis">Axis index: 0 = X, 1 = Y, 2 = Z</param>
        /// <returns>3×3 rotation matrix</returns>
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

        /// <summary>
        ///     Computes the orbital period (Kepler's third law) via the <paramref name="attractor" />'s
        ///     gravitational parameter.
        ///     <para>T = 2π · √(a³ / μ)</para>
        /// </summary>
        /// <param name="semiMajorAxis">Semi-major axis</param>
        /// <param name="attractor">Central body (provides μ)</param>
        /// <returns>Orbital period</returns>
        public static Duration CalculatePeriod(Length semiMajorAxis, IAttractor attractor) => CalculatePeriod(semiMajorAxis, attractor.Mu);

        /// <summary>
        ///     Computes the orbital period (Kepler's third law).
        ///     <para>T = 2π · √(a³ / μ)</para>
        /// </summary>
        /// <param name="semiMajorAxis">Semi-major axis</param>
        /// <param name="mu">Gravitational parameter of the central body</param>
        /// <returns>Orbital period</returns>
        public static Duration CalculatePeriod(Length semiMajorAxis, GravitationalParameter mu)
        {
            double period = Math.Pow(4 * Math.Pow(Math.PI, 2) / mu.Au3Y2 * Math.Pow(semiMajorAxis.AstronomicalUnits, 3), 0.5d);
            return Duration.FromYears365(period);
        }

        /// <summary>
        ///     Computes the two-body orbital period accounting for both masses.
        ///     <para>T = 2π · √(a³ / (G · (m₁ + m₂)))</para>
        /// </summary>
        /// <param name="semiMajorAxis">Semi-major axis</param>
        /// <param name="primary">Primary body mass</param>
        /// <param name="companion">Companion body mass</param>
        /// <returns>Orbital period</returns>
        public static Duration CalculateTwoBodyPeriod(Length semiMajorAxis, Mass primary, Mass companion)
        {
            double a  = semiMajorAxis.Meters;
            double m1 = primary.Kilograms;
            double m2 = companion.Kilograms;
            double G  = UniversalConstants.Celestial.GravitationalConstant;

            double T = UniversalConstants.Tri.Pi2 * Math.Sqrt(Math.Pow(a, 3) / (G * (m1 + m2)));
            return Duration.FromSeconds(T);
        }

        /// <summary>
        ///     Computes the combined mass of a two-body system from the semi-major axis and orbital period.
        ///     <para>M = 4π² · a³ / (G · T²)</para>
        /// </summary>
        /// <param name="semiMajorAxis">Semi-major axis</param>
        /// <param name="period">Orbital period</param>
        /// <returns>Combined mass (primary + companion) in kg</returns>
        public static Mass TwoBodyMass(Length semiMajorAxis, Duration period)
        {
            double a = semiMajorAxis.Meters;
            double T = period.Seconds;
            double G = UniversalConstants.Celestial.GravitationalConstant;

            double M = 4 * Math.PI * Math.PI * Math.Pow(a, 3) / (G * T * T);
            return Mass.FromKilograms(M);
        }

        /// <summary>
        ///     Computes the synodic period between two orbiting bodies.
        ///     <para>1 / S = |1 / P₁ − 1 / P₂|</para>
        /// </summary>
        /// <param name="p1">Orbital period of the first body</param>
        /// <param name="p2">Orbital period of the second body</param>
        /// <returns>Synodic period (time between successive conjunctions)</returns>
        public static Duration CalculateSynodicPeriod(Duration p1, Duration p2)
        {
            double sp = 1 / Math.Abs(1 / p1.Years365 - 1 / p2.Years365);
            return Duration.FromYears365(sp);
        }


        /// <summary>
        ///     Calculates the radius of the Laplace Sphere of Influence (SOI) for a body.
        ///     Uses the standard 2/5-power approximation.
        ///     <para>r = a · (μ / μ_parent)^(2/5)</para>
        /// </summary>
        /// <param name="attractor">Attractor body for which the SOI is computed.</param>
        /// <remarks>
        ///     <paramref name="attractor" /> must have an <see cref="OrbitState.Attractor" /> who is also a
        ///     <see cref="IAttractor" />.
        /// </remarks>
        /// <returns>Approximate radius of the Laplace Sphere of Influence.</returns>
        public static Length LaplaceRadius(IAttractor attractor)
        {
            double                 a        = attractor.OrbitalData.SemiMajorAxis.Kilometers;
            GravitationalParameter mu       = attractor.Mu;
            GravitationalParameter muParent = ((IAttractor)attractor.Parent).Mu;

            double soi = a * Math.Pow(mu.Km3S2 / muParent.Km3S2, 2d / 5d);
            return Length.FromKilometers(soi);
        }

        /// <summary>
        ///     Computes the Hill sphere radius (1/3-power approximation).
        ///     <para>r = a · (μ / (3 · μ_parent))^(1/3)</para>
        /// </summary>
        /// <param name="attracted">Body whose SOI is being computed</param>
        /// <param name="attractor">Parent body of <paramref name="attracted" /></param>
        public static Length CalculateSOI(CelestialBody attracted, CelestialBody attractor) => attracted.OrbitalData.SemiMajorAxis
                                                                                             * Math.Pow(attracted.Mu.M3S2 / (3 * attractor.Mu.M3S2),
                                                                                                   1 / 3d);

        /// <summary>
        ///     Calculates the escape velocity from a body at a given altitude.
        ///     <para>v_esc = √(2 · μ / r)</para>
        /// </summary>
        /// <param name="body">Central body</param>
        /// <param name="orbitHeight">Altitude above the body's surface</param>
        /// <returns>Escape velocity</returns>
        public static Speed CalculateEscapeVelocity(CelestialBody body, Length orbitHeight)
        {
            Length rTotal         = body.PhysicalData.Radius + orbitHeight;
            double escapeVelocity = Math.Pow(2 * body.Mu.M3S2 / rTotal.Meters, 0.5);
            return Speed.FromMetersPerSecond(escapeVelocity);
        }


        /// <summary>
        ///     Calculates the hyperbolic excess velocity (v∞) given the body's escape velocity.
        ///     <para>v∞ = √(v² − v_esc²)</para>
        /// </summary>
        /// <param name="velocity">Current velocity</param>
        /// <param name="vEscape">Local escape velocity at the same radius</param>
        /// <returns>Hyperbolic excess velocity</returns>
        public static Speed CalculateHyperbolicExcessVelocity(Speed velocity, Speed vEscape)
        {
            double vExcess = Math.Sqrt(Math.Pow(velocity.MetersPerSecond, 2) - Math.Pow(vEscape.MetersPerSecond, 2));
            return Speed.FromMetersPerSecond(vExcess);
        }

        /// <summary>
        ///     Calculates the hyperbolic excess velocity (v∞) from a velocity at a given altitude.
        ///     <para>v∞ = √(v² − 2 · μ / r)</para>
        /// </summary>
        /// <param name="velocity">Current velocity</param>
        /// <param name="body">Central body</param>
        /// <param name="orbitHeight">Altitude above the body's surface</param>
        /// <returns>Hyperbolic excess velocity</returns>
        public static Speed CalculateHyperbolicExcessVelocity(Speed velocity, CelestialBody body, Length orbitHeight)
        {
            Length rTotal  = body.PhysicalData.Radius + orbitHeight;
            double vExcess = Math.Sqrt(Math.Pow(velocity.MetersPerSecond, 2) - 2 * body.Mu.M3S2 / rTotal.Meters);
            return Speed.FromMetersPerSecond(vExcess);
        }

        /// <summary>
        ///     Calculates the circular orbital speed at a given altitude.
        ///     <para>v = √(μ / r)</para>
        /// </summary>
        /// <param name="body">Central body</param>
        /// <param name="orbitHeight">Altitude above the body's surface</param>
        /// <returns>Circular orbital speed</returns>
        public static Speed CalculateOrbitalSpeed(CelestialBody body, Length orbitHeight)
        {
            double vOrbital = Math.Sqrt(body.Mu.M3S2 / (body.PhysicalData.Radius + orbitHeight).Meters);
            return Speed.FromMetersPerSecond(vOrbital);
        }

        /// <summary>
        ///     Computes the circular Keplerian orbital speed.
        ///     <para>v = √(G · M / r)</para>
        /// </summary>
        /// <param name="mass">Mass of the central body</param>
        /// <param name="orbitalRadius">Orbital radius</param>
        /// <returns>Circular orbital speed</returns>
        public static Speed KeplerianVelocity(Mass mass, Length orbitalRadius)
        {
            double r   = orbitalRadius.Meters;
            double M   = mass.Kilograms;
            double G   = UniversalConstants.Celestial.GravitationalConstant;
            double v_k = math.sqrt(G * M / r);
            return Speed.FromMetersPerSecond(v_k);
        }

        /// <summary>
        ///     Converts a vector from AU to metres.
        /// </summary>
        /// <param name="from">Position vector in AU</param>
        /// <returns>Position vector in metres</returns>
        public static Vector3d AuToMetres(Vector3d from) => from * UniversalConstants.Celestial.MetresPerAu;

        /// <summary>
        ///     Converts a vector from AU/year to m/s.
        /// </summary>
        /// <param name="from">Velocity vector in AU/year</param>
        /// <returns>Velocity vector in m/s</returns>
        public static Vector3d AuYToMY(Vector3d from) => from * AuYToMY(1);

        /// <summary>
        ///     Converts a vector from metres to AU.
        /// </summary>
        /// <param name="position">Position vector in metres</param>
        /// <param name="scale">Optional scaling factor</param>
        /// <returns>Position vector in AU</returns>
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
        ///     Creates a range of evenly spaced dates from start to end.
        /// </summary>
        /// <param name="start">Starting date.</param>
        /// <param name="end">End date.</param>
        /// <param name="numValues">Number of dates to calculate (inclusive of start and end). Must be ≥ 2.</param>
        /// <returns>Array of numValues DateTime values spanning [start, end].</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when numValues &lt; 2.</exception>
        public static DateTime[] TimeRange(DateTime start, DateTime end, int numValues = 50)
        {
            if (numValues < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(numValues), "TimeRange requires at least 2 values.");
            }

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
        ///     Converts geodetic latitude and longitude to ECEF-like Cartesian coordinates.
        ///     <para>x = R · cos(lat) · sin(lon)</para>
        ///     <para>y = R · sin(lat)</para>
        ///     <para>z = R · cos(lat) · cos(lon)</para>
        /// </summary>
        /// <param name="latitude">Latitude in degrees (positive = North, negative = South)</param>
        /// <param name="longitude">Longitude in degrees (positive = East, negative = West)</param>
        /// <param name="radius">Radius of the body in metres</param>
        /// <returns>Cartesian position vector</returns>
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

        /// <summary>
        ///     Computes the mean motion (angular orbital frequency) for a circular orbit.
        ///     <para>Ω = √(G · M / a³)</para>
        /// </summary>
        /// <param name="stellarMass">Mass of the central body</param>
        /// <param name="a">Semi-major axis (orbital radius for circular orbit)</param>
        /// <returns>Angular frequency in rad/s</returns>
        public static RotationalSpeed OrbitalFrequency(Mass stellarMass, Length a) =>
            RotationalSpeed.FromRadiansPerSecond(math.sqrt(UniversalConstants.Celestial.GravitationalConstant
                                                         * stellarMass.Kilograms
                                                         / math.pow(a.Meters, 3)));


        /// <summary>
        ///     Computes surface gravitational acceleration from mass and radius.
        ///     <para>g = G · M / r²</para>
        /// </summary>
        /// <param name="mass">Body mass</param>
        /// <param name="radius">Body radius</param>
        /// <returns>Surface gravity in m/s²</returns>
        public static Acceleration GravityFromMassRadius(Mass mass, Length radius)
        {
            double M = mass.Kilograms;
            double r = radius.Meters;
            double G = UniversalConstants.Celestial.GravitationalConstant;

            return Acceleration.FromMetersPerSecondSquared(G * M / (r * r));
        }

        /// <summary>
        ///     Computes the planet's rotation quaternion in the solar system frame (SSF).
        ///     Uses the sidereal rotation angle and the angular momentum vector as the spin axis.
        /// </summary>
        /// <param name="planet">The planet to compute rotation for</param>
        /// <returns>Quaternion representing the planet's orientation in the SSF</returns>
        public static Quaterniond SiderealRotation(Planet planet)
        {
            double      siderealRotation  = planet.OrbitState.SiderealRotation.Degrees;
            Vector3d    spinAxisSSF       = planet.OrbitState.AngularMomentum.normalized.ToXZYd();
            Quaterniond planetRotationSSF = Quaterniond.AngleAxis(siderealRotation, spinAxisSSF);
            return planetRotationSSF;
        }

        public delegate double NewtonFunction(double p0, double M, double e);
    }
}