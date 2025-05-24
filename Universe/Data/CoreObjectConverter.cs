using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VindemiatrixCollective.Universe.Data
{
    public class CoreObjectConverter<T> : JsonConverter
        where T : class
    {
        private readonly IConverterReader<T> readerImplementation;
        private readonly Type[] types;
        public override bool CanWrite => true;

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
            JObject jo     = JObject.Load(reader);
            T       target = readerImplementation.Create(jo);
            readerImplementation.Read(jo, reader, serializer, ref target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jo = new();
            serializer.Serialize(writer, value);
            jo.WriteTo(writer);
        }
    }

    internal class DefaultReaderImplementation<T> : IConverterReader<T>
        where T : class
    {
        public T Create(JObject jo)
        {
            return Activator.CreateInstance<T>();
        }

        public void Read(JObject jo, JsonReader reader, JsonSerializer serializer, ref T target)
        {
            serializer.Populate(jo.CreateReader(), target);
        }
    }
}