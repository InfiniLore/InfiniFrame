// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowFeatureLifecycle {
    internal void Initialize();
    
    void WaitForClose();
    ValueTask WaitForCloseAsync(CancellationToken ct = default);
    
    void Close();
    ValueTask CloseAsync(CancellationToken ct = default);
    
    internal void MarkAsClosed();
    internal void CleanupNativeHandle();
    
    bool IsClosedOrClosing();
}
