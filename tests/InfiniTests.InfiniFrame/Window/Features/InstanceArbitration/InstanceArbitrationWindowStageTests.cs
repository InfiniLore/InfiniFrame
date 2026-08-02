// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.InstanceArbitration;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InstanceArbitrationWindowStageTests {

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_BuildWithArbitration_Succeeds(CancellationToken ct) {
        // Arrange & Act
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.InstanceArbitration.SetMode(InstanceArbitrationMode.Disabled);
        }, ct);

        // Assert
        await Assert.That(windowUtility.Window).IsNotNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_BuildWithArbitration_SecondInstance_Throws(CancellationToken ct) {
        // Arrange
        string mutexName = $"InfiniFrame.Test.{Guid.NewGuid():N}";
        global::InfiniFrame.InstanceArbitration.TryAcquirePrimaryInstance(mutexName);

        // Act & Assert
        await Assert.That(() => {
            using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
                builder.Features.InstanceArbitration.SetMode(InstanceArbitrationMode.PrimaryOnly);
                builder.Features.InstanceArbitration.SetMutexName(mutexName);
            }, ct);
        }).ThrowsExactly<InstanceAlreadyRunningException>();
    }
}
