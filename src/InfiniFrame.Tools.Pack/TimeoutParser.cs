// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Globalization;

namespace InfiniFrame.Tools.Pack;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Pure parsing logic for timeout values in multiple formats.
///     Extracted from <see cref="CommandLine"/> for testability.
/// </summary>
public static class TimeoutParser {

    /// <summary>
    ///     Parses a timeout string in formats: plain seconds, unit suffix (90s/5m/2h), or TimeSpan format.
    /// </summary>
    public static TimeSpan Parse(string value) {
        if (string.IsNullOrWhiteSpace(value)) throw new FormatException("Timeout value cannot be empty.");

        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int seconds) && seconds > 0) {
            return TimeSpan.FromSeconds(seconds);
        }

        if (TryParseUnitTimeout(value, out TimeSpan unitTimeout)) return unitTimeout;
        if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out TimeSpan timeSpan) && timeSpan > TimeSpan.Zero) return timeSpan;

        throw new FormatException($"Invalid timeout value '{value}'. Use a positive value like '600', '90s', '5m', or '00:10:00'.");
    }

    /// <summary>
    ///     Tries to parse a timeout string with a unit suffix (s/m/h).
    /// </summary>
    public static bool TryParseUnitTimeout(string value, out TimeSpan timeout) {
        timeout = default;
        if (value.Length < 2) return false;

        char unit = char.ToLowerInvariant(value[^1]);
        string numberPart = value[..^1];
        if (!double.TryParse(numberPart, NumberStyles.Float, CultureInfo.InvariantCulture, out double quantity) || quantity <= 0) {
            return false;
        }

        timeout = unit switch {
            's' => TimeSpan.FromSeconds(quantity),
            'm' => TimeSpan.FromMinutes(quantity),
            'h' => TimeSpan.FromHours(quantity),
            _ => default
        };

        return timeout > TimeSpan.Zero;
    }

    /// <summary>
    ///     Validates that a timeout is within the allowed range.
    /// </summary>
    public static void Validate(TimeSpan timeout) {
        if (timeout <= TimeSpan.Zero) {
            throw new FormatException($"Timeout must be greater than zero. Received '{timeout}'.");
        }

        if (timeout > PublishOptions.MaxProcessTimeout) {
            throw new FormatException(
                $"Timeout '{timeout}' exceeds the maximum supported value of '{PublishOptions.MaxProcessTimeout}'.");
        }
    }
}
