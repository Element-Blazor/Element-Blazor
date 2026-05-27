using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Element
{
    public class ElementColor
    {
        private static readonly Regex CssFunctionRegex = new Regex(@"^(?<name>rgba?|hsla?|hsva?)\((?<args>.*)\)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public ElementColor()
            : this(0, 100, 100, 1)
        {
        }

        public ElementColor(double hue, double saturation, double value, double alpha = 1)
        {
            Hue = NormalizeHue(hue);
            Saturation = Clamp(saturation, 0, 100);
            Value = Clamp(value, 0, 100);
            Alpha = Clamp(alpha, 0, 1);
        }

        public double Hue { get; set; }

        public double Saturation { get; set; }

        public double Value { get; set; }

        public double Alpha { get; set; } = 1;

        internal ElementColorFormat SourceFormat { get; set; } = ElementColorFormat.Auto;

        public int Red => ToRgb().Red;

        public int Green => ToRgb().Green;

        public int Blue => ToRgb().Blue;

        public ElementColor Clone()
        {
            return new ElementColor(Hue, Saturation, Value, Alpha)
            {
                SourceFormat = SourceFormat
            };
        }

        public string ToCssString(ElementColorFormat format = ElementColorFormat.Auto, bool showAlpha = false)
        {
            var resolvedFormat = format == ElementColorFormat.Auto
                ? SourceFormat == ElementColorFormat.Auto ? (showAlpha ? ElementColorFormat.Rgb : ElementColorFormat.Hex) : SourceFormat
                : format;
            var includeAlpha = showAlpha && Alpha < 1;
            return resolvedFormat switch
            {
                ElementColorFormat.Rgb => ToRgbString(includeAlpha),
                ElementColorFormat.Hsl => ToHslString(includeAlpha),
                ElementColorFormat.Hsv => ToHsvString(includeAlpha),
                _ => ToHexString(includeAlpha)
            };
        }

        public string ToRgbString(bool includeAlpha = false)
        {
            var (red, green, blue) = ToRgb();
            return includeAlpha
                ? string.Format(CultureInfo.InvariantCulture, "rgba({0}, {1}, {2}, {3})", red, green, blue, FormatAlpha(Alpha))
                : string.Format(CultureInfo.InvariantCulture, "rgb({0}, {1}, {2})", red, green, blue);
        }

        public string ToHexString(bool includeAlpha = false)
        {
            var (red, green, blue) = ToRgb();
            var value = $"#{red:X2}{green:X2}{blue:X2}".ToLowerInvariant();
            if (!includeAlpha)
            {
                return value;
            }

            var alpha = (int)Math.Round(Alpha * 255, MidpointRounding.AwayFromZero);
            return $"{value}{alpha:X2}".ToLowerInvariant();
        }

        public string ToHslString(bool includeAlpha = false)
        {
            var (hue, saturation, lightness) = ToHsl();
            return includeAlpha
                ? string.Format(CultureInfo.InvariantCulture, "hsla({0}, {1}%, {2}%, {3})", Round(hue), Round(saturation), Round(lightness), FormatAlpha(Alpha))
                : string.Format(CultureInfo.InvariantCulture, "hsl({0}, {1}%, {2}%)", Round(hue), Round(saturation), Round(lightness));
        }

        public string ToHsvString(bool includeAlpha = false)
        {
            return includeAlpha
                ? string.Format(CultureInfo.InvariantCulture, "hsva({0}, {1}, {2}, {3})", Round(Hue), Round(Saturation), Round(Value), FormatAlpha(Alpha))
                : string.Format(CultureInfo.InvariantCulture, "hsv({0}, {1}, {2})", Round(Hue), Round(Saturation), Round(Value));
        }

        public string ToOpaqueRgbString()
        {
            var (red, green, blue) = ToRgb();
            return string.Format(CultureInfo.InvariantCulture, "rgb({0}, {1}, {2})", red, green, blue);
        }

        public string ToAlphaGradientColor()
        {
            var (red, green, blue) = ToRgb();
            return string.Format(CultureInfo.InvariantCulture, "rgb({0}, {1}, {2})", red, green, blue);
        }

        public bool ColorEquals(ElementColor other, bool compareAlpha = true)
        {
            if (other == null)
            {
                return false;
            }

            var first = ToRgb();
            var second = other.ToRgb();
            return first.Red == second.Red
                && first.Green == second.Green
                && first.Blue == second.Blue
                && (!compareAlpha || Math.Abs(Alpha - other.Alpha) < 0.01);
        }

        public static ElementColor FromRgb(int red, int green, int blue, double alpha = 1)
        {
            var r = Clamp(red, 0, 255) / 255;
            var g = Clamp(green, 0, 255) / 255;
            var b = Clamp(blue, 0, 255) / 255;
            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));
            var delta = max - min;
            var hue = 0d;

            if (delta > 0)
            {
                if (Math.Abs(max - r) < 0.0001)
                {
                    hue = 60 * (((g - b) / delta) % 6);
                }
                else if (Math.Abs(max - g) < 0.0001)
                {
                    hue = 60 * (((b - r) / delta) + 2);
                }
                else
                {
                    hue = 60 * (((r - g) / delta) + 4);
                }
            }

            return new ElementColor(hue < 0 ? hue + 360 : hue, max == 0 ? 0 : delta / max * 100, max * 100, alpha)
            {
                SourceFormat = ElementColorFormat.Rgb
            };
        }

        public static bool TryParse(string value, out ElementColor color)
        {
            color = null;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            value = value.Trim();
            if (TryParseHex(value, out color))
            {
                return true;
            }

            var match = CssFunctionRegex.Match(value);
            if (!match.Success)
            {
                return false;
            }

            var name = match.Groups["name"].Value.ToLowerInvariant();
            var args = SplitArguments(match.Groups["args"].Value);
            if ((name == "rgb" || name == "rgba") && TryParseRgb(args, name == "rgba", out color))
            {
                return true;
            }
            if ((name == "hsl" || name == "hsla") && TryParseHsl(args, name == "hsla", out color))
            {
                return true;
            }
            if ((name == "hsv" || name == "hsva") && TryParseHsv(args, name == "hsva", out color))
            {
                return true;
            }

            return false;
        }

        public static ElementColor Parse(string value)
        {
            if (TryParse(value, out var color))
            {
                return color;
            }

            throw new FormatException($"'{value}' is not a supported color value.");
        }

        internal static ElementColorFormat ParseFormat(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ElementColorFormat.Auto;
            }

            return value.Trim().ToLowerInvariant() switch
            {
                "hex" => ElementColorFormat.Hex,
                "rgb" => ElementColorFormat.Rgb,
                "hsl" => ElementColorFormat.Hsl,
                "hsv" => ElementColorFormat.Hsv,
                _ => ElementColorFormat.Auto
            };
        }

        private (int Red, int Green, int Blue) ToRgb()
        {
            var hue = NormalizeHue(Hue);
            var saturation = Clamp(Saturation, 0, 100) / 100;
            var value = Clamp(Value, 0, 100) / 100;
            var chroma = value * saturation;
            var x = chroma * (1 - Math.Abs((hue / 60) % 2 - 1));
            var m = value - chroma;
            var (r1, g1, b1) = hue switch
            {
                < 60 => (chroma, x, 0d),
                < 120 => (x, chroma, 0d),
                < 180 => (0d, chroma, x),
                < 240 => (0d, x, chroma),
                < 300 => (x, 0d, chroma),
                _ => (chroma, 0d, x)
            };

            return (ToByte(r1 + m), ToByte(g1 + m), ToByte(b1 + m));
        }

        private (double Hue, double Saturation, double Lightness) ToHsl()
        {
            var (red, green, blue) = ToRgb();
            var r = red / 255d;
            var g = green / 255d;
            var b = blue / 255d;
            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));
            var delta = max - min;
            var lightness = (max + min) / 2;
            var saturation = delta == 0 ? 0 : delta / (1 - Math.Abs(2 * lightness - 1));
            var hue = 0d;

            if (delta > 0)
            {
                if (Math.Abs(max - r) < 0.0001)
                {
                    hue = 60 * (((g - b) / delta) % 6);
                }
                else if (Math.Abs(max - g) < 0.0001)
                {
                    hue = 60 * (((b - r) / delta) + 2);
                }
                else
                {
                    hue = 60 * (((r - g) / delta) + 4);
                }
            }

            return (hue < 0 ? hue + 360 : hue, saturation * 100, lightness * 100);
        }

        private static bool TryParseHex(string value, out ElementColor color)
        {
            color = null;
            if (!value.StartsWith("#", StringComparison.Ordinal))
            {
                return false;
            }

            var hex = value.Substring(1);
            if (hex.Length == 3 || hex.Length == 4)
            {
                hex = string.Concat(hex.Select(c => new string(c, 2)));
            }
            if (hex.Length != 6 && hex.Length != 8)
            {
                return false;
            }
            if (!int.TryParse(hex.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var red)
                || !int.TryParse(hex.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var green)
                || !int.TryParse(hex.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var blue))
            {
                return false;
            }

            var alpha = 1d;
            if (hex.Length == 8)
            {
                if (!int.TryParse(hex.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var alphaByte))
                {
                    return false;
                }
                alpha = alphaByte / 255d;
            }

            color = FromRgb(red, green, blue, alpha);
            color.SourceFormat = ElementColorFormat.Hex;
            return true;
        }

        private static bool TryParseRgb(string[] args, bool hasAlpha, out ElementColor color)
        {
            color = null;
            if (args.Length < 3 || (hasAlpha && args.Length < 4))
            {
                return false;
            }

            if (!TryParseRgbChannel(args[0], out var red)
                || !TryParseRgbChannel(args[1], out var green)
                || !TryParseRgbChannel(args[2], out var blue))
            {
                return false;
            }

            var alpha = 1d;
            if (hasAlpha && !TryParseAlpha(args[3], out alpha))
            {
                return false;
            }

            color = FromRgb(red, green, blue, alpha);
            color.SourceFormat = ElementColorFormat.Rgb;
            return true;
        }

        private static bool TryParseHsl(string[] args, bool hasAlpha, out ElementColor color)
        {
            color = null;
            if (args.Length < 3 || (hasAlpha && args.Length < 4))
            {
                return false;
            }

            if (!TryParseDouble(args[0], out var hue)
                || !TryParsePercent(args[1], out var saturation)
                || !TryParsePercent(args[2], out var lightness))
            {
                return false;
            }

            var alpha = 1d;
            if (hasAlpha && !TryParseAlpha(args[3], out alpha))
            {
                return false;
            }

            color = FromHsl(hue, saturation, lightness, alpha);
            color.SourceFormat = ElementColorFormat.Hsl;
            return true;
        }

        private static bool TryParseHsv(string[] args, bool hasAlpha, out ElementColor color)
        {
            color = null;
            if (args.Length < 3 || (hasAlpha && args.Length < 4))
            {
                return false;
            }

            if (!TryParseDouble(args[0], out var hue)
                || !TryParsePercent(args[1], out var saturation)
                || !TryParsePercent(args[2], out var value))
            {
                return false;
            }

            var alpha = 1d;
            if (hasAlpha && !TryParseAlpha(args[3], out alpha))
            {
                return false;
            }

            color = new ElementColor(hue, saturation, value, alpha)
            {
                SourceFormat = ElementColorFormat.Hsv
            };
            return true;
        }

        private static ElementColor FromHsl(double hue, double saturation, double lightness, double alpha)
        {
            hue = NormalizeHue(hue);
            saturation = Clamp(saturation, 0, 100) / 100;
            lightness = Clamp(lightness, 0, 100) / 100;
            var chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            var x = chroma * (1 - Math.Abs((hue / 60) % 2 - 1));
            var m = lightness - chroma / 2;
            var (r1, g1, b1) = hue switch
            {
                < 60 => (chroma, x, 0d),
                < 120 => (x, chroma, 0d),
                < 180 => (0d, chroma, x),
                < 240 => (0d, x, chroma),
                < 300 => (x, 0d, chroma),
                _ => (chroma, 0d, x)
            };

            return FromRgb(ToByte(r1 + m), ToByte(g1 + m), ToByte(b1 + m), alpha);
        }

        private static string[] SplitArguments(string value)
        {
            return value.Replace("/", " ", StringComparison.Ordinal)
                .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
        }

        private static bool TryParseRgbChannel(string value, out int result)
        {
            result = 0;
            if (!TryParsePercentOrNumber(value, 255, out var channel))
            {
                return false;
            }

            result = (int)Math.Round(Clamp(channel, 0, 255), MidpointRounding.AwayFromZero);
            return true;
        }

        private static bool TryParseAlpha(string value, out double result)
        {
            result = 1;
            return TryParsePercentOrNumber(value, 1, out result) && result >= 0 && result <= 1;
        }

        private static bool TryParsePercent(string value, out double result)
        {
            return TryParsePercentOrNumber(value, 100, out result);
        }

        private static bool TryParsePercentOrNumber(string value, double percentScale, out double result)
        {
            value = value.Trim();
            if (value.EndsWith("%", StringComparison.Ordinal))
            {
                if (!TryParseDouble(value.Substring(0, value.Length - 1), out result))
                {
                    return false;
                }
                result = result / 100 * percentScale;
                return true;
            }

            return TryParseDouble(value, out result);
        }

        private static bool TryParseDouble(string value, out double result)
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result)
                || double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out result);
        }

        private static int ToByte(double value)
        {
            return (int)Math.Round(Clamp(value, 0, 1) * 255, MidpointRounding.AwayFromZero);
        }

        private static double NormalizeHue(double hue)
        {
            hue %= 360;
            return hue < 0 ? hue + 360 : hue;
        }

        private static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        private static double Round(double value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        private static string FormatAlpha(double alpha)
        {
            return Math.Round(alpha, 3, MidpointRounding.AwayFromZero).ToString("0.###", CultureInfo.InvariantCulture);
        }
    }
}
