using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VindemiatrixCollective.Universe.Data
{
    public class CoreObjectConverter<T> : JsonConverter
    where T: new()
    {
        public override bool CanWrite => true;

        private readonly IConverterReader<T> readerImplementation;
        private readonly Type[] types;

        public CoreObjectConverter(params Type[] types)
        {
            this.types = types;
        }

        public CoreObjectConverter() : this(typeof(T))
        {
            readerImplementation = new DefaultReaderImplementation<T>();
        }

        public CoreObjectConverter(IConverterReader<T> readerImplementation) : this(typeof(T))
        {
            this.readerImplementation = readerImplementation;
        }

        public override bool CanConvert(Type objectType)
        {
            return types.Any(t => t == objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            T target = new T();
            readerImplementation.Read(reader, serializer, ref target);

            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            serializer.Serialize(writer, value);
            jo.WriteTo(writer);
        }
    }

    internal class DefaultReaderImplementation<T> : IConverterReader<T>
    {
        public void Read(JsonReader reader, JsonSerializer serializer, ref T target)
        {
            JObject jo = JObject.Load(reader);
            serializer.Populate(jo.CreateReader(), target);
        }
    }
}