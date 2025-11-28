// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniWindowBuilderExtensions {
    extension<T>(T builder) where T : IInfiniFrameWindowBuilder {
        public T SetMediaAutoplayEnabled(bool enable) {
            builder.Configuration.MediaAutoplayEnabled = enable;
            return builder;
        }

        /// <summary>
        ///     Sets the user agent on the browser control at initialization.
        /// </summary>
        public T SetUserAgent(string userAgent) {
            builder.Configuration.UserAgent = userAgent;
            return builder;
        }

        /// <summary>
        ///     Sets FileSystemAccessEnabled on the browser control at initialization.
        /// </summary>
        public T SetFileSystemAccessEnabled(bool enable) {
            builder.Configuration.FileSystemAccessEnabled = enable;
            return builder;
        }

        /// <summary>
        ///     Sets WebSecurityEnabled on the browser control at initialization.
        /// </summary>
        public T SetWebSecurityEnabled(bool enable) {
            builder.Configuration.WebSecurityEnabled = enable;
            return builder;
        }

        /// <summary>
        ///     Sets JavascriptClipboardAccessEnabled on the browser control at initialization.
        /// </summary>
        public T SetJavascriptClipboardAccessEnabled(bool enable) {
            builder.Configuration.JavascriptClipboardAccessEnabled = enable;
            return builder;
        }

        /// <summary>
        ///     Sets MediaStreamEnabled on the browser control at initialization.
        /// </summary>
        public T SetMediaStreamEnabled(bool enable) {
            builder.Configuration.MediaStreamEnabled = enable;
            return builder;
        }

        /// <summary>
        ///     Sets SmoothScrollingEnabled on the browser control at initialization.
        /// </summary>
        public T SetSmoothScrollingEnabled(bool enable = true) {
            builder.Configuration.SmoothScrollingEnabled = enable;
            return builder;
        }

        /// <summary>
        ///     Sets IgnoreCertificateErrorsEnabled on the browser control at initialization.
        /// </summary>
        public T SetIgnoreCertificateErrorsEnabled(bool enable = true) {
            builder.Configuration.IgnoreCertificateErrorsEnabled = enable;
            return builder;
        }

        /// <summary>
        ///     Sets NotificationsEnabled on the browser control at initialization.
        /// </summary>
        /// <remarks>
        ///     Only available on Windows.
        /// </remarks>
        /// <exception cref="ApplicationException">
        ///     Thrown if a platform is not Windows.
        /// </exception>
        public T SetNotificationsEnabled(bool enable = true) {
            builder.Configuration.NotificationsEnabled = enable;
            return builder;
        }

        /// <summary>
        ///     Gets or Sets whether the native browser control grants all requests for access to local resources
        ///     such as the user's camera and microphone. By default, this is set to true.
        /// </summary>
        /// <remarks>
        ///     This only works on Windows.
        /// </remarks>
        public T GrantBrowserPermissions(bool enable = true) {
            builder.Configuration.GrantBrowserPermissions = enable;
            return builder;
        }

        /// <summary>
        ///     Sets IgnoreCertificateErrorsEnabled on the browser control at initialization.
        /// </summary>
        public T SetHeight(int value)
            => SetSize(builder, builder.Configuration.Width, value);

        /// <summary>
        ///     Sets IgnoreCertificateErrorsEnabled on the browser control at initialization.
        /// </summary>
        /// <remarks>
        ///     This only works on Windows and Linux.
        /// </remarks>
        /// <value>
        ///     The file path to the icon.
        /// </value>
        public T SetIconFile(string? iconFilePath) {
            if (!IconFileUtilities.IsValidIconFile(iconFilePath)) return builder;

            builder.Configuration.IconFilePath = iconFilePath;
            return builder;
        }

        /// <summary>
        ///     Sets Location on the browser control at initialization.
        /// </summary>
        public T SetLocation(int left, int top) {
            builder.Configuration.Left = left;
            builder.Configuration.Top = top;

            builder.Configuration.UseOsDefaultLocation = false;
            builder.Configuration.Centered = false;
            return builder;
        }

        /// <summary>
        ///     Sets Location on the browser control at initialization.
        /// </summary>
        public T SetLocation(Point location)
            => SetLocation(builder, location.X, location.Y);

        /// <summary>
        ///     Sets Minimized on the browser control at initialization.
        /// </summary>
        public T SetMinimized(bool minimized) {
            builder.Configuration.Minimized = minimized;
            return builder;
        }

        /// <summary>
        ///     Sets Maximized on the browser control at initialization.
        /// </summary>
        public T SetMaximized(bool maximized) {
            builder.Configuration.Maximized = maximized;
            return builder;
        }

        /// <summary>
        ///     Sets MaxWidth on the browser control at initialization.
        /// </summary>
        public T SetMaxWidth(int value) {
            builder.Configuration.MaxWidth = value;
            return builder;
        }

        /// <summary>
        ///     Sets MaxHeight on the browser control at initialization.
        /// </summary>
        public T SetMaxHeight(int value) {
            builder.Configuration.MaxHeight = value;
            return builder;
        }

        /// <summary>
        ///     Sets MinWidth on the browser control at initialization.
        /// </summary>
        public T SetMinWidth(int value) {
            builder.Configuration.MinWidth = Math.Max(0, value);
            return builder;
        }

        /// <summary>
        ///     Sets MinHeight on the browser control at initialization.
        /// </summary>
        public T SetMinHeight(int value) {
            builder.Configuration.MinHeight = Math.Max(0, value);
            return builder;
        }

        /// <summary>
        /// Sets the minimum size constraints for the window.
        /// </summary>
        /// <param name="width">The minimum width of the window, must be non-negative.</param>
        /// <param name="height">The minimum height of the window, must be non-negative.</param>
        /// <returns>
        /// Returns the updated window builder instance to facilitate method chaining.
        /// </returns>
        public T SetMinSize(int width, int height) {
            builder.Configuration.MinHeight = Math.Max(0, height);
            builder.Configuration.MinWidth = Math.Max(0, width);

            return builder;
        }

        /// <summary>
        /// Sets the minimum width and height for the browser window.
        /// </summary>
        /// <param name="minSize">The <c>Size</c> object containing the minimum width and height values.</param>
        /// <returns>
        /// The updated builder instance with the minimum size applied.
        /// </returns>
        public T SetMinSize(Size minSize) 
            => SetMinSize(builder, minSize.Width, minSize.Height);

        /// <summary>
        ///     Sets FullScreen on the browser control at initialization.
        /// </summary>
        public T SetFullScreen(bool fullscreen) {
            builder.Configuration.FullScreen = fullscreen;
            return builder;
        }
        
        /// <summary>
        ///     Sets Resizable on the browser control at initialization.
        /// </summary>
        public T SetResizable(bool resizable) {
            builder.Configuration.Resizable = resizable;
            return builder;
        }
        
        /// <summary>
        ///     Sets Width on the browser control at initialization.
        /// </summary>
        public T SetWidth(int value) 
            => SetSize(builder, value, builder.Configuration.Height);
        
        /// <summary>
        ///     Sets Size on the browser control at initialization.
        /// </summary>
        public T SetSize(int width, int height) {
            builder.Configuration.Width = Math.Max(0, width);
            builder.Configuration.Height = Math.Max(0, height);

            builder.Configuration.UseOsDefaultSize = false;
            builder.Configuration.Centered = false;
            return builder;
        }
        
        /// <summary>
        ///     Sets Size on the browser control at initialization.
        /// </summary>
        public T SetSize(Size size) 
            => SetSize(builder, size.Width, size.Height);
        
        /// <summary>
        ///     Sets BrowserControlInitParameters on the browser control at initialization.
        ///     <remarks>
        ///         WINDOWS: WebView2 specific string. Space separated.
        ///         https://peter.sh/experiments/chromium-command-line-switches/
        ///         https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2environmentoptions.additionalbrowserarguments?view=webview2-dotnet-1.0.1938.49
        ///         viewFallbackFrom=webview2-dotnet-1.0.1901.177view%3Dwebview2-1.0.1901.177
        ///         https://www.chromium.org/developers/how-tos/run-chromium-with-flags/
        ///         LINUX: Webkit2Gtk specific string. Enter parameter names and values as JSON string.
        ///         e.g. { "set_enable_encrypted_media": true }
        ///         https://webkitgtk.org/reference/webkit2gtk/2.5.1/WebKitSettings.html
        ///         https://lazka.github.io/pgi-docs/WebKit2-4.0/classes/Settings.html
        ///         MAC: Webkit specific string. Enter parameter names and values as JSON string.
        ///         e.g. { "minimumFontSize": 8 }
        ///         https://developer.apple.com/documentation/webkit/wkwebviewconfiguration?language=objc
        ///         https://developer.apple.com/documentation/webkit/wkpreferences?language=objc
        ///     </remarks>
        /// </summary>
        public T SetBrowserControlInitParameters(string? parameters) {
            builder.Configuration.BrowserControlInitParameters = parameters;
            return builder;
        }
        
        /// <summary>
        ///     Sets TemporaryFilesPath on the browser control at initialization.
        /// </summary>
        public T SetTemporaryFilesPath(string? path) {
            builder.Configuration.TemporaryFilesPath = path;
            return builder;
        }
        
        /// <summary>
        ///     Sets NotificationRegistrationId on the browser control at initialization.
        /// </summary>
        /// <remarks>
        ///     Only available on Windows.
        /// </remarks>
        /// <exception cref="ApplicationException">
        ///     Thrown if a platform is not Windows.
        /// </exception>
        public T SetNotificationRegistrationId(string? id) {
            builder.Configuration.NotificationRegistrationId = id;
            return builder;
        }
        
        /// <summary>
        ///     Sets Title on the browser control at initialization.
        /// </summary>
        public T SetTitle(string? title) {
            builder.Configuration.Title = title ?? string.Empty;
            return builder;
        }
        
        /// <summary>
        ///     Sets TopMost on the browser control at initialization.
        /// </summary>
        public T SetTopMost(bool topmost) {
            builder.Configuration.TopMost = topmost;
            return builder;
        }
        
        /// <summary>
        ///     Sets UseOsDefaultLocation on the browser control at initialization.
        /// </summary>
        public T SetUseOsDefaultLocation(bool useOsDefaultLocation) {
            builder.Configuration.UseOsDefaultLocation = useOsDefaultLocation;
            return builder;
        }
        
        /// <summary>
        ///     Sets UseOsDefaultSize on the browser control at initialization.
        /// </summary>
        public T SetUseOsDefaultSize(bool useOsDefaultSize) {
            builder.Configuration.UseOsDefaultSize = useOsDefaultSize;
            return builder;
        }

        /// <summary>
        /// Sets the zoom level for the browser control.
        /// </summary>
        /// <param name="zoom">
        /// The zoom level to be applied.
        /// This value directly adjusts the scale of the browser content.
        /// </param>
        /// <returns>
        /// The current instance of the builder, allowing for method chaining.
        /// </returns>
        public T SetZoom(int zoom) {
            builder.Configuration.Zoom = zoom;
            return builder;
        }

        /// <summary>
        /// Enables or disables the zoom functionality for the browser control.
        /// </summary>
        /// <param name="isEnabled">
        /// A boolean value indicating whether zoom functionality should be enabled (true) or disabled (false).
        /// </param>
        /// <returns>
        /// The builder instance for method chaining.
        /// </returns>
        public T SetZoomEnabled(bool isEnabled) {
            builder.Configuration.ZoomEnabled = isEnabled;
            return builder;
        }

        /// <summary>
        /// Sets the start URL for the browser control at initialization.
        /// </summary>
        /// <param name="url">The URL to be set as the start URL. Can be null if no start URL is desired.</param>
        /// <returns>The instance of the builder for chaining further configuration.</returns>
        public T SetStartUrl(string? url) {
            builder.Configuration.StartUrl = url;
            return builder;
        }

        /// <summary>
        /// Sets the start URL for the browser control during its initialization.
        /// </summary>
        /// <param name="url">
        /// The URL to set as a starting point for the browser control. If null, no start URL is configured.
        /// </param>
        /// <returns>
        /// The builder instance with the updated start URL configuration.
        /// </returns>
        public T SetStartUrl(Uri? url) {
            builder.Configuration.StartUrl = url?.ToString();
            return builder;
        }

        /// <summary>
        /// Sets the start string that will be used during the initialization of the window.
        /// </summary>
        /// <param name="startString">The start string to initialize the window with. Can be null.</param>
        /// <returns>The updated instance of the builder.</returns>
        public T SetStartString(string? startString) {
            builder.Configuration.StartString = startString;
            return builder;
        }

        /// <summary>
        /// Configures whether the window should be displayed without standard window decorations (chromeless mode).
        /// </summary>
        /// <param name="chromeless">
        /// A boolean value indicating if chromeless mode should be enabled.
        /// If true, the window will not have standard decorations like borders or title bars; otherwise, it will be standard.
        /// </param>
        /// <returns>
        /// The modified instance of the builder, allowing for further configuration chaining.
        /// </returns>
        public T SetChromeless(bool chromeless) {
            builder.Configuration.Chromeless = chromeless;

            if (!OperatingSystem.IsWindows()) return builder;

            // Overrides the os defaults for you, as it does not work together on windows with chromeless
            builder.Configuration.UseOsDefaultLocation = !chromeless && builder.Configuration.UseOsDefaultLocation;
            builder.Configuration.UseOsDefaultSize = !chromeless && builder.Configuration.UseOsDefaultSize;
            builder.Configuration.Resizable = !chromeless && builder.Configuration.Resizable;

            return builder;
        }

        /// <summary>
        /// Sets the transparency state of the window.
        /// </summary>
        /// <param name="transparent">A boolean indicating whether the window should be transparent.</param>
        /// <returns>The current window builder instance for method chaining.</returns>
        public T SetTransparent(bool transparent) {
            builder.Configuration.Transparent = transparent;
            return builder;
        }
        
        /// <summary>
        ///     Sets SetCentered on the browser control at initialization.
        /// </summary>
        public T Center(bool enable = true) {
            builder.Configuration.Centered = enable;
            return builder;
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
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="scheme">The custom scheme</param>
        /// <param name="handler">
        ///     <see cref="EventHandler" />
        /// </param>
        /// <exception cref="ArgumentException">Thrown if no scheme or handler was provided</exception>
        /// <exception cref="ApplicationException">Thrown if more than 16 custom schemes were set</exception>
        public T RegisterCustomSchemeHandler(string scheme, NetCustomSchemeDelegate handler) {
            if (string.IsNullOrWhiteSpace(scheme)) throw new ArgumentException("A scheme must be provided. (for example 'app' or 'custom'");
            if (handler is null) throw new ArgumentException("A handler (method) with a signature matching NetCustomSchemeDelegate must be supplied.");

            scheme = scheme.ToLower();

            if (builder.CustomSchemeHandlers.Count > 15 && !builder.CustomSchemeHandlers.ContainsKey(scheme)) throw new ApplicationException("No more than 16 custom schemes can be set prior to initialization. Additional handlers can be added after initialization.");

            builder.CustomSchemeHandlers.TryAdd(scheme, null);
            builder.CustomSchemeHandlers[scheme] = handler;

            builder.Configuration.CustomSchemeNames.Add(scheme);

            return builder;
        }

        /// <summary>
        /// Sets the left position of the window and updates its configuration.
        /// </summary>
        /// <param name="left">
        /// An integer value specifying the left position of the window in pixels.
        /// </param>
        /// <returns>
        /// The builder instance with the updated configuration for chaining additional methods.
        /// </returns>
        public T SetLeft(int left) {
            builder.Configuration.Left = left;

            builder.Configuration.UseOsDefaultLocation = false;
            builder.Configuration.Centered = false;
            return builder;
        }

        /// <summary>
        /// Sets the top position of the window on the screen.
        /// </summary>
        /// <param name="top">The top position in pixels.</param>
        /// <returns>The updated window builder instance.</returns>
        public T SetTop(int top) {
            builder.Configuration.Top = top;

            builder.Configuration.UseOsDefaultLocation = false;
            builder.Configuration.Centered = false;
            return builder;
        }

        /// <summary>
        /// Enables or disables the context menu for the browser control.
        /// </summary>
        /// <param name="enabled">
        /// A boolean value indicating whether the context menu should be enabled.
        /// Set to <c>true</c> to enable, or <c>false</c> to disable the context menu.
        /// </param>
        /// <return>
        /// The updated instance of the window builder with the context menu setting applied.
        /// </return>
        public T SetContextMenuEnabled(bool enabled) {
            builder.Configuration.ContextMenuEnabled = enabled;
            return builder;
        }

        /// <summary>
        /// Enables or disables the Developer Tools in the browser control.
        /// </summary>
        /// <param name="enabled">A boolean value indicating whether Developer Tools should be enabled (true) or disabled (false).</param>
        /// <returns>The current instance of the builder, allowing for method chaining.</returns>
        public T SetDevToolsEnabled(bool enabled) {
            builder.Configuration.DevToolsEnabled = enabled;
            return builder;
        }

        /// <summary>
        /// Sets the maximum size for the window by specifying both width and height in pixels.
        /// </summary>
        /// <param name="width">The maximum width of the window in pixels.</param>
        /// <param name="height">The maximum height of the window in pixels.</param>
        /// <returns>The updated window builder instance.</returns>
        public T SetMaxSize(int width, int height) {
            builder.Configuration.MaxWidth = width;
            builder.Configuration.MaxHeight = height;

            return builder;
        }

        /// <summary>
        /// Sets the maximum size of the window for the builder.
        /// </summary>
        /// <param name="size">A <see cref="Size"/> object representing the maximum dimensions of the window in pixels.</param>
        /// <returns>
        /// An instance of the builder with the maximum size configured.
        /// </returns>
        public T SetMaxSize(Size size)
            => SetMaxSize(builder, size.Width, size.Height);
    }

}
