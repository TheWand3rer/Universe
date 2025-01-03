using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnitsNet;
using VindemiatrixCollective.Universe.CelestialMechanics;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Data
{
    public class PlanetConverter : IConverterReader<Planet>
    {
        public const string Satellites = nameof(Satellites);

        public void Read(JsonReader reader, JsonSerializer serializer, ref Planet planet)
        {
            JObject jo = JObject.Load(reader);

            float? gravity = (float?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(PhysicalData.Gravity)}");
            float? density = (float?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(PhysicalData.Density)}");
            float? mass    = (float?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(PhysicalData.Mass)}");
            float? radius  = (float?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(PhysicalData.Radius)}");
            float? gmKm3S2 = (float)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(GravitationalParameter)}");

            float? semiMajorAxis      = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.SemiMajorAxis)}");
            float? eccentricity       = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.Eccentricity)}");
            float? orbitalPeriod      = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.Period)}");
            float? siderealRotation   = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.SiderealRotationPeriod)}");
            float? orbitalInclination = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.Inclination)}");
            float? axialTilt          = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.AxialTilt)}");
            float? ascendingNode      = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.LongitudeAscendingNode)}");
            float? argumentPeriapsis  = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.ArgumentPeriapsis)}");
            float? meanAnomaly        = (float?)jo.SelectToken($"{nameof(OrbitalData)}.MeanAnomaly");
            float? trueAnomaly        = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitState.TrueAnomaly)}");

            JToken attributesToken = jo[nameof(Attributes)];
            var attributes      = serializer.Deserialize<Dictionary<string, string>>(attributesToken.CreateReader());

            PhysicalData physical;
            if (mass > 0)
            {
                physical = PhysicalData.Create(Mass.FromKilograms(mass ?? 0),
                                               Length.FromKilometers(radius ?? 0),
                                               Acceleration.FromMetersPerSecondSquared(gravity ?? 0),
                                               Density.FromGramsPerCubicCentimeter(density ?? 0));
            }
            else
            {
                physical = PhysicalData.Create(Density.FromGramsPerCubicCentimeter(density ?? 0),
                                               Length.FromKilometers(radius ?? 0),
                                               GravitationalParameter.FromKm3S2(gmKm3S2 ?? 0));
            }

            planet.Attributes.CopyFrom(attributes);

            if (planet.Type == CelestialBodyType.Planet)
            {
                semiMajorAxis = semiMajorAxis.Value * (float)UniversalConstants.Celestial.MetresPerAu;
                planet.Name = reader.ParentNameFromContainer(nameof(StarConverter.Planets));
            }
            else
            {
                semiMajorAxis *= 1000;
                planet.Name = reader.ParentNameFromContainer(nameof(Satellites));
            }

            OrbitalData orbital = OrbitalData.From(semiMajorAxis.Value, orbitalPeriod.Value, eccentricity.Value, siderealRotation.Value,
                                                   orbitalInclination.Value, axialTilt, ascendingNode.Value, argumentPeriapsis.Value, meanAnomaly,
                                                   trueAnomaly.Value);

            if (bool.TryParse(planet.Attributes.TryGet("Retrograde"), out bool retrograde))
            {
                orbital.Retrograde = retrograde;
            }
            else
            {
                orbital.Retrograde = false;
            }

            planet.OrbitalData = orbital;
            planet.PhysicalData = physical;

            JToken moons = jo[nameof(Satellites)];

            if ((moons != null) && moons.HasValues)
            {
                var satellites = serializer.Deserialize<Dictionary<string, Planet>>(moons.CreateReader());
                
                planet.AddPlanets(satellites.Values);
            }

#if UNITY_EDITOR
            var requiredStringFields = new[]
            {
                new KeyValuePair<string, string>(nameof(planet.Name), planet.Name),
            };

            var fields = new[]
            {
                new KeyValuePair<string, float?>(nameof(semiMajorAxis), semiMajorAxis),
                new KeyValuePair<string, float?>(nameof(eccentricity), eccentricity),
                new KeyValuePair<string, float?>(nameof(orbitalPeriod), orbitalPeriod),
                new KeyValuePair<string, float?>(nameof(meanAnomaly), meanAnomaly),
                new KeyValuePair<string, float?>(nameof(gravity), gravity),
                new KeyValuePair<string, float?>(nameof(density), density),
                new KeyValuePair<string, float?>(nameof(mass), mass),
                new KeyValuePair<string, float?>(nameof(radius), radius),
                new KeyValuePair<string, float?>(nameof(orbitalPeriod), orbitalPeriod),
                new KeyValuePair<string, float?>(nameof(siderealRotation), siderealRotation),
                new KeyValuePair<string, float?>(nameof(orbitalInclination), orbitalInclination),
                new KeyValuePair<string, float?>(nameof(axialTilt), axialTilt),
                new KeyValuePair<string, float?>(nameof(ascendingNode), ascendingNode),
                new KeyValuePair<string, float?>(nameof(argumentPeriapsis), argumentPeriapsis),
            };


            if (ConverterOptions.VerboseLog)
            {
                foreach (var f in requiredStringFields)
                {
                    ConverterExtensions.CheckValue(f.Key, f.Value);
                }

                foreach (var f in fields)
                {
                    ConverterExtensions.CheckValue(f.Key, f.Value);
                }
            }
#endif
        }
    }
}