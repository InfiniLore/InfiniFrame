// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>
///     An external host/window provider: supplies native windows, their native loop and the browser engine for an
///     InfiniFrame application. Selected explicitly, once, with <c>InfiniFrameApplicationBuilder.UseHostProvider</c>
///     before <c>Build()</c>. InfiniFrame keeps the public window facade, its lifecycle, managed dispatch, the Blazor
///     integration and resource dispatch; the provider never implements <see cref="IInfiniFrameWindow" />.
/// </summary>
public interface IInfiniFrameHostProvider {
    /// <summary>Diagnostic name only (logs and exception messages). Never used to select behaviour.</summary>
    string DisplayName { get; }

    /// <summary>Host contract version the provider was compiled against. See <see cref="InfiniFrameHostContract" />.</summary>
    Version ContractVersion { get; }

    /// <summary>
    ///     Optional operations and notifications the provider implements. InfiniFrame rejects window configuration that
    ///     needs an undeclared capability before any host or window is created.
    /// </summary>
    InfiniFrameHostCapabilities Capabilities { get; }

    /// <summary>
    ///     Creates the process-wide host. Called exactly once from <c>Build()</c>, after the contract version and
    ///     selection are validated. Must not start processes, threads or windows; that happens in
    ///     <see cref="IInfiniFrameHost.StartAsync" />. An exception fails <c>Build()</c>.
    /// </summary>
    IInfiniFrameHost CreateHost(InfiniFrameHostCreateContext context);
}
