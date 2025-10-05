// VindemiatrixCollective.Universe.Data © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System.Collections.Generic;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

#endregion

namespace VindemiatrixCollective.Universe.Data
{
    public class PlanetConverter : CoreObjectConverter<Planet, PlanetConverter.PlanetState>
    {
        public PlanetConverter()
        {
            Converter = new ObjectBuilder<Planet, PlanetState>.Builder()
               .SetProperty(nameof(CelestialBody.Name), Parse.String, (state, value) => state.Name                               = value)
               .SetProperty(nameof(CelestialBody.Attributes), Parse.Dictionary<string>, (state, value) => state.Attributes       = value)
               .SetProperty(nameof(CelestialBody.PhysicalData), Parse.Object<PhysicalData>, (state, value) => state.PhysicalData = value)
               .SetProperty(nameof(CelestialBody.OrbitalData), Parse.Object<OrbitalData>, (state, value) => state.OrbitalData    = value)
               .SetProperty(nameof(StarSystem.Orbiters), Parse.List<CelestialBody, CelestialBodyConverter>,
                            (state, value) => state.Orbiters = value, true)
               .SetCreate(Creator)
               .Build();
        }

        private Planet Creator(PlanetState state)
        {
            if (state.PhysicalData == null)
            {
                state.PhysicalData = PhysicalData.Null;
            }

            Planet planet = new(state.Name, state.PhysicalData, state.OrbitalData);

            if (state.Attributes != null)
            {
                planet.Attributes.CopyFrom(state.Attributes);
            }

            if (state.Orbiters != null)
            {
                planet.AddOrbiters(state.Orbiters);
            }

            return planet;
        }

        public class PlanetState
        {
            public Dictionary<string, string> Attributes;
            public List<CelestialBody> Orbiters;
            public OrbitalData OrbitalData;
            public PhysicalData PhysicalData;
            public string Name;
        }
    }
}