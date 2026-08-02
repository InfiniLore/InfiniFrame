// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameUnhandledExceptionSource {
    /// <summary>
    ///     Registers a handler for unhandled exceptions.
    /// </summary>
    /// <param name="handler">The event handler to invoke when an unhandled exception occurs.</param>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, unregisters the handler.</returns>
    IDisposable Register(UnhandledExceptionEventHandler handler);
}