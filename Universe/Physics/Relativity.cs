// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using UnitsNet;
using UnityEngine.Assertions;
using Math = System.Math;

#endregion

namespace VindemiatrixCollective.Universe.Physics
{
    public static class Relativity
    {
        /// <summary>
        ///     Calculates the time a ship would take to travel a distance <strong>d</strong>, reaching a max speed of
        ///     <strong>v</strong>,
        ///     under an acceleration <strong>a</strong>. The ship accelerates until it reaches max speed, then cruises,
        ///     and then decelerates (if decelerate is true).
        ///     Formulas sourced from: https://math.ucr.edu/home/baez/physics/Relativity/SR/Rocket/rocket.html
        /// </summary>
        /// <param name="distance">The distance to the destination star system.</param>
        /// <param name="deltaV">The available deltaV.</param>
        /// <param name="acceleration">The constant acceleration provided by the engines.</param>
        /// <param name="decelerate">If the ship should decelerate.</param>
        /// <returns>A <see cref="RelativisticTravelData" /> struct.</returns>
        public static RelativisticTravelData CalculateTravel(Length distance, Speed deltaV, Acceleration acceleration, bool decelerate = true)
        {
            Assert.IsTrue(!double.IsNaN(distance.Value) && distance.Value > 0, nameof(distance));
            Assert.IsTrue(!double.IsNaN(deltaV.Value) && deltaV.Value > 0, nameof(deltaV));
            Assert.IsTrue(!double.IsNaN(acceleration.Value) && acceleration.Value > 0, nameof(acceleration));

            int      multiplier           = decelerate ? 2 : 1;
            Speed    maxSpeed             = deltaV / multiplier;
            double   gamma                = CalculateTimeDilation(maxSpeed);
            Duration shipTimeAcceleration = CalculateShipTimeAcceleration(acceleration, maxSpeed);
            double   rapidity             = CalculateRapidity(acceleration, shipTimeAcceleration);
            Length   distanceAcceleration = CalculateAccelerationDistance(acceleration, rapidity);
            Length   distanceCruise       = distance - multiplier * distanceAcceleration;

            Duration observerTimeAcceleration = CalculateObserverTimeAcceleration(acceleration, rapidity);
            Duration observerTimeCruise       = distanceCruise / maxSpeed;
            Duration shipTimeCruise           = observerTimeCruise / gamma;
            Duration totalObserverTime        = observerTimeAcceleration * (decelerate ? 2 : 1) + observerTimeCruise;
            Duration totalShipTime            = shipTimeAcceleration * (decelerate ? 2 : 1) + shipTimeCruise;

            return new RelativisticTravelData(distance,
                                              deltaV,
                                              maxSpeed,
                                              acceleration,
                                              shipTimeAcceleration,
                                              observerTimeAcceleration,
                                              shipTimeCruise,
                                              observerTimeCruise,
                                              totalShipTime,
                                              totalObserverTime,
                                              distanceAcceleration,
                                              distanceCruise,
                                              rapidity,
                                              decelerate);
        }

        /// <summary>
        ///     Calculates the time a ship would take to travel a distance <strong>d</strong>, reaching a max speed of
        ///     <strong>v</strong>,
        ///     under an acceleration <strong>a</strong>. The ship accelerates until it reaches max speed, then cruises, and then
        ///     decelerates (if decelerate is true).
        ///     Formulas sourced from: https://math.ucr.edu/home/baez/physics/Relativity/SR/Rocket/rocket.html
        /// </summary>
        /// <param name="distanceLY">The distance to the destination star system, in light years.</param>
        /// <param name="deltaV">The total deltaV budget in km/s.</param>
        /// <param name="accelerationG">The constant acceleration provided by the engines, in standard g.</param>
        /// <param name="decelerate">If the ship should decelerate.</param>
        /// <returns>A <see cref="RelativisticTravelData" /> struct.</returns>
        public static RelativisticTravelData CalculateTravel(float distanceLY, float deltaV, float accelerationG, bool decelerate = true) =>
            CalculateTravel(Length.FromLightYears(distanceLY),
                            Speed.FromKilometersPerSecond(deltaV / 2),
                            Acceleration.FromStandardGravity(accelerationG),
                            decelerate);

        public static double CalculateRapidity(Acceleration acceleration, Duration shipTime)
        {
            double a = acceleration.MetersPerSecondSquared;
            double c = UniversalConstants.Celestial.LightSpeedMetresPerSecond;
            double t = shipTime.Seconds;

            return a * t / c;
        }

        public static double CalculateTimeDilation(Speed shipMaxSpeed)
        {
            double v = shipMaxSpeed.MetersPerSecond;
            double c = UniversalConstants.Celestial.LightSpeedMetresPerSecond;

            return 1 / Math.Sqrt(1 - v * v / (c * c));
        }

        public static Duration CalculateObserverTimeAcceleration(Acceleration acceleration, double rapidity)
        {
            double a = acceleration.MetersPerSecondSquared;
            double c = UniversalConstants.Celestial.LightSpeedMetresPerSecond;
            return Duration.FromSeconds(Math.Sinh(rapidity) * c / a);
        }

        public static Duration CalculateShipTimeAcceleration(Acceleration acceleration, Speed shipMaxSpeed)
        {
            double a        = acceleration.MetersPerSecondSquared;
            double v        = shipMaxSpeed.MetersPerSecond;
            double c        = UniversalConstants.Celestial.LightSpeedMetresPerSecond;
            double shipTime = c / a * Math.Atanh(v / c);

            return Duration.FromSeconds(shipTime);
        }

        public static Length CalculateAccelerationDistance(Acceleration acceleration, double rapidity)
        {
            double a = acceleration.MetersPerSecondSquared;
            double c = UniversalConstants.Celestial.LightSpeedMetresPerSecond;
            return Length.FromMeters((Math.Cosh(rapidity) - 1) * c * c / a);
        }

        public static Speed CalculateVelocity(double rapidity)
        {
            double c = UniversalConstants.Celestial.LightSpeedMetresPerSecond;

            return Speed.FromMetersPerSecond(Math.Tanh(rapidity) * c);
        }

        public static Speed SpeedFromFractionOfC(float fraction) =>
            Speed.FromMetersPerSecond(UniversalConstants.Celestial.LightSpeedMetresPerSecond * fraction);
    }
}