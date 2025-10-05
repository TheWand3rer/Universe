// VindemiatrixCollective.Universe.Data © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;

#endregion

namespace VindemiatrixCollective.Universe.Data
{
    public static class Serialize
    {
        public static void String(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }

        public static void String<TEnum>(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options) where TEnum : Enum
        {
            writer.WriteStringValue(value.ToString());
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
                        continue;

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