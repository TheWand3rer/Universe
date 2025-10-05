// VindemiatrixCollective.Universe © 2025 Vindemiatrix Collective
// Website and Documentation: https://vindemiatrixcollective.com

#region

using System;
using System.Text.RegularExpressions;
using Unity.Properties;
using UnityEngine;

#endregion

namespace VindemiatrixCollective.Universe.Model
{
    public enum StarClass
    {
        Undefined,
        WhiteDwarf,
        SubDwarf,
        Dwarf,
        SubGiant,
        Giant,
        BrightGiant,
        SuperGiant,
        HyperGiant
    }

    public enum StarType
    {
        Undefined = 0,
        W = 1,
        O = 2,
        B = 3,
        A = 4,
        F = 5,
        G = 6,
        K = 7,
        M = 8,
        L = 9,
        T = 10
    }

    [Serializable]
    public struct SpectralClass
    {
        public int SubType { get; }

        public LuminosityClass Class { get; }

        public StarType Type1 { get; }

        [CreateProperty]
        public readonly string Description => $"{ColorFromStarType(Type1)} {FindStarClass(Class.ToString().SplitCamelCase())}";

        public string Extra { get; }
        [CreateProperty] public readonly string Signature => $"{Type1}{SubType}{Class}";

        public SpectralClass(
            StarType type = StarType.Undefined, int subType = 0, LuminosityClass luminosityClass = LuminosityClass.Undefined,
            string extra = null)
        {
            Type1   = type;
            SubType = subType;
            Class   = luminosityClass;
            Extra   = extra ?? string.Empty;
        }

        public SpectralClass(string spectralClass)
        {
            Type1   = StarType.Undefined;
            Class   = LuminosityClass.Undefined;
            SubType = 0;
            Extra   = null;

            Regex           regex   = new(@"(([OBAFGKM]+)([0-9]*)([IVX]*)(\w)*)[\/\-\+]*");
            MatchCollection matches = regex.Matches(spectralClass);

            if (matches.Count == 0)
            {
                Debug.LogWarning($"Invalid Spectral Class: {spectralClass}");
                return;
            }

            Match m = null;

            foreach (Match candidate in matches)
            {
                if (string.IsNullOrEmpty(candidate.Groups[3].Value))
                {
                    Debug.LogWarning($"Invalid Spectral Class: {spectralClass}");
                }
                else
                {
                    m = candidate;
                    break;
                }
            }

            if (m == null)
            {
                Debug.LogWarning($"Invalid Spectral Class: {spectralClass}");
                return;
            }

            char t = m.Groups[2].Value.Trim()[0];
            Type1 = FindStarType(t);
            string sN     = m.Groups[3].Value;
            bool   result = float.TryParse(sN, out float fN);
            SubType = result ? (int)fN : 0;
            string sClass = m.Groups[4].Value;
            Class = FindLuminosityClass(sClass);
            Extra = m.Groups[5].Value;
        }

        public readonly override string ToString() => Signature;

        public static SpectralClass Sol => new(StarType.G, 2, LuminosityClass.V);
        public static SpectralClass Undefined => new(StarType.Undefined);

        public static StarType FindStarType(char t)
        {
            StarType starType = t switch
            {
                'O' => StarType.O,
                'B' => StarType.B,
                'A' => StarType.A,
                'F' => StarType.F,
                'G' => StarType.G,
                'K' => StarType.K,
                'M' => StarType.M,
                _   => StarType.Undefined
            };

            return starType;
        }

        public static string ColorFromStarType(StarType starType)
        {
            return starType switch
            {
                StarType.W => "Wolf-Rayet",
                StarType.O => "Blue",
                StarType.B => "Blue-White",
                StarType.A => "White",
                StarType.F => "Yellow-White",
                StarType.G => "Yellow",
                StarType.K => "Orange",
                StarType.M => "Red",
                StarType.L => "Brown",
                StarType.T => "Cool Brown",
                _          => "Error"
            };
        }

        public static StarClass FindStarClass(string luminosityClass)
        {
            StarClass starClass = luminosityClass switch
            {
                "V"                       => StarClass.Dwarf,
                "IV"                      => StarClass.SubGiant,
                "III" or "IIIa" or "IIIb" => StarClass.Giant,
                "II"                      => StarClass.BrightGiant,
                "Ia" or "Ib" or "Iab"     => StarClass.SuperGiant,
                "Ia0"                     => StarClass.HyperGiant,
                _                         => StarClass.Undefined
            };

            return starClass;
        }


        public static LuminosityClass FindLuminosityClass(string luminosityClass)
        {
            LuminosityClass luminosity = luminosityClass switch
            {
                "V"                       => LuminosityClass.V,
                "IV"                      => LuminosityClass.IV,
                "III" or "IIIa" or "IIIb" => LuminosityClass.III,
                "II"                      => LuminosityClass.II,
                "Ia"                      => LuminosityClass.Ia,
                "Ib" or "Iab"             => LuminosityClass.Ib,
                _                         => LuminosityClass.V
            };

            return luminosity;
        }

        // Where 0 = Ia0, 1 = Ia, 2 = Ib, 3 = II, ...,  8 = VII
        public static LuminosityClass FindLuminosityClassByIndex(int index)
        {
            return index switch
            {
                0 => LuminosityClass.Ia0,
                1 => LuminosityClass.Ia,
                2 => LuminosityClass.Ib,
                3 => LuminosityClass.II,
                4 => LuminosityClass.III,
                5 => LuminosityClass.IV,
                6 => LuminosityClass.V,
                7 => LuminosityClass.VI,
                8 => LuminosityClass.VII,
                _ => LuminosityClass.Undefined
            };
        }
    }

    public static class SpectralClassExtensions
    {
        private static readonly Regex regex1 = new(@"(\P{Ll})(\P{Ll}\p{Ll})", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex regex2 = new(@"(\p{Ll})(\P{Ll})", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static string SplitCamelCase(this string str) => regex2.Replace(regex1.Replace(str, "$1 $2"), "$1 $2");
    }
}