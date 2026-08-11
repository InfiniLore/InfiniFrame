// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace InfiniFrame.StaticAssets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class DisposableCompositeFileProvider(IList<IFileProvider> providers, PhysicalFileProvider physicalProvider) : IFileProvider, IDisposable {
    private readonly CompositeFileProvider _inner = new(providers);

    public IDirectoryContents GetDirectoryContents(string subpath)
        => _inner.GetDirectoryContents(subpath);

    public IFileInfo GetFileInfo(string subpath)
        => _inner.GetFileInfo(subpath);

    public IChangeToken Watch(string filter)
        => _inner.Watch(filter);

    public void Dispose() {
        physicalProvider.Dispose();
        foreach (IFileProvider provider in providers) {
            if (provider != physicalProvider && provider is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
}
