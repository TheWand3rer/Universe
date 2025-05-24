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
#if UNITY_EDITOR
        /// <summary>
        ///     Convenience variable to see some values in the inspector.
        /// </summary>
        public string DistanceFromSolLy;
#endif

        public CelestialBody this[string name] => _Orbiters[name];

        public Galaxy Galaxy { get; internal set; }

        public IEnumerable<CelestialBody> Hierarchy
        {
            get
            {
                foreach (CelestialBody body in _Orbiters.Values)
                {
                    foreach (CelestialBody orbiter in CelestialBody.PreOrderVisit(body))
                    {
                        yield return orbiter;
                    }
                }
            }
        }

        public IEnumerable<CelestialBody> Orbiters => _Orbiters.Values;

        public IEnumerable<Star> Stars => _Orbiters.Values.OfType<Star>().OrderByDescending(o => o.PhysicalData.Mass);

        public int StarCount => Stars.Count();

        [CreateProperty] public Length DistanceFromSol => Length.FromParsecs(Coordinates.magnitude);

        public Star this[int index] => Stars.ElementAt(index);

        public Star Primary => Stars.FirstOrDefault();

        public string Id { get; set; }

        [CreateProperty] public string Name { get; set; }

        public Vector3 Coordinates { get; set; }

        protected Dictionary<string, CelestialBody> _Orbiters { get; }

        public StarSystem() : this(nameof(StarSystem)) { }

        public StarSystem(string name)
        {
            Name      = name;
            _Orbiters = new Dictionary<string, CelestialBody>();
            Id        = MakeId(name);
        }

        public StarSystem(string name, Star primary) : this(name, new[] { primary }) { }

        public StarSystem(string name, IEnumerable<CelestialBody> orbiters) : this(name)
        {
            _Orbiters = orbiters.ToDictionary(star => star.Name, star => star);
        }

        public void AddOrbiter(CelestialBody body)
        {
            body.StarSystem = this;
            _Orbiters.Add(body.Name, body);

            foreach (CelestialBody orbiter in Hierarchy)
            {
                orbiter.StarSystem = this;
            }
        }

        public bool ContainsStar(string starName)
        {
            return _Orbiters.ContainsKey(starName);
        }

        public IEnumerator<CelestialBody> GetEnumerator()
        {
            return _Orbiters?.Values.GetEnumerator() ?? Enumerable.Empty<CelestialBody>().GetEnumerator();
        }

        public void Init()
        {
#if UNITY_EDITOR
            DistanceFromSolLy = DistanceFromSol.LightYears.ToString("0.00 LY");
#endif
        }

        public CelestialBody[] ToArray()
        {
            return _Orbiters.Values.OrderByDescending(star => star.PhysicalData.Mass.SolarMasses).ToArray();
        }

        public void VisitHierarchy<TBody>(Action<TBody> callback)
            where TBody : CelestialBody
        {
            foreach (CelestialBody body in Hierarchy)
            {
                if (body is TBody tBody)
                {
                    callback.Invoke(tBody);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Orbiters?.Values.GetEnumerator() ?? Enumerable.Empty<CelestialBody>().GetEnumerator();
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