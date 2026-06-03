// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

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

        public static float InGameScaleFactor(Angle angularSize, float sceneDistance) =>
            2 * sceneDistance * math.tan((float)angularSize.Radians / 2f);
    }
}