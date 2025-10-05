// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using UnitsNet;
using Unity.Mathematics;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics
{
    public static class Distance
    {
        public static Angle AngularSize(Length radius, Length distance)
        {
            double delta = 2 * math.atan(radius / distance);
            return Angle.FromRadians(delta);
        }

        public static float InGameScaleFactor(Angle angularSize, float sceneDistance)
        {
            return sceneDistance * math.tan((float)angularSize.Radians / 2f);
        }
    }
}