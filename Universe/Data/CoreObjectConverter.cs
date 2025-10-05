// VindemiatrixCollective.Universe.Data © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

#endregion

namespace VindemiatrixCollective.Universe.Data
{
    public interface IConverterImplementation<out T, in TState>
    {
        Type Type { get; }
        bool ReadProperty(string propertyName, ref Utf8JsonReader reader, JsonSerializerOptions options, TState state);
        bool Validate(TState state, out IEnumerable<string> missingProperties);
        T Create(TState state);
    }

    public interface ISerializerImplementation<in T>
    {
        void Serialize(Utf8JsonWriter writer, T value, JsonSerializerOptions options);
    }

    public delegate TProperty PropertyReader<out TProperty>(ref Utf8JsonReader reader, JsonSerializerOptions options);

    public delegate void PropertySetter<in TState>(ref Utf8JsonReader reader, JsonSerializerOptions options, TState state)
        where TState : class, new();

    public delegate void PropertyWriter<in T>(Utf8JsonWriter writer, T value, JsonSerializerOptions options);

    public class CoreObjectConverter<T, TState> : JsonConverter<T> where T : class where TState : class, new()
    {
        protected IConverterImplementation<T, TState> Converter { get; set; }
        protected ISerializerImplementation<T> Serializer { get; set; }

        public CoreObjectConverter(
            IConverterImplementation<T, TState> readerImplementation, ISerializerImplementation<T> serializerImplementation = null)
        {
            Converter  = readerImplementation;
            Serializer = serializerImplementation;
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

            if (!Converter.Validate(state, out IEnumerable<string> missingProperties))
                throw new JsonException($"{Type.Name}: Missing required properties {string.Join(", ", missingProperties)}");
            return Converter.Create(state);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (Serializer == null)
                throw new NotImplementedException($"No serializer for {typeof(T).Name}");

            writer.WriteStartObject();
            Serializer.Serialize(writer, value, options);
            writer.WriteEndObject();
        }
    }
}