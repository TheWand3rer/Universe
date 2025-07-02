using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;
using VindemiatrixCollective.Universe.Data;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Tests
{
    public class DeserializationHelper
    {
        public static readonly JsonConverter[] Converters =
        {
            new CoreObjectConverter<Galaxy>(new GalaxyConverter()),
            new CoreObjectConverter<StarSystem>(new StarSystemConverter()),
            new CoreObjectConverter<Star>(new StarConverter()),
            new CoreObjectConverter<Planet>(new PlanetConverter()),
            new CoreObjectConverter<CelestialBody>(new CelestialBodyConverter())
        };

        public Galaxy LoadGalaxy(string path = "Data/galaxy")
        {
            Galaxy galaxy = DeserializeFile<Galaxy>(path, Converters);
            return galaxy;
        }

        public Galaxy LoadSol(string path = "Data/SolarSystem")
        {
            Galaxy galaxy = new("Milky Way");
            LoadSol(ref galaxy);
            return galaxy;
        }

        public T DeserializeFile<T>(string filename, params JsonConverter[] converters)
        {
            TextAsset text = Resources.Load<TextAsset>(filename);
            Assert.IsNotNull(text, filename);
            T data = JsonConvert.DeserializeObject<T>(text.ToString(), converters);
            return data;
        }

        public T DeserializeObject<T>(string json, params JsonConverter[] converters)
        {
            T data = JsonConvert.DeserializeObject<T>(json, converters);
            return data;
        }

        public void LoadSol(ref Galaxy galaxy, string path = "Data/SolarSystem")
        {
            JsonSerializerSettings settings       = new();
            Galaxy                 additionalData = DeserializeFile<Galaxy>(path, Converters);
            foreach (StarSystem system in additionalData.Systems)
            {
                galaxy.AddSystem(system);
            }
        }
    }
}