// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using InfiniFrame.NativeBridge.Delegates;
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class MockFactory {
    public static Mock<IInfiniFrameWindow> CreateWindowMock() => Mock.Of<IInfiniFrameWindow>();
    public static Mock<IInfiniFrameWindowFeatures> CreateFeaturesMock() => Mock.Of<IInfiniFrameWindowFeatures>();
    public static Mock<IWebMessagingInfiniFrameWindowFeature> CreateWebMessagingMock() => Mock.Of<IWebMessagingInfiniFrameWindowFeature>();
    public static Mock<ILifecycleInfiniFrameWindowFeature> CreateLifecycleMock() => Mock.Of<ILifecycleInfiniFrameWindowFeature>();
    public static Mock<IBrowserInfiniFrameWindowFeature> CreateBrowserMock() => Mock.Of<IBrowserInfiniFrameWindowFeature>();
    public static Mock<IDebuggingInfiniFrameWindowFeature> CreateDebuggingMock() => Mock.Of<IDebuggingInfiniFrameWindowFeature>();
    public static Mock<IDecorationsInfiniFrameWindowFeature> CreateDecorationsMock() => Mock.Of<IDecorationsInfiniFrameWindowFeature>();
    public static Mock<IFilePickerDialogsInfiniFrameWindowFeature> CreateFilePickerDialogsMock() => Mock.Of<IFilePickerDialogsInfiniFrameWindowFeature>();
    public static Mock<IMonitorsInfiniFrameWindowFeature> CreateMonitorsMock() => Mock.Of<IMonitorsInfiniFrameWindowFeature>();
    public static Mock<INotificationsInfiniFrameWindowFeature> CreateNotificationsMock() => Mock.Of<INotificationsInfiniFrameWindowFeature>();
    public static Mock<IPageNavigationInfiniFrameWindowFeature> CreatePageNavigationMock() => Mock.Of<IPageNavigationInfiniFrameWindowFeature>();
    public static Mock<IPositionInfiniFrameWindowFeature> CreatePositionMock() => Mock.Of<IPositionInfiniFrameWindowFeature>();
    public static Mock<ISizeInfiniFrameWindowFeature> CreateSizeMock() => Mock.Of<ISizeInfiniFrameWindowFeature>();
    public static Mock<IStateInfiniFrameWindowFeature> CreateStateMock() => Mock.Of<IStateInfiniFrameWindowFeature>();
    public static Mock<IInvokeInfiniFrameWindowFeature> CreateInvokeMock() => Mock.Of<IInvokeInfiniFrameWindowFeature>();
    public static Mock<IInfiniFrameWindowBuilder> CreateWindowBuilderMock() => Mock.Of<IInfiniFrameWindowBuilder>();
    public static Mock<IInfiniFrameEvents> CreateEventsMock() => Mock.Of<IInfiniFrameEvents>();
    public static Mock<IInfiniFrameEventsStore> CreateEventsStoreMock() => Mock.Of<IInfiniFrameEventsStore>();
    public static Mock<IDragDropInfiniFrameWindowFeature> CreateDragDropMock() => Mock.Of<IDragDropInfiniFrameWindowFeature>();
    public static Mock<ITaskbarInfiniFrameWindowFeature> CreateTaskbarMock() => Mock.Of<ITaskbarInfiniFrameWindowFeature>();
    public static Mock<IMenuInfiniFrameWindowFeature> CreateMenuMock() => Mock.Of<IMenuInfiniFrameWindowFeature>();
    public static Mock<IJavaScriptInfiniFrameWindowFeature> CreateJavaScriptMock() => Mock.Of<IJavaScriptInfiniFrameWindowFeature>();
    public static Mock<IInfiniFrameWindowConfiguration> CreateWindowConfigurationMock() => Mock.Of<IInfiniFrameWindowConfiguration>();
    public static Mock<ILogger<T>> CreateLoggerMock<T>() => Mock.Of<ILogger<T>>();
    public static Mock<Dispatcher> CreateDispatcherMock() => Mock.Of<Dispatcher>();
    public static Mock<IInfiniFrameWebViewManager> CreateWebViewManagerMock() => Mock.Of<IInfiniFrameWebViewManager>();
    public static Mock<CppReleaseCustomSchemeResponseDelegate> CreateReleaseDelegateMock() => Mock.Of<CppReleaseCustomSchemeResponseDelegate>();
    public static Mock<IServiceProvider> CreateServiceProviderMock() => Mock.Of<IServiceProvider>();
    public static Mock<IDisposable> CreateDisposableMock() => Mock.Of<IDisposable>();
    public static Mock<IValidator<InfiniFrameNativeParameters>> CreateValidatorMock() => Mock.Of<IValidator<InfiniFrameNativeParameters>>();
}
