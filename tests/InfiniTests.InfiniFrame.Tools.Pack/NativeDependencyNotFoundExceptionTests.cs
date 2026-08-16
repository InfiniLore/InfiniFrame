// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Exceptions;

namespace InfiniTests.InfiniFrame.Tools.Pack;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NativeDependencyNotFoundExceptionTests {

    [Test]
    public async Task MessageConstructor_SetsMessage(CancellationToken ct = default) {
        // Arrange & Act
        var ex = new NativeDependencyNotFoundException("missing lib");

        // Assert
        await Assert.That(ex.Message).IsEqualTo("missing lib");
        await Assert.That(ex).IsTypeOf<InvalidOperationException>();
    }
}
