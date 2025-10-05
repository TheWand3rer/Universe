// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System.Text;
using UnitsNet;
using UnityEngine.Assertions;
using Math = System.Math;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics
{
    public readonly struct RelativisticTravelData
    {
        public Acceleration Acceleration { get; }

        public bool Orbit { get; }

        public double Rapidity { get; }

        public Duration ObserverTimeAcceleration { get; }

        public Duration ObserverTimeCruise { get; }
        public Duration ShipTimeAcceleration { get; }

        public Duration ShipTimeCruise { get; }

        public Duration TotalObserverTime { get; }

        public Duration TotalShipTime { get; }
        public Length Distance { get; }
        public Length DistanceAcceleration { get; }

        public Length DistanceCruise { get; }
        public Speed MaxSpeed { get; }

        public RelativisticTravelData(
            Length distance, Speed maxSpeed, Acceleration acceleration, Duration shipTimeAcceleration, Duration observerTimeAcceleration,
            Duration shipTimeCruise, Duration observerTimeCruise, Duration totalShipTime, Duration totalObserverTime,
            Length distanceAcceleration, Length distanceCruise, double rapidity, bool orbit = true)
        {
            Distance                 = distance;
            MaxSpeed                 = maxSpeed;
            Acceleration             = acceleration;
            ShipTimeAcceleration     = shipTimeAcceleration;
            ObserverTimeAcceleration = observerTimeAcceleration;
            ShipTimeCruise           = shipTimeCruise;
            ObserverTimeCruise       = observerTimeCruise;
            TotalShipTime            = totalShipTime;
            TotalObserverTime        = totalObserverTime;
            DistanceAcceleration     = distanceAcceleration;
            DistanceCruise           = distanceCruise;
            Rapidity                 = rapidity;
            Orbit                    = orbit;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Distance: {Distance.LightYears:0.00} ly");
            sb.AppendLine($"Maximum speed reached: {(MaxSpeed.KilometersPerSecond / UniversalConstants.Celestial.LightSpeedKilometresPerSecond):0.00} c");
            sb.AppendLine($"Acceleration: {Acceleration.StandardGravity:0.00} g");
            sb.AppendLine($"Total travel time (obs/ship): {TotalObserverTime.Years365:0.00} y / {TotalShipTime.Years365:0.00} y");
            sb.AppendLine($"Acceleration/deceleration phase (obs/ship): {ObserverTimeAcceleration.Years365:0.00} y / {ShipTimeAcceleration.Years365:0.00} y");
            sb.AppendLine($"Cruise phase (obs/ship): {ObserverTimeCruise.Years365:0.00} y / {ShipTimeCruise.Years365:0.00} y");
            if (Orbit)
                sb.AppendLine($"Distance acceleration - cruise - deceleration: {DistanceAcceleration.LightYears:0.00} ly + {DistanceCruise.LightYears:0.00} ly + {DistanceAcceleration.LightYears:0.00} ly");
            else
                sb.AppendLine($"Distance acceleration - cruise: {DistanceAcceleration.LightYears:0.00} ly + {DistanceCruise.LightYears:0.00} ly");
            sb.AppendLine($"Total distance: {(DistanceAcceleration * (Orbit ? 2 : 1) + DistanceCruise).LightYears:0.00} ly");
            sb.AppendLine($"Rapidity: {Rapidity:0.00}");

            return sb.ToString();
        }
    }

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
        /// <param name="shipMaxSpeed">The maximum speed the ship will reach.</param>
        /// <param name="acceleration">The constant acceleration provided by the engines.</param>
        /// <param name="decelerate">If the ship should decelerate.</param>
        /// <returns>A <see cref="RelativisticTravelData" /> struct.</returns>
        public static RelativisticTravelData CalculateTravel(
            Length distance, Speed shipMaxSpeed, Acceleration acceleration, bool decelerate = true)
        {
            Assert.IsTrue(!double.IsNaN(distance.Value) && distance.Value > 0, nameof(distance));
            Assert.IsTrue(!double.IsNaN(shipMaxSpeed.Value) && shipMaxSpeed.Value > 0, nameof(shipMaxSpeed));
            Assert.IsTrue(!double.IsNaN(acceleration.Value) && acceleration.Value > 0, nameof(acceleration));

            double   gamma                = CalculateTimeDilation(shipMaxSpeed);
            Duration shipTimeAcceleration = CalculateShipTimeAcceleration(acceleration, shipMaxSpeed);
            double   rapidity             = CalculateRapidity(acceleration, shipTimeAcceleration);
            Length   distanceAcceleration = CalculateAccelerationDistance(acceleration, rapidity);
            Length   distanceCruise       = distance - (decelerate ? 2 : 1) * distanceAcceleration;

            Duration observerTimeAcceleration = CalculateObserverTimeAcceleration(acceleration, rapidity);
            Duration observerTimeCruise       = distanceCruise / shipMaxSpeed;

            Duration shipTimeCruise    = observerTimeCruise / gamma;
            Duration totalObserverTime = (observerTimeAcceleration * (decelerate ? 2 : 1)) + observerTimeCruise;
            Duration totalShipTime     = (shipTimeAcceleration * (decelerate ? 2 : 1)) + shipTimeCruise;

            return new RelativisticTravelData(distance, shipMaxSpeed, acceleration, shipTimeAcceleration, observerTimeAcceleration,
                                              shipTimeCruise, observerTimeCruise, totalShipTime, totalObserverTime, distanceAcceleration,
                                              distanceCruise, rapidity, decelerate);
        }

        /// <summary>
        ///     Calculates the time a ship would take to travel a distance <strong>d</strong>, reaching a max speed of
        ///     <strong>v</strong>,
        ///     under an acceleration <strong>a</strong>. The ship accelerates until it reaches max speed, then cruises, and then
        ///     decelerates.
        ///     Formulas sourced from: https://math.ucr.edu/home/baez/physics/Relativity/SR/Rocket/rocket.html
        /// </summary>
        /// <param name="distanceLY">The distance to the destination star system, in light years.</param>
        /// <param name="deltaV">The total deltaV budget in km/s.</param>
        /// <param name="accelerationG">The constant acceleration provided by the engines, in standard g.</param>
        /// <param name="decelerate">If the ship should decelerate.</param>
        /// <returns>A <see cref="RelativisticTravelData" /> struct.</returns>
        public static RelativisticTravelData CalculateTravel(float distanceLY, float deltaV, float accelerationG, bool decelerate = true)
        {
            return CalculateTravel(Length.FromLightYears(distanceLY), Speed.FromKilometersPerSecond(deltaV / 2),
                                   Acceleration.FromStandardGravity(accelerationG), decelerate);
        }

        public static double CalculateRapidity(Acceleration acceleration, Duration shipTime)
        {
            double a = acceleration.MetersPerSecondSquared;
            double c = UniversalConstants.Celestial.LightSpeedMetresPerSecond;
            double t = shipTime.Seconds;

            return (a * t) / c;
        }

        public static double CalculateTimeDilation(Speed shipMaxSpeed)
        {
            double v = shipMaxSpeed.MetersPerSecond;
            double c = UniversalConstants.Celestial.LightSpeedMetresPerSecond;

            return 1 / Math.Sqrt(1 - ((v * v) / (c * c)));
        }

        public static Duration CalculateObserverTimeAcceleration(Acceleration acceleration, double rapidity)
        {
            double a = acceleration.MetersPerSecondSquared;
            double c = UniversalConstants.Celestial.LightSpeedMetresPerSecond;
            return Duration.FromSeconds((Math.Sinh(rapidity) * c) / a);
        }

        public static Duration CalculateShipTimeAcceleration(Acceleration acceleration, Speed shipMaxSpeed)
        {
            double a        = acceleration.MetersPerSecondSquared;
            double v        = shipMaxSpeed.MetersPerSecond;
            double c        = UniversalConstants.Celestial.LightSpeedMetresPerSecond;
            double shipTime = ((c / a) * Math.Atanh(v / c));

            return Duration.FromSeconds(shipTime);
        }

        public static Length CalculateAccelerationDistance(Acceleration acceleration, double rapidity)
        {
            double a = acceleration.MetersPerSecondSquared;
            double c = UniversalConstants.Celestial.LightSpeedMetresPerSecond;
            return Length.FromMeters(((Math.Cosh(rapidity) - 1) * c * c) / a);
        }

        public static Speed CalculateVelocity(double rapidity)
        {
            double c = UniversalConstants.Celestial.LightSpeedMetresPerSecond;

            return Speed.FromMetersPerSecond(Math.Tanh(rapidity) * c);
        }

        public static Speed SpeedFromFractionOfC(float fraction)
        {
            return Speed.FromMetersPerSecond(UniversalConstants.Celestial.LightSpeedMetresPerSecond * fraction);
        }
    }
}