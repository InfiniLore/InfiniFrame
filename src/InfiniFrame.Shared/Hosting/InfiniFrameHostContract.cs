// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>Version information for the external host/window provider contract.</summary>
/// <remarks>
///     A provider reports the version it was compiled against through
///     <see cref="IInfiniFrameHostProvider.ContractVersion" />. Use the constants (not <see cref="Version" />) so the
///     value is fixed at compile time: <c>new Version(InfiniFrameHostContract.MajorVersion, InfiniFrameHostContract.MinorVersion)</c>.
///     InfiniFrame rejects a provider with a different major version, or a newer minor version, before creating any host.
/// </remarks>
public static class InfiniFrameHostContract {
    /// <summary>Major version of the host contract implemented by this InfiniFrame build.</summary>
    public const int MajorVersion = 1;

    /// <summary>Minor version of the host contract implemented by this InfiniFrame build.</summary>
    public const int MinorVersion = 0;

    /// <summary>
    ///     Bridge script contract carried in <see cref="InfiniFrameHostWindowRequest.BridgeScript" />.
    ///     Version 1: in every main-frame document, before page scripts, the provider defines
    ///     <c>window.infiniframe.host.postData(message: string)</c> (page to host, reported through
    ///     <see cref="IInfiniFrameWindowHostEvents.MessageReceived" />) and
    ///     <c>window.infiniframe.host.receiveCallback(callback)</c> (registers the page callback that
    ///     <see cref="IInfiniFrameWindowHost.SendMessageAsync" /> invokes with each message), then runs the bridge script,
    ///     which wraps both.
    /// </summary>
    public const int BridgeContractVersion = 1;

    /// <summary>The contract version implemented by the running InfiniFrame build.</summary>
    public static Version Version { get; } = new(MajorVersion, MinorVersion);
}
