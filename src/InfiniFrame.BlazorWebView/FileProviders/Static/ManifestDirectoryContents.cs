// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections;
using Microsoft.Extensions.FileProviders;

namespace InfiniFrame.BlazorWebView.FileProviders;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class ManifestDirectoryContents(IReadOnlyList<IFileInfo> entries) : IDirectoryContents {
    public bool Exists => true;

    public IEnumerator<IFileInfo> GetEnumerator() => entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
