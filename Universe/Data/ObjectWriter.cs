// VindemiatrixCollective.Universe.Data © 2025 Vindemiatrix Collective

#region

using System;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

#endregion

namespace VindemiatrixCollective.Universe.Data
{
    public class ObjectWriter<T> : ISerializerImplementation<T> where T : class
    {
        private readonly Dictionary<string, Func<T, bool>> predicates = new();
        private readonly List<Action<Utf8JsonWriter, T, JsonSerializerOptions>> writers = new();

        public void Serialize(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            foreach (Action<Utf8JsonWriter, T, JsonSerializerOptions> action in writers)
            {
                action(writer, value, options);
            }
        }

        public class Builder
        {
            private readonly ObjectWriter<T> objectWriter = new();

            public ObjectWriter<T> Build() => objectWriter;

            public Builder SetPredicate(string propertyName, Func<T, bool> predicate)
            {
                objectWriter.predicates.Add(propertyName, predicate);
                return this;
            }

            public Builder SetProperty<TValue>(string propertyName, PropertyWriter<TValue> writer, Func<T, TValue> accessor)
            {
                objectWriter.writers.Add((w, value, o) =>
                {
                    TValue propValue = accessor(value);

                    if (propValue == null || string.IsNullOrEmpty(propValue.ToString()))
                    {
                        Debug.LogWarning($"[{typeof(T).Name}.{propertyName}]: attempted to serialize null or empty property");
                        return;
                    }

                    if (objectWriter.predicates.TryGetValue(propertyName, out Func<T, bool> predicate))
                    {
                        if (!predicate(value))
                        {
                            return;
                        }
                    }

                    try
                    {
                        w.WritePropertyName(propertyName);
                        writer(w, propValue, o);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error while serializing: <{typeof(T).Name}.{propertyName}> Value: {propValue.ToString()}");
                        throw;
                    }
                });

                return this;
            }
        }
    }
}