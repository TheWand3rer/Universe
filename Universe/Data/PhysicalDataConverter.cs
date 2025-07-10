// com.vindemiatrixcollective.universe.data © 2025 Vindemiatrix Collective
// Website and Documentation: https://dev.vindemiatrixcollective.com

#region

using UnitsNet;
using UnitsNet.Units;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.Model;
using static VindemiatrixCollective.Universe.Data.PhysicalDataConverter;

#endregion

namespace VindemiatrixCollective.Universe.Data
{
    public class PhysicalDataConverter : CoreObjectConverter<PhysicalData, PhysicalDataState>
    {
        public PhysicalDataConverter()
        {
            Converter = new ObjectBuilder<PhysicalData, PhysicalDataState>.Builder()
               .SetProperty(nameof(PhysicalData.Mass), Parse.ValueUnit, (state, value) => state.m = value, true, "m")
               .SetProperty(nameof(PhysicalData.Density), Parse.ValueUnit, (state, value) => state.d = value, true, "d")
               .SetProperty(nameof(PhysicalData.Gravity), Parse.ValueUnit, (state, value) => state.g = value, true, "g")
               .SetProperty(nameof(PhysicalData.Radius), Parse.ValueUnit, (state, value) => state.r = value, true, "r")
               .SetProperty(nameof(PhysicalData.Temperature), Parse.ValueUnit, (state, value) => state.t = value, true, "t")
               .SkipProperty("HillSphereRadius")
               .SetProperty(nameof(GravitationalParameter), Parse.Double, (state, value) => state.GmKm3S2 = value, true, "gm")
               .SetCreate(Creator)
               .Build();
        }

        private PhysicalData Creator(PhysicalDataState state)
        {
            Mass         mass    = state.m.ToQuantity<Mass, MassUnit>(MassUnit.Kilogram);
            Length       radius  = state.r.ToQuantity<Length, LengthUnit>(LengthUnit.Kilometer);
            Acceleration gravity = state.g.ToQuantity<Acceleration, AccelerationUnit>(AccelerationUnit.MeterPerSecondSquared);
            Density      density = state.d.ToQuantity<Density, DensityUnit>(DensityUnit.GramPerCubicCentimeter);
            Temperature  temp    = state.t.ToQuantity<Temperature, TemperatureUnit>(TemperatureUnit.Kelvin);

            GravitationalParameter gm = GravitationalParameter.FromKm3S2(state.GmKm3S2);

            PhysicalData data = state.m.v > 0
                ? new PhysicalData(mass, radius, gravity, density, temp)
                : new PhysicalData(density, radius, gm, temp);

            return data;
        }

        public class PhysicalDataState
        {
            public double GmKm3S2;

            public ValueUnit m, d, r, g, t;
        }
    }
}