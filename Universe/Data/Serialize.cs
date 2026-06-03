// VindemiatrixCollective.Universe.Data © 2025 Vindemiatrix Collective

#region

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace VindemiatrixCollective.Universe.Data
{
    public static class Serialize
    {
        public static void Bool(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }

        public static void String(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }

        public static void String<TEnum>(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options) where TEnum : Enum
        {
            writer.WriteStringValue(value.ToString());
        }

        public static void Vector3(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{value.x}, {value.y}, {value.z}");
        }

        public static void Array(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (string item in value)
            {
                writer.WriteStringValue(item);
            }

            writer.WriteEndArray();
        }

        public static void Array<T>(Utf8JsonWriter writer, IEnumerable<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (T item in value)
            {
                JsonSerializer.Serialize(writer, item, options);
            }

            writer.WriteEndArray();
        }

        public static void Type(Utf8JsonWriter writer, Type type, JsonSerializerOptions options)
        {
            string asmName = string.Join('.', type.Assembly.GetName().Name.Split('.')[1..]);
            string typeName;
            if (asmName.Contains("Module"))
            {
                asmName = Regex.Replace(asmName, "(.*)Module$", "Unity.$1");
            }

            if (typeof(Object).IsAssignableFrom(type))
            {
                List<string> ns      = new();
                string[]     nsParts = type.FullName.Split('.')[1..];
                foreach (string s in nsParts)
                {
                    if (asmName.Contains(s, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    ns.Add(s);
                }

                typeName = string.Join('.', ns);
            }
            else
            {
                asmName  = "Unity.Core";
                typeName = "TextAsset";
            }

            writer.WriteStringValue($"{asmName}:{typeName}");
        }
    }
}