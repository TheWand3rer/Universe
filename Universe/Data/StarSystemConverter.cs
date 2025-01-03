using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Data
{
    public class StarSystemConverter : IConverterReader<StarSystem>
    {
        //public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        //{
        //    StarSystem system = (StarSystem)value;
        //    writer.WriteStartObject();
        //    writer.WritePropertyName(nameof(StarSystem.Stars));
        //    serializer.Serialize(writer, system.Stars);
        //    writer.WritePropertyName(nameof(StarSystem.Coordinates));
        //    writer.WriteValue(DataUtils.Vector3ToString(system.Coordinates));
        //    writer.WriteEndObject();
        //}

        public void Read(JsonReader reader, JsonSerializer serializer, ref StarSystem starSystem)
        {
            JObject jo = JObject.Load(reader);
            starSystem.Name = ConverterExtensions.ParentNameFromContainer(reader, nameof(Galaxy.Systems));
            string coordinates = (string)jo[nameof(StarSystem.Coordinates)];

            starSystem.Coordinates = !string.IsNullOrEmpty(coordinates)
                ? ConverterExtensions.StringToVector3(coordinates)
                : Vector3.zero;

            JToken stars = jo[nameof(StarSystem.Stars)];

            if ((stars != null) && stars.HasValues)
            {
                starSystem.AddStars(serializer.Deserialize<Dictionary<string, Star>>(stars.CreateReader()).Values);
            }

            starSystem.Init();
        }
    }
}