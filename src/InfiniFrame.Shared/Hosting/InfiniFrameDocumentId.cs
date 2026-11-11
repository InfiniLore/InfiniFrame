// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>
///     Browser-derived identity of one main-frame document in one window. Assigned by the provider: non-zero and
///     strictly increasing per window, so a later document always has a greater id.
/// </summary>
/// <param name="Value">The provider-assigned id.</param>
public readonly record struct InfiniFrameDocumentId(ulong Value);
