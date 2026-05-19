#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <algorithm>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
inline constexpr int MaxWindowDimension = 10000;
inline constexpr int MinWindowDimension = 50;
inline constexpr int DefaultWindowWidth = 800;
inline constexpr int DefaultWindowHeight = 600;

template <typename T>
[[nodiscard]] constexpr T clampDimension(T value, T minVal = MinWindowDimension, T maxVal = MaxWindowDimension) {
    return std::clamp(value, minVal, maxVal);
}
