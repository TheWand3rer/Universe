using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VindemiatrixCollective.Universe.RandomNumberGeneration
{
    public static class RandomValues
    {
        private const int MaxIterations = 16;
        private static Gaussian gaussian;
        private static System.Random random;

        public static System.Random Random => random;

        public static double DoubleCloseTo(double value, double variation)
        {
            return value + (value * DoubleInRange(-variation, variation));
        }

        public static double DoubleInRange(double min, double max)
        {
            return (random.NextDouble() * (max - min)) + min;
        }

        public static double GaussianInRange(double mean, double std, double min, double max)
        {
            double result = 0;
            bool   test   = false;
            for (int i = 0; i < MaxIterations; i++)
            {
                result = (gaussian.RandomGauss() * std) + mean;
                test = (result < min) || (result > max);
                if (test)
                {
                    break;
                }
            }

            return test ? result : mean;
        }

        public static T PickRandomItemWeighted<T>(IList<(T Item, float Weight)> items)
        {
            if ((items?.Count ?? 0) == 0)
            {
                return default(T);
            }

            float offset = 0;
            (T Item, float RangeTo)[] rangedItems = items
                                                   .OrderBy(item => item.Weight)
                                                   .Select(entry => (entry.Item, RangeTo: offset += entry.Weight))
                                                   .ToArray();

            float randomNumber = UnityEngine.Random.value;
            return rangedItems.First(item => randomNumber <= item.RangeTo).Item;
        }

        public static T PickRandomItemWeighted<T>(IList<(T Item, double Weight)> items)
        {
            if ((items?.Count ?? 0) == 0)
            {
                return default(T);
            }

            double offset = 0;
            (T Item, double RangeTo)[] rangedItems = items
                                                    .OrderBy(item => item.Weight)
                                                    .Select(entry => (entry.Item, RangeTo: offset += entry.Weight))
                                                    .ToArray();

            float randomNumber = (float)random.NextDouble();
            return rangedItems.First(item => randomNumber <= item.RangeTo).Item;
        }


        public static Vector2 PointInUnitDisk(float radius)
        {
            float angle = (float)DoubleInRange(0, Math.PI * 2);
            float sqrtR = Mathf.Sqrt(UnityEngine.Random.Range(0, radius));
            float x     = Mathf.Cos(angle) * sqrtR;
            float y     = Mathf.Sin(angle) * sqrtR;

            return new Vector2(x, y);
        }

        public static Vector2 PointOnUnitCircle(float radius)
        {
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);
            float x     = Mathf.Cos(angle) * radius;
            float y     = Mathf.Sin(angle) * radius;
            return new Vector2(x, y);
        }

        public static void InitRandomGenerator(int seed)
        {
            random = new System.Random(seed);
            gaussian = new Gaussian(random);
            UnityEngine.Random.InitState(seed);
        }
    }
}