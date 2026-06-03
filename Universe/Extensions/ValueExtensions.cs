// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using System;

#endregion

namespace VindemiatrixCollective.Universe.Extensions
{
    public static class ValueExtensions
    {
        public static double ToDegrees(this double d) => d * (180 / Math.PI);

        public static double ToRadians(this double d) => d * (Math.PI / 180);
    }
}