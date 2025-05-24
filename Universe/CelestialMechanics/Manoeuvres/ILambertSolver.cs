#region

using UnitsNet;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics.Manoeuvres
{
    public interface ILambertSolver
    {
        bool LowPath { get; set; }
        bool Prograde { get; set; }
        int MaxIterations { get; }
        int Revolutions { get; set; }

        /// <summary>
        ///     Solves Lambert's problem.
        /// </summary>
        /// <returns>Returns velocities in km/s.</returns>
        (Vector3d v1, Vector3d v2) Lambert(GravitationalParameter gravitationalParameter, Vector3d initialPosition, Vector3d finalPosition, Duration timeOfFlight);
    }
}