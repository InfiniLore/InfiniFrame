// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents an action button on a notification.
/// </summary>
/// <param name="Label">The visible label displayed on the action button.</param>
/// <param name="Identifier">A unique identifier used to identify which action was activated.</param>
public readonly record struct InfiniFrameNotificationAction(string Label, string Identifier);
