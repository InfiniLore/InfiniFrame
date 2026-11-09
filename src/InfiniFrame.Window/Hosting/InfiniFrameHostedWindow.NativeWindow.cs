// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame.Hosting;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.Window.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <remarks>
///     Position, size, state and decorations. Operations backed by an optional capability call the provider and only
///     report success once the provider confirms the actual native result; undeclared operations throw
///     <see cref="NotSupportedException" />. Getters for settings the request mapping only accepts at their default
///     (size constraints, resizability, topmost, zoom, chrome) return those defaults, which are then true by contract.
/// </remarks>
internal sealed partial class InfiniFrameHostedWindow {
    private readonly object _nativeStateGate = new();
    private Rectangle? _lastReportedBounds;
    private InfiniFrameHostWindowState _lastReportedState;

    // -----------------------------------------------------------------------------------------------------------------
    // Position and size (Bounds)
    // -----------------------------------------------------------------------------------------------------------------
    private Rectangle GetBounds() {
        IInfiniFrameWindowHostBounds host = RequireHostCapability<IInfiniFrameWindowHostBounds>(InfiniFrameHostCapabilities.Bounds, "Reading window bounds");
        return WaitForHost(host.GetBoundsAsync, "GetBounds");
    }

    private void SetBounds(Rectangle bounds) {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bounds.Width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bounds.Height);
        IInfiniFrameWindowHostBounds host = RequireHostCapability<IInfiniFrameWindowHostBounds>(InfiniFrameHostCapabilities.Bounds, "Moving or resizing the window");
        WaitForHost(token => host.SetBoundsAsync(bounds, token), "SetBounds");
    }

    public Point Location => GetBounds().Location;
    public int Top => Location.Y;
    public int Left => Location.X;

    public void SetLocation(int left, int top) {
        Rectangle bounds = GetBounds();
        bounds.Location = new Point(left, top);
        SetBounds(bounds);
    }

    public void SetLocation(Point location) => SetLocation(location.X, location.Y);
    public void SetLeft(int left) => SetLocation(left, Top);
    public void SetTop(int top) => SetLocation(Left, top);

    public void Offset(int left, int top) {
        Point location = Location;
        SetLocation(checked(location.X + left), checked(location.Y + top));
    }

    public void Offset(Point offset) => Offset(offset.X, offset.Y);
    public void Offset(double left, double top) => Offset(checked((int)left), checked((int)top));
    public void Center() => throw Unsupported("Monitor-relative centering");
    public void CenterOnCurrentMonitor() => throw Unsupported("Monitor-relative centering");
    public void CenterOnMonitor(int monitorIndex) => throw Unsupported("Monitor-relative centering");
    public void MoveWithinCurrentMonitorArea(int left, int top) => throw Unsupported("Monitor-relative positioning");
    public void MoveWithinCurrentMonitorArea(Point location) => throw Unsupported("Monitor-relative positioning");
    public void MoveWithinCurrentMonitorArea(double left, double top) => throw Unsupported("Monitor-relative positioning");

    public Size Size => GetBounds().Size;
    public int Height => Size.Height;
    public int Width => Size.Width;

    public void SetSize(int width, int height) {
        Rectangle bounds = GetBounds();
        bounds.Size = new Size(width, height);
        SetBounds(bounds);
    }

    public void SetSize(Size size) => SetSize(size.Width, size.Height);
    public void SetHeight(int height) => SetSize(Width, height);
    public void SetWidth(int width) => SetSize(width, Height);
    public void Resize(int widthOffset, int heightOffset, ResizeOrigin origin) => throw Unsupported("Anchored resize");

    public Size MaxSize => new(_configuration.StartupParameters.MaxWidth, _configuration.StartupParameters.MaxHeight);
    public int MaxHeight => _configuration.StartupParameters.MaxHeight;
    public int MaxWidth => _configuration.StartupParameters.MaxWidth;
    public Size MinSize => new(_configuration.StartupParameters.MinWidth, _configuration.StartupParameters.MinHeight);
    public int MinHeight => _configuration.StartupParameters.MinHeight;
    public int MinWidth => _configuration.StartupParameters.MinWidth;
    public bool IsResizable => _configuration.StartupParameters.Resizable;
    public void SetMaxSize(int maxWidth, int maxHeight) => throw Unsupported("Size constraints");
    public void SetMaxSize(Size size) => throw Unsupported("Size constraints");
    public void SetMaxHeight(int maxHeight) => throw Unsupported("Size constraints");
    public void SetMaxWidth(int maxWidth) => throw Unsupported("Size constraints");
    public void SetMinSize(int minWidth, int minHeight) => throw Unsupported("Size constraints");
    public void SetMinSize(Size size) => throw Unsupported("Size constraints");
    public void SetMinHeight(int minHeight) => throw Unsupported("Size constraints");
    public void SetMinWidth(int minWidth) => throw Unsupported("Size constraints");
    public void SetResizable(bool resizable = true) => throw Unsupported("Changing resizability");

    // -----------------------------------------------------------------------------------------------------------------
    // State (WindowState, Focus)
    // -----------------------------------------------------------------------------------------------------------------
    private InfiniFrameHostWindowState GetHostState(InfiniFrameHostCapabilities capability, string operation) {
        IInfiniFrameWindowHostState host = RequireHostCapability<IInfiniFrameWindowHostState>(capability, operation);
        return WaitForHost(host.GetStateAsync, "GetState");
    }

    private void SetHostState(InfiniFrameHostCapabilities capability, InfiniFrameHostWindowStateChange change) {
        IInfiniFrameWindowHostState host = RequireHostCapability<IInfiniFrameWindowHostState>(capability, $"Window state change {change}");
        WaitForHost(token => host.SetStateAsync(change, token), $"SetState({change})");
    }

    public bool IsFullScreen => GetHostState(InfiniFrameHostCapabilities.WindowState, "Reading full-screen state").HasFlag(InfiniFrameHostWindowState.FullScreen);
    public bool IsMaximized => GetHostState(InfiniFrameHostCapabilities.WindowState, "Reading maximized state").HasFlag(InfiniFrameHostWindowState.Maximized);
    public bool IsMinimized => GetHostState(InfiniFrameHostCapabilities.WindowState, "Reading minimized state").HasFlag(InfiniFrameHostWindowState.Minimized);
    public bool IsFocused => GetHostState(InfiniFrameHostCapabilities.Focus, "Reading focus").HasFlag(InfiniFrameHostWindowState.Focused);
    public bool IsTopMost => _configuration.StartupParameters.Topmost;
    public int ZoomFactor => _configuration.StartupParameters.Zoom;
    public bool IsZoomEnabled => _configuration.StartupParameters.ZoomEnabled;
    public Rectangle CachedPreFullScreenBounds { get; set; }
    public Rectangle CachedPreMaximizedBounds { get; set; }

    public void SetMaximized(bool maximized = true) =>
        SetHostState(InfiniFrameHostCapabilities.WindowState, maximized ? InfiniFrameHostWindowStateChange.Maximize : InfiniFrameHostWindowStateChange.Restore);

    public void ToggleMaximized() => SetMaximized(!IsMaximized);

    public void SetMinimized(bool minimized = true) =>
        SetHostState(InfiniFrameHostCapabilities.WindowState, minimized ? InfiniFrameHostWindowStateChange.Minimize : InfiniFrameHostWindowStateChange.Restore);

    public void SetFullScreen(bool fullScreen = true) =>
        SetHostState(InfiniFrameHostCapabilities.WindowState, fullScreen ? InfiniFrameHostWindowStateChange.EnterFullScreen : InfiniFrameHostWindowStateChange.ExitFullScreen);

    public void SetFocused() => SetHostState(InfiniFrameHostCapabilities.Focus, InfiniFrameHostWindowStateChange.Focus);
    public void SetZoomFactor(int zoom) => throw Unsupported("Zoom");
    public void EnableZoom(bool zoomEnabled = true) => throw Unsupported("Zoom");
    public void SetTopMost(bool topMost = true) => throw Unsupported("Topmost windows");

    // -----------------------------------------------------------------------------------------------------------------
    // Decorations (Title, Icon)
    // -----------------------------------------------------------------------------------------------------------------
    public bool IsChromeless => _configuration.StartupParameters.Chromeless;
    public bool IsTransparent => _configuration.StartupParameters.Transparent;
    public string? BackgroundColor => throw Unsupported("Background color");
    public string? IconFilePath => Volatile.Read(ref _iconPath);
    public bool LimitLinuxWindowTitleLength => false;

    public string? Title {
        get {
            IInfiniFrameWindowHostTitle host = RequireHostCapability<IInfiniFrameWindowHostTitle>(InfiniFrameHostCapabilities.Title, "Reading the title");
            return WaitForHost(host.GetTitleAsync, "GetTitle");
        }
    }

    public void SetTitle(string? title) {
        IInfiniFrameWindowHostTitle host = RequireHostCapability<IInfiniFrameWindowHostTitle>(InfiniFrameHostCapabilities.Title, "Changing the title");
        WaitForHost(token => host.SetTitleAsync(title ?? string.Empty, token), "SetTitle");
    }

    public void SetIconFile(string iconFilePath) {
        IInfiniFrameWindowHostIcon host = RequireHostCapability<IInfiniFrameWindowHostIcon>(InfiniFrameHostCapabilities.Icon, "Changing the icon");
        byte[] png = InfiniFrameHostedWindowRequestFactory.ReadPngIcon(iconFilePath);
        WaitForHost(token => host.SetIconAsync(png, token), "SetIcon");
        Volatile.Write(ref _iconPath, Path.GetFullPath(iconFilePath));
    }

    public void SetTransparent(bool enabled = true) => throw Unsupported("Transparency");
    public void SetBackgroundColor(string? color) => throw Unsupported("Background color");
    public void SetLimitLinuxWindowTitleLength(bool enabled = true) => throw Unsupported("Title truncation");

    // -----------------------------------------------------------------------------------------------------------------
    // Provider notifications (BoundsEvents, StateEvents, FocusEvents, CloseCancellation)
    // -----------------------------------------------------------------------------------------------------------------
    private void OnHostBoundsChanged(Rectangle bounds) {
        if (!_runtime.Capabilities.HasFlag(InfiniFrameHostCapabilities.BoundsEvents)) {
            _logger.LogDebug("Ignored BoundsChanged from a provider that does not declare {Capability}", InfiniFrameHostCapabilities.BoundsEvents);
            return;
        }
        if (Volatile.Read(ref _terminal) != 0) return;
        bool moved, resized;
        lock (_nativeStateGate) {
            moved = _lastReportedBounds?.Location != bounds.Location;
            resized = _lastReportedBounds?.Size != bounds.Size;
            _lastReportedBounds = bounds;
        }
        if (moved) PostWindowEvent(() => _events.OnLocationChanged(bounds.X, bounds.Y));
        if (resized) PostWindowEvent(() => _events.OnSizeChanged(bounds.Width, bounds.Height));
    }

    private void OnHostStateChanged(InfiniFrameHostWindowState state) {
        bool stateEvents = _runtime.Capabilities.HasFlag(InfiniFrameHostCapabilities.StateEvents);
        bool focusEvents = _runtime.Capabilities.HasFlag(InfiniFrameHostCapabilities.FocusEvents);
        if (!stateEvents && !focusEvents) {
            _logger.LogDebug("Ignored StateChanged from a provider that declares neither state nor focus events");
            return;
        }
        if (Volatile.Read(ref _terminal) != 0) return;
        InfiniFrameHostWindowState previous;
        lock (_nativeStateGate) {
            previous = _lastReportedState;
            _lastReportedState = state;
        }
        InfiniFrameHostWindowState changed = previous ^ state;
        if (stateEvents) {
            const InfiniFrameHostWindowState sized = InfiniFrameHostWindowState.Minimized | InfiniFrameHostWindowState.Maximized;
            if (changed.HasFlag(InfiniFrameHostWindowState.Minimized) && state.HasFlag(InfiniFrameHostWindowState.Minimized)) PostWindowEvent(_events.OnMinimized);
            if (changed.HasFlag(InfiniFrameHostWindowState.Maximized) && state.HasFlag(InfiniFrameHostWindowState.Maximized)) PostWindowEvent(_events.OnMaximized);
            if ((previous & sized) != 0 && (state & sized) == 0) PostWindowEvent(_events.OnRestored);
        }
        if (focusEvents && changed.HasFlag(InfiniFrameHostWindowState.Focused))
            PostWindowEvent(state.HasFlag(InfiniFrameHostWindowState.Focused) ? _events.OnFocusIn : _events.OnFocusOut);
    }

    private void PostWindowEvent(Action callback) {
        if (!_runtime.Dispatcher.TryPost(() => {
                if (Volatile.Read(ref _terminal) == 0) callback();
            }, payloadBytes: 0))
            FaultWindow(new InvalidOperationException("The hosted dispatcher budget is exhausted; window events cannot be delivered."));
    }

    private async ValueTask<bool> OnHostCloseRequestedAsync(CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_runtime.Capabilities.HasFlag(InfiniFrameHostCapabilities.CloseCancellation)) {
            _logger.LogDebug("CloseRequestedAsync from a provider without {Capability}; closing proceeds", InfiniFrameHostCapabilities.CloseCancellation);
            return true;
        }
        if (Volatile.Read(ref _terminal) != 0) return true;
        // CloseRequested also means the first request's cancellable handlers may still be queued. A duplicate
        // must never authorize native closure before that decision; the provider can ask again after a veto.
        if (!((IInfiniFrameWindow)this).RequestClose()) return false;
        try {
            bool cancelled = await RunClosingHandlersAsync(raiseClosingRequested: false, cancellationToken).ConfigureAwait(false);
            return !cancelled;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
            // The provider stopped asking and will not close natively, so the window stays open.
            ((IInfiniFrameWindow)this).CancelCloseRequest();
            throw;
        }
    }
}
