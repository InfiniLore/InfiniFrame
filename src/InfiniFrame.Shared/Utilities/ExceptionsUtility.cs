// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Utilities;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides utility methods for working with exceptions.
/// </summary>
internal static class ExceptionsUtility {
    /// <summary>
    ///     Determines whether the specified exception is considered non-fatal.
    /// </summary>
    /// <param name="exception">The exception to evaluate.</param>
    /// <returns><c>true</c> if the exception is non-fatal; otherwise, <c>false</c>.</returns>
    public static bool IsNonFatalException(Exception exception)
        => exception is not (ApplicationException 
            or OutOfMemoryException 
            or AccessViolationException 
            or StackOverflowException
        );
}
