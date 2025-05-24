#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    [Serializable]
    public class Galaxy : IEnumerable<StarSystem>
    {
        private string name;

        public Galaxy()
        {
            name     = nameof(Galaxy);
            _Systems = new Dictionary<string, StarSystem>();
        }

        public Galaxy(string name) : this()
        {
            this.name = name;
        }

        public Galaxy(string name, IEnumerable<Star> stars) : this(name)
        {
            _Systems = new Dictionary<string, StarSystem>();

            foreach (Star star in stars)
            {
                if (!_Systems.ContainsKey(star.Name))
                {
                    _Systems.Add(star.Name, new StarSystem(star.Name));
                }

                this[star.Name].AddOrbiter(star);
            }
        }

        public int SystemCount => _Systems.Count;

        public IEnumerable<StarSystem> Systems => _Systems.Values;
        protected Dictionary<string, StarSystem> _Systems { get; private set; }

        public StarSystem this[string name]
        {
            get => _Systems[name];
            set => _Systems[name] = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public IEnumerator<StarSystem> GetEnumerator()
        {
            return _Systems?.Values.GetEnumerator() ?? Enumerable.Empty<StarSystem>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddSystem(StarSystem system)
        {
            _Systems.Add(system.Name, system);
            system.Galaxy = this;
        }

        public void AddSystems(IEnumerable<StarSystem> systems)
        {
            foreach (StarSystem system in systems)
            {
                _Systems.Add(system.Name, system);
            }
        }

        public TBody GetBody<TBody>(string path)
            where TBody : CelestialBody
        {
            TBody body = (TBody)GetBody(path);
            return body;
        }

        /// <summary>
        ///     Traverses the Galaxy structure to retrieve the chosen body (Star or Planet/Satellite).
        /// </summary>
        /// <param name="path">
        ///     Must be in the format "SystemName/StarName*/PlanetName*/SatelliteName*", e.g.: "Sol/A/Earth/Moon".
        ///     Asterisks indicate optional parts. Only the SystemName is required.
        /// </param>
        /// <returns>The specified body</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public CelestialBody GetBody(string path)
        {
            Regex           regex   = new(@"(\w+)\/*");
            MatchCollection matches = regex.Matches(path);

            if (_Systems.TryGetValue(matches[0].Groups[1].Value, out StarSystem system))
            {
                if (matches.Count > 1)
                {
                    Star star = (Star)system[matches[1].Groups[1].Value];
                    if (matches.Count > 2)
                    {
                        Planet planet = star[matches[2].Groups[1].Value];
                        if (matches.Count > 3)
                        {
                            for (int i = 3; i < matches.Count; i++)
                            {
                                Match  match    = matches[i];
                                string pathPart = match.Groups[1].Value;

                                planet = planet[pathPart];
                            }
                        }

                        return planet;
                    }

                    return star;
                }

                return system[0];
            }

            throw new InvalidOperationException($"Invalid path: <{path}>");
        }
    }
}