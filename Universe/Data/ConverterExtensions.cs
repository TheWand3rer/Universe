using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

namespace VindemiatrixCollective.Universe.Data
{
    public static class ConverterExtensions
    {
        public static string ParentNameFromContainer(this JsonReader reader, string containerName)
        {
            string pattern = @$"{containerName}[.\[']*([A-Za-z0-9.\s]+)['\]]*";
            Match  m       = Regex.Match(reader.Path, pattern);
            string name    = m.Groups[1].Value;
            return name;
        }

        public static string ObjectNameFromPath(this JsonReader reader)
        {
            return reader.Path.Split('.')[^1];
        }

        public static string KeyValueFromPath(this JsonReader reader)
        {
            string pattern = @"\[\'([A-Za-z0-0.]+)\']";
            Match  m       = Regex.Match(reader.Path, pattern);
            string name    = m.Groups[1].Value;
            return name;
        }

        public static Vector3 StringToVector3(string input, char delimiter = ',')
        {
            string[] array = input.Split(delimiter);
            return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
        }

        internal static void CheckValue(string fieldName, double? field)
        {
            if (!field.HasValue)
            {
                Debug.LogWarning($"No value entered for {fieldName}.");
            }
        }

        internal static void CheckValue(string fieldName, float? field)
        {
            if (!field.HasValue)
            {
                Debug.LogWarning($"No value entered for {fieldName}.");
            }
        }

        internal static void CheckValue(string fieldName, string field, bool required = true)
        {
            if (string.IsNullOrEmpty(field))
            {
                string message = $"No value entered for {fieldName}.";
                Debug.LogWarning(message);
                if (required)
                {
                    throw new ArgumentException(message);
                }
            }
        }
    }
}