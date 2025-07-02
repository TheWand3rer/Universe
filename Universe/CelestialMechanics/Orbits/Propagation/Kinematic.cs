#region

using UnitsNet;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics.Orbits.Propagation
{
    public class Kinematic : IPropagator
    {
        private readonly double period;

        public Kinematic(OrbitalData data) { period = data.Period.Seconds; }

        public (Angle nu, Angle E, Angle M) PropagateOrbit(OrbitState state, Duration tof)
        {
            double e  = state.Eccentricity.Value;
            double nu = state.TrueAnomaly.Radians;
            double P  = period;
            double t0 = DeltaTFromNu(nu, e, P);
            double t  = t0 + tof.Seconds;

            double E = TimeToEccentricAnomaly(t, P, e, out double M);
            nu = OrbitalMechanics.EccentricToTrueAnomaly(E, e);
            return (Angle.FromRadians(nu), Angle.FromRadians(E), Angle.FromRadians(M));
        }

        private static double DeltaTFromNu(double nu, double e, double P)
        {
            double E = OrbitalMechanics.TrueToEccentricAnomaly(nu, e);
            double M = OrbitalMechanics.EccentricToMeanAnomaly(E, e);

            double t0 = M * P / UniversalConstants.Tri.Pi2;

            return t0;
        }

        private static double TimeToEccentricAnomaly(double t, double P, double e, out double M)
        {
            double Pi2 = UniversalConstants.Tri.Pi2;

            // Calculate Mean anomaly over one period
            M = Pi2 * t / P;
            M %= Pi2;

            double E = OrbitalMechanics.MeanToEccentricAnomaly(M, e);

            return E;
        }
    }
}