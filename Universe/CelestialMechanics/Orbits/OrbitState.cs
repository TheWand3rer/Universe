﻿using System;
using System.Text;
using UnitsNet;
using Unity.Mathematics;
using UnityEngine;
using VindemiatrixCollective.Universe.CelestialMechanics.Manoeuvres;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits.Propagation;
using VindemiatrixCollective.Universe.Model;
using Impulse = VindemiatrixCollective.Universe.CelestialMechanics.Manoeuvres.Impulse;

namespace VindemiatrixCollective.Universe.CelestialMechanics.Orbits
{
    public class OrbitState
    {
        public Angle ArgumentPeriapsis => Angle.FromRadians(argP);
        public Angle EccentricAnomaly => Angle.FromRadians(E);
        public Angle Inclination => Angle.FromRadians(i);

        public Angle LongitudeAscendingNode => Angle.FromRadians(loAN);

        public Angle TrueAnomaly => Angle.FromRadians(nu);
        public CelestialBody Attractor => attractor;

        public DateTime Epoch => epoch;

        public Duration Period => Duration.FromSeconds(period);
        public GravitationalParameter GravitationalParameter => mu;

        public Length ApoapsisDistance => (1 + Eccentricity.Value) * SemiMajorAxis;
        public Length PeriapsisDistance => (1 - Eccentricity.Value) * SemiMajorAxis;
        public Length SemiLatusRectum => Length.FromMeters(OrbitalMechanics.SemiLatusRectum(a, e));
        public Length SemiMajorAxis => Length.FromMeters(a);

        public Length SemiMinorAxis => Length.FromMeters(b);

        public Ratio Eccentricity => Ratio.FromDecimalFractions(e);
        public Vector3d AngularMomentum => Vector3d.Cross(Position, Velocity);

        public Vector3d EccentricityVector => (1 / mu.M3S2) *
                                              (((Velocity.sqrMagnitude - (mu.M3S2 / Position.magnitude)) * Position) -
                                               (Vector3d.Dot(Position, Velocity) * Velocity));

        public Vector3d NodeVector => Vector3d.Cross(Vector3d.Z, AngularMomentum);

        /// <summary>
        /// Orbital position (m).
        /// </summary>
        public Vector3d Position => position;
        
        /// <summary>
        /// Orbital velocity (m/s).
        /// </summary>
        public Vector3d Velocity => velocity;

        public (Length p, Ratio e, Angle i, Angle loAN, Angle argP, Angle nu) ToElements() =>
            (SemiLatusRectum, Eccentricity, Inclination, LongitudeAscendingNode, ArgumentPeriapsis, TrueAnomaly);

        public static (Vector3d r, Vector3d v) ClassicalElementsToVectors(double a, double e, double mu, double i, double loAN, double argP, double nu)
        {
            double3x3 mRotation = OrbitalMechanics.RotationMatrix(loAN, 2);
            mRotation = math.mul(mRotation, OrbitalMechanics.RotationMatrix(i, 0));
            mRotation = math.mul(mRotation, OrbitalMechanics.RotationMatrix(argP, 2));

            double p = OrbitalMechanics.SemiLatusRectum(a, e);
            (Vector3d r, Vector3d v) = OrbitalMechanics.RVinPerifocalFrame(mu, p, e, nu);

            double3 vr = math.mul(mRotation, new double3(r[0], r[1], r[2]));
            double3 vv = math.mul(mRotation, new double3(v[0], v[1], v[2]));

            r = new Vector3d(vr[0], vr[1], vr[2]);
            v = new Vector3d(vv[0], vv[1], vv[2]);

            return (r, v);
        }

        public (Vector3d r, Vector3d v) ToVectors()
        {
            return ClassicalElementsToVectors(a, e, mu.M3S2, i, loAN, argP, nu);
        }

        public double[] ToArrayElements() => new[]
        {
            SemiLatusRectum.Meters, Eccentricity.Value, Inclination.Radians, LongitudeAscendingNode.Radians, ArgumentPeriapsis.Radians,
            TrueAnomaly.Radians
        };

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.AppendLine($"OrbitState at {epoch: dd/MM/yyyy}:");
            sb.AppendLine($"  {nameof(SemiMajorAxis)}: {SemiMajorAxis.AstronomicalUnits:F2} AU");
            sb.AppendLine($"  {nameof(Eccentricity)}: {Eccentricity.DecimalFractions}");
            sb.AppendLine($"  {nameof(Period)}: {Period.Days:F2} d");
            sb.AppendLine($"  {nameof(Inclination)}: {Inclination.Degrees:F2}°");
            sb.AppendLine($"  {nameof(TrueAnomaly)}: {TrueAnomaly.Degrees:F2}°");
            sb.AppendLine($"  {nameof(Position)}: {Position.FromMetresToKm()} km");
            sb.AppendLine($"  {nameof(Velocity)}: {Velocity.FromMetresToKm()} km/s");

            return sb.ToString();
        }

