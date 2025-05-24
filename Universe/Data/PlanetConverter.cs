using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnitsNet;
using UnitsNet.Units;
using UnityEngine.Assertions;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Data
{
    public struct ValueUnit
    {
        public string u;
        public double v;
    }

    public class PlanetConverter : IConverterReader<Planet>
    {
        public const string Orbiters = nameof(Orbiters);

        public Planet Create(JObject jo)
        {
            return new Planet();
        }

        public void Read(JObject jo, JsonReader reader, JsonSerializer serializer, ref Planet planet)
        {
            double? gravity = (double?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(PhysicalData.Gravity)}");
            double? density = (double?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(PhysicalData.Density)}");
            double? mass    = (double?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(PhysicalData.Mass)}");
            double? radius  = (double?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(PhysicalData.Radius)}");
            double? gmKm3S2 = (double)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(GravitationalParameter)}");

            JToken semiMajorAxisToken = jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.SemiMajorAxis)}");
            semiMajorAxisToken ??= jo.SelectToken($"{nameof(OrbitalData)}.a");

            double? eccentricity       = (double?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.Eccentricity)}");
            double? orbitalPeriod      = (double?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.Period)}");
            double? siderealRotation   = (double?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.SiderealRotationPeriod)}");
            double? orbitalInclination = (double?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.Inclination)}");
            double? axialTilt          = (double?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.AxialTilt)}");
            double? ascendingNode      = (double?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.LongitudeAscendingNode)}");
            double? argumentPeriapsis  = (double?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.ArgumentPeriapsis)}");
            double? meanAnomaly        = (double?)jo.SelectToken($"{nameof(OrbitalData)}.MeanAnomaly");
            double? trueAnomaly        = (double?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitState.TrueAnomaly)}");

            JToken                     attributesToken = jo[nameof(Attributes)];
            Dictionary<string, string> attributes      = serializer.Deserialize<Dictionary<string, string>>(attributesToken.CreateReader());

            planet.Name = reader.ParentNameFromContainer(nameof(CelestialBody.Orbiters));
            Assert.IsFalse(string.IsNullOrEmpty(planet.Name));

            PhysicalData physical;
            if (mass > 0)
            {
                physical = new PhysicalData(Mass.FromKilograms(mass ?? 0),
                                            Length.FromKilometers(radius ?? 0),
                                            Acceleration.FromMetersPerSecondSquared(gravity ?? 0),
                                            Density.FromGramsPerCubicCentimeter(density ?? 0));
            }
            else
            {
                physical = new PhysicalData(Density.FromGramsPerCubicCentimeter(density ?? 0),
                                            Length.FromKilometers(radius ?? 0),
                                            GravitationalParameter.FromKm3S2(gmKm3S2 ?? 0));
            }

            planet.Attributes.CopyFrom(attributes);

            double semiMajorAxis;

            if (semiMajorAxisToken.HasValues)
            {
                ValueUnit a = serializer.Deserialize<ValueUnit>(semiMajorAxisToken.CreateReader());
                IQuantity l = Quantity.FromUnitAbbreviation(a.v, a.u);
                semiMajorAxis = (float)l.ToUnit(LengthUnit.Meter).Value;
            }
            else
            {
                semiMajorAxis = semiMajorAxisToken.Value<double>() * UniversalConstants.Celestial.MetresPerAu;
            }

            OrbitalData orbital = new(Length.FromMeters(semiMajorAxis),
                                      Ratio.FromDecimalFractions(eccentricity.Value),
                                      Angle.FromDegrees(orbitalInclination.Value),
                                      Angle.FromDegrees(ascendingNode.Value),
                                      Angle.FromDegrees(argumentPeriapsis.Value),
                                      Duration.FromSeconds(orbitalPeriod.Value * UniversalConstants.Time.SecondsPerDay),
                                      Duration.FromSeconds(siderealRotation.Value * UniversalConstants.Time.SecondsPerHour),
                                      Angle.FromDegrees(axialTilt ?? 0),
                                      Angle.FromDegrees(trueAnomaly.Value),
                                      Angle.FromDegrees(meanAnomaly.Value));

            if (bool.TryParse(planet.Attributes.TryGet("Retrograde"), out bool retrograde))
            {
                orbital.Retrograde = retrograde;
            }
            else
            {
                orbital.Retrograde = false;
            }

            planet.OrbitalData  = orbital;
            planet.PhysicalData = physical;

            JToken moons = jo[nameof(Orbiters)];

            if (moons is { HasValues: true })
            {
                Dictionary<string, Planet> orbiters = serializer.Deserialize<Dictionary<string, Planet>>(moons.CreateReader());

                planet.AddOrbiters(orbiters.Values);
            }

#if UNITY_EDITOR
            KeyValuePair<string, string>[] requiredStringFields = { new(nameof(planet.Name), planet.Name) };

            KeyValuePair<string, double?>[] fields =
            {
                new(nameof(semiMajorAxis), semiMajorAxis), new(nameof(eccentricity), eccentricity), new(nameof(orbitalPeriod), orbitalPeriod),
                new(nameof(meanAnomaly), meanAnomaly), new(nameof(gravity), gravity), new(nameof(density), density), new(nameof(mass), mass),
                new(nameof(radius), radius), new(nameof(orbitalPeriod), orbitalPeriod), new(nameof(siderealRotation), siderealRotation),
                new(nameof(orbitalInclination), orbitalInclination), new(nameof(axialTilt), axialTilt), new(nameof(ascendingNode), ascendingNode),
                new(nameof(argumentPeriapsis), argumentPeriapsis)
            };


            if (ConverterOptions.VerboseLog)
            {
                foreach (KeyValuePair<string, string> f in requiredStringFields)
                {
                    ConverterExtensions.CheckValue(f.Key, f.Value);
                }

                foreach (KeyValuePair<string, double?> f in fields)
                {
                    ConverterExtensions.CheckValue(f.Key, f.Value);
                }
            }
#endif
        }
    }
}