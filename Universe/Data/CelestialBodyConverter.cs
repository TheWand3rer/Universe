// VindemiatrixCollective.Universe.Data © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using VindemiatrixCollective.Universe.Model;

#endregion

namespace VindemiatrixCollective.Universe.Data
{
    public class CelestialBodyConverter : JsonConverter<CelestialBody>
    {
        public override CelestialBody Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument jsonDoc    = JsonDocument.ParseValue(ref reader);
            JsonElement        jsonObject = jsonDoc.RootElement;
            CelestialBody      body       = null;

            if (jsonObject.TryGetProperty(nameof(Star.StellarData), out _))
            {
                body = jsonObject.Deserialize<Star>(options);
            }

            if (jsonObject.TryGetProperty(nameof(Star.Attributes), out JsonElement value))
            {
                if (value.TryGetProperty("otypes", out _))
                {
                    body = jsonObject.Deserialize<Star>(options);
                }
            }

            if (jsonObject.TryGetProperty(nameof(Planet.PhysicalData), out _))
            {
                body = jsonObject.Deserialize<Planet>(options);
            }

            if (body == null)
            {
                throw new InvalidOperationException($"Unknown {nameof(CelestialBody)} type in: {jsonObject.ToString()}");
            }

            return body;
        }

        public override void Write(Utf8JsonWriter writer, CelestialBody value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}