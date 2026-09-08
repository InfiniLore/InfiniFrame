#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <algorithm>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/// Maximum allowed window dimension in pixels.
inline constexpr int MaxWindowDimension = 10000;
/// Minimum allowed window dimension in pixels.
inline constexpr int MinWindowDimension = 50;
/// Default initial window width in pixels.
inline constexpr int DefaultWindowWidth = 800;
/// Default initial window height in pixels.
inline constexpr int DefaultWindowHeight = 600;

/// Clamp a window dimension value to the range [minVal, maxVal].
/// @param value The dimension value to clamp.
/// @param minVal Minimum allowed value (defaults to MinWindowDimension).
/// @param maxVal Maximum allowed value (defaults to MaxWindowDimension).
/// @return The clamped value.
template <typename T>
[[nodiscard]] constexpr T clampDimension(T value, T minVal = MinWindowDimension, T maxVal = MaxWindowDimension) {
    return std::clamp(value, minVal, maxVal);
}
