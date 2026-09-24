// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
/// Represents a marker type used to indicate the presence of Blazor WebView integration
/// within an InfiniFrame application. The primary purpose of this class is to act as a
/// service registration marker, enabling runtime validation to ensure that only one
/// Blazor WebView instance is registered per DI container.
/// </summary>
internal sealed class BlazorWebViewMarker;
