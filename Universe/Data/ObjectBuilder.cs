using System;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine.Assertions;

namespace VindemiatrixCollective.Universe.Data
{
    public class ObjectBuilder<T, TState> : IConverterImplementation<T, TState> where TState : class, new()
    {
        // readonly Dictionary<string, PropertySetter> propertyReaders = new();
        private readonly Dictionary<string, PropertySetter<TState>> propertyReaders = new();

        private readonly List<string> optionalProperties = new();
        private Func<TState, T> createAction;

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

        public T Create(TState state) => createAction(state);

        public class Builder
        {
            private readonly ObjectBuilder<T, TState> objectBuilder = new();

            public ObjectBuilder<T, TState> Build()
            {
                Assert.IsNotNull(objectBuilder.createAction, nameof(objectBuilder.createAction));
                return objectBuilder;
            }

            public Builder SetCreate(Func<TState, T> creator)
            {
                objectBuilder.createAction = creator;
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

            //public Builder SetProperty<TProperty>(
            //    string propertyName, PropertyReader<TProperty> getter, Action<TProperty> setter, bool optional = false,
            //    string alternativeName = null)
            //{
            //    SetProperty(propertyName, (ref Utf8JsonReader reader, JsonSerializerOptions options) =>
            //    {
            //        TProperty value = getter(ref reader, options);
            //        setter(value);
            //    }, optional);

            //    if (!string.IsNullOrEmpty(alternativeName))
            //    {
            //        SetProperty(alternativeName, (ref Utf8JsonReader reader, JsonSerializerOptions options) =>
            //        {
            //            TProperty value = getter(ref reader, options);
            //            setter(value);
            //        }, optional);
            //    }

            //    return this;
            //}

            public Builder SetProperty<TProperty>(
                string propertyName, PropertyReader<TProperty> getter, Action<TState, TProperty> setter, bool optional = false,
                string alternativeName = null)
            {
                SetProperty(propertyName, propertySetter, optional);

                if (!string.IsNullOrEmpty(alternativeName))
                {
                    SetProperty(alternativeName, propertySetter, optional);
                }

                return this;

                void propertySetter(ref Utf8JsonReader reader, JsonSerializerOptions options, TState state)
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