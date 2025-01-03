using Newtonsoft.Json;

namespace VindemiatrixCollective.Universe.Data
{
    public interface IConverterReader<T>
    {
        void Read(JsonReader reader, JsonSerializer serializer, ref T target);
    }
}