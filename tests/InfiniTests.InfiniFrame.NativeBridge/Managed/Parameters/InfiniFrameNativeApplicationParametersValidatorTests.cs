// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters.Application;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

public sealed class InfiniFrameNativeApplicationParametersValidatorTests {
    private static readonly InfiniFrameNativeApplicationParametersValidator Validator = new();

    [Test]
    public async Task Validate_DefaultParameters_IsValid(CancellationToken ct = default) {
        var parameters = new InfiniFrameNativeApplicationParameters();

        FluentValidation.Results.ValidationResult result = await Validator.ValidateAsync(parameters, ct);

        await Assert.That(result.IsValid).IsTrue();
    }
}
