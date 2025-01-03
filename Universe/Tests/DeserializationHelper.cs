using Newtonsoft.Json;
using UnityEngine;
using VindemiatrixCollective.Universe.Data;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Tests
{
    public class DeserializationHelper
    {
        public T DeserializeFile<T>(string filename, params JsonConverter[] converters)
        {
            TextAsset text = Resources.Load<TextAsset>(filename);
            T data = JsonConvert.DeserializeObject<T>(text.ToString(), converters);
            return data;
        }

        public Galaxy LoadGalaxy(string path = "Data/galaxy")
        {
            Galaxy galaxy = DeserializeFile<Galaxy>(path,
                new CoreObjectConverter<Galaxy>(new GalaxyConverter()),
                new CoreObjectConverter<StarSystem>(new StarSystemConverter()),
                new CoreObjectConverter<Star>(new StarConverter()),
                new CoreObjectConverter<Planet>(new PlanetConverter()));

            //EventLog.Info($"Loaded {galaxy.Systems.Count} systems");
            return galaxy;
        }

        public void LoadSol(ref Galaxy galaxy, string path = "Data/SolarSystem")
        {
            JsonSerializerSettings settings = new();
            Galaxy additionalData = DeserializeFile<Galaxy>(path,
                new CoreObjectConverter<Galaxy>(new GalaxyConverter()),
                new CoreObjectConverter<StarSystem>(new StarSystemConverter()),
                new CoreObjectConverter<Star>(new StarConverter()),
                new CoreObjectConverter<Planet>(new PlanetConverter()));
            foreach ((string key, StarSystem system) in additionalData.Systems)
            {
                galaxy[key] = system;
            }

        }

        public Galaxy LoadSol(string path = "Data/SolarSystem")
        {
            Galaxy galaxy = new Galaxy("Milky Way");
            LoadSol(ref galaxy);
            return galaxy;
        }
    }
}