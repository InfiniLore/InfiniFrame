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
    public async Task RegisterWindow_DuplicateIdThrows(CancellationToken ct = default) {
        // Arrange
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();

        // Act
        application.RegisterWindow("main", configure: static _ => {});

        // Assert
        // ReSharper disable once AccessToDisposedClosure
        await Assert.That(() => {
                application.RegisterWindow("main", configure: static _ => {});
            })
            .Throws<ArgumentException>();
    }
    
    [Test]
    [NotInParallelInfiniTests]
    public async Task RegisterWindow_AfterRunFails(CancellationToken ct = default) {
        // Arrange
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();

        // Act
        await application.RunAsync(ct);

        // Assert
        await Assert.That(() => {
                // ReSharper disable once AccessToDisposedClosure
                application.RegisterWindow(static _ => {});
            })
            .Throws<InvalidOperationException>();
    }
}
