// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Pure utility methods for parsing and validating hex color strings.
/// </summary>
public static class ColorUtility {

    /// <summary>
    ///     Validates whether a color string is a valid hex color format.
    /// </summary>
    /// <param name="color">The color string to validate (e.g. "#RRGGBB", "#AARRGGBB", null, or "transparent").</param>
    /// <returns><c>true</c> if the color is valid; otherwise <c>false</c>.</returns>
    public static bool IsValidBackgroundColor(string? color) {
        if (color is null or "transparent") return true;
        if (!color.StartsWith('#')) return false;

        string hex = color[1..];
        return hex.Length is 6 or 8 && hex.All(IsHexDigit);
    }

    /// <summary>
    ///     Parses a hex color string into its RGBA components.
    /// </summary>
    public static void ParseBackgroundColor(string? color, out byte r, out byte g, out byte b, out byte a) {
        if (color is null or "transparent") {
            r = g = b = a = 0;
            return;
        }

        string hex = color.StartsWith('#') ? color[1..] : color;

        if (hex.Length == 8) {
            a = (byte)(HexDigitValue(hex[0]) << 4 | HexDigitValue(hex[1]));
            r = (byte)(HexDigitValue(hex[2]) << 4 | HexDigitValue(hex[3]));
            g = (byte)(HexDigitValue(hex[4]) << 4 | HexDigitValue(hex[5]));
            b = (byte)(HexDigitValue(hex[6]) << 4 | HexDigitValue(hex[7]));
        } else {
            r = (byte)(HexDigitValue(hex[0]) << 4 | HexDigitValue(hex[1]));
            g = (byte)(HexDigitValue(hex[2]) << 4 | HexDigitValue(hex[3]));
            b = (byte)(HexDigitValue(hex[4]) << 4 | HexDigitValue(hex[5]));
            a = 255;
        }
    }

    internal static bool IsHexDigit(char c) =>
        c is >= '0' and <= '9' or >= 'A' and <= 'F' or >= 'a' and <= 'f';

    internal static int HexDigitValue(char c) =>
        c switch {
            >= '0' and <= '9' => c - '0',
            >= 'A' and <= 'F' => c - 'A' + 10,
            >= 'a' and <= 'f' => c - 'a' + 10,
            _ => -1
        };
}
