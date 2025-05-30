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
            double? m   = (double?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(StellarData.Mass)}");
            double? l   = (double?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(StellarData.Luminosity)}");
            double? g   = (double?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(StellarData.Gravity)}");
            double? r   = (double?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(StellarData.Radius)}");
            double? t   = (double?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(StellarData.Temperature)}");
            double? age = (double?)jo.SelectToken($"{nameof(PhysicalData)}.{nameof(StellarData.Age)}");

            m   ??= (double?)jo.SelectToken($"{nameof(PhysicalData)}.m");
            l   ??= (double?)jo.SelectToken($"{nameof(PhysicalData)}.l");
            g   ??= (double?)jo.SelectToken($"{nameof(PhysicalData)}.g");
            r   ??= (double?)jo.SelectToken($"{nameof(PhysicalData)}.r");
            t   ??= (double?)jo.SelectToken($"{nameof(PhysicalData)}.t");
            age ??= (double?)jo.SelectToken($"{nameof(PhysicalData)}.age");

            string sc = jo.Value<string>(nameof(SpectralClass)) ?? jo.Value<string>("SC");
            if (string.IsNullOrEmpty(sc))
            {
                star.SpectralClass = SpectralClass.Undefined;
                Debug.LogWarning($"{reader.Path} SC is empty.");
            }
            else
            {
                star.SpectralClass = new SpectralClass(sc);
            }

            string name = jo.Value<string>(nameof(Star.Name)) ?? reader.ParentNameFromContainer(nameof(StarSystem.Orbiters));
            if (!string.IsNullOrEmpty(name))
            {
                star.Name = name;
            }

            Assert.IsFalse(string.IsNullOrEmpty(star.Name));

            if (string.IsNullOrEmpty(star.Name))
            {
                Debug.LogWarning($"{reader.Path} name is empty.");
                star.Name = "Unknown";
            }

            try
            {
                Mass   mass   = Mass.FromSolarMasses(m ?? 0);
                Length radius = Length.FromSolarRadiuses(r ?? 0);
                Density density = r is > 0
                    ? Density.FromGramsPerCubicCentimeter(3 * mass.Grams / (4 * UniversalConstants.Tri.Pi * Math.Pow(radius.Centimeters, 3)))
                    : Density.Zero;

                star.StellarData = new StellarData(Luminosity.FromSolarLuminosities(l ?? 0),
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
                double? a    = (double?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.SemiMajorAxis)}");
                double? e    = (double?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.Eccentricity)}");
                double? P    = (double?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.Period)}");
                double? i    = (double?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.Inclination)}");
                double? lan  = (double?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.LongitudeAscendingNode)}");
                double? argp = (double?)jo.SelectToken($"{nameof(OrbitalData)}.{nameof(OrbitalData.ArgumentPeriapsis)}");

                a    ??= (double?)jo.SelectToken($"{nameof(OrbitalData)}.a");
                e    ??= (double?)jo.SelectToken($"{nameof(OrbitalData)}.e");
                P    ??= (double?)jo.SelectToken($"{nameof(OrbitalData)}.P");
                i    ??= (double?)jo.SelectToken($"{nameof(OrbitalData)}.i");
                lan  ??= (double?)jo.SelectToken($"{nameof(OrbitalData)}.lan");
                argp ??= (double?)jo.SelectToken($"{nameof(OrbitalData)}.argp");

                star.OrbitalData = new OrbitalData(Length.FromAstronomicalUnits(a.Value),
                                                   Ratio.FromDecimalFractions(e.Value),
                                                   Angle.FromDegrees(i.Value),
                                                   Angle.FromDegrees(lan.Value),
                                                   Angle.FromDegrees(argp.Value),
                                                   Duration.FromSeconds(P.Value * UniversalConstants.Time.SecondsPerJulianYear),
                                                   Duration.Zero, Angle.Zero, Angle.Zero, Angle.Zero);
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
            Tuple<string, double?>[] fields = { new(nameof(m), m), new(nameof(age), age), new(nameof(t), t), new(nameof(r), r), new(nameof(l), l) };

            if (ConverterOptions.VerboseLog)
            {
                foreach (Tuple<string, double?> f in fields)
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