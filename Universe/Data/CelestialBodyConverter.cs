using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Data
{
    public class CelestialBodyConverter : JsonConverter<CelestialBody>
    {
        public override CelestialBody Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument jsonDoc    = JsonDocument.ParseValue(ref reader);
            JsonElement        jsonObject = jsonDoc.RootElement;

            if (jsonObject.TryGetProperty(nameof(Star.StellarData), out _))
            {
                return jsonObject.Deserialize<Star>(options);
            }

            if (jsonObject.TryGetProperty(nameof(Star.Attributes), out JsonElement value))
            {
                if (value.TryGetProperty("otypes", out _))
                {
                    return jsonObject.Deserialize<Star>(options);
                }
            }

            if (jsonObject.TryGetProperty(nameof(Planet.PhysicalData), out _))
            {
                return jsonObject.Deserialize<Planet>(options);
            }

            throw new InvalidOperationException($"Unknown {nameof(CelestialBody)} type in: {jsonObject.ToString()}");
        }

        public override void Write(Utf8JsonWriter writer, CelestialBody value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}