using Microsoft.Extensions.Logging;

namespace InfiniLore.Photino.NET;

public partial class PhotinoWindow
{
    private bool IsNotInitialized() => _nativeInstance == IntPtr.Zero;
    
    private void ThrowIfNotInitialized()
    {
        if (_nativeInstance != IntPtr.Zero) return;
        _logger.LogCritical("The Photino window hasn't been initialized yet.");
        throw new ApplicationException("The Photino window hasn't been initialized yet.");
    }
    
    private void ThrowIfNotWindowsEnvironment()
    {
        if (IsWindowsPlatform) return;
        _logger.LogCritical("This is only supported on Windows.");
        throw new PlatformNotSupportedException("This is only supported on Windows.");
    }
}
