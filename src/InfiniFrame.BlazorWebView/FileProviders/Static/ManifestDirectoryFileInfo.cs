// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.FileProviders;

namespace InfiniFrame.BlazorWebView.FileProviders;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class ManifestDirectoryFileInfo(string name) : IFileInfo {
    public bool Exists => true;
    public long Length => -1;
    public string PhysicalPath => string.Empty;
    public string Name => name;
    public DateTimeOffset LastModified => DateTimeOffset.MinValue;
    public bool IsDirectory => true;
    public Stream CreateReadStream() => throw new InvalidOperationException("Cannot create stream for a directory.");
}
