// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using System.Text;
using UnitsNet;

#endregion

namespace VindemiatrixCollective.Universe.Physics
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
        public Speed DeltaV { get; }

        public RelativisticTravelData(
            Length distance, Speed deltaV, Speed maxSpeed, Acceleration acceleration, Duration shipTimeAcceleration,
            Duration observerTimeAcceleration,
            Duration shipTimeCruise, Duration observerTimeCruise, Duration totalShipTime, Duration totalObserverTime,
            Length distanceAcceleration, Length distanceCruise, double rapidity, bool orbit = true)
        {
            Distance                 = distance;
            DeltaV                   = deltaV;
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
            StringBuilder sb = new();
            sb.AppendLine($"Distance: {Distance.LightYears:0.00} ly");
            sb.AppendLine($"Available deltaV: {DeltaV.KilometersPerSecond} km/s");
            sb.AppendLine(
                $"Maximum speed reached: {MaxSpeed.KilometersPerSecond / UniversalConstants.Celestial.LightSpeedKilometresPerSecond:0.00} c");
            sb.AppendLine($"Acceleration: {Acceleration.StandardGravity:0.00} g");
            sb.AppendLine($"Total travel time (obs/ship): {TotalObserverTime.Years365:0.00} y / {TotalShipTime.Years365:0.00} y");
            sb.AppendLine(
                $"Acceleration/deceleration phase (obs/ship): {ObserverTimeAcceleration.Years365:0.00} y / {ShipTimeAcceleration.Years365:0.00} y");
            sb.AppendLine($"Cruise phase (obs/ship): {ObserverTimeCruise.Years365:0.00} y / {ShipTimeCruise.Years365:0.00} y");
            if (Orbit)
            {
                sb.AppendLine(
                    $"Distance acceleration - cruise - deceleration: {DistanceAcceleration.LightYears:0.00} ly + {DistanceCruise.LightYears:0.00} ly + {DistanceAcceleration.LightYears:0.00} ly");
            }
            else
            {
                sb.AppendLine($"Distance acceleration - cruise: {DistanceAcceleration.LightYears:0.00} ly + {DistanceCruise.LightYears:0.00} ly");
            }

            sb.AppendLine($"Total distance: {(DistanceAcceleration * (Orbit ? 2 : 1) + DistanceCruise).LightYears:0.00} ly");
            sb.AppendLine($"Rapidity: {Rapidity:0.00}");

            return sb.ToString();
        }
    }
}