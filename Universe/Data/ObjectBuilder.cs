// VindemiatrixCollective.Universe.Data © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using UnityEngine.Assertions;

#endregion

namespace VindemiatrixCollective.Universe.Data
{
    public class ObjectBuilder<T, TState> : IConverterImplementation<T, TState> where TState : class, new()
    {
        private readonly Dictionary<string, PropertySetter<TState>> propertyReaders = new();

        private readonly List<string> optionalProperties = new();
        private Func<TState, T> create;

        public Type Type => typeof(T);

        public bool ReadProperty(string propertyName, ref Utf8JsonReader reader, JsonSerializerOptions options, TState state)
        {
            if (!propertyReaders.TryGetValue(propertyName, out PropertySetter<TState> propertyReader))
            {
                return optionalProperties.Contains(propertyName);
            }

            try
            {
                propertyReader(ref reader, options, state);
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Error while reading property [{Type.Name}.{propertyName}]", ex);
            }

            return true;
        }

        public bool Validate(TState state, out IEnumerable<string> missingProperties)
        {
            FieldInfo[]  fields  = state.GetType().GetFields();
            List<string> missing = new();
            bool         result  = true;
            foreach (FieldInfo field in fields)
            {
                if (optionalProperties.Contains(field.Name))
                    continue;

                object value = field.GetValue(state);

                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    result = false;
                    missing.Add(field.Name);
                }
            }

            missingProperties = missing;
            return result;
        }

        public T Create(TState state) => create(state);

        public class Builder
        {
            private readonly ObjectBuilder<T, TState> objectBuilder = new();

            public ObjectBuilder<T, TState> Build()
            {
                Assert.IsNotNull(objectBuilder.create, nameof(objectBuilder.create));
                return objectBuilder;
            }

            public Builder SetCreate(Func<TState, T> creator)
            {
                objectBuilder.create = creator;
                return this;
            }

            public Builder SetProperty(string propertyName, PropertySetter<TState> propertyReader, bool optional)
            {
                objectBuilder.propertyReaders[propertyName] = propertyReader;
                if (optional)
                {
                    objectBuilder.optionalProperties.Add(propertyName);
                }

                return this;
            }

            public Builder SetProperty<TProperty>(
                string propertyName, PropertyReader<TProperty> getter, Action<TState, TProperty> setter, bool optional = false,
                string alternativeName = null)
            {
                SetProperty(propertyName, PropertySetter, optional);

                if (!string.IsNullOrEmpty(alternativeName))
                {
                    SetProperty(alternativeName, PropertySetter, optional);
                }

                return this;

                void PropertySetter(ref Utf8JsonReader reader, JsonSerializerOptions options, TState state)
                {
                    TProperty value = getter(ref reader, options);
                    setter(state, value);
                }
            }

            public Builder SkipProperty(string propertyName)
            {
                objectBuilder.propertyReaders[propertyName] = (ref Utf8JsonReader reader, JsonSerializerOptions options, TState state) =>
                    reader.Skip();
                return this;
            }
        }
    }
}