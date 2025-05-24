using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Data
{
    public class StarSystemConverter : IConverterReader<StarSystem>
    {
        public StarSystem Create(JObject jo)
        {
            return new StarSystem();
        }

        public void Read(JObject jo, JsonReader reader, JsonSerializer serializer, ref StarSystem starSystem)
        {
            starSystem.Name = reader.ParentNameFromContainer(nameof(Galaxy.Systems));
            string coordinates = jo.Value<string>(nameof(StarSystem.Coordinates)) ?? jo.Value<string>("c");

            starSystem.Coordinates = !string.IsNullOrEmpty(coordinates)
                ? ConverterExtensions.StringToVector3(coordinates)
                : Vector3.zero;

            JToken orbiters = jo[nameof(StarSystem.Orbiters)];

            if (orbiters is { HasValues: true })
            {
                foreach (CelestialBody orbiter in serializer.Deserialize<Dictionary<string, CelestialBody>>(orbiters.CreateReader()).Values)
                {
                    starSystem.AddOrbiter(orbiter);
                }
            }

            starSystem.Init();
        }
    }
}