// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a notification activation event carrying the result and optional action identifier.
/// </summary>
/// <param name="Result">The outcome of the notification interaction.</param>
/// <param name="ActionIdentifier">
///     The identifier of the activated action button, or <c>null</c> when the user
///     clicked the notification body or the notification was dismissed.
/// </param>
public readonly record struct InfiniFrameNotificationActivation(InfiniFrameNotificationResult Result, string? ActionIdentifier = null);
