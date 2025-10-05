// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using System.Runtime.CompilerServices;

#endregion

namespace VindemiatrixCollective.Universe.CelestialMechanics
{
    public struct Mathd
    {
        private static readonly int[] p =
        {
            151, 160, 137, 91, 90, 15,
            131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
            190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
            88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
            77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
            102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
            135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
            5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
            223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
            129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
            251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
            49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
            138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
            // 2nd copy
            151, 160, 137, 91, 90, 15,
            131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
            190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
            88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
            77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
            102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
            135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
            5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
            223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
            129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
            251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
            49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
            138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
        };

        //
        // Summary:
        //     Returns the arc-sine of f - the angle in radians whose sine is f.
        //
        // Parameters:
        //   f:
        public static double Asin(double f)
        {
            return Math.Asin(f);
        }

        //
        // Summary:
        //     Returns the arc-cosine of f - the angle in radians whose cosine is f.
        //
        // Parameters:
        //   f:
        public static double Acos(double f)
        {
            return Math.Acos(f);
        }

        //
        // Summary:
        //     Returns the arc-tangent of f - the angle in radians whose tangent is f.
        //
        // Parameters:
        //   f:
        public static double Atan(double f)
        {
            return Math.Atan(f);
        }

        //
        // Summary:
        //     Returns the angle in radians whose Tan is y/x.
        //
        // Parameters:
        //   y:
        //
        //   x:
        public static double Atan2(double y, double x)
        {
            return Math.Atan2(y, x);
        }

        //
        // Summary:
        //     Returns square root of f.
        //
        // Parameters:
        //   f:
        public static double Sqrt(double f)
        {
            return Math.Sqrt(f);
        }

        //
        // Summary:
        //     Returns the absolute value of f.
        //
        // Parameters:
        //   f:
        public static double Abs(double f)
        {
            return Math.Abs(f);
        }

        //
        // Summary:
        //     Returns the absolute value of value.
        //
        // Parameters:
        //   value:
        public static int Abs(int value)
        {
            return Math.Abs(value);
        }

        //
        // Summary:
        //     Returns the smallest of two or more values.
        //
        // Parameters:
        //   a:
        //
        //   b:
        //
        //   values:
        public static double Min(double a, double b)
        {
            return a < b ? a : b;
        }

        //
        // Summary:
        //     Returns the smallest of two or more values.
        //
        // Parameters:
        //   a:
        //
        //   b:
        //
        //   values:
        public static double Min(params double[] values)
        {
            int num = values.Length;
            if (num == 0)
            {
                return 0f;
            }

            double num2 = values[0];
            for (int i = 1; i < num; i++)
            {
                if (values[i] < num2)
                {
                    num2 = values[i];
                }
            }

            return num2;
        }

        //
        // Summary:
        //     Returns the smallest of two or more values.
        //
        // Parameters:
        //   a:
        //
        //   b:
        //
        //   values:
        public static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        //
        // Summary:
        //     Returns the smallest of two or more values.
        //
        // Parameters:
        //   a:
        //
        //   b:
        //
        //   values:
        public static int Min(params int[] values)
        {
            int num = values.Length;
            if (num == 0)
            {
                return 0;
            }

            int num2 = values[0];
            for (int i = 1; i < num; i++)
            {
                if (values[i] < num2)
                {
                    num2 = values[i];
                }
            }

            return num2;
        }

        //
        // Summary:
        //     Returns the largest of two or more values. When comparing negative values, values
        //     closer to zero are considered larger.
        //
        // Parameters:
        //   a:
        //
        //   b:
        //
        //   values:
        public static double Max(double a, double b)
        {
            return a > b ? a : b;
        }

        //
        // Summary:
        //     Returns the largest of two or more values. When comparing negative values, values
        //     closer to zero are considered larger.
        //
        // Parameters:
        //   a:
        //
        //   b:
        //
        //   values:
        public static double Max(params double[] values)
        {
            int num = values.Length;
            if (num == 0)
            {
                return 0f;
            }

            double num2 = values[0];
            for (int i = 1; i < num; i++)
            {
                if (values[i] > num2)
                {
                    num2 = values[i];
                }
            }

            return num2;
        }

