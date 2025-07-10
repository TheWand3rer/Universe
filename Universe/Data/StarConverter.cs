// com.vindemiatrixcollective.universe.data © 2025 Vindemiatrix Collective
// Website and Documentation: https://dev.vindemiatrixcollective.com

using System.Collections.Generic;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Data
{
    public class StarConverter : CoreObjectConverter<Star, StarConverter.StarState>
    {
        public StarConverter()
        {
            Converter = new ObjectBuilder<Star, StarState>.Builder()
               .SetProperty("Id", Parse.String, (state, value) => state.Id = value, true)
               .SetProperty(nameof(CelestialBody.Name), Parse.String, (state, value) => state.Name = value)
               .SetProperty(nameof(CelestialBody.Attributes), Parse.Dictionary<string>, (state, value) => state.Attributes = value)
               .SetProperty(nameof(CelestialBody.OrbitalData), Parse.Object<OrbitalData>, (state, value) => state.OrbitalData = value, true)
               .SetProperty(nameof(Star.StellarData), Parse.Object<StellarData>, (state, value) => state.StellarData = value,
                            alternativeName: nameof(CelestialBody.PhysicalData))
               .SetProperty(nameof(Star.SpectralClass), Parse.String, (state, value) => state.SpectralClass = value, alternativeName: "SC")
               .SetProperty(nameof(StarSystem.Orbiters), Parse.List<CelestialBody, CelestialBodyConverter>,
                            (state, value) => state.Orbiters = value)
               .SetCreate(Creator)
               .Build();
        }

        private Star Creator(StarState state)
        {
            if (state.StellarData == null)
            {
                state.StellarData = StellarData.Null;
            }

            Star star = new(state.Name, state.StellarData);
            if (state.Attributes != null)
            {
                star.Attributes.CopyFrom(state.Attributes);
            }

            star.OrbitalData = state.OrbitalData;
            if (!string.IsNullOrEmpty(state.SpectralClass))
            {
                star.SpectralClass = new SpectralClass(state.SpectralClass);
            }

            if (state.Orbiters != null)
            {
                star.AddOrbiters(state.Orbiters);
            }

            return star;
        }

        public class StarState
        {
            public Dictionary<string, string> Attributes;
            public List<CelestialBody> Orbiters;
            public OrbitalData OrbitalData;
            public StellarData StellarData;
            public string Name, Id;
            public string SpectralClass;
        }
    }
}