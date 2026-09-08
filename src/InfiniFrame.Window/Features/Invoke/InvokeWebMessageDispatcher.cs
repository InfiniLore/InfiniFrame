// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Window.Features.WebMessaging.Handlers;

namespace InfiniFrame.Window.Features.Invoke;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class InvokeWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<IInvokeInfiniFrameWindowFeature> {
    public override string FeatureName => "invoke";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected override IInvokeInfiniFrameWindowFeature SelectFeature(IInfiniFrameWindowFeatures features)
        => features.Invoke;
}
