// VindemiatrixCollective.Universe.Data © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System.Collections.Generic;
using System.Text.Json.Serialization;
using VindemiatrixCollective.Universe.Model;
using static VindemiatrixCollective.Universe.Data.GalaxyConverter;

#endregion

namespace VindemiatrixCollective.Universe.Data
{
    public class GalaxyConverter : CoreObjectConverter<Galaxy, GalaxyState>
    {
        public GalaxyConverter()
        {
            Converter = new ObjectBuilder<Galaxy, GalaxyState>.Builder()
               .SetProperty(nameof(CelestialBody.Name), Parse.String, (state, value) => state.Name                               = value)
               .SetProperty(nameof(Galaxy.Systems), Parse.List<StarSystem, StarSystemConverter>, (state, value) => state.Systems = value)
               .SetCreate(Creator)
               .Build();
        }

        private Galaxy Creator(GalaxyState state)
        {
            Galaxy galaxy = new(state.Name);
            galaxy.AddSystems(state.Systems);
            return galaxy;
        }

        public static JsonConverter[] Converters => new JsonConverter[]
        {
            new GalaxyConverter(), new StarSystemConverter(),
            new StarConverter(), new PlanetConverter(), new CelestialBodyConverter(),
            new PhysicalDataConverter(), new StellarDataConverter(), new OrbitalDataConverter()
        };

        public class GalaxyState
        {
            public List<StarSystem> Systems;
            public string Name;
        }
    }
}