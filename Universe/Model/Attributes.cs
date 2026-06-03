// VindemiatrixCollective.Universe © 2025-2026 Vindemiatrix Collective

#region using

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

        public bool ContainsKey(string key) => data.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => data.GetEnumerator();

        public bool TryGet(string key, out string value)
        {
            bool result = data.TryGetValue(key, out value);
            return result;
        }

        public TEnum TryGet<TEnum>(string alternativeKey = null) where TEnum : struct, Enum
        {
            if (!TryGet(typeof(TEnum).Name, out string value))
            {
                if (!string.IsNullOrEmpty(alternativeKey))
                    TryGet(alternativeKey, out value);
            }

            return string.IsNullOrEmpty(value) ? default : Enum.Parse<TEnum>(value);
        }

        public TEnum Get<TEnum>() where TEnum : struct, Enum
        {
            string value = data[typeof(TEnum).Name];
            return Enum.Parse<TEnum>(value);
        }

        public TEnum Get<TEnum>(string key) where TEnum : struct, Enum
        {
            string value = data[key];
            return Enum.Parse<TEnum>(value);
        }

        public void CopyFrom(Attributes attributes)
        {
            CopyFrom(attributes.data);
        }

        public void CopyFrom(IDictionary<string, string> attributes)
        {
            foreach ((string key, string value) in attributes)
            {
                data[key] = value;
            }
        }

        public void Set<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            data[typeof(TEnum).Name] = value.ToString();
        }


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}