        //
        // Summary:
        //     Returns the largest value. When comparing negative values, values closer to zero
        //     are considered larger.
        //
        // Parameters:
        //   a:
        //
        //   b:
        //
        //   values:
        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        //
        // Summary:
        //     Returns the largest value. When comparing negative values, values closer to zero
        //     are considered larger.
        //
        // Parameters:
        //   a:
        //
        //   b:
        //
        //   values:
        public static int Max(params int[] values)
        {
            int num = values.Length;
            if (num == 0)
            {
                return 0;
            }

            int num2 = values[0];
            for (int i = 1; i < num; i++)
            {
                if (values[i] > num2)
                {
                    num2 = values[i];
                }
            }

            return num2;
        }

        //
        // Summary:
        //     Returns f raised to power p.
        //
        // Parameters:
        //   f:
        //
        //   p:
        public static double Pow(double f, double p)
        {
            return Math.Pow(f, p);
        }

        //
        // Summary:
        //     Returns e raised to the specified power.
        //
        // Parameters:
        //   power:
        public static double Exp(double power)
        {
            return Math.Exp(power);
        }

        //
        // Summary:
        //     Returns the logarithm of a specified number in a specified base.
        //
        // Parameters:
        //   f:
        //
        //   p:
        public static double Log(double f, double p)
        {
            return Math.Log(f, p);
        }

        //
        // Summary:
        //     Returns the natural (base e) logarithm of a specified number.
        //
        // Parameters:
        //   f:
        public static double Log(double f)
        {
            return Math.Log(f);
        }

        //
        // Summary:
        //     Returns the base 10 logarithm of a specified number.
        //
        // Parameters:
        //   f:
        public static double Log10(double f)
        {
            return Math.Log10(f);
        }

        //
        // Summary:
        //     Returns the smallest integer greater than or equal to f.
        //
        // Parameters:
        //   f:
        public static double Ceil(double f)
        {
            return Math.Ceiling(f);
        }

        //
        // Summary:
        //     Returns the largest integer smaller than or equal to f.
        //
        // Parameters:
        //   f:
        public static double Floor(double f)
        {
            return Math.Floor(f);
        }

        //
        // Summary:
        //     Returns f rounded to the nearest integer.
        //
        // Parameters:
        //   f:
        public static double Round(double f)
        {
            return Math.Round(f);
        }

        //
        // Summary:
        //     Returns the smallest integer greater to or equal to f.
        //
        // Parameters:
        //   f:
        public static int CeilToInt(double f)
        {
            return (int)Math.Ceiling(f);
        }

        //
        // Summary:
        //     Returns the largest integer smaller to or equal to f.
        //
        // Parameters:
        //   f:
        public static int FloorToInt(double f)
        {
            return (int)Math.Floor(f);
        }

        //
        // Summary:
        //     Returns f rounded to the nearest integer.
        //
        // Parameters:
        //   f:
        public static int RoundToInt(double f)
        {
            return (int)Math.Round(f);
        }

        //
        // Summary:
        //     Returns the sign of f.
        //
        // Parameters:
        //   f:

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sign(double f)
        {
            return f >= 0f ? 1f : -1f;
        }

        //
        // Summary:
        //     Clamps the given value between the given minimum double and maximum double values.
        //     Returns the given value if it is within the minimum and maximum range.
        //
        // Parameters:
        //   value:
        //     The double point value to restrict inside the range defined by the minimum
        //     and maximum values.
        //
        //   min:
        //     The minimum double point value to compare against.
        //
        //   max:
        //     The maximum double point value to compare against.
        //
        // Returns:
        //     The double result between the minimum and maximum values.
        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }

