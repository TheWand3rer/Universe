// com.vindemiatrixcollective.universe.data © 2025 Vindemiatrix Collective
// Website and Documentation: https://dev.vindemiatrixcollective.com

using UnitsNet;
using UnitsNet.Units;
using VindemiatrixCollective.Universe.Model;
using static VindemiatrixCollective.Universe.Data.StellarDataConverter;

namespace VindemiatrixCollective.Universe.Data
{
    public class StellarDataConverter : CoreObjectConverter<StellarData, StellarDataState>
    {
        public StellarDataConverter()
        {
            Converter = new ObjectBuilder<StellarData, StellarDataState>.Builder()
               .SetProperty(nameof(StellarData.Luminosity), Parse.ValueUnit, (state, value) => state.l = value, alternativeName: "l")
               .SetProperty(nameof(StellarData.Mass), Parse.ValueUnit, (state, value) => state.m = value, true, "m")
               .SetProperty(nameof(StellarData.Density), Parse.ValueUnit, (state, value) => state.d = value, true, "d")
               .SetProperty(nameof(StellarData.Gravity), Parse.ValueUnit, (state, value) => state.g = value, true, "g")
               .SetProperty(nameof(StellarData.Radius), Parse.ValueUnit, (state, value) => state.r = value, true, "r")
               .SetProperty(nameof(StellarData.Temperature), Parse.ValueUnit, (state, value) => state.t = value, true, "t")
               .SetProperty(nameof(StellarData.Age), Parse.ValueUnit, (state, value) => state.y = value, true, "age")
               .SetCreate(Creator)
               .Build();
        }

        private StellarData Creator(StellarDataState state)
        {
            if (state.l.u == "sl")
            {
                state.l = new ValueUnit(state.l.v, "L⊙");
            }

            if (state.m.u == "sm")
            {
                state.m = new ValueUnit(state.m.v, "M☉");
            }

            if (state.r.u == "sr")
            {
                state.r = new ValueUnit(state.r.v, "R⊙");
            }

            Luminosity   lum     = state.l.ToQuantity<Luminosity, LuminosityUnit>(LuminosityUnit.SolarLuminosity);
            Mass         mass    = state.m.ToQuantity<Mass, MassUnit>(MassUnit.SolarMass);
            Length       radius  = state.r.ToQuantity<Length, LengthUnit>(LengthUnit.SolarRadius);
            Acceleration gravity = state.g.ToQuantity<Acceleration, AccelerationUnit>(AccelerationUnit.MeterPerSecondSquared);
            Density      density = state.d.ToQuantity<Density, DensityUnit>(DensityUnit.GramPerCubicCentimeter);
            Temperature  temp    = state.t.ToQuantity<Temperature, TemperatureUnit>(TemperatureUnit.Kelvin);
            Duration     age     = (state.y * 1E9).ToQuantity<Duration, DurationUnit>(DurationUnit.Year365);


            return new StellarData(lum, mass, radius, gravity, temp, density, age);
        }

        public class StellarDataState
        {
            public ValueUnit l, m, d, r, g, t, y;
        }
    }
}