        public OrbitState Clone()
        {
            OrbitState state = new()
            {
                attractor = attractor,
                a = a,
                b = b,
                e = e,
                i = i,
                loAN = loAN,
                argP = argP,
                M = M,
                nu = nu,
                period = period,
                mu = mu,
                position = position,
                velocity = velocity,
                epoch = epoch
            };
            return state;
        }

        public static OrbitState FromOrbitalElements(OrbitalData orbitalData, CelestialBody attractor)
        {
            OrbitState state = FromOrbitalElements(orbitalData);
            state.mu = GravitationalParameter.FromMass(attractor.PhysicalData.Mass);
            state.SetAttractor(attractor);
            return state;
        }

        public static OrbitState FromOrbitalElements(OrbitalData orbitalData)
        {
            OrbitState state = new()
            {
                a = orbitalData.SemiMajorAxis.Meters,
                e = orbitalData.Eccentricity.Value,
                i = orbitalData.Inclination.Radians,
                loAN = orbitalData.LongitudeAscendingNode.Radians,
                argP = orbitalData.ArgumentPeriapsis.Radians,
                M = orbitalData.MeanAnomalyAtEpoch.Radians,
                nu = orbitalData.TrueAnomalyAtEpoch.Radians
            };
            return state;
        }

        public static OrbitState FromVectors(Vector3d r, Vector3d v, CelestialBody attractor, DateTime epoch)
        {
            OrbitState state = new()
            {
                position = r,
                velocity = v,
                attractor = attractor,
                epoch = epoch
            };

            if (attractor != null)
                state.mu = GravitationalParameter.FromMass(attractor.PhysicalData.Mass);

            state.CalculateStateFromVectors();

            state.period = UniversalConstants.Tri.Pi2 * Math.Sqrt(Math.Pow(state.SemiMajorAxis.Meters, 3) / state.GravitationalParameter.M3S2);
            return state;
        }

        public static OrbitState FromEphemerides(CelestialBody attractor, CelestialBody body, DateTime epoch)
        {
            throw new NotImplementedException();
        }

        public static OrbitState Propagate(OrbitState state, Duration tof)
        {
            OrbitState newState = state.Clone();
            newState.Propagate(tof);
            return newState;
        }

        public void CalculateOrbitPointsMtoAu(ref Vector3[] array, int points, float scale)
        {
            double eccentricAnomaly = 0;
            double step             = UniversalConstants.Tri.Pi2 / points;
            if (e < 1)
            {
                for (int index = 0; index < points; index++)
                {
                    double trueAnomaly = OrbitalMechanics.EccentricToTrueAnomaly(eccentricAnomaly, e);
                    (Vector3d r, Vector3d v) = ClassicalElementsToVectors(a, e, mu.M3S2, i, loAN, argP, trueAnomaly);
                    array[index] = OrbitalMechanics.MetresToAu(r).ToXZY() * scale;

                    eccentricAnomaly += step;
                }
            }
        }

        public void Propagate(DateTime toDate)
        {
            Duration tof = Duration.FromSeconds((toDate - Epoch).TotalSeconds);
            Propagate(tof);
        }

        public void Propagate(Duration tof)
        {
            epoch = epoch.AddSeconds(tof.Seconds);
            nu = ((nu + UniversalConstants.Tri.Pi) % UniversalConstants.Tri.Pi2) - UniversalConstants.Tri.Pi;
            nu = Farnocchia.PropagateOrbit(this, tof).Radians;
            (Vector3d r, Vector3d v) = ToVectors();
            position = r;
            velocity = v;
        }

        public void Propagate(float tofSeconds)
        {
            Propagate((double)tofSeconds);
        }

        public void Propagate(double tofSeconds)
        {
            Propagate(Duration.FromSeconds(tofSeconds));
        }

        public OrbitState PropagateAsNew(DateTime toDate)
        {
            Duration tof = Duration.FromSeconds((toDate - Epoch).TotalSeconds);
            return OrbitState.Propagate(this, tof);
        }

        public void SetAttractor(CelestialBody attractor)
        {
            if (attractor == this.attractor)
            {
                return;
            }

            this.attractor = attractor;
            mu = GravitationalParameter.FromMass(attractor.PhysicalData.Mass);
            CalculateStateFromElements();
        }

        public void SetDate(DateTime date)
        {
            this.epoch = date;
        }

        public OrbitState ApplyManoeuvre(Manoeuvre manoeuvre, int impulseCount = 0)
        {
            OrbitState newOrbit  = this.Clone();
            Impulse[]  impulses = manoeuvre.Impulses;
            if (impulseCount == 0)
                impulseCount = impulses.Length;
            for (int i = 0; i < impulseCount; i++)
            {
                Impulse impulse = manoeuvre.Impulses[i];
                // has loitering time
                if (impulse.DeltaTime.Seconds > 0)
                    newOrbit.Propagate(impulse.DeltaTime);
                (Vector3d r, Vector3d v) = newOrbit.ToVectors();
                Vector3d vNew = v + impulse.DeltaVelocity;
                newOrbit = OrbitState.FromVectors(r, vNew, newOrbit.attractor, newOrbit.Epoch);
            }

            return newOrbit;
        }

