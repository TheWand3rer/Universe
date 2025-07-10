using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace VindemiatrixCollective.Universe.Data
{
    public static class Parse
    {
        public static TEnum EnumFromInt<TEnum>(ref Utf8JsonReader reader, JsonSerializerOptions options) where TEnum : struct, Enum
        {
            int level = reader.GetInt32();
            return (TEnum)Enum.ToObject(typeof(TEnum), level);
        }

        public static TEnum EnumFromString<TEnum>(ref Utf8JsonReader reader, JsonSerializerOptions options) where TEnum : struct, Enum
        {
            string value = reader.GetString();
            return Enum.Parse<TEnum>(value);
        }

        public static string String(ref Utf8JsonReader reader, JsonSerializerOptions options) => reader.GetString();

        public static float Float(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            float value;

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out int numberInt))
                {
                    value = numberInt;
                }
                else if (reader.TryGetDouble(out double numberDouble))
                {
                    value = (float)numberDouble;
                }
                else
                {
                    throw new JsonException("Invalid number format");
                }
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                string text = reader.GetString();
                if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
                {
                    throw new JsonException($"Encountered text while parsing string: \"{text}\", but it is not a number");
                }

                value = result;
                Debug.LogWarning($"Encountered text while parsing number: \"{text}\": please change it to a number by removing the \" in the JSON file.");
            }
            else
            {
                throw new JsonException($"Invalid token type: {reader.TokenType}");
            }

            try
            {
                return value;
            }
            catch (InvalidCastException ex)
            {
                throw new JsonException($"Value is <{value.GetType().Name}> instead of <float>");
            }
        }

        public static double Double(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            double value;

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out int numberInt))
                {
                    value = numberInt;
                }
                else if (reader.TryGetDouble(out double numberDouble))
                {
                    value = numberDouble;
                }
                else
                {
                    throw new JsonException("Invalid number format");
                }
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                string text = reader.GetString();
                if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                {
                    throw new JsonException($"Encountered text while parsing string: \"{text}\", but it is not a number");
                }

                value = result;
                Debug.LogWarning($"Encountered text while parsing number: \"{text}\": please change it to a number by removing the \" in the JSON file.");
            }
            else
            {
                throw new JsonException($"Invalid token type: {reader.TokenType}");
            }

            try
            {
                return value;
            }
            catch (InvalidCastException ex)
            {
                throw new JsonException($"Value is <{value.GetType().Name}> instead of <double>");
            }
        }

        public static T Object<T>(ref Utf8JsonReader reader, JsonSerializerOptions options) =>
            JsonSerializer.Deserialize<T>(ref reader, options);

        public static ValueUnit ValueUnit(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            double v = 0;
            string u = string.Empty;
            if (reader.TokenType == JsonTokenType.Number)
            {
                v = Double(ref reader, options);
                return new ValueUnit(v, u);
            }

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string innerPropertyName = reader.GetString();
                    reader.Read(); // Move to the value

                    if (innerPropertyName == "v")
                    {
                        v = reader.GetDouble();
                    }
                    else if (innerPropertyName == "u")
                    {
                        u = reader.GetString();
                        // superscripts
                        u = u.Replace('2', '\u00B2');
                        u = u.Replace('3', '\u00B3');
                    }
                }
            }

            return new ValueUnit(v, u);
        }

        public static T[] Array<T>(ref Utf8JsonReader reader, JsonSerializerOptions options) where T : struct =>
            JsonSerializer.Deserialize<T[]>(ref reader, options);

        public static string[] Array(ref Utf8JsonReader reader, JsonSerializerOptions options) =>
            JsonSerializer.Deserialize<string[]>(ref reader, options);

        public static Vector2 Vector2(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            float[] array = Array<float>(ref reader, options);
            return new Vector2(array[0], array[1]);
        }

        public static Vector3 Vector3(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            float[] array = null;
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                array = Array<float>(ref reader, options);
                return new Vector3(array[0], array[1], array[2]);
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                string  text   = reader.GetString();
                Vector3 vector = ConverterExtensions.StringToVector3(text);
                return vector;
            }

            throw new JsonException($"Cannot parse {nameof(Vector3)}");
        }

        public static Dictionary<string, TValue> Dictionary<TValue>(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Invalid Token type: <{reader.TokenType}>");
            }

            Dictionary<string, TValue> dict = new();

            string tag = $"Dictionary<string, {typeof(TValue).Name}>";

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dict;
                }

                string key = reader.GetString();
                if (string.IsNullOrEmpty(key))
                {
                    throw new JsonException($"{tag}: key cannot be null");
                }

                reader.Read();

                object value = null;
                switch (reader.TokenType)
                {
                    case JsonTokenType.Number:
                        value = typeof(TValue).Name switch
                        {
                            nameof(Int32)  => reader.GetInt32(),
                            nameof(Double) => reader.GetDouble(),
                            nameof(Single) => Float(ref reader, options),
                            _              => throw new JsonException($"{tag}.{key}: invalid number format <{typeof(TValue)}>")
                        };
                        break;

                    case JsonTokenType.String:
                        value = reader.GetString();
                        break;
                    case JsonTokenType.True:
                        value = true;
                        break;

                    case JsonTokenType.False:
                        value = false;
                        break;

                    default:
                        throw new JsonException($"{tag} {key}: Unsupported value type: {reader.TokenType}");
                }

                try
                {
                    TValue tValue = (TValue)Convert.ChangeType(value, typeof(TValue));
                    dict.Add(key, tValue);
                }
                catch (InvalidCastException ex)
                {
                    throw new JsonException($"[{tag}] Value is <{value?.GetType().Name ?? "null"}> instead of <{typeof(TValue).Name}>");
                }
            }

            return dict;
        }

        public static List<T> List<T, TConverter>(ref Utf8JsonReader reader, JsonSerializerOptions options)
            where TConverter : JsonConverter<T>, new()
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException($"Invalid Token type: <{reader.TokenType}>");
            }

            //List<T> list = new();

            //while (reader.Read())
            //{
            //    if (reader.TokenType == JsonTokenType.EndArray)
            //    {
            //        return list;
            //    }

            //    TConverter converter = new();
            //    T          item      = converter.Read(ref reader, typeof(T), options);
            //    list.Add(item);
            //}

            return JsonSerializer.Deserialize<List<T>>(ref reader, options);
        }
    }
}