#region

using UnitsNet;
using Unity.Mathematics;
using UnityEngine;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics.Orbits.Propagation
{
    public class Kinematic : IPropagator
    {
        public Angle PropagateOrbit(OrbitState state, Duration tof)
        {
            double e    = state.Eccentricity.Value;
            double nu   = state.TrueAnomaly.Radians;
            double P    = Duration.FromYears365(80).Seconds; //  state.Period.Seconds;
            double t0   = DeltaTFromNu(nu, e, P);
            double t    = t0 + tof.Seconds;
            double a    = state.SemiMajorAxis.Meters;
            double loAN = state.LongitudeAscendingNode.Radians;
            double i    = state.Inclination.Degrees;
            double argP = state.LongitudeAscendingNode.Radians;

            double E = TimeToEccentricAnomaly(t, P, e);
            //double nu = OrbitalMechanics.EccentricToTrueAnomaly(E, e);
            //return CalculatePosition(a, e, E, loAN, i, argP);
            Debug.LogError("P: " + state.Period.Years365);
            return Angle.FromRadians(OrbitalMechanics.EccentricToTrueAnomaly(E, e));
        }

        private static double DeltaTFromNu(double nu, double e, double P)
        {
            double E = OrbitalMechanics.TrueToEccentricAnomaly(nu, e);
            double M = OrbitalMechanics.EccentricToMeanAnomaly(E, e);

            double t0 = M * P / UniversalConstants.Tri.Pi2;

            return t0;
        }

        private static double TimeToEccentricAnomaly(double t, double P, double e)
        {
            double Pi2 = UniversalConstants.Tri.Pi2;

            // Calculate Mean anomaly over one period
            double M = Pi2 * t / P;
            M %= Pi2;

            // Mean to Eccentric anomaly
            double E = OrbitalMechanics.MeanToEccentricAnomaly(M, e);

            return E;
        }

        private static Vector3d CalculatePosition(double a, double e, double E, double loAN, double i, double argP)
        {
            double x = a * math.cos(E) - e;
            double y = a * math.sqrt(1 - e * e) * math.sin(E);
            double z = 0;

            double cos_lan  = math.cos(loAN);
            double sin_lan  = math.sin(loAN);
            double cos_i    = math.cos(i);
            double sin_i    = math.sin(i);
            double cos_argp = math.cos(argP);
            double sin_argp = math.sin(argP);

            double3 r_orbital = new(x, y, z);

            double3x3 mRotation = OrbitalMechanics.RotationMatrix(loAN, 2);
            mRotation = math.mul(mRotation, OrbitalMechanics.RotationMatrix(i, 0));
            mRotation = math.mul(mRotation, OrbitalMechanics.RotationMatrix(argP, 2));

            double3 pos = math.mul(mRotation, r_orbital);

            return new Vector3d(pos[0], pos[1], pos[2]);
        }
    }
}