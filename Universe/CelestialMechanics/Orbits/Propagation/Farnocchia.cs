#region

using System;
using UnitsNet;
using UnityEngine.Assertions;
using Angle = UnitsNet.Angle;
using Length = UnitsNet.Length;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics.Orbits.Propagation
{
    /// <summary>
    ///     Propagates orbit using the algorithm described by Davide Farnocchia in
    ///     http://dx.doi.org/10.1007/s10569-013-9476-9.
    ///     Implementation based on Poliastro & Hapsira
    ///     https://github.com/pleiszenburg/hapsira
    /// </summary>
    public class Farnocchia : IPropagator
    {
        /// <summary>
        ///     Propagates orbit using mean motion.
        /// </summary>
        /// <param name="state">Current orbit state</param>
        /// <param name="tof">Time of flight (s)</param>
        public Angle PropagateOrbit(OrbitState state, Duration tof)
        {
            Assert.IsNotNull(state.Attractor, $"{nameof(OrbitState)}.{nameof(OrbitState.Attractor)} cannot be null");

            double p       = state.SemiLatusRectum.Meters;
            double q       = p / (1 + state.Eccentricity.Value);
            double deltaT0 = DeltaTFromNu(state.TrueAnomaly.Radians, state.Eccentricity.Value, state.Attractor.Mu.M3S2, q);
            double deltaT  = deltaT0 + tof.Seconds;

            double nu = NuFromDeltaT(deltaT, state.Eccentricity.Value, state.GravitationalParameter.M3S2, q);
            return Angle.FromRadians(nu);
        }

        /// <summary>
        ///     Time elapsed since periapsis (q) for the given true anomaly.
        /// </summary>
        /// <param name="nu">True anomaly (rad)</param>
        /// <param name="e">Eccentricity</param>
        /// <param name="muM3S2">Gravitational Parameter (M3/S2)</param>
        /// <param name="q">
        ///     Periapsis distance (m)/param>
        ///     <param name="delta"></param>
        ///     <returns>Time elapsed since periapsis (s)</returns>
        public static double DeltaTFromNu(double nu, double e, double muM3S2, double q, double delta = 1e-2)
        {
            const double pi = UniversalConstants.Tri.Pi;
            if (nu is < -pi or > pi)
            {
                throw new InvalidOperationException("nu must be between -pi <= nu < pi");
            }

            double E; // Eccentric Anomaly
            double M; // Mean Anomaly
            double n; // Term of equation (4) in the paper

            if (e < 1 - delta)
            {
                E = OrbitalMechanics.TrueToEccentricAnomaly(nu, e);
                M = OrbitalMechanics.EccentricToMeanAnomaly(E, e);
                n = Math.Sqrt(muM3S2 * Math.Pow(1 - e, 3) / Math.Pow(q, 3));
            }
            else if (e >= 1 - delta && e < 1)
            {
                E = OrbitalMechanics.TrueToEccentricAnomaly(nu, e);
                if (delta <= 1 - e * Math.Cos(E))
                {
                    M = OrbitalMechanics.EccentricToMeanAnomaly(E, e);
                    n = Math.Sqrt(muM3S2 * Math.Pow(1 - e, 3) / Math.Pow(q, 3));
                }
                else
                {
                    throw new NotImplementedException("Near parabolic case not implemented");
                }
            }
            else if (Mathd.Approximately(e, 1))
            {
                throw new NotImplementedException("Parabolic case not implemented");
            }
            else if (1 + e * Math.Cos(nu) < 0)
            {
                return double.NaN;
            }
            else if (e > 1 && e <= 1 + delta)
            {
                throw new NotImplementedException("Hyperbolic case not implemented");
            }
            else if (e > 1 + delta)
            {
                throw new NotImplementedException("Strong Hyperbolic case not implemented");
            }
            else
            {
                throw new InvalidOperationException("Invalid case");
            }


            return M / n;
        }

        /// <summary>
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="e"></param>
        /// <param name="mu">Gravitational parameter (M3/S2)</param>
        /// <param name="q"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static double NuFromDeltaT(double dt, double e, double mu, double q, double delta = 1e-2)
        {
            const double pi  = UniversalConstants.Tri.Pi;
            const double pi2 = UniversalConstants.Tri.Pi2;

            double E; // Eccentric Anomaly
            double M; // Mean Anomaly
            double n;
            double nu;
            if (e < 1 - delta)
            {
                n  = Math.Sqrt(mu * Math.Pow(1 - e, 3) / Math.Pow(q, 3));
                M  = n * dt;
                E  = OrbitalMechanics.MeanToEccentricAnomaly((M + pi) % pi2 - pi, e);
                nu = OrbitalMechanics.EccentricToTrueAnomaly(E, e);
            }
            else if (e > 1 - delta && e < 1)
            {
                double Edelta = Math.Acos((1 - delta) / e);
                n = Math.Sqrt(mu * Math.Pow(1 - e, 3) / Math.Pow(q, 3));
                M = n * dt;
                if (OrbitalMechanics.EccentricToMeanAnomaly(Edelta, e) <= Math.Abs(M))
                {
                    // Strong elliptic
                    E  = OrbitalMechanics.MeanToEccentricAnomaly((M + pi) % pi2 - pi, e);
                    nu = OrbitalMechanics.EccentricToMeanAnomaly(E, e);
                }
                else
                {
                    throw new NotImplementedException("Near parabolic case not implemented");
                }
            }
            else if (Mathd.Approximately(e, 1))
            {
                throw new NotImplementedException("Parabolic case not implemented");
            }
            else if (e > 1 && e <= 1 + delta)
            {
                throw new NotImplementedException("Hyperbolic case not implemented");
            }
            else
            {
                throw new NotImplementedException("Strong Hyperbolic case not implemented");
            }

            return nu;
        }


        /// <summary>
        ///     Time elapsed since periapsis (q) for the given true anomaly.
        /// </summary>
        /// <param name="nu">True anomaly (rad)</param>
        /// <param name="e">Eccentricity</param>
        /// <param name="mu">Gravitational Parameter (M3/S2)</param>
        /// <param name="q">Periapsis distance (m)</param>
        /// <param name="delta"></param>
        /// <returns>Time elapsed since periapsis (s)</returns>
        public static Duration DeltaTFromNu(Angle nu, Ratio e, GravitationalParameter mu, Length q, double delta = 1e-2)
        {
            double dt = DeltaTFromNu(nu.Radians, e.Value, mu.M3S2, q.Meters, delta);
            return Duration.FromSeconds(dt);
        }
    }
}