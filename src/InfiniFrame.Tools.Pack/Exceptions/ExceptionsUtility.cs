// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Exceptions;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class ExceptionsUtility {
    public static bool IsNonFatalException(Exception exception)
        => exception is not (ApplicationException
            or OutOfMemoryException
            or AccessViolationException
            or StackOverflowException
            or BadImageFormatException
            or System.Runtime.InteropServices.SEHException);
}