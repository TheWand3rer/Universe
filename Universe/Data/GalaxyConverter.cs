using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Data
{
    public class GalaxyConverter : IConverterReader<Galaxy>
    {
        public Galaxy Create(JObject jo)
        {
            return new Galaxy();
        }

        public void Read(JObject jo, JsonReader reader, JsonSerializer serializer, ref Galaxy galaxy)
        {
            galaxy.Name = (string)jo[nameof(Galaxy.Name)];
            JToken systems = jo[nameof(Galaxy.Systems)];

            if (systems is { HasValues: true })
            {
                Dictionary<string, StarSystem> systemDict = serializer.Deserialize<Dictionary<string, StarSystem>>(systems.CreateReader());

                foreach ((string key, StarSystem system) in systemDict)
                {
                    system.Id   = StarSystem.MakeId(key);
                    system.Name = system.Name = key;
                    galaxy.AddSystem(system);
                }
            }
        }
    }
}