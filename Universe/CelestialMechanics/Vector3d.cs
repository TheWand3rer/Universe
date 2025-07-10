#region

using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics
{
    public struct Vector3d
    {
        public const float kEpsilon = 1E-05f;
        public double x;
        public double y;
        public double z;

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException("Invalid index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3d index!");
                }
            }
        }

        public double magnitude => Math.Sqrt(x * x + y * y + z * z);

        public double sqrMagnitude => x * x + y * y + z * z;

        public Vector3d normalized => Normalize(this);

        public Vector3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3d(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3d(Vector3 v3)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
        }

        public Vector3d(double x, double y)
        {
            this.x = x;
            this.y = y;
            z      = 0d;
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector3d))
            {
                return false;
            }

            Vector3d vector3d = (Vector3d)other;
            if (x.Equals(vector3d.x) && y.Equals(vector3d.y))
            {
                return z.Equals(vector3d.z);
            }

            return false;
        }

        public double[] ToArray()
        {
            return new[] { x, y, z };
        }

        public override int GetHashCode() => x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);

        public override string ToString() => "(" + x + ", " + y + ", " + z + ")";

        public Vector3 ToXZY()
        {
            Vector3 v = (Vector3)this;
            return new Vector3(v.x, v.z, v.y);
        }

        public Vector3d ToXZYd() => new(x, z, y);

        public void Normalize()
        {
            double num = Magnitude(this);
            if (num > 9.99999974737875E-06)
            {
                this = this / num;
            }
            else
            {
                this = zero;
            }
        }

        public void Scale(Vector3d scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }

        public void Set(double new_x, double new_y, double new_z)
        {
            x = new_x;
            y = new_y;
            z = new_z;
        }

        public static Vector3d back => new(0d, 0d, -1d);

        public static Vector3d down => new(0d, -1d, 0d);

        public static Vector3d forward => new(0d, 0d, 1d);

        [Obsolete("Use Vector3d.forward instead.")]
        public static Vector3d fwd => new(0d, 0d, 1d);

        public static Vector3d left => new(-1d, 0d, 0d);

        public static Vector3d one => new(1d, 1d, 1d);

        public static Vector3d right => new(1d, 0d, 0d);

        public static Vector3d up => new(0d, 1d, 0d);

        public static Vector3d X => new(1d, 0d, 0d);
        public static Vector3d Y => new(0d, 1d, 0d);
        public static Vector3d Z => new(0d, 0d, 1d);

        public static Vector3d zero => new(0d, 0d, 0d);

        public static Vector3d operator +(Vector3d a, Vector3d b) => new(a.x + b.x, a.y + b.y, a.z + b.z);

        public static Vector3d operator -(Vector3d a, Vector3d b) => new(a.x - b.x, a.y - b.y, a.z - b.z);

        public static Vector3d operator -(Vector3d a) => new(-a.x, -a.y, -a.z);

        public static Vector3d operator *(Vector3d a, double d) => new(a.x * d, a.y * d, a.z * d);

        public static Vector3d operator *(double d, Vector3d a) => new(a.x * d, a.y * d, a.z * d);

        public static Vector3d operator /(Vector3d a, double d) => new(a.x / d, a.y / d, a.z / d);

        public static bool operator ==(Vector3d lhs, Vector3d rhs) => SqrMagnitude(lhs - rhs) < 0.0 / 1.0;

        public static bool operator !=(Vector3d lhs, Vector3d rhs) => SqrMagnitude(lhs - rhs) >= 0.0 / 1.0;

        public static explicit operator Vector3(Vector3d vector3d) => new((float)vector3d.x, (float)vector3d.y, (float)vector3d.z);

        public static explicit operator Vector3d(Vector3 vector3) => new(vector3.x, vector3.y, vector3.z);

        public static Vector3d Lerp(Vector3d from, Vector3d to, double t)
        {
            t = t < 0 ? 0 : t > 1.0 ? 1.0 : t;
            return new Vector3d(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t,
                                from.z + (to.z - from.z) * t);
        }

        public static Vector3d Slerp(Vector3d from, Vector3d to, double t)
        {
            Vector3 v3 = Vector3.Slerp((Vector3)from, (Vector3)to, (float)t);
            return new Vector3d(v3);
        }

        public static void OrthoNormalize(ref Vector3d normal, ref Vector3d tangent)
        {
            Vector3 v3normal  = new();
            Vector3 v3tangent = new();
            v3normal  = (Vector3)normal;
            v3tangent = (Vector3)tangent;
            Vector3.OrthoNormalize(ref v3normal, ref v3tangent);
            normal  = new Vector3d(v3normal);
            tangent = new Vector3d(v3tangent);
        }

        public static void OrthoNormalize(ref Vector3d normal, ref Vector3d tangent, ref Vector3d binormal)
        {
            Vector3 v3normal   = new();
            Vector3 v3tangent  = new();
            Vector3 v3binormal = new();
            v3normal   = (Vector3)normal;
            v3tangent  = (Vector3)tangent;
            v3binormal = (Vector3)binormal;
            Vector3.OrthoNormalize(ref v3normal, ref v3tangent, ref v3binormal);
            normal   = new Vector3d(v3normal);
            tangent  = new Vector3d(v3tangent);
            binormal = new Vector3d(v3binormal);
        }

        public static Vector3d MoveTowards(Vector3d current, Vector3d target, double maxDistanceDelta)
        {
            Vector3d vector3   = target - current;
            double   magnitude = vector3.magnitude;
            if (magnitude <= maxDistanceDelta || magnitude == 0.0d)
            {
                return target;
            }

            return current + vector3 / magnitude * maxDistanceDelta;
        }

        public static Vector3d RotateTowards(
            Vector3d current, Vector3d target, double maxRadiansDelta,
            double maxMagnitudeDelta)
        {
            Vector3 v3 = Vector3.RotateTowards((Vector3)current, (Vector3)target, (float)maxRadiansDelta,
                                               (float)maxMagnitudeDelta);
            return new Vector3d(v3);
        }

        public static Vector3d SmoothDamp(
            Vector3d current, Vector3d target, ref Vector3d currentVelocity,
            double smoothTime, double maxSpeed)
        {
            double deltaTime = Time.deltaTime;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static Vector3d SmoothDamp(
            Vector3d current, Vector3d target, ref Vector3d currentVelocity,
            double smoothTime)
        {
            double deltaTime = Time.deltaTime;
            double maxSpeed  = double.PositiveInfinity;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static Vector3d SmoothDamp(
            Vector3d current, Vector3d target, ref Vector3d currentVelocity,
            double smoothTime, double maxSpeed, double deltaTime)
        {
            smoothTime = Math.Max(0.0001d, smoothTime);
            double num1 = 2d / smoothTime;
            double num2 = num1 * deltaTime;
            double num3 = 1.0d / (1.0d + num2 + 0.479999989271164d * num2 * num2 +
                                  0.234999999403954d * num2 * num2 * num2);
            Vector3d vector    = current - target;
            Vector3d vector3_1 = target;
            double   maxLength = maxSpeed * smoothTime;
            Vector3d vector3_2 = ClampMagnitude(vector, maxLength);
            target = current - vector3_2;
            Vector3d vector3_3 = (currentVelocity + num1 * vector3_2) * deltaTime;
            currentVelocity = (currentVelocity - num1 * vector3_3) * num3;
            Vector3d vector3_4 = target + (vector3_2 + vector3_3) * num3;
            if (Dot(vector3_1 - current, vector3_4 - vector3_1) > 0.0)
            {
                vector3_4       = vector3_1;
                currentVelocity = (vector3_4 - vector3_1) / deltaTime;
            }

            return vector3_4;
        }

        public static Vector3d Scale(Vector3d a, Vector3d b) => new(a.x * b.x, a.y * b.y, a.z * b.z);

        public static Vector3d Cross(Vector3d lhs, Vector3d rhs) =>
            new(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.x * rhs.y - lhs.y * rhs.x);

        public static Vector3d Reflect(Vector3d inDirection, Vector3d inNormal) =>
            -2d * Dot(inNormal, inDirection) * inNormal + inDirection;

        public static Vector3d Normalize(Vector3d value)
        {
            double num = Magnitude(value);
            if (num > 9.99999974737875E-06)
            {
                return value / num;
            }

            return zero;
        }

        public static double Dot(Vector3d lhs, Vector3d rhs) => lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;

        public static Vector3d Project(Vector3d vector, Vector3d onNormal)
        {
            double num = Dot(onNormal, onNormal);
            if (num < 1.40129846432482E-45d)
            {
                return zero;
            }

            return onNormal * Dot(vector, onNormal) / num;
        }

        public static Vector3d Exclude(Vector3d excludeThis, Vector3d fromThat) => fromThat - Project(fromThat, excludeThis);

        /// <summary>
        ///     Returns the angle between from and to in degrees.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>The angle in degrees.</returns>
        public static double Angle(Vector3d from, Vector3d to) =>
            Math.Acos(Math.Clamp(Dot(from.normalized, to.normalized), -1d, 1d)) * 57.29578d;

        public static double Distance(Vector3d a, Vector3d b)
        {
            Vector3d vector3d = new(a.x - b.x, a.y - b.y, a.z - b.z);
            return Math.Sqrt(vector3d.x * vector3d.x + vector3d.y * vector3d.y + vector3d.z * vector3d.z);
        }

        public static Vector3d ClampMagnitude(Vector3d vector, double maxLength)
        {
            if (vector.sqrMagnitude > maxLength * maxLength)
            {
                return vector.normalized * maxLength;
            }

            return vector;
        }

        public static double Magnitude(Vector3d a) => Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);

        public static double SqrMagnitude(Vector3d a) => a.x * a.x + a.y * a.y + a.z * a.z;

        public static Vector3d Min(Vector3d lhs, Vector3d rhs) =>
            new(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));

        public static Vector3d Max(Vector3d lhs, Vector3d rhs) =>
            new(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));

        // Projects a vector onto a plane defined by a normal orthogonal to the plane.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d ProjectOnPlane(Vector3d vector, Vector3d planeNormal)
        {
            double sqrMag = Dot(planeNormal, planeNormal);
            if (sqrMag < kEpsilon)
            {
                return vector;
            }

            double dot = Dot(vector, planeNormal);
            return new Vector3d(vector.x - planeNormal.x * dot / sqrMag,
                                vector.y - planeNormal.y * dot / sqrMag,
                                vector.z - planeNormal.z * dot / sqrMag);
        }


        public static double SignedAngle(Vector3d from, Vector3d to, Vector3d axis)
        {
            double unsignedAngle = Angle(from, to);

            double cross_x = from.y * to.z - from.z * to.y;
            double cross_y = from.z * to.x - from.x * to.z;
            double cross_z = from.x * to.y - from.y * to.x;
            double sign    = math.sign(axis.x * cross_x + axis.y * cross_y + axis.z * cross_z);
            return unsignedAngle * sign;
        }
    }
}