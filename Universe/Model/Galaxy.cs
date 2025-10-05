// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

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

        public IEnumerable<StarSystem> Systems => _Systems.Values;

        public int SystemCount => _Systems.Count;

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

        protected Dictionary<string, StarSystem> _Systems { get; private set; }

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

        public bool ContainsSystem(string name) => _Systems.ContainsKey(name);

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

        public IEnumerator<StarSystem> GetEnumerator() =>
            _Systems?.Values.GetEnumerator() ?? Enumerable.Empty<StarSystem>().GetEnumerator();

        public TBody GetBody<TBody>(string path) where TBody : CelestialBody
        {
            TBody body = (TBody)GetBody(path);
            return body;
        }

        public void AddSystem(StarSystem system)
        {
            system.Galaxy = this;
            system.Index  = _Systems.Count;
            _Systems.Add(system.Name, system);
        }

        public void AddSystems(IEnumerable<StarSystem> systems)
        {
            foreach (StarSystem system in systems)
            {
                AddSystem(system);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}