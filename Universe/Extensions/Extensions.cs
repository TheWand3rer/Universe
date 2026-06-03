// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

using Unity.Mathematics;
using UnityEngine;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

#endregion

namespace VindemiatrixCollective.Universe.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 ToVector3(this double3 d3) => new((float)d3.x, (float)d3.y, (float)d3.z);

        /// <summary>
        ///     Converts this vector to Km. Assumes the original vector is in m.
        /// </summary>
        /// <param name="vMetres"></param>
        /// <returns></returns>
        public static Vector3d FromMetresToKm(this Vector3d vMetres) => vMetres / 1000;

        /// <summary>
        ///     Converts this vector to m. Assumes the original vector is in Km.
        /// </summary>
        /// <param name="vKm"></param>
        /// <returns></returns>
        public static Vector3d FromKmToMetres(this Vector3d vKm) => vKm * 1000;

        /// <summary>
        ///     Converts this vector to AU. Assumes the original vector is in m.
        /// </summary>
        /// <param name="vMetres"></param>
        /// <param name="unitsPerAU">Use to scale this vector, in terms of Unity units per AU.</param>
        /// <returns></returns>
        public static Vector3d FromMetresToAU(this Vector3d vMetres, float unitsPerAU = 1f) => OrbitalMechanics.MetresToAu(vMetres, unitsPerAU);
    }
}