using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace InfiniLore.Photino.NET;

public partial class PhotinoWindow : IPhotinoWindow
{
    public event EventHandler<Point>? WindowLocationChanged;
    public event EventHandler<Size>? WindowSizeChanged;
    public event EventHandler? WindowFocusIn;
    public event EventHandler? WindowMaximized;
    public event EventHandler? WindowRestored;
    public event EventHandler? WindowFocusOut;
    public event EventHandler? WindowMinimized;
    public event EventHandler<string>? WebMessageReceived;
    public event NetClosingDelegate? WindowClosing;
    public event EventHandler? WindowCreating;
    public event EventHandler? WindowCreated;
    
    //Pointers to the type and instance.
    private static IntPtr _nativeType = IntPtr.Zero;
    public IntPtr InstanceHandle { get; private set; }
    private PhotinoNativeParameters _startupParameters;

    //There can only be 1 message loop for all windows.
    private static bool _messageLoopIsStarted;

    private readonly int _managedThreadId = Environment.CurrentManagedThreadId;

    private readonly Dictionary<string, NetCustomSchemeDelegate?> _customSchemes = [];

    private readonly ILogger<PhotinoWindow> _logger;


    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the PhotinoWindow class.
    /// </summary>
    /// <remarks>
    ///     This class represents a native window with a native browser control taking up the entire client area.
    ///     If a parent window is specified, this window will be created as a child of the specified parent window.
    /// </remarks>
    /// <param name="parameters"></param>
    /// <param name="logger">THe logger used by the main application</param>
    /// <param name="parent">The parent PhotinoWindow. This is optional and defaults to null.</param>
    public PhotinoWindow(PhotinoNativeParameters parameters, ILogger<PhotinoWindow> logger, PhotinoWindow? parent = null)
    {
        _startupParameters = parameters;
        _logger = logger;
        Parent = parent;
        MaxWidth = parameters.MaxWidth;
        MaxHeight = parameters.MaxHeight;
        
        //This only has to be done once
        if (_nativeType == IntPtr.Zero)
            _nativeType = NativeLibrary.GetMainProgramHandle();

        //These are for the callbacks from C++ to C#.
        _startupParameters.ClosingHandler = OnWindowClosing;
        _startupParameters.ResizedHandler = OnSizeChanged;
        _startupParameters.MaximizedHandler = OnMaximized;
        _startupParameters.RestoredHandler = OnRestored;
        _startupParameters.MinimizedHandler = OnMinimized;
        _startupParameters.MovedHandler = OnLocationChanged;
        _startupParameters.FocusInHandler = OnFocusIn;
        _startupParameters.FocusOutHandler = OnFocusOut;
        _startupParameters.WebMessageReceivedHandler = OnWebMessageReceived;
        _startupParameters.CustomSchemeHandler = OnCustomScheme;
        
        MaxWidth = _startupParameters.MaxWidth;
        MaxHeight = _startupParameters.MaxHeight;
        MinWidth = _startupParameters.MinWidth;
        MinHeight = _startupParameters.MinHeight;
    }
    #endregion

