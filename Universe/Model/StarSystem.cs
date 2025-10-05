// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using Unity.Properties;
using UnityEngine;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    public class StarSystem : IEnumerable<CelestialBody>
    {
        private Barycentre barycentre;
        public Barycentre Barycentre => barycentre ??= new Barycentre(this);
        public CelestialBody this[string name] => _Orbiters[name];
        public Galaxy Galaxy { get; internal set; }

        public IEnumerable<CelestialBody> Hierarchy
        {
            get
            {
                foreach (CelestialBody body in _Orbiters.Values)
                {
                    foreach (ITreeNode orbiter in Tree.PreOrderVisit(body))
                    {
                        yield return (CelestialBody)orbiter;
                    }
                }
            }
        }

        public IEnumerable<CelestialBody> Orbiters => _Orbiters.Values;

        public IEnumerable<Star> Stars => _Orbiters.Values.OfType<Star>().OrderByDescending(o => o.PhysicalData.Mass);

        public int Index { get; internal set; }

        public int StarCount => Stars.Count();

        [CreateProperty] public Length DistanceFromSol => Length.FromParsecs(Coordinates.magnitude);

        public Mass Mass => Mass.FromSolarMasses(Stars.Sum(star => star.CalculatePlanetaryMass().SolarMasses));

        public Star this[int index] => Stars.ElementAt(index);

        public Star Primary => Stars.FirstOrDefault();

        public string Id { get; set; }

        [CreateProperty] public string Name { get; set; }

        public Vector3 Coordinates { get; set; }

        protected Dictionary<string, CelestialBody> _Orbiters { get; }

        public StarSystem()
        {
            _Orbiters = new Dictionary<string, CelestialBody>();
        }

        public StarSystem(string name) : this()
        {
            Name = name;
            Id   = MakeId(name);
        }

        public StarSystem(string name, Star primary) : this(name, new[] { primary }) { }

        public StarSystem(string name, IEnumerable<CelestialBody> orbiters) : this(name)
        {
            foreach (CelestialBody body in orbiters)
            {
                AddOrbiter(body);
            }
        }

        public bool ContainsStar(string starName) => _Orbiters.ContainsKey(starName);

        public CelestialBody[] ToArray()
        {
            return _Orbiters.Values.OrderByDescending(star => star.PhysicalData.Mass.SolarMasses).ToArray();
        }

        public IEnumerator<CelestialBody> GetEnumerator() =>
            _Orbiters?.Values.GetEnumerator() ?? Enumerable.Empty<CelestialBody>().GetEnumerator();

        public Length DistanceFrom(StarSystem system) => Length.FromParsecs((Coordinates - system.Coordinates).magnitude);

        public string SystemTree()
        {
            string tree = "*\n";
            foreach (CelestialBody body in Orbiters)
            {
                tree += Tree.RenderTree(body);
            }

            return tree;
        }

        public void AddOrbiter(CelestialBody body)
        {
            body.StarSystem = this;
            body.Index      = _Orbiters.Count;
            _Orbiters.Add(body.Name, body);

            foreach (CelestialBody orbiter in Hierarchy)
            {
                orbiter.StarSystem = this;
            }
        }

        public void SetBarycentre(Barycentre barycentre)
        {
            this.barycentre = barycentre;
        }

        public void VisitHierarchy<TBody>(Action<TBody> callback, Func<ITreeNode, IEnumerable<ITreeNode>> visitAlgorithm = null)
            where TBody : ITreeNode
        {
            visitAlgorithm ??= Tree.PreOrderVisit;

            foreach (ITreeNode body in _Orbiters.Values)
            {
                foreach (ITreeNode orbiter in visitAlgorithm(body))
                {
                    if (orbiter is TBody tBody)
                    {
                        callback.Invoke(tBody);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => _Orbiters?.Values.GetEnumerator() ?? Enumerable.Empty<CelestialBody>().GetEnumerator();


        /// <summary>
        ///     Returns a basic <see cref="StarSystem" /> model containing the Sun, Earth, the Moon, and Mars.
        /// </summary>
        public static StarSystem Sol
        {
            get
            {
                Star   sun   = Star.Sun;
                Planet earth = Planet.Earth;
                Planet moon  = Planet.Moon;
                Planet mars  = Planet.Mars;

                sun.AddOrbiter(earth);
                sun.AddOrbiter(mars);
                earth.AddOrbiter(moon);

                StarSystem sol = new("Sol", sun);
                return sol;
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
    }
}