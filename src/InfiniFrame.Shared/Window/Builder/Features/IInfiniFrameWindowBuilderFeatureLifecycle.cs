// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilderFeatureLifecycle {
    internal void Initialize();
    
    void WaitForClose();
    ValueTask WaitForCloseAsync(CancellationToken ct = default);
    
    void Close();
    ValueTask CloseAsync(CancellationToken ct = default);
    
    internal void MarkAsClosed();
    
    bool IsClosedOrClosing();
}
