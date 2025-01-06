using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using Unity.Properties;
using UnityEngine;

namespace VindemiatrixCollective.Universe.Model
{
    [Serializable]
    public class StarSystem : IEnumerable<Star>
    {
        public Dictionary<string, Star> Stars { get; }

        public Star this[int index] => ToArray()[index];
        public Star MainStar => ToArray().FirstOrDefault();

        public Star[] ToArray() => Stars.Values.OrderByDescending(star => star.PhysicalData.Mass.SolarMasses).ToArray();

        [CreateProperty]
        public string Name { get; set; }

        public Length DistanceFromSol => Length.FromParsecs(Coordinates.magnitude);

        public string Id { get; set; }

        public Vector3 Coordinates { get; set; }

        public bool ContainsStar(string starName)
        {
            return Stars.ContainsKey(starName);
        }

        public IEnumerable<Planet> CelestialBodyEnumerator()
        {
            foreach (Star star in this)
            {
                foreach (Planet planet in star)
                {
                    yield return planet;

                    foreach (Planet satellite in planet)
                    {
                        yield return satellite;
                    }
                }
            }
        }

        public Planet SelectPlanet(string filter)
        {
            return CelestialBodyEnumerator().FirstOrDefault(p => p.Name == filter);
        }

        public StarSystem() : this(nameof(StarSystem))
        {
        }

        public StarSystem(string name)
        {
            Name = name;
            Stars = new Dictionary<string, Star>();
            Id = MakeId(name);
        }

        public StarSystem(string name, Star star) : this(name, new[] { star }) {}

        public StarSystem(string name, IEnumerable<Star> stars) : this(name)
        {
            Stars = stars.ToDictionary(star => star.Name, star => star);
        }

        public void AddStar(Star star)
        {
            star.StarSystem = this;

            char starChar = (char)(65 + Stars.Count);
            Stars.Add(starChar.ToString(), star);

            // TODO: implement a more elegant visit algorithm
            foreach (Planet planet in star)
            {
                planet.StarSystem = this;
                foreach (Planet moon in planet)
                {
                    moon.StarSystem = this;
                }
            }
        }

        public void AddStars(IEnumerable<Star> stars)
        {
            foreach (Star star in stars)
            {
                AddStar(star);
            }
        }

        public static string MakeId(string name)
        {
            string[]  words = name.Split(' ');
            string    id    = string.Empty;
            const int n     = 3;

            foreach (string word in words)
            {
                id += word.Length <= n ? word : word[..n];
                if (id.Length >= 6)
                {
                    id = id[..6];
                    break;
                }
            }

            return id;
        }

        public void Init()
        {
#if UNITY_EDITOR
            DistanceFromSolLy = DistanceFromSol.LightYears.ToString("0.00 LY");
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Convenience variable to see some values in the inspector.
        /// </summary>
        public string DistanceFromSolLy;
#endif

        public IEnumerator<Star> GetEnumerator()
        {
            return Stars?.Values.GetEnumerator() ?? Enumerable.Empty<Star>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}