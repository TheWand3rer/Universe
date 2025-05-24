#region

using System;
using UnitsNet;

#endregion

namespace VindemiatrixCollective.Universe.Rocketry
{
    public static class Rocket
    {
        public static double CalculateMassRatio(Speed desiredDeltaV, Speed exhaustVelocity)
        {
            return Math.Exp(desiredDeltaV / exhaustVelocity);
        }

        public static Speed CalculateDeltaV(Speed exhaustVelocity, Mass payload, Mass fuelMass)
        {
            Speed c = Speed.FromMetersPerSecond(UniversalConstants.Celestial.LightSpeedMetresPerSecond);

            return c * Math.Tanh((exhaustVelocity / c) * Math.Log((payload + fuelMass) / payload));
        }
    }
}