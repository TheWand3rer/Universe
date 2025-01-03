using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnitsNet;
using UnityEngine;
using VindemiatrixCollective.Universe.Model;

namespace VindemiatrixCollective.Universe.Data
{
    public class StarConverter : IConverterReader<Star>
    {
        public const string Planets = nameof(Planets);
        public void Read(JsonReader reader, JsonSerializer serializer, ref Star star)
        {
            JObject jo = JObject.Load(reader);

            float? massSM     = (float?)jo[nameof(Star.PhysicalData.Mass)];
            float? age        = (float?)jo[nameof(Star.Age)];
            float? temp       = (float?)jo[nameof(Star.Temperature)];
            float? radiusKm   = (float?)jo[nameof(PhysicalData.Radius)];
            float? luminosity = (float?)jo[nameof(Star.Luminosity)];
            //string coordinates = (string) jo[nameof(Star.Coordinates)];
            star.Name = reader.ParentNameFromContainer(nameof(StarSystem.Stars));

            if (string.IsNullOrEmpty(star.Name))
            {
                Debug.LogWarning($"{nameof(Star)} name is empty.");
                star.Name = "Unknown";
            }

            star.Age = Duration.FromYears365(age * 1E9 ?? 0);
            star.Temperature = Temperature.FromKelvins(temp ?? 0);
            star.SpectralClass = new SpectralClass((string)jo[nameof(Star.SpectralClass)]);
            star.Luminosity = Luminosity.FromSolarLuminosities(luminosity ?? 0);

            Mass    mass    = Mass.FromSolarMasses(massSM ?? 0);
            Length  radius  = Length.FromSolarRadiuses(radiusKm ?? 0);
            Density density = Density.FromGramsPerCubicCentimeter((3 * mass.Grams) / (4 * UniversalConstants.Tri.Pi * Math.Pow(radius.Centimeters, 3)));
            Acceleration gravity =
                Acceleration.FromMetersPerSecondSquared((UniversalConstants.Celestial.GravitationalConstant * mass.Kilograms) / Math.Pow(radius.Meters, 2));
            star.PhysicalData = PhysicalData.Create(mass, radius, gravity, density);

            star.Attributes[nameof(Type)] = CelestialBodyType.Star.ToString();

            JToken planets = jo[nameof(Planets)];

            if ((planets != null) && planets.HasValues)
            {
                star.AddPlanets(serializer.Deserialize<Dictionary<string, Planet>>(planets.CreateReader()).Values);
            }

#if UNITY_EDITOR
            var fields = new[]
            {
                new Tuple<string, float?>(nameof(massSM), massSM),
                new Tuple<string, float?>(nameof(age), age),
                new Tuple<string, float?>(nameof(temp), temp),
                new Tuple<string, float?>(nameof(radiusKm), radiusKm),
                new Tuple<string, float?>(nameof(luminosity), luminosity),
            };

            if (ConverterOptions.VerboseLog)
            {
                foreach (var f in fields)
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