using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VindemiatrixCollective.Universe.Data
{
    public interface IConverterImplementation<out T, in TState>
    {
        Type Type { get; }
        bool ReadProperty(string propertyName, ref Utf8JsonReader reader, JsonSerializerOptions options, TState state);
        T Create(TState state);
    }

    public delegate TProperty PropertyReader<out TProperty>(ref Utf8JsonReader reader, JsonSerializerOptions options);

    public delegate void PropertySetter(ref Utf8JsonReader reader, JsonSerializerOptions options);

    public delegate void PropertySetter<TState>(ref Utf8JsonReader reader, JsonSerializerOptions options, TState state)
        where TState : class, new();

    public class CoreObjectConverter<T, TState> : JsonConverter<T> where T : class where TState : class, new()
    {
        protected IConverterImplementation<T, TState> Converter { get; set; }

        public CoreObjectConverter(IConverterImplementation<T, TState> readerImplementation)
        {
            Converter = readerImplementation;
        }

        protected CoreObjectConverter() { }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"{Type.Name}: Expected start of object but found {reader.TokenType}");
            }

            TState state = new();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString();

                    reader.Read();

                    bool result = Converter.ReadProperty(propertyName, ref reader, options, state);
                    if (!result)
                    {
                        throw new JsonException($"{Type.Name}: Property not recognised: <{propertyName}> | {reader.TokenType}");
                    }
                }
                else
                {
                    throw new JsonException($"{Type.Name}: Unexpected token: <{reader.TokenType}> Value: {reader.GetString()}");
                }
            }

            return Converter.Create(state);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}