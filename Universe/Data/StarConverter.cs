using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnitsNet;
using UnityEngine;
using UnityEngine.Assertions;
using VindemiatrixCollective.Universe.CelestialMechanics.Orbits;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Data
{
    public class StarConverter : IConverterReader<Star>
    {
        public const string Planets = nameof(Planets);

        public Star Create(JObject jo)
        {
            return new Star();
        }

        public void Read(JObject jo, JsonReader reader, JsonSerializer serializer, ref Star star)
        {
            float? m   = (float?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(StellarData.Mass)}");
            float? l   = (float?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(StellarData.Luminosity)}");
            float? g   = (float?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(StellarData.Gravity)}");
            float? r   = (float?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(StellarData.Radius)}");
            float? t   = (float?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(StellarData.Temperature)}");
            float? age = (float?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(StellarData.Age)}");

            m   ??= (float?)jo.SelectToken($"{nameof(PhysicalData)}.m");
            l   ??= (float?)jo.SelectToken($"{nameof(PhysicalData)}.l");
            g   ??= (float?)jo.SelectToken($"{nameof(PhysicalData)}.g");
            r   ??= (float?)jo.SelectToken($"{nameof(PhysicalData)}.r");
            t   ??= (float?)jo.SelectToken($"{nameof(PhysicalData)}.t");
            age ??= (float?)jo.SelectToken($"{nameof(PhysicalData)}.age");

            star.SpectralClass = new SpectralClass(jo.Value<string>(nameof(SpectralClass)) ?? jo.Value<string>("SC"));

            string name = jo.Value<string>(nameof(Star.Name)) ?? reader.ParentNameFromContainer(nameof(StarSystem.Orbiters));
            if (!string.IsNullOrEmpty(name))
            {
                star.Name = name;
            }

            Assert.IsFalse(string.IsNullOrEmpty(star.Name));

            if (string.IsNullOrEmpty(star.Name))
            {
                Debug.LogWarning($"{nameof(Star)} name is empty.");
                star.Name = "Unknown";
            }

            try
            {
                Mass   mass   = Mass.FromSolarMasses(m ?? 0);
                Length radius = Length.FromSolarRadiuses(r ?? 0);
                Density density = r is > 0
                    ? Density.FromGramsPerCubicCentimeter(3 * mass.Grams / (4 * UniversalConstants.Tri.Pi * Math.Pow(radius.Centimeters, 3)))
                    : Density.Zero;

                star.PhysicalData = new StellarData(Luminosity.FromSolarLuminosities(l ?? 0),
                                                    mass,
                                                    Acceleration.FromCentimetersPerSecondSquared(g ?? 0),
                                                    radius,
                                                    Temperature.FromKelvins(t ?? 0),
                                                    Duration.FromYears365(age * 1E9 ?? 0),
                                                    density);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"m: {m}\nl: {l}\ng: {g}\nr: {r}\nt: {t}\nage: {age}\n{ex.Message}", ex);
            }

            if (jo.ContainsKey(nameof(OrbitalData)))
            {
                float? a    = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.SemiMajorAxis)}");
                float? e    = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.Eccentricity)}");
                float? P    = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.Period)}");
                float? i    = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.Inclination)}");
                float? lan  = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.LongitudeAscendingNode)}");
                float? argp = (float?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.ArgumentPeriapsis)}");

                a    ??= (float)jo.SelectToken($"{nameof(OrbitalData)}.a");
                e    ??= (float)jo.SelectToken($"{nameof(OrbitalData)}.e");
                P    ??= (float)jo.SelectToken($"{nameof(OrbitalData)}.P");
                i    ??= (float)jo.SelectToken($"{nameof(OrbitalData)}.i");
                lan  ??= (float)jo.SelectToken($"{nameof(OrbitalData)}.lan");
                argp ??= (float)jo.SelectToken($"{nameof(OrbitalData)}.argp");

                star.OrbitalData = new OrbitalData(a.Value * (float)UniversalConstants.Celestial.MetresPerAu,
                                                   e.Value, i.Value, lan.Value, argp.Value, P.Value * 365);
            }

            star.Attributes[nameof(Type)] = nameof(CelestialBodyType.Star);
            star.Attributes["Class"]      = nameof(CelestialBodyType.Star);

            JToken orbiters = jo[nameof(CelestialBody.Orbiters)];

            if (orbiters is { HasValues: true })
            {
                Dictionary<string, CelestialBody> planetDict = serializer.Deserialize<Dictionary<string, CelestialBody>>(orbiters.CreateReader());
                star.AddOrbiters(planetDict.Values);
            }

#if UNITY_EDITOR
            Tuple<string, float?>[] fields = { new(nameof(m), m), new(nameof(age), age), new(nameof(t), t), new(nameof(r), r), new(nameof(l), l) };

            if (ConverterOptions.VerboseLog)
            {
                foreach (Tuple<string, float?> f in fields)
                {
                    ConverterExtensions.CheckValue(f.Item1, f.Item2);
                }
            }
#endif
        }

        //public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        //{
        //    Star star = (Star)value;
        //    writer.WriteStartObject();
        //    writer.WritePropertyName(nameof(SpectralClass));
        //    writer.WriteValue(star.SpectralClass.Signature);
        //    writer.WritePropertyName(nameof(Luminosity));
        //    writer.WriteValue(star.Luminosity.SolarLuminosities);
        //    writer.WritePropertyName(nameof(Mass));
        //    writer.WriteValue(star.PhysicalData.Mass.SolarMasses);
        //    writer.WritePropertyName(nameof(Temperature));
        //    writer.WriteValue(star.Temperature.Kelvins);
        //    writer.WritePropertyName(nameof(Star.Age));
        //    writer.WriteValue(star.Age.Years365 / 1E9);
        //    writer.WritePropertyName(nameof(PhysicalData.Radius));
        //    writer.WriteValue(star.PhysicalData.Radius.SolarRadiuses);
        //    writer.WriteEndObject();
        //}
    }
}