        private void CalculateStateFromElements()
        {
            if (e < 1.0)
            {
                b = a * Math.Sqrt(1 - (e * e));
                period = UniversalConstants.Tri.Pi2 * Math.Sqrt(Math.Pow(SemiMajorAxis.Meters, 3) / mu.M3S2);
            }
            else if (e > 1.0)
            {
                b = a * Math.Sqrt((e * e) - 1);
                period = double.MaxValue;
            }
            else
            {
                b = a;
                period = UniversalConstants.Tri.Pi2 * Math.Sqrt(Math.Pow(SemiMajorAxis.Meters, 3) / mu.M3S2);
            }

            const double pi = UniversalConstants.Tri.Pi;
            if (nu is >= -pi and < pi)
            {
                nu = ((nu + pi) % (2 * pi)) - pi;
            }

            (Vector3d r, Vector3d v) = ToVectors();

            position = r;
            velocity = v;
        }

        private void CalculateStateFromVectors()
        {
            Vector3d r    = Position;
            Vector3d v    = Velocity;
            Vector3d h    = AngularMomentum;
            Vector3d n    = NodeVector;
            Vector3d Evec = EccentricityVector;

            e = Evec.magnitude;
            i = Math.Acos(h.z / h.magnitude);
            double p = h.sqrMagnitude / mu.M3S2;
            a = p / (1 - (e * e));
            b = a; // if circular a = b, will be overriden later

            const double tol        = 1e-8;
			bool         circular = e < tol;
			bool         equatorial   = Math.Abs(i) < tol;

            if (equatorial && !circular)
            {
                loAN = 0;
                argP = Math.Atan2(Evec.y, Evec.x) % UniversalConstants.Tri.Pi2;
                nu = Math.Atan2(Vector3d.Dot(h, Vector3d.Cross(Evec, r)) / h.magnitude, Vector3d.Dot(r, Evec));
            }
            else if (!equatorial && circular)
            {
                loAN = Math.Atan2(n.y, n.x) % UniversalConstants.Tri.Pi2;
                argP = 0;
                nu = Math.Atan2(Vector3d.Dot(r, Vector3d.Cross(h, n)) / h.magnitude, Vector3d.Dot(r, n));
            }
            else if (equatorial && circular)
            {
                loAN = 0;
                argP = 0;
                nu = Math.Atan2(r.y, r.x) % UniversalConstants.Tri.Pi2;
            }
            else
            {
                if (a > 0)
                {
                    double eSinE = Vector3d.Dot(r, v) / Math.Sqrt(mu.M3S2 * SemiMajorAxis.Meters);
                    double eCosE = ((r.magnitude * v.sqrMagnitude) / mu.M3S2) - 1;
                    E = Math.Atan2(eSinE, eCosE);
                    nu = OrbitalMechanics.EccentricToTrueAnomaly(E, Eccentricity.Value);
                }
                else
                {
                    double eSinH = Vector3d.Dot(r, v) / Math.Sqrt(mu.M3S2 * -SemiMajorAxis.Meters);
                    double eCosH = ((r.magnitude * v.sqrMagnitude) / mu.M3S2) - 1;
                    nu = OrbitalMechanics.HyperbolicAnomalyToTrueAnomaly(Math.Log((eCosH + eSinH) / (eCosH - eSinH)) / 2, Eccentricity.Value);
                }

                loAN = Math.Atan2(n.y, n.x) % UniversalConstants.Tri.Pi2;
                double px = Vector3d.Dot(r, n);
                double py = Vector3d.Dot(r, Vector3d.Cross(h, n)) / h.magnitude;
                argP = (Math.Atan2(py, px) - nu) % UniversalConstants.Tri.Pi2;
                if (argP < 0)
                {
                    argP += UniversalConstants.Tri.Pi2;
                }
            }

            nu = ((nu + UniversalConstants.Tri.Pi) % UniversalConstants.Tri.Pi2) - UniversalConstants.Tri.Pi;
        }


        #region private fields

        /// <summary>
        /// Semi-Major Axis (Metres)
        /// </summary>
        private double a;

        /// <summary>
        /// Semi-Minor Axis (Metres)
        /// </summary>
        private double b;

        /// <summary>
        /// Orbit eccentricity 
        /// </summary>
        private double e;

        /// <summary>
        /// Eccentric anomaly (rad)
        /// </summary>
        private double E;

        /// <summary>
        /// Inclination (rad)
        /// </summary>
        private double i;

        /// <summary>
        /// Longitude of Ascending Node or RAAN (rad)
        /// </summary>
        private double loAN;

        /// <summary>
        /// Argument of Periapsis (rad)
        /// </summary>
        private double argP;

        /// <summary>
        /// Mean Anomaly (rad)
        /// </summary>
        private double M;

        /// <summary>
        /// True Anomaly (rad) ν
        /// </summary>
        private double nu;

        /// <summary>
        /// Period (s)
        /// </summary>
        private double period;

        private GravitationalParameter mu;

        private Vector3d position;
        private Vector3d velocity;

        private CelestialBody attractor;

        private DateTime epoch = UniversalConstants.Time.J2000;

        #endregion
    }
}