        //
        // Summary:
        //     Clamps the given value between a range defined by the given minimum integer and
        //     maximum integer values. Returns the given value if it is within min and max.
        //
        //
        // Parameters:
        //   value:
        //     The integer point value to restrict inside the min-to-max range.
        //
        //   min:
        //     The minimum integer point value to compare against.
        //
        //   max:
        //     The maximum integer point value to compare against.
        //
        // Returns:
        //     The int result between min and max values.
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }

        //
        // Summary:
        //     Clamps value between 0 and 1 and returns value.
        //
        // Parameters:
        //   value:
        public static double Clamp01(double value)
        {
            if (value < 0f)
            {
                return 0f;
            }

            if (value > 1f)
            {
                return 1f;
            }

            return value;
        }

        //
        // Summary:
        //     Linearly interpolates between a and b by t.
        //
        // Parameters:
        //   a:
        //     The start value.
        //
        //   b:
        //     The end value.
        //
        //   t:
        //     The interpolation value between the two doubles.
        //
        // Returns:
        //     The interpolated double result between the two double values.
        public static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * Clamp01(t);
        }

        //
        // Summary:
        //     Linearly interpolates between a and b by t with no limit to t.
        //
        // Parameters:
        //   a:
        //     The start value.
        //
        //   b:
        //     The end value.
        //
        //   t:
        //     The interpolation between the two doubles.
        //
        // Returns:
        //     The double value as a result from the linear interpolation.
        public static double LerpUnclamped(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        //
        // Summary:
        //     Same as Lerp but makes sure the values interpolate correctly when they wrap around
        //     360 degrees.
        //
        // Parameters:
        //   a:
        //     The start angle. A double expressed in degrees.
        //
        //   b:
        //     The end angle. A double expressed in degrees.
        //
        //   t:
        //     The interpolation value between the start and end angles. This value is clamped
        //     to the range [0, 1].
        //
        // Returns:
        //     Returns the interpolated double result between angle a and angle b, based on the
        //     interpolation value t.
        public static double LerpAngle(double a, double b, double t)
        {
            double num = Repeat(b - a, 360f);
            if (num > 180f)
            {
                num -= 360f;
            }

            return a + num * Clamp01(t);
        }

        //
        // Summary:
        //     Moves a value current towards target.
        //
        // Parameters:
        //   current:
        //     The current value.
        //
        //   target:
        //     The value to move towards.
        //
        //   maxDelta:
        //     The maximum change applied to the current value.
        public static double MoveTowards(double current, double target, double maxDelta)
        {
            if (Abs(target - current) <= maxDelta)
            {
                return target;
            }

            return current + Sign(target - current) * maxDelta;
        }

        //
        // Summary:
        //     Same as MoveTowards but makes sure the values interpolate correctly when they
        //     wrap around 360 degrees.
        //
        // Parameters:
        //   current:
        //
        //   target:
        //
        //   maxDelta:
        public static double MoveTowardsAngle(double current, double target, double maxDelta)
        {
            double num = DeltaAngle(current, target);
            if (0f - maxDelta < num && num < maxDelta)
            {
                return target;
            }

            target = current + num;
            return MoveTowards(current, target, maxDelta);
        }

        //
        // Summary:
        //     Interpolates between from and to with smoothing at the limits.
        //
        // Parameters:
        //   from:
        //     The start of the range.
        //
        //   to:
        //     The end of the range.
        //
        //   t:
        //     The interpolation value between the from and to range limits.
        //
        // Returns:
        //     The interpolated double result between from and to.
        public static double SmoothStep(double from, double to, double t)
        {
            t = Clamp01(t);
            t = -2f * t * t * t + 3f * t * t;
            return to * t + from * (1f - t);
        }

        public static double Gamma(double value, double absmax, double gamma)
        {
            bool   flag = value < 0f;
            double num  = Abs(value);
            if (num > absmax)
            {
                return flag ? 0f - num : num;
            }

            double num2 = Pow(num / absmax, gamma) * absmax;
            return flag ? 0f - num2 : num2;
        }

        //
        // Summary:
        //     Compares two double point values and returns true if they are similar.
        //
        // Parameters:
        //   a:
        //
        //   b:
        public static bool Approximately(double a, double b)
        {
            return Abs(b - a) < Max(1E-06f * Max(Abs(a), Abs(b)), Epsilon * 8f);
        }

        public static double SmoothDamp(
            double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
        {
            smoothTime = Max(0.0001f, smoothTime);
            double num   = 2f / smoothTime;
            double num2  = num * deltaTime;
            double num3  = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            double value = current - target;
            double num4  = target;
            double num5  = maxSpeed * smoothTime;
            value  = Clamp(value, 0f - num5, num5);
            target = current - value;
            double num6 = (currentVelocity + num * value) * deltaTime;
            currentVelocity = (currentVelocity - num * num6) * num3;
            double num7 = target + (value + num6) * num3;
            if (num4 - current > 0f == num7 > num4)
            {
                num7            = num4;
                currentVelocity = (num7 - num4) / deltaTime;
            }

            return num7;
        }


        public static double SmoothDampAngle(
            double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
        {
            target = current + DeltaAngle(current, target);
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        //
        // Summary:
        //     Loops the value t, so that it is never larger than length and never smaller than
        //     0.
        //
        // Parameters:
        //   t:
        //
        //   length:
        public static double Repeat(double t, double length)
        {
            return Clamp(t - Floor(t / length) * length, 0f, length);
        }

        //
        // Summary:
        //     PingPong returns a value that increments and decrements between zero and the
        //     length. It follows the triangle wave formula where the bottom is set to zero
        //     and the peak is set to length.
        //
        // Parameters:
        //   t:
        //
        //   length:
        public static double PingPong(double t, double length)
        {
            t = Repeat(t, length * 2f);
            return length - Abs(t - length);
        }

        //
        // Summary:
        //     Determines where a value lies between two points.
        //
        // Parameters:
        //   a:
        //     The start of the range.
        //
        //   b:
        //     The end of the range.
        //
        //   value:
        //     The point within the range you want to calculate.
        //
        // Returns:
        //     A value between zero and one, representing where the "value" parameter falls
        //     within the range defined by a and b.
        public static double InverseLerp(double a, double b, double value)
        {
            if (a != b)
            {
                return Clamp01((value - a) / (b - a));
            }

            return 0f;
        }

        //
        // Summary:
        //     Calculates the shortest difference between two angles.
        //
        // Parameters:
        //   current:
        //     The current angle in degrees.
        //
        //   target:
        //     The target angle in degrees.
        //
        // Returns:
        //     A value between -179 and 180, in degrees.
        public static double DeltaAngle(double current, double target)
        {
            double num = Repeat(target - current, 360f);
            if (num > 180f)
            {
                num -= 360f;
            }

            return num;
        }

        // internal static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 result)
        // {
        // 	double num = p2.x - p1.x;
        // 	double num2 = p2.y - p1.y;
        // 	double num3 = p4.x - p3.x;
        // 	double num4 = p4.y - p3.y;
        // 	double num5 = num * num4 - num2 * num3;
        // 	if (num5 == 0f)
        // 	{
        // 		return false;
        // 	}

        // 	double num6 = p3.x - p1.x;
        // 	double num7 = p3.y - p1.y;
        // 	double num8 = (num6 * num4 - num7 * num3) / num5;
        // 	result.x = p1.x + num8 * num;
        // 	result.y = p1.y + num8 * num2;
        // 	return true;
        // }

        // internal static bool LineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 result)
        // {
        // 	double num = p2.x - p1.x;
        // 	double num2 = p2.y - p1.y;
        // 	double num3 = p4.x - p3.x;
        // 	double num4 = p4.y - p3.y;
        // 	double num5 = num * num4 - num2 * num3;
        // 	if (num5 == 0f)
        // 	{
        // 		return false;
        // 	}

        // 	double num6 = p3.x - p1.x;
        // 	double num7 = p3.y - p1.y;
        // 	double num8 = (num6 * num4 - num7 * num3) / num5;
        // 	if (num8 < 0f || num8 > 1f)
        // 	{
        // 		return false;
        // 	}

        // 	double num9 = (num6 * num2 - num7 * num) / num5;
        // 	if (num9 < 0f || num9 > 1f)
        // 	{
        // 		return false;
        // 	}

        // 	result.x = p1.x + num8 * num;
        // 	result.y = p1.y + num8 * num2;
        // 	return true;
        // }

        internal static long RandomToLong(Random r)
        {
            byte[] array = new byte[8];
            r.NextBytes(array);
            return (long)(BitConverter.ToUInt64(array, 0) & 0x7FFFFFFFFFFFFFFFL);
        }

        internal static double ClampToDouble(double value)
        {
            if (double.IsPositiveInfinity(value))
            {
                return double.PositiveInfinity;
            }

            if (double.IsNegativeInfinity(value))
            {
                return double.NegativeInfinity;
            }

            if (value < -3.4028234663852886E+38)
            {
                return double.MinValue;
            }

            if (value > 3.4028234663852886E+38)
            {
                return double.MaxValue;
            }

            return value;
        }

        internal static int ClampToInt(long value)
        {
            if (value < int.MinValue)
            {
                return int.MinValue;
            }

            if (value > int.MaxValue)
            {
                return int.MaxValue;
            }

            return (int)value;
        }

        internal static uint ClampToUInt(long value)
        {
            if (value < 0)
            {
                return 0u;
            }

            if (value > uint.MaxValue)
            {
                return uint.MaxValue;
            }

            return (uint)value;
        }

        internal static double RoundToMultipleOf(double value, double roundingValue)
        {
            if (roundingValue == 0f)
            {
                return value;
            }

            return Round(value / roundingValue) * roundingValue;
        }

        internal static double GetClosestPowerOfTen(double positiveNumber)
        {
            if (positiveNumber <= 0f)
            {
                return 1f;
            }

            return Pow(10f, RoundToInt(Log10(positiveNumber)));
        }

        internal static int GetNumberOfDecimalsForMinimumDifference(double minDifference)
        {
            return Clamp(-FloorToInt(Log10(Abs(minDifference))), 0, 15);
        }


        internal static double RoundBasedOnMinimumDifference(double valueToRound, double minDifference)
        {
            if (minDifference == 0f)
            {
                return DiscardLeastSignificantDecimal(valueToRound);
            }

            return Math.Round(valueToRound, GetNumberOfDecimalsForMinimumDifference(minDifference), MidpointRounding.AwayFromZero);
        }

        internal static double DiscardLeastSignificantDecimal(double v)
        {
            int digits = Clamp((int)(5f - Log10(Abs(v))), 0, 15);
            return Math.Round(v, digits, MidpointRounding.AwayFromZero);
        }


        //
        // Summary:
        //     Returns the next power of two that is equal to, or greater than, the argument.
        //
        //
        // Parameters:
        //   value:
        public static int NextPowerOfTwo(int value)
        {
            value--;
            value |= value >> 16;
            value |= value >> 8;
            value |= value >> 4;
            value |= value >> 2;
            value |= value >> 1;
            return value + 1;
        }

        //
        // Summary:
        //     Returns the closest power of two value.
        //
        // Parameters:
        //   value:
        public static int ClosestPowerOfTwo(int value)
        {
            int num  = NextPowerOfTwo(value);
            int num2 = num >> 1;
            if (value - num2 < num - value)
            {
                return num2;
            }

            return num;
        }

        //
        // Summary:
        //     Returns true if the value is power of two.
        //
        // Parameters:
        //   value:
        public static bool IsPowerOfTwo(int value)
        {
            return (value & (value - 1)) == 0;
        }

        // [MethodImpl(MethodImplOptions.InternalCall)]
        // private static extern void CorrelatedColorTemperatureToRGB_Injected(double kelvin, out Color ret);


        // Summary:
        //     Perlin Noise 1D
        //
        // Parameters:
        //   Dimension 1:
        public static double PerlinNoise(double x)
        {
            return PerlinNoise(x, 0);
        }

        // Summary:
        //     Perlin Noise 2D
        //
        // Parameters:
        //   Dimension 1:
        //   Dimension 2:
        // type is casted to a float, idk whether statically, probably (bytes interpreted in place)
        public static double PerlinNoise(double x, double y, int seed = 0)
        {
            // Determine grid cell coordinates
            int X = (int)(x + seed) & 255;
            int Y = (int)(y + seed) & 255;
            // Relative x, y in cell
            x -= (int)x;
            y -= (int)y;

            // Compute fade curves
            double u = fade(x);
            double v = fade(y);

            // Hash coordinates of the 4 square corners
            int aa = p[p[X] + Y];
            int ab = p[p[X] + Y + 1];
            int ba = p[p[X + 1] + Y];
            int bb = p[p[X + 1] + Y + 1];

            // Add blended results from the corners
            double res = Lerp(Lerp(grad(aa, x, y), grad(ba, x - 1, y), u), Lerp(grad(ab, x, y - 1), grad(bb, x - 1, y - 1), u), v);

            // Map result from [-1,1] to [0,1]
            return (res + 1f) * 0.5f;
        }

        // Summary:
        //     Perlin Noise 3D
        //
        // Parameters:
        //   Dimension 1:
        public static double PerlinNoise3D(double x, double y, double z)
        {
            double total = 0;
            total += PerlinNoise(x, y);
            total += PerlinNoise(y, z);
            total += PerlinNoise(z, x);
            total += PerlinNoise(y, x);
            total += PerlinNoise(z, y);
            total += PerlinNoise(x, z);
            total /= 6;
            return total;
        }

        private static double fade(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static double lerp(double t, double a, double b)
        {
            return a + t * (b - a);
        }

        private static double grad(int hash, double x, double y)
        {
            int    h = hash & 7; // Convert low 3 bits of hash code
            double u = h < 4 ? x : y;
            double v = h < 4 ? y : x;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        #region Constants

        public const double Pi = Math.PI;
        public const double PI = Math.PI;
        public const double Rad2Deg = 180d / Pi;
        public const double Deg2Rad = Pi / 180d;
        public const double Infinity = double.PositiveInfinity;
        public const double NegativeInfinity = double.NegativeInfinity;
        public const double Epsilon = double.Epsilon;

        #endregion

        #region Methods

        public static double Sin(double f)
        {
            return Math.Sin(f);
        }

        public static double Cos(double f)
        {
            return Math.Cos(f);
        }

        public static double Tan(double f)
        {
            return Math.Tan(f);
        }

        #endregion
    }
}