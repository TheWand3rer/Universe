// VindemiatrixCollective.Universe.Data © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using UnitsNet;

#endregion

namespace VindemiatrixCollective.Universe.Data
{
    public readonly struct ValueUnit
    {
        public readonly double v;
        public readonly string u;

        public ValueUnit(double v, string u)
        {
            this.v = v;
            this.u = u;
        }

        public T ToQuantity<T, TEnum>(TEnum defaultUnit) where T : IQuantity where TEnum : Enum
        {
            TEnum unitType = string.IsNullOrEmpty(u) ? defaultUnit : UnitParser.Default.Parse<TEnum>(u);
            return (T)Quantity.From(v, unitType);
        }

        public static ValueUnit Null => new(0, string.Empty);

        public static ValueUnit operator *(ValueUnit a, ValueUnit b)
        {
            if (a.u != b.u)
            {
                throw new ArgumentException($"Values must have the same unit: {a.u} != {b.u}");
            }

            return new ValueUnit(a.v * b.v, a.u);
        }

        public static ValueUnit operator *(ValueUnit a, double b) => new(a.v * b, a.u);
    }
}