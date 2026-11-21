// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     The engine-neutral description of one window, mapped by InfiniFrame from the window builder. InfiniFrame rejects
///     builder settings that have no field here (or need an undeclared capability) before calling the provider, so a
///     provider must honour every field exactly or throw <see cref="NotSupportedException" /> from
///     <see cref="IInfiniFrameHost.CreateWindowAsync" /> before creating anything.
/// </summary>
public sealed class InfiniFrameHostWindowRequest {
    /// <summary>The InfiniFrame window id (<c>IInfiniFrameWindow.Id</c>).</summary>
    public required Guid WindowId { get; init; }

    /// <summary>The first main-frame URL. Checked against the trust policy by InfiniFrame.</summary>
    public required Uri StartUrl { get; init; }

    /// <summary>Lower-case schemes with registered handlers. The provider routes their requests to <see cref="IInfiniFrameWindowHostEvents.GetResourceAsync" />.</summary>
    public required IReadOnlyList<string> CustomSchemes { get; init; }

    /// <summary>Origins the window trusts for navigation and messaging. The provider must block main-frame navigation elsewhere.</summary>
    public required IReadOnlyList<Uri> TrustedOrigins { get; init; }

    /// <summary>When <see langword="true" />, the application explicitly trusts every origin.</summary>
    public bool TrustAllOrigins { get; init; }

    /// <summary>InfiniFrame's generated page bridge. Inject it into every main-frame document before page scripts run.</summary>
    public required string BridgeScript { get; init; }

    /// <summary>The bridge contract implemented by <see cref="BridgeScript" />; see <see cref="InfiniFrameHostContract.BridgeContractVersion" />.</summary>
    public required int BridgeContractVersion { get; init; }

    /// <summary>Initial native title.</summary>
    public string Title { get; init; } = "";

    /// <summary>Initial location in screen coordinates, or <see langword="null" /> for the provider's default.</summary>
    public Point? InitialLocation { get; init; }

    /// <summary>Initial outer size, or <see langword="null" /> for the provider's default.</summary>
    public Size? InitialSize { get; init; }

    /// <summary>Initial state. A non-normal value is only sent when <see cref="InfiniFrameHostCapabilities.WindowState" /> is declared.</summary>
    public InfiniFrameHostWindowState InitialState { get; init; }

    /// <summary>Initial PNG icon, empty for none. Only sent when <see cref="InfiniFrameHostCapabilities.Icon" /> is declared.</summary>
    public ReadOnlyMemory<byte> InitialIconPng { get; init; }

    /// <summary>Browser settings the provider must apply or reject.</summary>
    public required InfiniFrameHostBrowserPolicy BrowserPolicy { get; init; }
}
