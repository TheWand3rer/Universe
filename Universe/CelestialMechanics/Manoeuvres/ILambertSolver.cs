using UnitsNet;

namespace VindemiatrixCollective.Universe.CelestialMechanics.Manoeuvres
{
    public interface ILambertSolver
    {
        int MaxIterations { get; }
        int Revolutions { get; set; }
        bool Prograde { get; set; }
        bool LowPath { get; set; }

        /// <summary>
        /// Solves Lambert's problem.
        /// </summary>
        /// <returns>Returns velocities in km/s.</returns>
        (Vector3d v1, Vector3d v2) Lambert(GravitationalParameter gravitationalParameter, Vector3d initialPosition, Vector3d finalPosition, Duration timeOfFlight);
    }
}