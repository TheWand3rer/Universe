#region

using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

#endregion

public static class VectorExtensions
{
    /// <summary>
    ///     Converts this vector to Km. Assumes the original vector is in m.
    /// </summary>
    /// <param name="vMetres"></param>
    /// <returns></returns>
    public static Vector3d FromMetresToKm(this Vector3d vMetres)
    {
        return vMetres / 1000;
    }

    /// <summary>
    ///     Converts this vector to m. Assumes the original vector is in Km.
    /// </summary>
    /// <param name="vKm"></param>
    /// <returns></returns>
    public static Vector3d FromKmToMetres(this Vector3d vKm)
    {
        return vKm * 1000;
    }

    /// <summary>
    ///     Converts this vector to AU. Assumes the original vector is in m.
    /// </summary>
    /// <param name="vMetres"></param>
    /// <param name="unitsPerAU">Use to scale this vector, in terms of Unity units per AU.</param>
    /// <returns></returns>
    public static Vector3d FromMetrestoAU(this Vector3d vMetres, float unitsPerAU = 1f)
    {
        return OrbitalMechanics.MetresToAu(vMetres, unitsPerAU);
    }
}