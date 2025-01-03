using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace VindemiatrixCollective.Universe.Model
{
    public enum StarClass
    {
        Unknown,
        WhiteDwarf,
        SubDwarf,
        Dwarf,
        SubGiant,
        Giant,
        BrightGiant,
        SuperGiant,
    }

    public enum StarType
    {
        Unknown = -1,
        W = 0,
        O = 1,
        B = 2,
        A = 3,
        F = 4,
        G = 5,
        K = 6,
        M = 7,
        L = 8,
        T = 9
    }

    [Serializable]
    public struct SpectralClass
    {
        public StarType Type;

        public int SubType;

        public LuminosityClass LuminosityClass;
        public string Extra;
        public string Signature => $"{Type}{SubType}{LuminosityClass}";

        public readonly string Description => $"{ColorFromStarType(Type)} {FindStarClass(LuminosityClass.ToString().SplitCamelCase())}";

        public SpectralClass(StarType type = StarType.Unknown,
                             int subType = 0,
                             LuminosityClass luminosityClass = LuminosityClass.Undefined,
                             string extra = default(string))
        {
            Type = type;
            SubType = subType;
            LuminosityClass = luminosityClass;
            Extra = extra ?? string.Empty;
        }

        public static SpectralClass Sol => new(StarType.G, 2, LuminosityClass.V);
        public static SpectralClass Undefined => new(StarType.Unknown);

        public override string ToString()
        {
            return Signature;
        }

        public SpectralClass(string spectralClass)
        {
            Type = StarType.Unknown;
            LuminosityClass = LuminosityClass.Undefined;
            SubType = 0;
            Extra = default(string);

            Regex           regex   = new Regex(@"(([OBAFGKM]+)([0-9]*)([IVX]*)(\w)*)[\/\-\+]*");
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
            Type = FindStarType(t);
            string sN     = m.Groups[3].Value;
            bool   result = float.TryParse(sN, out float fN);
            SubType = result ? (int)fN : 0;
            string sClass = m.Groups[4].Value;
            LuminosityClass = FindLuminosityClass(sClass);
            Extra = m.Groups[5].Value;
        }

        public static StarType FindStarType(char t)
        {
            StarType starType;
            switch (t)
            {
                case 'O':
                    starType = StarType.O;
                    break;

                case 'B':
                    starType = StarType.B;
                    break;

                case 'A':
                    starType = StarType.A;
                    break;

                case 'F':
                    starType = StarType.F;
                    break;

                case 'G':
                    starType = StarType.G;
                    break;

                case 'K':
                    starType = StarType.K;
                    break;

                case 'M':
                    starType = StarType.M;
                    break;

                default:
                    starType = StarType.Unknown;
                    break;
            }

            return starType;
        }

        public static string ColorFromStarType(StarType starType)
        {
            switch (starType)
            {
                case StarType.W:
                    return "Wolf-Rayet";
                case StarType.O:
                    return "Blue";
                case StarType.B:
                    return "Blue-White";
                case StarType.A:
                    return "White";
                case StarType.F:
                    return "Yellow-White";
                case StarType.G:
                    return "Yellow";
                case StarType.K:
                    return "Orange";
                case StarType.M:
                    return "Red";
                case StarType.L:
                    return "Brown";
                case StarType.T:
                    return "Cool Brown";
                default:
                    return "Error";
            }
        }

        public static StarClass FindStarClass(string luminosityClass)
        {
            StarClass starClass;
            switch (luminosityClass)
            {
                case "V":
                    starClass = StarClass.Dwarf;
                    break;

                case "IV":
                    starClass = StarClass.SubGiant;
                    break;

                case "III":
                case "IIIa":
                case "IIIb":
                    starClass = StarClass.Giant;
                    break;

                case "II":
                    starClass = StarClass.BrightGiant;
                    break;

                case "Ia":
                case "Ib":
                case "Iab":
                    starClass = StarClass.SuperGiant;
                    break;

                default:
                    starClass = StarClass.Unknown;
                    break;
            }

            return starClass;
        }


        public static LuminosityClass FindLuminosityClass(string luminosityClass)
        {
            LuminosityClass luminosity;
            switch (luminosityClass)
            {
                default:
                case "V":
                    luminosity = LuminosityClass.V;
                    break;

                case "IV":
                    luminosity = LuminosityClass.IV;
                    break;

                case "III":
                case "IIIa":
                case "IIIb":
                    luminosity = LuminosityClass.III;
                    break;

                case "II":
                    luminosity = LuminosityClass.II;
                    break;

                case "Ia":
                    luminosity = LuminosityClass.Ia;
                    break;

                case "Ib":
                case "Iab":
                    luminosity = LuminosityClass.Ib;
                    break;
            }

            return luminosity;
        }

        // Where 0 = Ia0, 1 = Ia, 2 = Ib, 3 = II, ...,  8 = VII
        public static LuminosityClass FindLuminosityClassByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return LuminosityClass.Ia0;
                case 1:
                    return LuminosityClass.Ia;
                case 2:
                    return LuminosityClass.Ib;
                case 3:
                    return LuminosityClass.II;
                case 4:
                    return LuminosityClass.III;
                case 5:
                    return LuminosityClass.IV;
                case 6:
                    return LuminosityClass.V;
                case 7:
                    return LuminosityClass.VI;
                case 8:
                    return LuminosityClass.VII;
                default:
                    return LuminosityClass.Undefined;
            }
        }
    }

    public static class SpectralClassExtensions
    {
        private static readonly Regex regex1 = new(@"(\P{Ll})(\P{Ll}\p{Ll})", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex regex2 = new(@"(\p{Ll})(\P{Ll})", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static string SplitCamelCase(this string str)
        {
            return regex2.Replace(regex1.Replace(str, "$1 $2"), "$1 $2");
        }
    }
}