    #region READ ONLY PROPERTIES
    /// <summary>
    ///     Indicates whether the current platform is Windows.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the current platform is Windows; otherwise, <c>false</c>.
    /// </value>
    public static bool IsWindowsPlatform { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    /// <summary>
    ///     Indicates whether the current platform is macOS.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the current platform is macOS; otherwise, <c>false</c>.
    /// </value>
    public static bool IsMacOsPlatform { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    
    /// <summary>
    ///     Indicates the version of macOS
    /// </summary>
    public static Version? MacOsVersion => IsMacOsPlatform ? Version.Parse(RuntimeInformation.OSDescription.Split(' ')[1]) : null;

    /// <summary>
    ///     Indicates whether the current platform is Linux.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the current platform is Linux; otherwise, <c>false</c>.
    /// </value>
    public static bool IsLinuxPlatform { get; } =  RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    /// <summary>
    ///     Represents a property that gets the handle of the native window on a Windows platform.
    /// </summary>
    /// <remarks>
    ///     Only available on the Windows platform.
    ///     If this property is accessed from a non-Windows platform, a PlatformNotSupportedException will be thrown.
    ///     If this property is accessed before the window is initialized, an ApplicationException will be thrown.
    /// </remarks>
    /// <value>
    ///     The handle of the native window. The value is of type <see cref="IntPtr" />.
    /// </value>
    /// <exception cref="System.ApplicationException">Thrown when the window is not initialized yet.</exception>
    /// <exception cref="System.PlatformNotSupportedException">Thrown when accessed from a non-Windows platform.</exception>
    public IntPtr WindowHandle
    {
        get
        {
            ThrowIfNotWindowsEnvironment();
            ThrowIfNotInitialized();

            var handle = IntPtr.Zero;
            Invoke(() => handle = PhotinoNative.GetWindowHandlerWin32(InstanceHandle));
            return handle;
        }
    }

    /// <summary>
    ///     Gets a list of information for each monitor from the native window.
    ///     This property represents a list of Monitor objects associated with each display monitor.
    /// </summary>
    /// <remarks>
    ///     If called when the native instance of the window is not initialized, it will throw an ApplicationException.
    /// </remarks>
    /// <exception cref="ApplicationException">Thrown when the native instance of the window is not initialized.</exception>
    /// <returns>
    ///     A read-only list of Monitor objects representing information about each display monitor.
    /// </returns>
    public IReadOnlyList<Monitor> Monitors
    {
        get
        {
            ThrowIfNotInitialized();
            Monitor[] monitors = [];

            Invoke(() =>
            {
                monitors = MonitorsUtility.GetMonitorsAsArray(InstanceHandle);
            });

            return monitors;
        }
    }

    /// <summary>
    ///     Retrieves the primary monitor information from the native window instance.
    /// </summary>
    /// <exception cref="ApplicationException"> Thrown when the window hasn't been initialized yet. </exception>
    /// <returns>
    ///     Returns a Monitor object representing the main monitor. The main monitor is the first monitor in the list of
    ///     available monitors.
    /// </returns>
    public Monitor MainMonitor
    {
        get
        {
            ThrowIfNotInitialized();
            return Monitors[0];
        }
    }

    /// <summary>
    ///     Gets the dots per inch (DPI) for the primary display from the native window.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     An ApplicationException is thrown if the window hasn't been initialized yet.
    /// </exception>
    public uint ScreenDpi
    {
        get
        {
            ThrowIfNotInitialized();

            uint dpi = 0;
            Invoke(() => dpi = PhotinoNative.GetScreenDpi(InstanceHandle));
            return dpi;
        }
    }

    /// <summary>
    ///     Gets a unique GUID to identify the native window.
    /// </summary>
    /// <remarks>
    ///     This property is not currently used by the Photino framework.
    /// </remarks>
    public Guid Id { get; } = Guid.NewGuid();

    public int ManagedThreadId => _managedThreadId;
    #endregion

    #region READ WRITE PROPERTIES
    /// <summary>
    ///     Gets the value indicating whether the native window is chromeless.
    /// </summary>
    /// <remarks>
    ///     The user has to supply titlebar, border, dragging and resizing manually.
    /// </remarks>
    public bool Chromeless => _startupParameters.Chromeless;

    /// <summary>
    ///     When true, the native window and browser control can be displayed with a transparent background.
    ///     HTML document's body background must have alpha-based value.
    ///     WebView2 on Windows can only be fully transparent or fully opaque.
    ///     By default, this is set to false.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     On Windows, thrown if trying to set a value after a native window is initialized.
    /// </exception>
    public bool Transparent
    {
        get
        {
            var enabled = false;
            Invoke(() => PhotinoNative.GetTransparentEnabled(InstanceHandle, out enabled));
            return enabled;
        }
    }

    /// <summary>
    ///     When true, the user can access the browser control's context menu.
    ///     By default, this is set to true.
    /// </summary>
    public bool ContextMenuEnabled
    {
        get
        {
            var enabled = false;
            Invoke(() => PhotinoNative.GetContextMenuEnabled(InstanceHandle, out enabled));
            return enabled;
        }
    }

    /// <summary>
    ///     When true, the user can access the browser control's developer tools.
    ///     By default, this is set to true.
    /// </summary>
    public bool DevToolsEnabled
    {
        get
        {
            var enabled = false;
            Invoke(() => PhotinoNative.GetDevToolsEnabled(InstanceHandle, out enabled));
            return enabled;
        }
    }

    public bool MediaAutoplayEnabled
    {
        get
        {
            var enabled = false;
            Invoke(() => PhotinoNative.GetMediaAutoplayEnabled(InstanceHandle, out enabled));
            return enabled;
        }
    }

    public string UserAgent
    {
        get
        {
            var userAgent = string.Empty;
            Invoke(() =>
            {
                var ptr = PhotinoNative.GetUserAgent(InstanceHandle);
                userAgent = Marshal.PtrToStringAuto(ptr);
            });
            return userAgent;
        }
    }

    public bool FileSystemAccessEnabled
    {
        get
        {
            var enabled = false;
            Invoke(() => PhotinoNative.GetFileSystemAccessEnabled(InstanceHandle, out enabled));
            return enabled;
        }
    }

    public bool WebSecurityEnabled
    {
        get
        {
            var enabled = true;
            Invoke(() => PhotinoNative.GetWebSecurityEnabled(InstanceHandle, out enabled));
            return enabled;
        }
    }

    public bool JavascriptClipboardAccessEnabled
    {
        get
        {
            var enabled = true;
            Invoke(() => PhotinoNative.GetJavascriptClipboardAccessEnabled(InstanceHandle, out enabled));
            return enabled;
        }
    }

    public bool MediaStreamEnabled
    {
        get
        {
            var enabled = true;
            Invoke(() => PhotinoNative.GetMediaStreamEnabled(InstanceHandle, out enabled));
            return enabled;
        }
    }

    public bool SmoothScrollingEnabled
    {
        get
        {
            var enabled = false;
            Invoke(() => PhotinoNative.GetSmoothScrollingEnabled(InstanceHandle, out enabled));
            return enabled;
        }
    }

    public bool IgnoreCertificateErrorsEnabled
    {
        get
        {

            var enabled = false;
            Invoke(() => PhotinoNative.GetIgnoreCertificateErrorsEnabled(InstanceHandle, out enabled));
            return enabled;
        }
    }

    public bool NotificationsEnabled
    {
        get
        {
            var enabled = false;
            Invoke(() => PhotinoNative.GetNotificationsEnabled(InstanceHandle, out enabled));
            return enabled;
        }
    }
    
    /// <summary>
    ///     This property returns or sets the fullscreen status of the window.
    ///     When set to true, the native window will cover the entire screen, similar to kiosk mode.
    ///     By default, this is set to false.
    /// </summary>
    public bool FullScreen
    {
        get
        {
            var fullScreen = false;
            Invoke(() => PhotinoNative.GetFullScreen(InstanceHandle, out fullScreen));
            return fullScreen;
        }
    }
    private Rectangle _preFullscreenArea;
    
    /// <summary>
    ///     Gets whether the native browser control grants all requests for access to local resources
    ///     such as the user's camera and microphone. By default, this is set to true.
    /// </summary>
    public bool GrantBrowserPermissions
    {
        get
        {
            var grant = false;
            Invoke(() => PhotinoNative.GetGrantBrowserPermissions(InstanceHandle, out grant));
            return grant;
        }
    }

    /// <summary>
    ///     Gets or Sets the Height property of the native window in pixels.
    ///     The default value is 0.
    /// </summary>
    /// <seealso cref="UseOsDefaultSize" />
    public int Height
    {
        get
        {
            var height = 0; 
            Invoke(() => PhotinoNative.GetSize(InstanceHandle, out _, out height));
            return height;
        }
    }
    
    /// <summary>
    ///     Gets or sets the icon file for the native window title bar.
    ///     The file must be located on the local machine and cannot be a URL. The default is none.
    /// </summary>
    /// <exception cref="System.ArgumentException">Icon file: {value} does not exist.</exception>
    public string? IconFilePath { get; private set; }

    /// <summary>
    ///     Gets or sets the native window Left (X) and Top coordinates (Y) in pixels.
    ///     Default is 0,0 that means the window will be aligned to the top-left edge of the screen.
    /// </summary>
    /// <seealso cref="UseOsDefaultLocation" />
    public Point Location
    {
        get
        {
            var left = 0;
            var top = 0;
            Invoke(() => PhotinoNative.GetPosition(InstanceHandle, out left, out top));
            return new Point(left, top);
        }
    }

    /// <summary>
    ///     Gets or sets the native window Left (X) coordinate in pixels.
    ///     This represents the horizontal position of the window relative to the screen.
    ///     The default value is 0, which means the window will be aligned to the left edge of the screen.
    /// </summary>
    /// <seealso cref="UseOsDefaultLocation" />
    public int Left
    {
        get
        {
            var left = 0;
            Invoke(() => PhotinoNative.GetPosition(InstanceHandle, out left, out _));
            return left;
        }
    }
    /// <summary>
    ///     Gets or sets whether the native window is maximized.
    ///     Default is false.
    /// </summary>
    public bool Maximized
    {
        get
        {
            var maximized = false;
            Invoke(() => PhotinoNative.GetMaximized(InstanceHandle, out maximized));
            return maximized;
        }
    }

    ///<summary>Gets or set the maximum size of the native window in pixels.</summary>
    public Point MaxSize => new Point(MaxWidth, MaxHeight);

    ///<summary>Gets or sets the native window maximum height in pixels.</summary>
    public int MaxHeight { get; private set; }

    ///<summary>Gets or sets the native window maximum width in pixels.</summary>
    public int MaxWidth { get; private set; }
    
    /// <summary>
    ///     Gets or sets whether the native window is minimized (hidden).
    ///     Default is false.
    /// </summary>
    public bool Minimized
    {
        get
        {
            var minimized = false;
            Invoke(() => PhotinoNative.GetMinimized(InstanceHandle, out minimized));
            return minimized;
        }
    }

    ///<summary>Gets or set the minimum size of the native window in pixels.</summary>
    public Point MinSize => new Point(MinWidth, MinHeight);

    ///<summary>Gets or sets the native window minimum height in pixels.</summary>
    public int MinHeight { get; }

    ///<summary>Gets or sets the native window minimum height in pixels.</summary>
    public int MinWidth { get; }
    
    /// <summary>
    ///     Gets the reference to the parent PhotinoWindow instance.
    ///     This property can only be set in the constructor, and it is optional.
    /// </summary>
    public IPhotinoWindow? Parent { get; }
    
    /// <summary>
    ///     Gets or sets whether the user can resize the native window.
    ///     Default is true.
    /// </summary>
    public bool Resizable
    {
        get
        {
            var resizable = false;
            Invoke(() => PhotinoNative.GetResizable(InstanceHandle, out resizable));
            return resizable;
        }
    }

    /// <summary>
    ///     Gets or sets the native window Size. This represents the width and the height of the window in pixels.
    ///     The default Size is 0,0.
    /// </summary>
    /// <seealso cref="UseOsDefaultSize" />
    public Size Size
    {
        get
        {
            var width = 0;
            var height = 0;
            Invoke(() => PhotinoNative.GetSize(InstanceHandle, out width, out height));
            return new Size(width, height);
        }
    }

    /// <summary>
    ///     Gets or sets platform-specific initialization parameters for the native browser control on startup.
    ///     Default is none.
    ///     WINDOWS: WebView2 specific string. Space separated.
    ///     https://peter.sh/experiments/chromium-command-line-switches/
    ///     https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2environmentoptions.additionalbrowserarguments?view=webview2-dotnet-1.0.1938.49
    ///     viewFallbackFrom=webview2-dotnet-1.0.1901.177view%3Dwebview2-1.0.1901.177
    ///     https://www.chromium.org/developers/how-tos/run-chromium-with-flags/
    ///     LINUX: Webkit2Gtk specific string. Enter parameter names and values as JSON string.
    ///     e.g. { "set_enable_encrypted_media": true }
    ///     https://webkitgtk.org/reference/webkit2gtk/2.5.1/WebKitSettings.html
    ///     https://lazka.github.io/pgi-docs/WebKit2-4.0/classes/Settings.html
    ///     MAC: Webkit specific string. Enter parameter names and values as JSON string.
    ///     e.g. { "minimumFontSize": 8 }
    ///     https://developer.apple.com/documentation/webkit/wkwebviewconfiguration?language=objc
    ///     https://developer.apple.com/documentation/webkit/wkpreferences?language=objc
    /// </summary>
    public string? BrowserControlInitParameters => _startupParameters.BrowserControlInitParameters;

    /// <summary>
    ///     Gets or sets an HTML string that the browser control will render when initialized.
    ///     Default is none.
    /// </summary>
    /// <remarks>
    ///     Either StartString or StartUrl must be specified.
    /// </remarks>
    /// <seealso cref="StartUrl" />
    /// <exception cref="ApplicationException">
    ///     Thrown if trying to set a value after a native window is initialized.
    /// </exception>
    public string? StartString => _startupParameters.StartString;

    /// <summary>
    ///     Gets or sets a URL that the browser control will navigate to when initialized.
    ///     Default is none.
    /// </summary>
    /// <remarks>
    ///     Either StartString or StartUrl must be specified.
    /// </remarks>
    /// <seealso cref="StartString" />
    /// <exception cref="ApplicationException">
    ///     Thrown if trying to set a value after a native window is initialized.
    /// </exception>
    public string? StartUrl => _startupParameters.StartUrl;

    /// <summary>
    ///     Gets or sets the local path to store temp files for browser control.
    ///     Default is the user's AppDataLocal folder.
    /// </summary>
    /// <remarks>
    ///     Only available on Windows.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown if a platform is not Windows.
    /// </exception>
    public string? TemporaryFilesPath => _startupParameters.TemporaryFilesPath;

    /// <summary>
    ///     Gets or sets the registration id for doing toast notifications.
    ///     The default is to use the window title.
    /// </summary>
    /// <remarks>
    ///     Only available on Windows.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown if a platform is not Windows.
    /// </exception>
    public string? NotificationRegistrationId => _startupParameters.NotificationRegistrationId;

    /// <summary>
    ///     Gets or sets the native window title.
    ///     Default is "Photino".
    /// </summary>
    public string? Title
    {
        get
        {
            var title = string.Empty;
            Invoke(() => 
            {
                var ptr = PhotinoNative.GetTitle(InstanceHandle);
                title = Marshal.PtrToStringAuto(ptr);
            });
            return title;
        }
    }
    
    /// <summary>
    ///     Gets or sets the native window Top (Y) coordinate in pixels.
    ///     Default is 0.
    /// </summary>
    /// <seealso cref="UseOsDefaultLocation" />
    public int Top
    {
        get
        {
            var top = 0;
            Invoke(() => PhotinoNative.GetPosition(InstanceHandle, out _, out top));
            return top;
        }
    }
    
    // TODO CONTINUE HERE

    /// <summary>
    ///     Gets or sets whether the native window is always at the top of the z-order.
    ///     Default is false.
    /// </summary>
    public bool Topmost
    {
        get
        {
            if (IsNotInitialized())
                return _startupParameters.Topmost;

            var topmost = false;
            Invoke(() => PhotinoNative.GetTopmost(InstanceHandle, out topmost));
            return topmost;
        }
        set
        {
            if (Topmost == value) return;
            if (IsNotInitialized())
                _startupParameters.Topmost = value;
            else
                Invoke(() => PhotinoNative.SetTopmost(InstanceHandle, value));
        }
    }

    /// <summary>
    ///     When true, the native window starts up at the OS Default location.
    ///     Default is true.
    /// </summary>
    /// <remarks>
    ///     Overrides Left (X) and Top (Y) properties.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown if trying to set a value after a native window is initialized.
    /// </exception>
    public bool UseOsDefaultLocation
    {
        get
        {
            return _startupParameters.UseOsDefaultLocation;
        }
        set
        {
            if (InstanceHandle != IntPtr.Zero) throw new ApplicationException("UseOsDefaultLocation can only be set before the native window is instantiated.");
            if (UseOsDefaultLocation == value) return;
            _startupParameters.UseOsDefaultLocation = value;
        }
    }

    /// <summary>
    ///     When true, the native window starts at the OS Default size.
    ///     Default is true.
    /// </summary>
    /// <remarks>
    ///     Overrides Height and Width properties.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown if trying to set a value after a native window is initialized.
    /// </exception>
    public bool UseOsDefaultSize
    {
        get
        {
            return _startupParameters.UseOsDefaultSize;
        }
        set
        {
            if (IsNotInitialized())
            {
                if (UseOsDefaultSize != value)
                    _startupParameters.UseOsDefaultSize = value;
            }
            else
                throw new ApplicationException("UseOsDefaultSize can only be set before the native window is instantiated.");
        }
    }

    /// <summary>
    ///     Gets or sets handlers for WebMessageReceived event.
    ///     Set assigns a new handler to the event.
    /// </summary>
    /// <seealso cref="WebMessageReceived" />
    public EventHandler<string>? WebMessageReceivedHandler
    {
        get
        {
            return WebMessageReceived;
        }
        set
        {
            WebMessageReceived += value;
        }
    }

    /// <summary>
    ///     Gets or Sets the native window width in pixels.
    ///     Default is 0.
    /// </summary>
    /// <seealso cref="UseOsDefaultSize" />
    public int Width
    {
        get
        {
            return Size.Width;
        }
    }

    /// <summary>
    ///     Gets or sets the native browser control <see cref="PhotinoWindow.Zoom" />.
    ///     Default is 100.
    /// </summary>
    /// <example>100 = 100%, 50 = 50%</example>
    public int Zoom
    {
        get
        {
            if (IsNotInitialized())
                return _startupParameters.Zoom;

            var zoom = 0;
            Invoke(() => PhotinoNative.GetZoom(InstanceHandle, out zoom));
            return zoom;
        }
        set
        {
            if (Zoom == value) return;
            if (IsNotInitialized()) _startupParameters.Zoom = value;
            else Invoke(() => PhotinoNative.SetZoom(InstanceHandle, value));
        }
    }
    
    #endregion

    #region FLIENT METHODS
    //FLUENT METHODS FOR INITIALIZING STARTUP PARAMETERS FOR NEW WINDOWS
    //CAN ALSO BE CALLED AFTER INITIALIZATION TO SET VALUES
    //ONE OF THESE 3 METHODS *MUST* BE CALLED PRIOR TO CALLING WATERCOLORIST() OR CREATECHILDWINDOW()

    /// <summary>
    ///     Dispatches an Action to the UI thread if called from another thread.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="PhotinoWindow" /> instance.
    /// </returns>
    /// <param name="workItem"> The delegate encapsulating a method / action to be executed in the UI thread.</param>
    public void Invoke(Action workItem)
    {
        // If we're already on the UI thread, no need to dispatch
        if (Environment.CurrentManagedThreadId == _managedThreadId)
            workItem();
        else
            PhotinoNative.Invoke(InstanceHandle, workItem.Invoke);
    }

    /// <summary>
    ///     Loads specified <see cref="Uri" /> into the browser control.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="PhotinoWindow" /> instance.
    /// </returns>
    /// <remarks>
    ///     Load() or LoadString() must be called before a native window is initialized.
    /// </remarks>
    /// <param name="uri">A Uri pointing to the file or the URL to load.</param>
    public IPhotinoWindow Load(Uri uri)
    {
        _logger.LogDebug(".Load({uri})", uri);
        
        if (IsNotInitialized()) _startupParameters.StartUrl = uri.ToString();
        else Invoke(() => PhotinoNative.NavigateToUrl(InstanceHandle, uri.ToString()));
        return this;
    }

    /// <summary>
    ///     Loads a specified path into the browser control.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <remarks>
    ///     Load() or LoadString() must be called before a native window is initialized.
    /// </remarks>
    /// <param name="path">A path pointing to the resource to load.</param>
    public IPhotinoWindow Load(string path)
    {
        _logger.LogDebug(".Load({Path})", path);

        // ––––––––––––––––––––––
        // SECURITY RISK!
        // This needs validation!
        // ––––––––––––––––––––––
        // Open a web URL string path
        if (path.Contains("http://") || path.Contains("https://"))
            return Load(new Uri(path));

        // Open a file resource string path
        var absolutePath = Path.GetFullPath(path);

        // For a bundled app it can be necessary to consider
        // the app context base directory. Check there too.
        if (File.Exists(absolutePath)) return Load(new Uri(absolutePath, UriKind.Absolute));
        absolutePath = $"{AppContext.BaseDirectory}/{path}";

        if (File.Exists(absolutePath)) return Load(new Uri(absolutePath, UriKind.Absolute));
        
        _logger.LogWarning("File not found: {Path}", absolutePath);
        return this;
    }

    /// <summary>
    ///     Loads a raw string into the browser control.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <remarks>
    ///     Used to load HTML into the browser control directly.
    ///     Load() or LoadString() must be called before a native window is initialized.
    /// </remarks>
    /// <param name="content">Raw content (such as HTML)</param>
    public IPhotinoWindow LoadRawString(string content)
    {
        var shortContent = content.Length > 50 ? string.Concat(content.AsSpan(0, 50), "...") : content;
        _logger.LogDebug(".LoadRawString({Content})", shortContent);
        if (IsNotInitialized()) _startupParameters.StartString = content;
        else Invoke(() => PhotinoNative.NavigateToString(InstanceHandle, content));
        return this;
    }

    /// <summary>
    ///     Centers the native window on the primary display.
    /// </summary>
    /// <remarks>
    ///     If called prior to window initialization, overrides Left (X) and Top (Y) properties.
    /// </remarks>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <seealso cref="UseOsDefaultLocation" />
    public IPhotinoWindow Center()
    {
        _logger.LogDebug(".Center()");
        Invoke(() => PhotinoNative.Center(InstanceHandle));
        return this;
    }

    /// <summary>
    ///     Moves the native window to the specified location on the screen in pixels using a Point.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="location">Position as <see cref="Point" /></param>
    /// <param name="allowOutsideWorkArea">Whether the window can go off-screen (work area)</param>
    public IPhotinoWindow MoveTo(Point location, bool allowOutsideWorkArea = false)
    {
        _logger.LogDebug(".MoveTo({location}, {allowOutsideWorkArea})", location, allowOutsideWorkArea);
        _logger.LogDebug("Current location: {Location}", Location);
        _logger.LogDebug("New location: {NewLocation}", location);

        // If the window is outside the work area,
        // recalculate the position and continue.
        //When a window isn't initialized yet, cannot determine screen size.
        if (!allowOutsideWorkArea && InstanceHandle != IntPtr.Zero)
        {
            var horizontalWindowEdge = location.X + Width;
            var verticalWindowEdge = location.Y + Height;

            var horizontalWorkAreaEdge = MainMonitor.WorkArea.Width;
            var verticalWorkAreaEdge = MainMonitor.WorkArea.Height;

            var isOutsideHorizontalWorkArea = horizontalWindowEdge > horizontalWorkAreaEdge;
            var isOutsideVerticalWorkArea = verticalWindowEdge > verticalWorkAreaEdge;

            var locationInsideWorkArea = new Point(
                isOutsideHorizontalWorkArea ? horizontalWorkAreaEdge - Width : location.X,
                isOutsideVerticalWorkArea ? verticalWorkAreaEdge - Height : location.Y
            );

            location = locationInsideWorkArea;
        }

        // Bug:
        // For some reason the vertical position is not handled correctly.
        // Whenever a positive value is set, the window appears at the
        // very bottom of the screen, and the only visible thing is the
        // application window title bar. As a workaround we make a
        // negative value out of the vertical position to "pull" the window up.
        // Note:
        // This behavior seems to be a macOS thing. In the Photino.Native
        // project files it is commented to be expected behavior for macOS.
        // There is some code trying to mitigate this problem, but it might
        // not work as expected. Further investigation is necessary.
        // Update:
        // This behavior seems to have changed with macOS Sonoma.
        // Therefore, we determine the version of macOS and only apply the
        // workaround for older versions.
        if (IsMacOsPlatform && MacOsVersion?.Major < 23)
        {
            var workArea = MainMonitor.WorkArea.Size;
            location.Y = location.Y >= 0
                ? location.Y - workArea.Height
                : location.Y;
        }

        SetLocation(location);

        return this;
    }

    /// <summary>
    ///     Moves the native window to the specified location on the screen in pixels
    ///     using <see cref="IPhotinoWindow.Left" /> (X) and <see cref="IPhotinoWindow.Top" /> (Y) properties.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="left">Position from left in pixels</param>
    /// <param name="top">Position from top in pixels</param>
    /// <param name="allowOutsideWorkArea">Whether the window can go off-screen (work area)</param>
    public IPhotinoWindow MoveTo(int left, int top, bool allowOutsideWorkArea = false)
    {
        _logger.LogDebug(".MoveTo({left}, {top}, {allowOutsideWorkArea})", left, top, allowOutsideWorkArea);
        return MoveTo(new Point(left, top), allowOutsideWorkArea);
    }

    /// <summary>
    ///     Moves the native window relative to its current location on the screen
    ///     using a <see cref="Point" />.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="offset">Relative offset</param>
    public IPhotinoWindow Offset(Point offset)
    {
        _logger.LogDebug(".Offset({offset})", offset);
        var location = Location;
        var left = location.X + offset.X;
        var top = location.Y + offset.Y;
        return MoveTo(left, top);
    }

    /// <summary>
    ///     Moves the native window relative to its current location on the screen in pixels
    ///     using <see cref="IPhotinoWindow.Left" /> (X) and <see cref="IPhotinoWindow.Top" /> (Y) properties.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="left">Relative offset from left in pixels</param>
    /// <param name="top">Relative offset from top in pixels</param>
    public IPhotinoWindow Offset(int left, int top)
    {
        _logger.LogDebug(".Offset({left}, {top})", left, top);
        return Offset(new Point(left, top));
    }

    /// <summary>
    ///     When true, the native window will appear without a title bar or border.
    ///     By default, this is set to false.
    /// </summary>
    /// <remarks>
    ///     The user has to supply titlebar, border, dragging and resizing manually.
    /// </remarks>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="chromeless">Whether the window should be chromeless</param>
    public IPhotinoWindow SetChromeless(bool chromeless)
    {
        _logger.LogDebug(".SetChromeless({chromeless})", chromeless);
        if (InstanceHandle != IntPtr.Zero)
            throw new ApplicationException("Chromeless can only be set before the native window is instantiated.");

        _startupParameters.Chromeless = chromeless;
        return this;
    }

    /// <summary>
    ///     When true, the native window can be displayed with a transparent background.
    ///     Chromeless must be set to true. HTML document's body background must have alpha-based value.
    ///     By default, this is set to false.
    /// </summary>
    public IPhotinoWindow SetTransparent(bool enabled)
    {
        _logger.LogDebug(".SetTransparent({Enabled})", enabled);

        if (IsWindowsPlatform)
        {
            _logger.LogWarning("Transparent can only be set on Windows before the native window is instantiated.");
            return this;
        }
        
        _logger.LogDebug("Invoking PhotinoNative.SetTransparentEnabled({value})", enabled);
        Invoke(() => PhotinoNative.SetTransparentEnabled(InstanceHandle, enabled));
        return this;
    }

    /// <summary>
    ///     When true, the user can access the browser control's context menu.
    ///     By default, this is set to true.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="enabled">Whether the context menu should be available</param>
    public IPhotinoWindow SetContextMenuEnabled(bool enabled)
    {
        _logger.LogDebug(".SetContextMenuEnabled({Enabled})", enabled);
        
        Invoke(() =>
        {
            PhotinoNative.GetContextMenuEnabled(InstanceHandle, out var isEnabled);
            if (isEnabled == enabled) return;
            PhotinoNative.SetContextMenuEnabled(InstanceHandle, enabled);
        });
        return this;
    }

    /// <summary>
    ///     When true, the user can access the browser control's developer tools.
    ///     By default, this is set to true.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="enabled">Whether developer tools should be available</param>
    public IPhotinoWindow SetDevToolsEnabled(bool enabled)
    {
        _logger.LogDebug(".SetDevTools({Enabled})", enabled);
        
        Invoke(() =>
        {
            PhotinoNative.GetDevToolsEnabled(InstanceHandle, out var isEnabled);
            if (isEnabled == enabled) return;
            PhotinoNative.SetDevToolsEnabled(InstanceHandle, enabled);
        });
        return this;
    }

    /// <summary>
    ///     When set to true, the native window will cover the entire screen, similar to kiosk mode.
    ///     By default, this is set to false.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="fullScreen">Whether the window should be fullscreen</param>
    public IPhotinoWindow SetFullScreen(bool fullScreen)
    {
        _logger.LogDebug(".SetFullScreen({FullScreen})", fullScreen);

        if (FullScreen == fullScreen)
        {
            _logger.LogDebug("Window is already of the same fullscreen state of {fullscreen}", fullScreen);
            return this;
        }

        if (fullScreen)
        {
            Invoke(() =>
            {
                var monitors = MonitorsUtility.GetMonitors(InstanceHandle);
                PhotinoNative.GetPosition(InstanceHandle, out var left, out var top);
                PhotinoNative.GetSize(InstanceHandle, out var width, out var height);
                    
                _preFullscreenArea = new Rectangle(left, top, width, height);

                Rectangle currentMonitorArea = default;
                foreach (var monitor in monitors)
                {
                    if (!monitor.MonitorArea.IntersectsWith(_preFullscreenArea)) continue;
                    currentMonitorArea = monitor.MonitorArea;
                    break;
                }
                    
                if (currentMonitorArea == default)
                {
                    PhotinoNative.SetFullScreen(InstanceHandle, true);
                    return;
                }
                    
                PhotinoNative.SetFullScreen(InstanceHandle, true);
                PhotinoNative.SetPosition(InstanceHandle, currentMonitorArea.X, currentMonitorArea.Y);
                PhotinoNative.SetSize(InstanceHandle, currentMonitorArea.Width, currentMonitorArea.Height);
            });
            return this;
        }
        
        // Set Fullscreen to false => Restore to previous state
        Invoke(() =>
        {
            PhotinoNative.SetFullScreen(InstanceHandle, false);
            PhotinoNative.SetPosition(InstanceHandle, _preFullscreenArea.X, _preFullscreenArea.Y);
            PhotinoNative.SetSize(InstanceHandle, _preFullscreenArea.Width, _preFullscreenArea.Height);
        });

        return this;
    }
    
    /// <summary>
    ///     Sets the native window <see cref="IPhotinoWindow.Height" /> in pixels.
    ///     Default is 0.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <seealso cref="UseOsDefaultSize" />
    /// <param name="height">Height in pixels</param>
    public IPhotinoWindow SetHeight(int height)
    {
        _logger.LogDebug(".SetHeight({Height})", height);
        
        Invoke(() =>
        {
            PhotinoNative.GetSize(InstanceHandle, out var width, out _ );
            PhotinoNative.SetSize(InstanceHandle, width, height);
        });
        
        return this;
    }
    
    /// <summary>
    ///     Sets the icon file for the native window title bar.
    ///     The file must be located on the local machine and cannot be a URL. The default is none.
    /// </summary>
    /// <remarks>
    ///     This only works on Windows and Linux.
    /// </remarks>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <exception cref="System.ArgumentException">Icon file: {value} does not exist.</exception>
    /// <param name="iconFilePath">The file path to the icon.</param>
    public IPhotinoWindow SetIconFile(string iconFilePath)
    {
        _logger.LogDebug(".SetIconFile({IconFile})", iconFilePath);

        if (IconFilePath == iconFilePath)
        {
            _logger.LogDebug("Icon file is already set to {IconFile}, skipping assignment", iconFilePath);
            return this;
        }
        if (!IconFileUtilities.IsValidIconFile(iconFilePath))
        {
            _logger.LogWarning("Icon file {IconFile} does not exist or is an invalid file path.", iconFilePath);
            return this;
        }
        
        IconFilePath = iconFilePath;
        Invoke(() => PhotinoNative.SetIconFile(InstanceHandle, iconFilePath));
        return this;
    }

    /// <summary>
    ///     Sets the native window to a new <see cref="IPhotinoWindow.Left" /> (X) coordinate in pixels.
    ///     Default is 0.
    /// </summary>
    /// <seealso cref="UseOsDefaultLocation" />
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="left">Position in pixels from the left (X).</param>
    public IPhotinoWindow SetLeft(int left)
    {
        _logger.LogDebug(".SetLeft({Left})", Left);
        
        Invoke(() =>
        {
            PhotinoNative.GetPosition(InstanceHandle, out var oldLeft, out var top);
            if (left == oldLeft) return;
            PhotinoNative.SetPosition(InstanceHandle, left, top);
        });
        return this;
    }

    /// <summary>
    ///     Sets whether the user can resize the native window.
    ///     Default is true.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="resizable">Whether the window is resizable</param>
    public IPhotinoWindow SetResizable(bool resizable)
    {
        _logger.LogDebug(".SetResizable({Resizable})", resizable);
        Invoke(() => PhotinoNative.SetResizable(InstanceHandle, resizable));
        return this;
    }

    /// <summary>
    ///     Sets the native window Size. This represents the <see cref="IPhotinoWindow.Width" /> and the
    ///     <see cref="IPhotinoWindow.Height" /> of the window in pixels.
    ///     The default Size is 0,0.
    /// </summary>
    /// <seealso cref="UseOsDefaultSize" />
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="size">Width &amp; Height</param>
    public IPhotinoWindow SetSize(Size size)
    {
        _logger.LogDebug(".SetSize({Size})", size);
        Invoke(() => PhotinoNative.SetSize(InstanceHandle, size.Width, size.Height));
        return this;
    }

    /// <summary>
    ///     Sets the native window Size. This represents the <see cref="IPhotinoWindow.Width" /> and the
    ///     <see cref="IPhotinoWindow.Height" /> of the window in pixels.
    ///     The default Size is 0,0.
    /// </summary>
    /// <seealso cref="UseOsDefaultSize" />
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="width">Width in pixels</param>
    /// <param name="height">Height in pixels</param>
    public IPhotinoWindow SetSize(int width, int height)
    {
        _logger.LogDebug(".SetSize({Width}, {Height})", width, height);
        
        Invoke(() => PhotinoNative.SetSize(InstanceHandle, width, height));
        return this;
    }

    /// <summary>
    ///     Sets the native window <see cref="IPhotinoWindow.Left" /> (X) and <see cref="IPhotinoWindow.Top" /> coordinates (Y)
    ///     in pixels.
    ///     Default is 0,0 that means the window will be aligned to the top-left edge of the screen.
    /// </summary>
    /// <seealso cref="UseOsDefaultLocation" />
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="location">Location as a <see cref="Point" /></param>
    public IPhotinoWindow SetLocation(Point location)
    {
        _logger.LogDebug(".SetLocation({Location})", location);
        Invoke(() =>
        {
            PhotinoNative.GetPosition(InstanceHandle, out var left, out var top);
            if (left == location.X && top == location.Y) return;
            PhotinoNative.SetPosition(InstanceHandle, location.X, location.Y);
        });
        return this;
    }

    /// <summary>
    ///     Sets whether the native window is maximized.
    ///     Default is false.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="maximized">Whether the window should be maximized.</param>
    public IPhotinoWindow SetMaximized(bool maximized)
    {
        _logger.LogDebug(".SetMaximized({Maximized})", maximized);
        Invoke(() => PhotinoNative.SetMaximized(InstanceHandle, maximized));
        return this;
    }

    ///<summary>Native window maximum Width and Height in pixels.</summary>
    public IPhotinoWindow SetMaxSize(int maxWidth, int maxHeight)
    {
        _logger.LogDebug(".SetMaxSize({MaxWidth}, {MaxHeight})", maxWidth, maxHeight);
        Invoke(() => PhotinoNative.SetMaxSize(InstanceHandle, maxWidth, maxHeight));
        return this;
    }

    ///<summary>Native window maximum Height in pixels.</summary>
    public IPhotinoWindow SetMaxHeight(int maxHeight)
    {
        _logger.LogDebug(".SetMaxHeight({MaxHeight})", maxHeight);
        MaxHeight = maxHeight;
        return this;
    }

    ///<summary>Native window maximum Width in pixels.</summary>
    public IPhotinoWindow SetMaxWidth(int maxWidth)
    {
        _logger.LogDebug(".SetMaxWidth({MaxWidth})", maxWidth);
        MaxWidth = maxWidth;
        return this;
    }

    /// <summary>
    ///     Sets whether the native window is minimized (hidden).
    ///     Default is false.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="minimized">Whether the window should be minimized.</param>
    public IPhotinoWindow SetMinimized(bool minimized)
    {
        _logger.LogDebug(".SetMinimized({Minimized})", minimized);
        Invoke(() => PhotinoNative.SetMinimized(InstanceHandle, minimized));
        return this;
    }

    ///<summary>Native window maximum Width and Height in pixels.</summary>
    public IPhotinoWindow SetMinSize(int minWidth, int minHeight)
    {
        _logger.LogDebug(".SetMinSize({MinWidth}, {MinHeight})", minWidth, minHeight);
        Invoke(() => PhotinoNative.SetMinSize(InstanceHandle, minWidth, minHeight));
        return this;
    }

    ///<summary>Native window maximum Height in pixels.</summary>
    public IPhotinoWindow SetMinHeight(int minHeight)
    {
        _logger.LogDebug(".SetMinHeight({MinHeight})", minHeight);
        SetMinSize(MinWidth, minHeight);
        return this;
    }

    ///<summary>Native window maximum Width in pixels.</summary>
    public IPhotinoWindow SetMinWidth(int minWidth)
    {
        _logger.LogDebug(".SetMinWidth({MinWidth})", minWidth);
        SetMinSize(minWidth, MinHeight);
        return this;
    }

    /// <summary>
    ///     Sets the native window <see cref="IPhotinoWindow.Title" />.
    ///     Default is "Photino".
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="title">Window title</param>
    public IPhotinoWindow SetTitle(string? title)
    {
        _logger.LogDebug(".SetTitle({Title})", title);
        
        Invoke(() => 
        {
            var ptr = PhotinoNative.GetTitle(InstanceHandle);
            var oldTitle = Marshal.PtrToStringAuto(ptr);
            if (title == oldTitle) return;
            if (IsLinuxPlatform && title?.Length > 31) title = title[..31]; // Due to Linux/Gtk platform limitations, the window title has to be no more than 31 chars
            PhotinoNative.SetTitle(InstanceHandle, title ?? string.Empty);
        });
        
        return this;
    }

    /// <summary>
    ///     Sets the native window <see cref="IPhotinoWindow.Top" /> (Y) coordinate in pixels.
    ///     Default is 0.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <seealso cref="UseOsDefaultLocation" />
    /// <param name="top">Position in pixels from the top (Y).</param>
    public IPhotinoWindow SetTop(int top)
    {
        _logger.LogDebug(".SetTop({Top})", top);
        Invoke(() =>
        {
            PhotinoNative.GetPosition(InstanceHandle, out var left, out var oldTop);
            if (top == oldTop) return;
            PhotinoNative.SetPosition(InstanceHandle, left, top);
        });
        return this;
    }

    /// <summary>
    ///     Sets whether the native window is always at the top of the z-order.
    ///     Default is false.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="topMost">Whether the window is at the top</param>
    public IPhotinoWindow SetTopMost(bool topMost)
    {
        _logger.LogDebug(".SetTopMost({TopMost})", topMost);
        Topmost = topMost;
        return this;
    }

    /// <summary>
    ///     Sets the native window width in pixels.
    ///     Default is 0.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <seealso cref="UseOsDefaultSize" />
    /// <param name="width">Width in pixels</param>
    public IPhotinoWindow SetWidth(int width)
    {
        _logger.LogDebug(".SetWidth({Width})", width);
        
        Invoke(() =>
        {
            PhotinoNative.GetSize(InstanceHandle, out _, out var height );
            PhotinoNative.SetSize(InstanceHandle,width, height);
        });
        return this;
    }

    /// <summary>
    ///     Sets the native browser control <see cref="IPhotinoWindow.Zoom" />.
    ///     Default is 100.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="zoom">Zoomlevel as integer</param>
    /// <example>100 = 100%, 50 = 50%</example>
    public IPhotinoWindow SetZoom(int zoom)
    {
        _logger.LogDebug(".SetZoom({Zoom})", zoom);
        Zoom = zoom;
        return this;
    }

    /// <summary>
    ///     When true, the native window starts up at the OS Default location.
    ///     Default is true.
    /// </summary>
    /// <remarks>
    ///     Overrides <see cref="IPhotinoWindow.Left" /> (X) and <see cref="IPhotinoWindow.Top" /> (Y) properties.
    /// </remarks>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="useOsDefault">Whether the OS Default should be used.</param>
    public IPhotinoWindow SetUseOsDefaultLocation(bool useOsDefault)
    {
        _logger.LogDebug(".SetUseOsDefaultLocation({UseOsDefault})", useOsDefault);
        UseOsDefaultLocation = useOsDefault;
        return this;
    }

    /// <summary>
    ///     When true, the native window starts at the OS Default size.
    ///     Default is true.
    /// </summary>
    /// <remarks>
    ///     Overrides <see cref="IPhotinoWindow.Height" /> and <see cref="IPhotinoWindow.Width" /> properties.
    /// </remarks>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="useOsDefault">Whether the OS Default should be used.</param>
    public IPhotinoWindow SetUseOsDefaultSize(bool useOsDefault)
    {
        _logger.LogDebug(".SetUseOsDefaultSize({UseOsDefault})", useOsDefault);
        UseOsDefaultSize = useOsDefault;
        return this;
    }

    /// <summary>
    ///     Set the runtime path for WebView2 so that developers can use Photino on Windows using the "Fixed Version" deployment
    ///     module of the WebView2 runtime.
    /// </summary>
    /// <remarks>
    ///     This only works on Windows.
    /// </remarks>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <seealso href="https://docs.microsoft.com/en-us/microsoft-edge/webview2/concepts/distribution" />
    /// <param name="data">Runtime path for WebView2</param>
    public IPhotinoWindow Win32SetWebView2Path(string data)
    {
        if (IsWindowsPlatform)
            Invoke(() => PhotinoNative.setWebView2RuntimePath_win32(_nativeType, data));
        else
            _logger.LogDebug("Win32SetWebView2Path is only supported on the Windows platform");

        return this;
    }

    /// <summary>
    ///     Clears the autofill data in the browser control.
    /// </summary>
    /// <remarks>
    ///     This method is only supported on the Windows platform.
    /// </remarks>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    public IPhotinoWindow ClearBrowserAutoFill()
    {
        if (IsWindowsPlatform)
            Invoke(() => PhotinoNative.ClearBrowserAutoFill(InstanceHandle));
        else
            _logger.LogWarning("ClearBrowserAutoFill is only supported on the Windows platform");

        return this;
    }

    //NON-FLUENT METHODS - CAN ONLY BE CALLED AFTER WINDOW IS INITIALIZED
    //ONE OF THESE 2 METHODS *MUST* BE CALLED TO CREATE THE WINDOW

    /// <summary>
    ///     Responsible for the initialization of the primary native window and remains in operation until the window is
    ///     closed.
    ///     This method is also applicable for initializing child windows, but in this case, it does not inhibit operation.
    /// </summary>
    /// <remarks>
    ///     The operation of the message loop is exclusive to the main native window only.
    /// </remarks>
    public void WaitForClose()
    {
        //fill in the fixed size array of custom scheme names
        var i = 0;
        foreach (var name in _customSchemes.Take(16))
        {
            _startupParameters.CustomSchemeNames[i] = name.Key;
            i++;
        }

        _startupParameters.NativeParent = Parent is PhotinoWindow parent
            ? parent.InstanceHandle 
            : IntPtr.Zero;

        if (PhotinoNativeParametersValidator.Validate(_startupParameters, _logger))
        {
            OnWindowCreating();
            try //All C++ exceptions will bubble up to here.
            {
                _nativeType = NativeLibrary.GetMainProgramHandle();

                if (IsWindowsPlatform)
                    Invoke(() => PhotinoNative.RegisterWin32(_nativeType));
                else if (IsMacOsPlatform)
                    Invoke(() => PhotinoNative.RegisterMac());

                Invoke(() => InstanceHandle = PhotinoNative.Ctor(ref _startupParameters));
            }
            catch (Exception ex)
            {
                var lastError = 0;
                if (IsWindowsPlatform)
                    lastError = Marshal.GetLastWin32Error();

                _logger.LogError(ex, "Error #{LastErrorCode} while creating native window", lastError);
                throw new ApplicationException($"Native code exception. Error # {lastError}  See inner exception for details.", ex);
            }
            OnWindowCreated();

            if (_messageLoopIsStarted) return;
            
            _messageLoopIsStarted = true;
            try
            {
                Invoke(() => PhotinoNative.WaitForExit(InstanceHandle));//start the message loop. there can only be 1 message loop for all windows.
            }
            catch (Exception ex)
            {
                var lastError = 0;
                if (IsWindowsPlatform)
                    lastError = Marshal.GetLastWin32Error();

                _logger.LogError(ex, "Error #{LastErrorCode} while creating native window", lastError);
                throw new ApplicationException($"Native code exception. Error # {lastError}  See inner exception for details.", ex);
            }
        }
        else
        {
            _logger.LogCritical("Startup Parameters Are Not Valid, please check the log file");
            throw new ArgumentException("Startup Parameters Are Not Valid, please check the log file");
        }
    }

    /// <summary>
    ///     Closes the native window.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    public void Close()
    {
        _logger.LogDebug(".Close()");
        if (IsNotInitialized())
            throw new ApplicationException("Close cannot be called until after the Photino window is initialized.");
        Invoke(() => PhotinoNative.Close(InstanceHandle));
    }

    /// <summary>
    ///     Send a message to the native window's native browser control's JavaScript context.
    /// </summary>
    /// <remarks>
    ///     In JavaScript, messages can be received via <code>window.external.receiveMessage(message)</code>
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="message">Message as string</param>
    public void SendWebMessage(string message)
    {
        _logger.LogDebug(".SendWebMessage({Message})", message);
        if (IsNotInitialized())
            throw new ApplicationException("SendWebMessage cannot be called until after the Photino window is initialized.");
        Invoke(() => PhotinoNative.SendWebMessage(InstanceHandle, message));
    }

    public async Task SendWebMessageAsync(string message)
    {
        await Task.Run(() =>
        {
            _logger.LogDebug(".SendWebMessage({Message})", message);
            if (IsNotInitialized())
                throw new ApplicationException("SendWebMessage cannot be called until after the Photino window is initialized.");
            Invoke(() => PhotinoNative.SendWebMessage(InstanceHandle, message));
        });
    }

    /// <summary>
    ///     Sends a native notification to the OS.
    ///     Sometimes referred to as Toast notifications.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">The title of the notification</param>
    /// <param name="body">The text of the notification</param>
    public void SendNotification(string title, string body)
    {
        _logger.LogDebug(".SendNotification({Title}, {Body})", title, body);
        if (IsNotInitialized())
            throw new ApplicationException("SendNotification cannot be called until after the Photino window is initialized.");
        Invoke(() => PhotinoNative.ShowNotification(InstanceHandle, title, body));
    }

    /// <summary>
    ///     Show an open file dialog native to the OS.
    /// </summary>
    /// <remarks>
    ///     Filter names are not used on macOS. Use async version for InfiniLore.Photino.Blazor as the synchronous version crashes.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">Title of the dialog</param>
    /// <param name="defaultPath">Default path. Defaults to <see cref="Environment.SpecialFolder.MyDocuments" /></param>
    /// <param name="multiSelect">Whether multiple selections are allowed</param>
    /// <param name="filters">Array of Extensions for filtering.</param>
    /// <returns>Array of file paths as strings</returns>
    public string?[] ShowOpenFile(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null)
    {
        return ShowOpenDialog(false, title, defaultPath, multiSelect, filters);
    }

    /// <summary>
    ///     Async version is required for InfiniLore.Photino.Blazor
    /// </summary>
    /// <remarks>
    ///     Filter names are not used on macOS. Use async version for InfiniLore.Photino.Blazor as the synchronous version crashes.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">Title of the dialog</param>
    /// <param name="defaultPath">Default path. Defaults to <see cref="Environment.SpecialFolder.MyDocuments" /></param>
    /// <param name="multiSelect">Whether multiple selections are allowed</param>
    /// <param name="filters">Array of Extensions for filtering.</param>
    /// <returns>Array of file paths as strings</returns>
    public async Task<string?[]> ShowOpenFileAsync(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null)
    {
        return await Task.Run(() => ShowOpenFile(title, defaultPath, multiSelect, filters));
    }

    /// <summary>
    ///     Show an open folder dialog native to the OS.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">Title of the dialog</param>
    /// <param name="defaultPath">Default path. Defaults to <see cref="Environment.SpecialFolder.MyDocuments" /></param>
    /// <param name="multiSelect">Whether multiple selections are allowed</param>
    /// <returns>Array of folder paths as strings</returns>
    public string?[] ShowOpenFolder(string title = "Select folder", string? defaultPath = null, bool multiSelect = false)
    {
        return ShowOpenDialog(true, title, defaultPath, multiSelect, null);
    }

    /// <summary>
    ///     Async version is required for InfiniLore.Photino.Blazor
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">Title of the dialog</param>
    /// <param name="defaultPath">Default path. Defaults to <see cref="Environment.SpecialFolder.MyDocuments" /></param>
    /// <param name="multiSelect">Whether multiple selections are allowed</param>
    /// <returns>Array of folder paths as strings</returns>
    public async Task<string?[]> ShowOpenFolderAsync(string title = "Choose file", string? defaultPath = null, bool multiSelect = false)
    {
        return await Task.Run(() => ShowOpenFolder(title, defaultPath, multiSelect));
    }

    /// <summary>
    ///     Show a save folder dialog native to the OS.
    /// </summary>
    /// <remarks>
    ///     Filter names are not used on macOS.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">Title of the dialog</param>
    /// <param name="defaultPath">Default path. Defaults to <see cref="Environment.SpecialFolder.MyDocuments" /></param>
    /// <param name="filters">Array of Extensions for filtering.</param>
    /// <returns></returns>
    public string? ShowSaveFile(string title = "Save file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null)
    {
        defaultPath ??= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        filters ??= Array.Empty<(string, string[])>();

        string? result = null;
        var nativeFilters = GetNativeFilters(filters);

        Invoke(() =>
        {
            var ptrResult = PhotinoNative.ShowSaveFile(InstanceHandle, title, defaultPath, nativeFilters, filters.Length);
            result = Marshal.PtrToStringAuto(ptrResult);
        });

        return result;
    }

    /// <summary>
    ///     Async version is required for InfiniLore.Photino.Blazor
    /// </summary>
    /// <remarks>
    ///     Filter names are not used on macOS.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">Title of the dialog</param>
    /// <param name="defaultPath">Default path. Defaults to <see cref="Environment.SpecialFolder.MyDocuments" /></param>
    /// <param name="filters">Array of Extensions for filtering.</param>
    /// <returns></returns>
    public async Task<string?> ShowSaveFileAsync(string title = "Choose file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null)
    {
        return await Task.Run(() => ShowSaveFile(title, defaultPath, filters));
    }

    /// <summary>
    ///     Show a message dialog native to the OS.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">Title of the dialog</param>
    /// <param name="text">Text of the dialog</param>
    /// <param name="buttons">Available interaction buttons <see cref="PhotinoDialogButtons" /></param>
    /// <param name="icon">Icon of the dialog <see cref="PhotinoDialogButtons" /></param>
    /// <returns>
    ///     <see cref="PhotinoDialogResult" />
    /// </returns>
    public PhotinoDialogResult ShowMessage(string title, string? text, PhotinoDialogButtons buttons = PhotinoDialogButtons.Ok, PhotinoDialogIcon icon = PhotinoDialogIcon.Info)
    {
        var result = PhotinoDialogResult.Cancel;
        Invoke(() => result = PhotinoNative.ShowMessage(InstanceHandle, title, text ?? string.Empty, buttons, icon));
        return result;
    }

    /// <summary>
    ///     Show a native open dialog.
    /// </summary>
    /// <param name="foldersOnly">Whether files are hidden</param>
    /// <param name="title">Title of the dialog</param>
    /// <param name="defaultPath">Default path. Defaults to <see cref="Environment.SpecialFolder.MyDocuments" /></param>
    /// <param name="multiSelect">Whether multiple selections are allowed</param>
    /// <param name="filters">Array of Extensions for filtering.</param>
    /// <returns>Array of paths</returns>
    private string?[] ShowOpenDialog(bool foldersOnly, string title, string? defaultPath, bool multiSelect, (string Name, string[] Extensions)[]? filters)
    {
        defaultPath ??= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        filters ??= Array.Empty<(string, string[])>();

        var results = Array.Empty<string?>();
        var nativeFilters = GetNativeFilters(filters, foldersOnly);

        Invoke(() =>
        {
            var ptrResults = foldersOnly ?
                PhotinoNative.ShowOpenFolder(InstanceHandle, title, defaultPath, multiSelect, out var resultCount) :
                PhotinoNative.ShowOpenFile(InstanceHandle, title, defaultPath, multiSelect, nativeFilters, nativeFilters.Length, out resultCount);
            if (resultCount == 0) return;

            var ptrArray = new IntPtr[resultCount];
            results = new string?[resultCount];
            Marshal.Copy(ptrResults, ptrArray, 0, resultCount);
            for (var i = 0; i < resultCount; i++)
            {
                results[i] = Marshal.PtrToStringAuto(ptrArray[i]);
            }
        });

        return results;
    }

    /// <summary>
    ///     Returns an array of strings for native filters
    /// </summary>
    /// <param name="filters"></param>
    /// <param name="empty"></param>
    /// <returns>String array of filters</returns>
    private static string[] GetNativeFilters((string Name, string[] Extensions)[] filters, bool empty = false)
    {
        var nativeFilters = Array.Empty<string>();
        if (!empty && filters is { Length: > 0 })
        {
            nativeFilters = IsMacOsPlatform ?
                filters.SelectMany(t => t.Extensions.Select(s => s == "*" ? s : s.TrimStart('*', '.'))).ToArray() :
                filters.Select(t => $"{t.Name}|{t.Extensions.Select(s => s.StartsWith('.') ? $"*{s}" : !s.StartsWith("*.") ? $"*.{s}" : s).Aggregate((e1, e2) => $"{e1};{e2}")}").ToArray();
        }
        return nativeFilters;
    }
    
        /// <summary>
    ///     Invokes registered user-defined handler methods when the native window's location changes.
    /// </summary>
    /// <param name="left">Position from left in pixels</param>
    /// <param name="top">Position from top in pixels</param>
    private void OnLocationChanged(int left, int top)
    {
        var location = new Point(left, top);
        WindowLocationChanged?.Invoke(this, location);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window's size changes.
    /// </summary>
    private void OnSizeChanged(int width, int height)
    {
        var size = new Size(width, height);
        WindowSizeChanged?.Invoke(this, size);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window focuses in.
    /// </summary>
    private void OnFocusIn()
    {
        WindowFocusIn?.Invoke(this, EventArgs.Empty);
    }
    

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is maximized.
    /// </summary>
    private void OnMaximized()
    {
        WindowMaximized?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is restored.
    /// </summary>
    private void OnRestored()
    {
        WindowRestored?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window focuses out.
    /// </summary>
    private void OnFocusOut()
    {
        WindowFocusOut?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is minimized.
    /// </summary>
    private void OnMinimized()
    {
        WindowMinimized?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window sends a message.
    /// </summary>
    private void OnWebMessageReceived(string message)
    {
        WebMessageReceived?.Invoke(this, message);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is about to close.
    /// </summary>
    private byte OnWindowClosing()
    {
        //C++ handles bool values as a single byte, C# uses 4 bytes
        byte noClose = 0;
        var doNotClose = WindowClosing?.Invoke(this, null);
        if (doNotClose ?? false)
            noClose = 1;

        return noClose;
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods before the native window is created.
    /// </summary>
    private void OnWindowCreating()
    {
        WindowCreating?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods after the native window is created.
    /// </summary>
    private void OnWindowCreated()
    {
        WindowCreated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Registers user-defined custom schemes (other than 'http', 'https' and 'file') and handler methods to receive
    ///     callbacks
    ///     when the native browser control encounters them.
    /// </summary>
    /// <remarks>
    ///     Only 16 custom schemes can be registered before initialization. Additional handlers can be added after
    ///     initialization.
    /// </remarks>
    /// <returns>
    ///     Returns the current <see cref="PhotinoWindow" /> instance.
    /// </returns>
    /// <param name="scheme">The custom scheme</param>
    /// <param name="handler">
    ///     <see cref="EventHandler" />
    /// </param>
    /// <exception cref="ArgumentException">Thrown if no scheme or handler was provided</exception>
    /// <exception cref="ApplicationException">Thrown if more than 16 custom schemes were set</exception>
    public IPhotinoWindow RegisterCustomSchemeHandler(string scheme, NetCustomSchemeDelegate handler)
    {
        if (string.IsNullOrWhiteSpace(scheme))
            throw new ArgumentException("A scheme must be provided. (for example 'app' or 'custom'");

        if (handler is null)
            throw new ArgumentException("A handler (method) with a signature matching NetCustomSchemeDelegate must be supplied.");

        scheme = scheme.ToLower();

        if (IsNotInitialized())
        {
            if (_customSchemes.Count > 15 && !_customSchemes.ContainsKey(scheme))
                throw new ApplicationException("No more than 16 custom schemes can be set prior to initialization. Additional handlers can be added after initialization.");
            _customSchemes.TryAdd(scheme, null);
        }
        else
        {
            PhotinoNative.AddCustomSchemeName(InstanceHandle, scheme);
        }

        _customSchemes[scheme] += handler;

        return this;
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods for user-defined custom schemes (other than 'http','https', and
    ///     'file')
    ///     when the native browser control encounters them.
    /// </summary>
    /// <param name="url">URL of the Scheme</param>
    /// <param name="numBytes">Number of bytes of the response</param>
    /// <param name="contentType">Content type of the response</param>
    /// <returns>
    ///     <see cref="IntPtr" />
    /// </returns>
    /// <exception cref="ApplicationException">
    ///     Thrown when the URL does not contain a colon.
    /// </exception>
    /// <exception cref="ApplicationException">
    ///     Thrown when no handler is registered.
    /// </exception>
    private IntPtr OnCustomScheme(string url, out int numBytes, out string? contentType)
    {
        contentType = null;
        numBytes = 0;
        var colonPos = url.IndexOf(':');

        if (colonPos < 0)
            throw new ApplicationException($"URL: '{url}' does not contain a colon.");

        var scheme = url[..colonPos].ToLower();

        if (!_customSchemes.TryGetValue(scheme, out var handler)) {
            throw new ApplicationException($"A handler for the custom scheme '{scheme}' has not been registered.");
        }
        
        var responseStream = handler?.Invoke(this, scheme, url, out contentType);

        if (responseStream is null)
        {
            // Webview should pass through request to normal handlers (e.g., network)
            // or handle as 404 otherwise
            return 0;
        }

        // Read the stream into memory and serve the bytes
        // In the future, it would be possible to pass the stream through into C++
        using (responseStream)
        using (var ms = new MemoryStream())
        {
            responseStream.CopyTo(ms);

            numBytes = (int)ms.Position;
            var buffer = Marshal.AllocHGlobal(numBytes);
            Marshal.Copy(ms.GetBuffer(), 0, buffer, numBytes);
            return buffer;
        }
    }
    #endregion
}
