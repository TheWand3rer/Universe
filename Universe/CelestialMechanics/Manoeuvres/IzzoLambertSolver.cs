// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using UnitsNet;
using Unity.Mathematics;
using UnityEngine.Assertions;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics.Manoeuvres
{
    public class IzzoLambertSolver : ILambertSolver
    {
        private const double pi = math.PI_DBL;

        public bool LowPath { get; set; }

        public bool Prograde { get; set; }

        public double Tolerance { get; set; }

        public int MaxIterations { get; }

        public int Revolutions { get; set; }

        public IzzoLambertSolver()
        {
            Revolutions   = 0;
            Prograde      = true;
            LowPath       = true;
            MaxIterations = 32;
            Tolerance     = 1e-8;
        }

        /// <summary>
        ///     Solves the Lambert problem using the Izzo Algorithm.
        /// </summary>
        /// <param name="gravitationalParameter">Gravitational constant of main attractor (km^3 / s^2).</param>
        /// <param name="initialPosition">Initial position (km).</param>
        /// <param name="finalPosition">Final position (km).</param>
        /// <param name="timeOfFlight">Time of flight (s).</param>
        public (Vector3d v1, Vector3d v2) Lambert(
            GravitationalParameter gravitationalParameter, Vector3d initialPosition, Vector3d finalPosition, Duration timeOfFlight)
        {
            double   k   = gravitationalParameter.Km3S2;
            Vector3d r1  = initialPosition;
            Vector3d r2  = finalPosition;
            double   tof = timeOfFlight.Seconds;

            Assert.IsTrue(tof > 0, nameof(timeOfFlight));
            Assert.IsTrue(k > 0, nameof(gravitationalParameter));
            Assert.IsTrue(Tolerance > 0, nameof(Tolerance));

            if (Vector3d.Cross(r1, r2) == Vector3d.zero)
                throw new ArgumentException("Lambert solution cannot be computed for collinear vectors");

            Vector3d c   = (r2 - r1);
            double   cN  = c.magnitude;
            double   r1N = r1.magnitude;
            double   r2N = r2.magnitude;

            // Semiperimeter
            double s = (r1N + r2N + cN) * 0.5;

            // Versors
            Vector3d ir1 = r1.normalized;
            Vector3d ir2 = r2.normalized;
            Vector3d ih  = Vector3d.Cross(ir1, ir2).normalized;

            // Geometry of the problem
            double ll = math.sqrt(1 - math.min(1.0, cN / s));

            // Compute the fundamental tangential directions
            Vector3d it1, it2;
            if (ih.z < 0)
            {
                ll  = -ll;
                it1 = Vector3d.Cross(ir1, ih);
                it2 = Vector3d.Cross(ir2, ih);
            }
            else
            {
                it1 = Vector3d.Cross(ih, ir1);
                it2 = Vector3d.Cross(ih, ir2);
            }

            // Correct transfer angle parameter and tangential vectors if required
            (ll, it1, it2) = Prograde ? (ll, it1, it2) : (-ll, -it1, -it2);

            // Non-dimensional time of flight
            double T = math.sqrt(2 * k / math.pow(s, 3)) * tof;

            (double x, double y) = find_xy(ll, T, Revolutions);

            // Reconstruct
            double gamma = math.sqrt(k * s / 2);
            double rho   = (r1N - r2N) / cN;
            double sigma = math.sqrt(1 - math.pow(rho, 2));

            (double V_r1, double V_r2, double V_t1, double V_t2) = reconstruct(x, y, r1N, r2N, ll, gamma, rho, sigma);

            // Solve for the initial and final velocity
            Vector3d v1 = V_r1 * (r1 / r1N) + V_t1 * it1;
            Vector3d v2 = V_r2 * (r2 / r2N) + V_t2 * it2;

            return (v1, v2);
        }

        /// <summary>
        ///     Compute minimum T.
        /// </summary>
        (double xTmin, double Tmin) compute_Tmin(double ll, int M)
        {
            double xTmin, Tmin;
            if (math.abs(ll - 1) < Tolerance)
            {
                xTmin = 0;
                Tmin  = tofEquation(xTmin, 0, ll, M);
            }
            else
            {
                if (M == 0)
                {
                    xTmin = double.PositiveInfinity;
                    Tmin  = 0;
                }
                else
                {
                    // Set x_i > 0 to avoid problems at ll = -1
                    double xi = 0.1;
                    double Ti = tofEquation(xi, 0.0, ll, M);
                    xTmin = halley(xi, Ti, ll);
                    Tmin  = tofEquation(xTmin, 0, ll, M);
                }
            }

            return (xTmin, Tmin);
        }

        double compute_y(double x, double ll)
        {
            return math.sqrt(1 - math.pow(ll, 2) * (1 - x * x));
        }

        /// <summary>
        ///     Computes psi.
        ///     The auxiliary angle psi is computed using Eq.(17) by the appropriate inverse function.
        /// </summary>
        /// <returns>The angle psi.</returns>
        double computePsi(double x, double y, double ll)
        {
            if (x is >= -1 and < 1)
            {
                // Elliptic motion
                // Use arc cosine to avoid numerical errors
                return math.acos(x * y + ll * (1 - math.pow(x, 2)));
            }

            if (x > 1)
            {
                // Hyperbolic motion
                // The hyperbolic sine is bijective
                return math.sinh(y - x * ll) * math.sqrt(math.pow(x, 2) - 1);
            }

            // Parabolic motion
            return 0;
        }

        /// <summary>
        ///     Computes all x, y for given number of revolutions.
        /// </summary>
        (double x, double y) find_xy(double ll, double T, int M)
        {
            // For abs(ll) == 1 the derivative is not continuous
            Assert.IsTrue(math.abs(ll) < 1);
            Assert.IsTrue(T > 0); // Mistake in the original paper
            int    Mmax = (int)math.floor(T / pi);
            double T00  = math.acos(ll) + ll * math.sqrt(1 - math.pow(ll, 2)); // T_xM

            // Refine maximum number of revolutions if necessary
            if (T < T00 + Mmax * pi && Mmax > 0)
            {
                (_, double Tmin) = compute_Tmin(ll, Mmax);
                if (T < Tmin)
                    Mmax -= 1;
            }

            // Check if a feasible solution exist for the given number of revolutions
            // This departs from the original paper in that we do not compute all solutions
            if (M > Mmax)
                throw new InvalidOperationException("No feasible solution, try lower M");

            double x0 = initialGuess(T, ll, M);

            // Start Householder iterations from x_0 and find x, y
            double x = householder(x0, T, ll, M);
            double y = compute_y(x, ll);

            return (x, y);
        }

        /// <summary>
        ///     Find a minimum of time of flight equation using the Halley method.
        /// </summary>
        private double halley(double p0, double T0, double ll)
        {
            for (int i = 0; i < MaxIterations; i++)
            {
                double y     = compute_y(p0, ll);
                double fDer  = tofEquation_p(p0, y, T0, ll);
                double fDer2 = tofEquation_p2(p0, y, T0, fDer, ll);
                if (math.abs(fDer2) < Tolerance)
                    throw new InvalidOperationException("Derivative was zero");
                double fDer3 = tofEquation_p3(p0, y, T0, fDer, fDer2, ll);

                // Halley step (cubic)
                double p = p0 - 2 * fDer * fDer2 / (2 * math.pow(fDer2, 2) - fDer * fDer3);

                if (math.abs(p - p0) < Tolerance)
                    return p;
                p0 = p;
            }

            throw new InvalidOperationException($"Failed to converge after {MaxIterations} iterations using {Tolerance:E0}");
        }

        /// <summary>
        ///     Find a zero of time of flight equation using the Householder method.
        /// </summary>
        private double householder(double p0, double T0, double ll, int M)
        {
            for (int i = 0; i < MaxIterations; i++)
            {
                double y     = compute_y(p0, ll);
                double fVal  = tofEquation_y(p0, y, T0, ll, M);
                double T     = fVal + T0;
                double fDer  = tofEquation_p(p0, y, T, ll);
                double fDer2 = tofEquation_p2(p0, y, T, fDer, ll);
                double fDer3 = tofEquation_p3(p0, y, T, fDer, fDer2, ll);

                // Householder step (quartic)
                double p = p0 - fVal * ((math.pow(fDer, 2) - fVal * fDer2 / 2)
                                      / (fDer * (math.pow(fDer, 2) - fVal * fDer2) + fDer3 * math.pow(fVal, 2) / 6));

                if (math.abs(p - p0) < Tolerance)
                    return p;
                p0 = p;
            }

            throw new InvalidOperationException($"Failed to converge after {MaxIterations} iterations using {Tolerance:E0}");
        }

        double hypergeometric2f1b(double x, double rtol = 1e8)
        {
            if (x >= 0)
                return double.PositiveInfinity;
            double res  = 1.0;
            double term = 1.0;
            double i    = 0;
            while (true)
            {
                term = term * (3 + i) * (1 + i) / (5 / 2d + i) * x / (i + 1);
                double resOld = res;
                res += term;
                if (math.abs(resOld - res) < rtol)
                    return res;
                i += 1;
            }
        }

        private double initialGuess(double T, double ll, int M)
        {
            double x0;
            if (M == 0)
            {
                // Single revolution
                // Equation 19
                double T0 = math.acos(ll) + ll * math.sqrt(1 - math.pow(ll, 2)) + M * pi;
                // Equation 21
                double T1 = 2 * (1 - math.pow(ll, 3)) / 3;

                if (T >= T0)
                    x0 = math.pow(T0 / T, 2 / 3d) - 1;
                else if (T < T1)
                    x0 = 5 / 2d * T1 / T * (T1 - T) / (1 - math.pow(ll, 5)) + 1;
                else
                {
                    // This is the real condition, which is not exactly equivalent
                    // elif T_1 < T < T_0
                    // Corrected initial guess,
                    // piecewise equation right after expression (30) in the original paper is incorrect
                    // See https://github.com/hapsira/hapsira/issues/1362
                    x0 = math.exp(math.log(2) * math.log(T / T0) / math.log(T1 / T0)) - 1;
                }

                return x0;
            }

            // Multiple revolution
            double x0l = (math.pow((M * pi + pi) / (8 * T), 2 / 3d) - 1) / (math.pow((M * pi + pi) / (8 * T), 2 / 3d) + 1);
            double x0r = (math.pow((8 * T) / (M * pi), 2 / 3d) - 1) / (math.pow((8 * T) / (M * pi), 2 / 3d) + 1);

            // Select one of the solutions according to desired type of path

            x0 = LowPath ? math.max(x0l, x0r) : math.min(x0l, x0r);

            return x0;
        }

        /// <summary>
        ///     Reconstruct solution velocity vectors.
        /// </summary>
        private (double V_r1, double V_r2, double V_t1, double V_t2) reconstruct(
            double x, double y, double r1, double r2, double ll, double gamma, double rho, double sigma)
        {
            double V_r1 = gamma * ((ll * y - x) - rho * (ll * y + x)) / r1;
            double V_r2 = -gamma * ((ll * y - x) + rho * (ll * y + x)) / r2;
            double V_t1 = gamma * sigma * (y + ll * x) / r1;
            double V_t2 = gamma * sigma * (y + ll * x) / r2;

            return (V_r1, V_r2, V_t1, V_t2);
        }

        double tofEquation(double x, double T0, double ll, int M)
        {
            return tofEquation_y(x, compute_y(x, ll), T0, ll, M);
        }

        private double tofEquation_p(double x, double y, double T, double ll)
        {
            return (3 * T * x - 2 + 2 * math.pow(ll, 3) * x / y) / (1 - math.pow(x, 2));
        }

        private double tofEquation_p2(double x, double y, double T, double dT, double ll)
        {
            return (3 * T + 5 * x * dT + 2 * (1 - math.pow(ll, 2)) * math.pow(ll, 3) / math.pow(y, 3)) / (1 - math.pow(x, 2));
        }

        double tofEquation_p3(double x, double y, double _, double dT, double ddT, double ll)
        {
            return (7 * x * ddT + 8 * dT - 6 * (1 - math.pow(ll, 2)) * math.pow(ll, 5) * x / math.pow(y, 5)) / (1 - math.pow(x, 2));
        }

        /// <summary>
        ///     Time of flight equation with externally computated y.
        /// </summary>
        /// <returns></returns>
        double tofEquation_y(double x, double y, double T0, double ll, int M)
        {
            double T;
            if (math.abs(ll) < Tolerance && x > math.sqrt(0.6) && x < math.sqrt(1.4))
            {
                double eta = y - ll * x;
                double s1  = (1 - ll - x * eta) * 0.5;
                double Q   = 4 / 3d * hypergeometric2f1b(s1);
                T = (math.pow(eta, 3) * Q + 4 * ll * eta) * 0.5;
            }
            else
            {
                double psi = computePsi(x, y, ll);
                // np.divide(
                // np.divide(psi + M * pi, np.sqrt(np.abs(1 - x**2))) - x + ll * y,
                // (1 - x**2)

                double t0 = (psi + M * pi) / math.sqrt(math.abs(1 - math.pow(x, 2)));
                T = (t0 - x + ll * y) / (1 - math.pow(x, 2));
            }

            return T - T0;
        }
    }
}