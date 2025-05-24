#region

using System;
using UnitsNet;
using UnityEngine;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using Angle = UnitsNet.Angle;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics
{
    public class Ellipse
    {
        public double A;
        public Vector3d AxisMain;
        public Vector3d AxisSecondary;
        public double B;
        public Vector3d Center;
        public Ratio Eccentricity;
        public Vector3d Focus0;
        public Vector3d Focus1;
        public Vector3d FocusDistance;
        public Vector3d Normal => Vector3d.Cross(AxisMain, AxisSecondary).normalized;

        public Ellipse(Vector3d focus0, Vector3d focus1, Vector3d p0)
        {
            Focus0        = focus0;
            Focus1        = focus1;
            FocusDistance = Focus0 - Focus1;
            A             = ((Focus0 - p0).magnitude + (focus1 - p0).magnitude) * 0.5;
            if (A < 0)
            {
                A = -A;
            }

            Eccentricity = Ratio.FromDecimalFractions((FocusDistance.magnitude * 0.5) / A);
            B            = A * Math.Sqrt(1 - (Eccentricity.Value * Eccentricity.Value));
            AxisMain     = FocusDistance.normalized;
            Vector3d tempNorm = Vector3d.Cross(AxisMain, p0 - Focus0).normalized;
            AxisSecondary = Vector3d.Cross(AxisMain, tempNorm).normalized;
            Center        = Focus1 + (FocusDistance * 0.5);

            if (Vector3d.Dot(Normal, Vector3d.Z) < 0)
            {
                AxisSecondary = -AxisSecondary;
            }
        }

        public Vector3[] CalculateEllipseArcPoints(
            Angle eStart, Angle eEnd, Vector3d startPosition, Vector3d endPosition, int steps = 64,
            float scale = 1, bool ccw = true)
        {
            Vector3[] points = new Vector3[steps];
            Vector3d  point  = startPosition;
            float     fSteps = steps;
            points[0] = scale * point.ToXZY();

            double delta = eEnd.Radians - eStart.Radians;

            if (!ccw)
            {
                delta *= -1;
            }


            for (int i = 1; i <= steps - 1; i++)
            {
                float ratio = i / fSteps;
                point     = scale * GetSamplePoint(eStart.Radians + (delta * ratio));
                points[i] = point.ToXZY();
            }

            points[steps - 1] = scale * endPosition.ToXZY();

            return points;
        }

        /// <summary>
        ///     Calculate eccentric anomaly in radians for point.
        /// </summary>
        /// <param name="point">Point in plane of elliptic shape.</param>
        /// <returns>Eccentric anomaly radians.</returns>
        public Angle GetEccentricAnomalyForPoint(Vector3d point)
        {
            Vector3d vector      = point - Focus0;
            double   trueAnomaly = Vector3d.Angle(vector, AxisMain) * UniversalConstants.Tri.DegreeToRad;

            if (Vector3d.Dot(vector, AxisSecondary) > 0)
            {
                trueAnomaly = UniversalConstants.Tri.Pi2 - trueAnomaly;
            }

            Angle result = OrbitalMechanics.TrueToEccentricAnomaly(Angle.FromRadians(trueAnomaly), Eccentricity);
            return result;
        }

        /// <summary>
        ///     Get point on ellipse at specified angle from center.
        /// </summary>
        /// <param name="eccentricAnomaly">Angle from center in radians</param>
        /// <returns></returns>
        public Vector3d GetSamplePoint(double eccentricAnomaly)
        {
            return Center + (AxisMain * (A * Math.Cos(eccentricAnomaly))) + (AxisSecondary * (B * Math.Sin(eccentricAnomaly)));
        }

        public void PrintOut()
        {
            Debug.Log($"Ellipse C({Center.x:F2},{Center.y:F2},{Center.z:F2} a:{A:F2} b:{B:F2}");
        }
    }
}