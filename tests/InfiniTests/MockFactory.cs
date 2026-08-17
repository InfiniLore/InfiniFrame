// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class MockFactory
{
    public static Mock<InfiniFrame.IInfiniFrameWindow> CreateWindowMock() => Mock.Of<InfiniFrame.IInfiniFrameWindow>();
    public static Mock<InfiniFrame.IInfiniFrameWindowFeatures> CreateFeaturesMock() => Mock.Of<InfiniFrame.IInfiniFrameWindowFeatures>();
    public static Mock<InfiniFrame.IWebMessagingInfiniFrameWindowFeature> CreateWebMessagingMock() => Mock.Of<InfiniFrame.IWebMessagingInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.ILifecycleInfiniFrameWindowFeature> CreateLifecycleMock() => Mock.Of<InfiniFrame.ILifecycleInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.IBrowserInfiniFrameWindowFeature> CreateBrowserMock() => Mock.Of<InfiniFrame.IBrowserInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.IDebuggingInfiniFrameWindowFeature> CreateDebuggingMock() => Mock.Of<InfiniFrame.IDebuggingInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.IDecorationsInfiniFrameWindowFeature> CreateDecorationsMock() => Mock.Of<InfiniFrame.IDecorationsInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.IFilePickerDialogsInfiniFrameWindowFeature> CreateFilePickerDialogsMock() => Mock.Of<InfiniFrame.IFilePickerDialogsInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.IMonitorsInfiniFrameWindowFeature> CreateMonitorsMock() => Mock.Of<InfiniFrame.IMonitorsInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.INotificationsInfiniFrameWindowFeature> CreateNotificationsMock() => Mock.Of<InfiniFrame.INotificationsInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.IPageNavigationInfiniFrameWindowFeature> CreatePageNavigationMock() => Mock.Of<InfiniFrame.IPageNavigationInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.IPositionInfiniFrameWindowFeature> CreatePositionMock() => Mock.Of<InfiniFrame.IPositionInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.ISizeInfiniFrameWindowFeature> CreateSizeMock() => Mock.Of<InfiniFrame.ISizeInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.IStateInfiniFrameWindowFeature> CreateStateMock() => Mock.Of<InfiniFrame.IStateInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.IInvokeInfiniFrameWindowFeature> CreateInvokeMock() => Mock.Of<InfiniFrame.IInvokeInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.IInfiniFrameWindowBuilder> CreateWindowBuilderMock() => Mock.Of<InfiniFrame.IInfiniFrameWindowBuilder>();
    public static Mock<InfiniFrame.IInfiniFrameEvents> CreateEventsMock() => Mock.Of<InfiniFrame.IInfiniFrameEvents>();
    public static Mock<InfiniFrame.IInfiniFrameEventsStore> CreateEventsStoreMock() => Mock.Of<InfiniFrame.IInfiniFrameEventsStore>();
    public static Mock<InfiniFrame.IDragDropInfiniFrameWindowFeature> CreateDragDropMock() => Mock.Of<InfiniFrame.IDragDropInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.ITaskbarInfiniFrameWindowFeature> CreateTaskbarMock() => Mock.Of<InfiniFrame.ITaskbarInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.IMenuInfiniFrameWindowFeature> CreateMenuMock() => Mock.Of<InfiniFrame.IMenuInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.IJavaScriptInfiniFrameWindowFeature> CreateJavaScriptMock() => Mock.Of<InfiniFrame.IJavaScriptInfiniFrameWindowFeature>();
    public static Mock<InfiniFrame.IInfiniFrameWindowConfiguration> CreateWindowConfigurationMock() => Mock.Of<InfiniFrame.IInfiniFrameWindowConfiguration>();
    public static Mock<Microsoft.Extensions.Logging.ILogger<T>> CreateLoggerMock<T>() => Mock.Of<Microsoft.Extensions.Logging.ILogger<T>>();
    public static Mock<Microsoft.AspNetCore.Components.Dispatcher> CreateDispatcherMock() => Mock.Of<Microsoft.AspNetCore.Components.Dispatcher>();
    public static Mock<InfiniFrame.BlazorWebView.IInfiniFrameWebViewManager> CreateWebViewManagerMock() => Mock.Of<InfiniFrame.BlazorWebView.IInfiniFrameWebViewManager>();
    public static Mock<InfiniFrame.NativeBridge.Delegates.CppReleaseCustomSchemeResponseDelegate> CreateReleaseDelegateMock() => Mock.Of<InfiniFrame.NativeBridge.Delegates.CppReleaseCustomSchemeResponseDelegate>();
    public static Mock<IServiceProvider> CreateServiceProviderMock() => Mock.Of<IServiceProvider>();
    public static Mock<IDisposable> CreateDisposableMock() => Mock.Of<IDisposable>();
    public static Mock<FluentValidation.IValidator<InfiniFrame.NativeBridge.Parameters.InfiniFrameNativeParameters>> CreateValidatorMock() => Mock.Of<FluentValidation.IValidator<InfiniFrame.NativeBridge.Parameters.InfiniFrameNativeParameters>>();
}
