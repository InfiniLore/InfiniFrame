// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeaturesTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Record_ShouldStoreAllFeatures(CancellationToken ct = default) {
        // Arrange
        Mock<IDebuggingInfiniFrameWindowFeature> debugging = MockFactory.CreateDebuggingMock();
        Mock<ILifecycleInfiniFrameWindowFeature> lifecycle = MockFactory.CreateLifecycleMock();
        Mock<IInvokeInfiniFrameWindowFeature> invoke = MockFactory.CreateInvokeMock();
        Mock<IWebMessagingInfiniFrameWindowFeature> webMessaging = MockFactory.CreateWebMessagingMock();
        Mock<INotificationsInfiniFrameWindowFeature> notifications = MockFactory.CreateNotificationsMock();
        Mock<IFilePickerDialogsInfiniFrameWindowFeature> filePickerDialogs = MockFactory.CreateFilePickerDialogsMock();
        Mock<IMonitorsInfiniFrameWindowFeature> monitors = MockFactory.CreateMonitorsMock();
        Mock<IPageNavigationInfiniFrameWindowFeature> pageNavigation = MockFactory.CreatePageNavigationMock();
        Mock<IPositionInfiniFrameWindowFeature> position = MockFactory.CreatePositionMock();
        Mock<ISizeInfiniFrameWindowFeature> size = MockFactory.CreateSizeMock();
        Mock<IDecorationsInfiniFrameWindowFeature> decorations = MockFactory.CreateDecorationsMock();
        Mock<IStateInfiniFrameWindowFeature> state = MockFactory.CreateStateMock();
        Mock<IBrowserInfiniFrameWindowFeature> browser = MockFactory.CreateBrowserMock();
        Mock<IDragDropInfiniFrameWindowFeature> dragDrop = MockFactory.CreateDragDropMock();
        Mock<ITaskbarInfiniFrameWindowFeature> taskbar = MockFactory.CreateTaskbarMock();
        Mock<IMenuInfiniFrameWindowFeature> menu = MockFactory.CreateMenuMock();
        Mock<IJavaScriptInfiniFrameWindowFeature> javaScript = MockFactory.CreateJavaScriptMock();

        // Act
        var features = new InfiniFrameWindowFeatures(
            debugging.Object,
            lifecycle.Object,
            invoke.Object,
            webMessaging.Object,
            notifications.Object,
            filePickerDialogs.Object,
            monitors.Object,
            pageNavigation.Object,
            position.Object,
            size.Object,
            decorations.Object,
            state.Object,
            browser.Object,
            dragDrop.Object,
            taskbar.Object,
            menu.Object,
            javaScript.Object
        );

        // Assert
        await Assert.That(features.Debugging).IsSameReferenceAs(debugging.Object);
        await Assert.That(features.Lifecycle).IsSameReferenceAs(lifecycle.Object);
        await Assert.That(features.Invoke).IsSameReferenceAs(invoke.Object);
        await Assert.That(features.WebMessaging).IsSameReferenceAs(webMessaging.Object);
        await Assert.That(features.Notifications).IsSameReferenceAs(notifications.Object);
        await Assert.That(features.FilePickerDialogs).IsSameReferenceAs(filePickerDialogs.Object);
        await Assert.That(features.Monitors).IsSameReferenceAs(monitors.Object);
        await Assert.That(features.PageNavigation).IsSameReferenceAs(pageNavigation.Object);
        await Assert.That(features.Position).IsSameReferenceAs(position.Object);
        await Assert.That(features.Size).IsSameReferenceAs(size.Object);
        await Assert.That(features.Decorations).IsSameReferenceAs(decorations.Object);
        await Assert.That(features.State).IsSameReferenceAs(state.Object);
        await Assert.That(features.Browser).IsSameReferenceAs(browser.Object);
        await Assert.That(features.DragDrop).IsSameReferenceAs(dragDrop.Object);
        await Assert.That(features.Taskbar).IsSameReferenceAs(taskbar.Object);
        await Assert.That(features.Menu).IsSameReferenceAs(menu.Object);
        await Assert.That(features.JavaScript).IsSameReferenceAs(javaScript.Object);
    }

    [Test]
    public async Task Record_Equality_SameValues_ShouldBeEqual(CancellationToken ct = default) {
        // Arrange
        Mock<IDebuggingInfiniFrameWindowFeature> debugging = MockFactory.CreateDebuggingMock();
        Mock<ILifecycleInfiniFrameWindowFeature> lifecycle = MockFactory.CreateLifecycleMock();
        Mock<IInvokeInfiniFrameWindowFeature> invoke = MockFactory.CreateInvokeMock();
        Mock<IWebMessagingInfiniFrameWindowFeature> webMessaging = MockFactory.CreateWebMessagingMock();
        Mock<INotificationsInfiniFrameWindowFeature> notifications = MockFactory.CreateNotificationsMock();
        Mock<IFilePickerDialogsInfiniFrameWindowFeature> filePickerDialogs = MockFactory.CreateFilePickerDialogsMock();
        Mock<IMonitorsInfiniFrameWindowFeature> monitors = MockFactory.CreateMonitorsMock();
        Mock<IPageNavigationInfiniFrameWindowFeature> pageNavigation = MockFactory.CreatePageNavigationMock();
        Mock<IPositionInfiniFrameWindowFeature> position = MockFactory.CreatePositionMock();
        Mock<ISizeInfiniFrameWindowFeature> size = MockFactory.CreateSizeMock();
        Mock<IDecorationsInfiniFrameWindowFeature> decorations = MockFactory.CreateDecorationsMock();
        Mock<IStateInfiniFrameWindowFeature> state = MockFactory.CreateStateMock();
        Mock<IBrowserInfiniFrameWindowFeature> browser = MockFactory.CreateBrowserMock();
        Mock<IDragDropInfiniFrameWindowFeature> dragDrop = MockFactory.CreateDragDropMock();
        Mock<ITaskbarInfiniFrameWindowFeature> taskbar = MockFactory.CreateTaskbarMock();
        Mock<IMenuInfiniFrameWindowFeature> menu = MockFactory.CreateMenuMock();
        Mock<IJavaScriptInfiniFrameWindowFeature> javaScript = MockFactory.CreateJavaScriptMock();

        // Act
        var features1 = new InfiniFrameWindowFeatures(
            debugging.Object, lifecycle.Object, invoke.Object, webMessaging.Object,
            notifications.Object, filePickerDialogs.Object, monitors.Object, pageNavigation.Object,
            position.Object, size.Object, decorations.Object, state.Object,
            browser.Object, dragDrop.Object, taskbar.Object, menu.Object, javaScript.Object);
        var features2 = new InfiniFrameWindowFeatures(
            debugging.Object, lifecycle.Object, invoke.Object, webMessaging.Object,
            notifications.Object, filePickerDialogs.Object, monitors.Object, pageNavigation.Object,
            position.Object, size.Object, decorations.Object, state.Object,
            browser.Object, dragDrop.Object, taskbar.Object, menu.Object, javaScript.Object);

        // Assert
        await Assert.That(features1).IsEqualTo(features2);
    }
}
