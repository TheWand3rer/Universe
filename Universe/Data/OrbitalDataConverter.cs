// VindemiatrixCollective.Universe.Data © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using UnitsNet;
using UnitsNet.Units;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;

#endregion

namespace VindemiatrixCollective.Universe.Data
{
    public class OrbitalDataConverter : CoreObjectConverter<OrbitalData, OrbitalDataConverter.OrbitalDataState>
    {
        public OrbitalDataConverter()
        {
            Converter = new ObjectBuilder<OrbitalData, OrbitalDataState>.Builder()
               .SetProperty(nameof(OrbitalData.SemiMajorAxis), Parse.ValueUnit, (state, value) => state.a = value, alternativeName: "a")
               .SetProperty(nameof(OrbitalData.Eccentricity), Parse.ValueUnit, (state, value) => state.e  = value, alternativeName: "e")
               .SetProperty(nameof(OrbitalData.Inclination), Parse.ValueUnit, (state, value) => state.i   = value, alternativeName: "i")
               .SetProperty(nameof(OrbitalData.LongitudeAscendingNode), Parse.ValueUnit, (state, value) => state.lan = value,
                            alternativeName: "lan")
               .SetProperty(nameof(OrbitalData.ArgumentPeriapsis), Parse.ValueUnit, (state, value) => state.argp = value,
                            alternativeName: "argp")
               .SetProperty(nameof(OrbitalData.TrueAnomaly), Parse.ValueUnit, (state, value) => state.ta = value, alternativeName: "ta")
               .SetProperty(nameof(OrbitalData.MeanAnomaly), Parse.ValueUnit, (state, value) => state.ma = value, true, "ma")
               .SetProperty(nameof(OrbitalData.Period), Parse.ValueUnit, (state, value) => state.p = value, true, "p")
               .SetProperty(nameof(OrbitalData.AxialTilt), Parse.ValueUnit, (state, value) => state.tilt = value, true, "tilt")
               .SetProperty(nameof(OrbitalData.SiderealRotationPeriod), Parse.ValueUnit, (state, value) => state.srp = value, true, "srp")
               .SetCreate(Creator)
               .Build();
        }

        private OrbitalData Creator(OrbitalDataState state)
        {
            Length   sma      = state.a.ToQuantity<Length, LengthUnit>(LengthUnit.AstronomicalUnit);
            Ratio    ecc      = state.e.ToQuantity<Ratio, RatioUnit>(RatioUnit.DecimalFraction);
            Angle    inc      = state.i.ToQuantity<Angle, AngleUnit>(AngleUnit.Degree);
            Duration period   = state.p.ToQuantity<Duration, DurationUnit>(DurationUnit.Day);
            Angle    laNode   = state.lan.ToQuantity<Angle, AngleUnit>(AngleUnit.Degree);
            Angle    argPer   = state.argp.ToQuantity<Angle, AngleUnit>(AngleUnit.Degree);
            Angle    trueAn   = state.ta.ToQuantity<Angle, AngleUnit>(AngleUnit.Degree);
            Angle    meanAn   = state.ma.ToQuantity<Angle, AngleUnit>(AngleUnit.Degree);
            Angle    axial    = state.tilt.ToQuantity<Angle, AngleUnit>(AngleUnit.Degree);
            Duration sidereal = state.srp.ToQuantity<Duration, DurationUnit>(DurationUnit.Hour);


            return new OrbitalData(sma, ecc, inc, laNode, argPer, trueAn, period, sidereal,
                                   axial, meanAn);
        }

        public class OrbitalDataState
        {
            public ValueUnit a, e, p, i, lan, argp, ma, ta, tilt, srp;
        }
    }
}