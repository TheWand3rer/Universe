// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using System;
using UnitsNet;
using UnityEngine.Assertions;

#endregion

namespace VindemiatrixCollective.Universe.Physics
{
    public static class Kinematics
    {
        /// <summary>
        /// Calculates the time to reach a given distance under constant acceleration,
        /// and an optional initial velocity.
        /// Solves: d = v0*t + ½at²
        /// </summary>
        /// <param name="distance">Total distance to cover.</param>
        /// <param name="acceleration">Constant acceleration.</param>
        /// <param name="initialSpeed">Initial speed (zero otherwise).</param>
        /// <returns>Time to reach the target distance.</returns>
        public static Duration Time(
            Length distance,
            Acceleration acceleration,
            Speed? initialSpeed = null)
        {
            Assert.IsTrue(distance.Value > 0 && acceleration.Value > 0,
                          $"{nameof(distance)} and {nameof(acceleration)} must be positive and non-zero");
            double d  = distance.Meters;
            double a  = acceleration.MetersPerSecondSquared;
            double v0 = initialSpeed?.MetersPerSecond ?? 0.0;

            double discriminant = v0 * v0 + 2.0 * a * d;
            double seconds      = (-v0 + Math.Sqrt(discriminant)) / a;

            return Duration.FromSeconds(seconds);
        }

        public static Duration Time(Length distance, Speed deltaV) => distance / deltaV;

        public static Speed Velocity(Acceleration acceleration, Duration time, Speed? initialSpeed = null)
        {
            double a  = acceleration.MetersPerSecondSquared;
            double t  = time.Seconds;
            double v0 = initialSpeed?.MetersPerSecond ?? 0.0;

            double v = v0 + a * t;

            return Speed.FromMetersPerSecond(v);
        }
    }
}