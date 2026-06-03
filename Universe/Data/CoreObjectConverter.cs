// VindemiatrixCollective.Universe.Data © 2025 Vindemiatrix Collective

#region

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

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
        private string lastPropertyRead;
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
                    lastPropertyRead = propertyName;
                    reader.Read();

                    bool result = Converter.ReadProperty(propertyName, ref reader, options, state);
                    if (!result)
                    {
                        Debug.LogWarning($"Property not recognised: <{Type.Name}.{propertyName}> | {reader.TokenType}");
                    }
                }
                else
                {
                    throw new JsonException($"Unexpected token: <{Type.Name}.{reader.TokenType}> Last property read: {lastPropertyRead}");
                }
            }

            if (!Converter.Validate(state, out IEnumerable<string> missingProperties))
            {
                throw new JsonException($"Missing required properties in {Type.Name}: {string.Join(", ", missingProperties)}");
            }

            return Converter.Create(state);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (Serializer == null)
            {
                throw new NotImplementedException($"No serializer for {typeof(T).Name}");
            }

            writer.WriteStartObject();
            Serializer.Serialize(writer, value, options);
            writer.WriteEndObject();
        }
    }
}