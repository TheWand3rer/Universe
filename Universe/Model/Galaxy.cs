#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    [Serializable]
    public class Galaxy : IEnumerable<StarSystem>
    {
        private string name;

        public Galaxy()
        {
            name = nameof(Galaxy);
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

        public bool ContainsSystem(string name)
        {
            return _Systems.ContainsKey(name);
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
        ///     Must be in the format "SystemName/StarName*/PlanetName*/SatelliteName*", e.g.: "Sol/Sun/Earth/Moon".
        ///     Asterisks indicate optional parts. Only the SystemName is required. If no StarName is specified,
        ///     it will return the primary object.
        /// </param>
        /// <returns>The specified body</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public CelestialBody GetBody(string path)
        {
            Assert.IsFalse(string.IsNullOrEmpty(path), $"Path cannot be null or empty: {nameof(path)}");
            string[] parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(parts.Length > 1, $"Invalid path format: [{path}]");

            if (!_Systems.TryGetValue(parts[0], out StarSystem system))
            {
                throw new InvalidOperationException($"System {parts[0]} not found: [{path}]");
            }

            if (parts.Length == 1)
            {
                return system.Primary;
            }

            CelestialBody current = system[parts[1]];
            for (int i = 2; i < parts.Length; i++)
            {
                current = current[parts[i]];
                if (current == null)
                {
                    throw new InvalidOperationException($"{parts[i]} not found: [{path}]");
                }
            }

            return current;
        }
    }
}