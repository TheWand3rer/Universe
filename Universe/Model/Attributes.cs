// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    public class Attributes : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> data;

        public CelestialBodyType Type => TryGet<CelestialBodyType>(nameof(Type));

        public string this[string key]
        {
            get => data[key];
            set => data[key] = value;
        }

        public Attributes()
        {
            data = new Dictionary<string, string>();
        }

        public Attributes(Dictionary<string, string> data)
        {
            this.data = data;
        }

        public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public string TryGet(string key)
        {
            data.TryGetValue(key, out string value);
            return value;
        }

        public TEnum TryGet<TEnum>(string alternativeKey = null) where TEnum : struct, Enum
        {
            string value = TryGet(typeof(TEnum).Name);
            if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(alternativeKey))
            {
                value = TryGet(alternativeKey);
            }

            if (string.IsNullOrEmpty(value))
                return default(TEnum);
            return Enum.Parse<TEnum>(value);
        }

        public void CopyFrom(Attributes attributes)
        {
            CopyFrom(attributes.data);
        }

        public void CopyFrom(IDictionary<string, string> attributes)
        {
            foreach (var kvp in attributes)
            {
                data[kvp.Key] = kvp.Value;
            }
        }

        public void Set<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            data[typeof(TEnum).Name] = value.ToString();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}