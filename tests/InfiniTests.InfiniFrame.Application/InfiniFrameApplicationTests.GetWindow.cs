// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Application;

namespace InfiniTests.InfiniFrame.Application;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed partial class InfiniFrameApplicationTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task GetWindow_LookupBeforeRunFailsClearly(CancellationToken ct = default) {
        // Arrange
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();

        // Act
        application.RegisterWindow("main", configure: static _ => {});

        // Assert
        // ReSharper disable once AccessToDisposedClosure
        await Assert.That(() => application.GetWindow("main"))
            .Throws<InvalidOperationException>();
        await Assert.That(application.TryGetWindow("main")).IsNull();
    }
}
