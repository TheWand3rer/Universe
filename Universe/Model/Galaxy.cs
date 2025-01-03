using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VindemiatrixCollective.Universe.Model
{
    [Serializable]
    public class Galaxy : IEnumerable<StarSystem>
    {
        private string name;

        public int SystemCount => Systems.Count;

        public SortedList<string, StarSystem> Systems { get; internal set; }

        public StarSystem this[string name]
        {
            get => Systems[name];
            set => Systems[name] = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public Galaxy()
        {
            name = nameof(Galaxy);
            Systems = new SortedList<string, StarSystem>();
        }

        public Galaxy(string name) : this()
        {
            this.name = name;
        }

        public void AddSystem(StarSystem system)
        {
            Systems.Add(system.Name, system);
        }

        public void AddSystems(IEnumerable<StarSystem> systems)
        {
            foreach (StarSystem system in systems)
            {
                Systems.Add(system.Name, system);
            }
        }

        public Galaxy(string name, IEnumerable<Star> stars) : this(name)
        {
            Systems = new SortedList<string, StarSystem>();

            foreach (Star star in stars)
            {
                if (!Systems.ContainsKey(star.Name))
                {
                    Systems.Add(star.Name, new StarSystem(star.Name));
                }

                this[star.Name].AddStar(star);
            }

        }

        public Planet GetPlanet(string locationPath)
        {
            string[] data = locationPath.Split('/');
            return Systems[data[0]][int.Parse(data[1])][data[2]];
        }

        public IEnumerator<StarSystem> GetEnumerator()
        {
            return Systems?.Values.GetEnumerator() ?? Enumerable.Empty<StarSystem>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}