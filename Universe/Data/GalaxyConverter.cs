using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Data
{
    public class GalaxyConverter : IConverterReader<Galaxy>
    {
        public void Read(JsonReader reader, JsonSerializer serializer, ref Galaxy galaxy)
        {
            JObject jo = JObject.Load(reader);
            galaxy.Name = (string)jo[nameof(Galaxy.Name)];
            JToken systems = jo[nameof(Galaxy.Systems)];

            if ((systems != null) && systems.HasValues)
            {
                galaxy.AddSystems(serializer.Deserialize<SortedList<string, StarSystem>>(systems.CreateReader()).Values);
                foreach (var kvp in galaxy.Systems)
                {
                    StarSystem system = kvp.Value;
                    system.Id = StarSystem.MakeId(kvp.Key);
                    system.Name = system.Name = kvp.Key;
                    
                }
            }
        }
    }
}