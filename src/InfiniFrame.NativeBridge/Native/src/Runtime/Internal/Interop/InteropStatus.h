#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/// Status codes returned by native interop functions.
enum class InteropStatus : int {
    /// Operation completed successfully.
    Success = 0,
    /// A required argument was null or otherwise invalid.
    InvalidArgument = 22,
    /// An output parameter was null when it must not be.
    OutParameterSetToInvalidNull = 2001,
    /// The requested operation failed.
    OperationFailed = 14
};
