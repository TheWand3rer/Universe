#region

using System;
using Random = System.Random;

#endregion

namespace VindemiatrixCollective.Universe.RandomNumberGeneration
{
    public class Gaussian
    {
        private readonly Random rng;
        private bool available;
        private double nextGauss;

        public Gaussian() : this(new Random()) { }

        public Gaussian(Random random)
        {
            rng = random;
        }

        public double RandomGauss()
        {
            if (available)
            {
                available = false;
                return nextGauss;
            }

            double u1    = rng.NextDouble();
            double u2    = rng.NextDouble();
            double temp1 = Math.Sqrt(-2.0 * Math.Log(u1));
            double temp2 = 2.0 * Math.PI * u2;

            nextGauss = temp1 * Math.Sin(temp2);
            available = true;
            return temp1 * Math.Cos(temp2);
        }

        public double RandomGauss(double mu, double sigma)
        {
            return mu + (sigma * RandomGauss());
        }

        public double RandomGauss(double sigma)
        {
            return sigma * RandomGauss();
        }
    }
}