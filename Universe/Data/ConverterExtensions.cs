// VindemiatrixCollective.Universe.Data © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System.Globalization;
using UnityEngine;

#endregion

namespace VindemiatrixCollective.Universe.Data
{
    public static class ConverterExtensions
    {
        public static Vector3 StringToVector3(string input, char delimiter = ',')
        {
            string[] array = input.Split(delimiter);
            return new Vector3(float.Parse(array[0], CultureInfo.InvariantCulture), float.Parse(array[1], CultureInfo.InvariantCulture),
                               float.Parse(array[2], CultureInfo.InvariantCulture));
        }
    }
}