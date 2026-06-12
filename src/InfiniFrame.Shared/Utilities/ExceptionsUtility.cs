// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Utilities;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class ExceptionsUtility {
    public static bool IsNonFatalException(Exception exception)
        => exception is not (ApplicationException or OutOfMemoryException or AccessViolationException);
}
