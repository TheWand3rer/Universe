using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VindemiatrixCollective.Universe.Data
{
    public interface IConverterReader<T>
    {
        T Create(JObject jo);
        void Read(JObject jo, JsonReader reader, JsonSerializer serializer, ref T target);
    }
}