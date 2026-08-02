// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class ExitCodes {
    public const int Success = 0;
    public const int GenericFailure = 1;
    public const int NativeDependencyMissing = 2;
    public const int MissingMainOutput = 3;
    public const int UnexpectedOutputShape = 4;
}