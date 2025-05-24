using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Data
{
    public class CelestialBodyConverter : IConverterReader<CelestialBody>
    {
        private readonly PlanetConverter planetConverter = new();
        private readonly StarConverter starConverter = new();

        private CelestialBodyType type;

        public CelestialBody Create(JObject jo)
        {
            if (jo.TryGetValue(nameof(SpectralClass), out JToken value) || jo.TryGetValue("SC", out value))
            {
                type = CelestialBodyType.Star;
                return new Star();
            }

            if (jo.SelectToken($"{nameof(Attributes)}.{nameof(CelestialBody.Type)}") != null)
            {
                type = CelestialBodyType.Planet;
                return new Planet();
            }

            throw new InvalidOperationException($"Invalid Orbiter type: {jo.Path}");
        }

        public void Read(JObject jo, JsonReader reader, JsonSerializer serializer, ref CelestialBody target)
        {
            switch (type)
            {
                case CelestialBodyType.Star:
                    Star star = (Star)target;
                    starConverter.Read(jo, reader, serializer, ref star);
                    break;

                case CelestialBodyType.Planet:
                    Planet planet = (Planet)target;
                    planetConverter.Read(jo, reader, serializer, ref planet);
                    break;
                default:
                    throw new InvalidOperationException("Invalid orbiter type");
            }
        }
    }
}