// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters.Application;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public sealed class InfiniFrameNativeApplicationParametersEqualityComparerTests {
    private static InfiniFrameNativeApplicationParametersEqualityComparer Comparer
        => InfiniFrameNativeApplicationParametersEqualityComparer.Instance;

    [Test]
    public async Task Equals_SameValues_ReturnsTrue(CancellationToken ct = default) {
        var first = new InfiniFrameNativeApplicationParameters {
            WebView2RuntimePath = "runtime",
            NotificationRegistrationId = "notifications",
            AppUserModelId = "app",
            DefaultNotificationIcon = "icon"
        };
        InfiniFrameNativeApplicationParameters second = first;

        await Assert.That(Comparer.Equals(first, second)).IsTrue();
    }

    [Test]
    [Arguments(nameof(InfiniFrameNativeApplicationParameters.WebView2RuntimePath))]
    [Arguments(nameof(InfiniFrameNativeApplicationParameters.NotificationRegistrationId))]
    [Arguments(nameof(InfiniFrameNativeApplicationParameters.AppUserModelId))]
    [Arguments(nameof(InfiniFrameNativeApplicationParameters.DefaultNotificationIcon))]
    public async Task Equals_DifferentConfigurationValue_ReturnsFalse(
        string field,
        CancellationToken ct = default
    ) {
        var first = new InfiniFrameNativeApplicationParameters();
        InfiniFrameNativeApplicationParameters second = first;

        switch (field) {
            case nameof(InfiniFrameNativeApplicationParameters.WebView2RuntimePath):
                second.WebView2RuntimePath = "runtime";
                break;
            case nameof(InfiniFrameNativeApplicationParameters.NotificationRegistrationId):
                second.NotificationRegistrationId = "notifications";
                break;
            case nameof(InfiniFrameNativeApplicationParameters.AppUserModelId):
                second.AppUserModelId = "app";
                break;
            case nameof(InfiniFrameNativeApplicationParameters.DefaultNotificationIcon):
                second.DefaultNotificationIcon = "icon";
                break;
        }

        await Assert.That(Comparer.Equals(first, second)).IsFalse();
    }
}
