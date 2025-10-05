// VindemiatrixCollective.Universe.Tests © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System.Text.Json;
using UnityEngine;
using UnityEngine.Assertions;
using VindemiatrixCollective.Universe.Data;
using VindemiatrixCollective.Universe.Model;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

#endregion

namespace VindemiatrixCollective.Universe.Tests
{
    public class DeserializationHelper
    {
        public static readonly JsonConverter[] Converters =
        {
            new GalaxyConverter(), new StarSystemConverter(),
            new StarConverter(), new PlanetConverter(), new CelestialBodyConverter(),
            new PhysicalDataConverter(), new StellarDataConverter(), new OrbitalDataConverter()
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
            JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

            foreach (JsonConverter converter in converters)
            {
                options.Converters.Add(converter);
            }

            return JsonSerializer.Deserialize<T>(text.text, options);
        }

        public T DeserializeObject<T>(string data, params JsonConverter[] converters)
        {
            JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

            foreach (JsonConverter converter in converters)
            {
                options.Converters.Add(converter);
            }

            return JsonSerializer.Deserialize<T>(data, options);
        }

        public void LoadSol(ref Galaxy galaxy, string path = "Data/SolarSystem")
        {
            Galaxy additionalData = DeserializeFile<Galaxy>(path, Converters);
            foreach (StarSystem system in additionalData.Systems)
            {
                galaxy.AddSystem(system);
            }
        }
    }
}