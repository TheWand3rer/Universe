using System.Collections.Generic;
using UnityEngine;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Data
{
    public class StarSystemConverter : CoreObjectConverter<StarSystem, StarSystemConverter.StarSystemState>
    {
        public StarSystemConverter()
        {
            Converter = new ObjectBuilder<StarSystem, StarSystemState>.Builder()
               .SetProperty(nameof(CelestialBody.Name), Parse.String, (state, value) => state.Name = value)
               .SetProperty(nameof(StarSystem.Orbiters), Parse.List<CelestialBody, CelestialBodyConverter>,
                            (state, value) => state.Orbiters = value)
               .SetProperty(nameof(StarSystem.Coordinates), Parse.Vector3, (state, value) => state.Coordinates = value,
                            alternativeName: "c")
               .SetCreate(Creator)
               .Build();
        }

        private StarSystem Creator(StarSystemState state)
        {
            StarSystem system = new(state.Name, state.Orbiters)
            {
                Coordinates = state.Coordinates
            };
            return system;
        }

        public class StarSystemState
        {
            public List<CelestialBody> Orbiters;
            public string Name;
            public Vector3 Coordinates;
        }
    }
}