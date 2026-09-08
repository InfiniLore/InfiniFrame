// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation.Results;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.NativeBridge.Parameters.Window;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameNativeWindowParametersValidatorTests {
    private static readonly InfiniFrameNativeWindowParametersValidator Validator = new();

    [Test]
    public async Task Validate_ValidParameters_Passes(CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeWindowParameters parameters = CreateValidParameters();

        // Act
        ValidationResult? result = await Validator.ValidateAsync(parameters, ct);

        // Assert
        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(int.MaxValue)]
    public async Task Validate_InvalidWidth_FailsValidation(int width, CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeWindowParameters parameters = CreateValidParameters();
        parameters.Width = width;

        // Act
        ValidationResult? result = await Validator.ValidateAsync(parameters, ct);

        // Assert
        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(error => error.PropertyName == nameof(InfiniFrameNativeWindowParameters.Width))).IsTrue();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(int.MaxValue)]
    public async Task Validate_InvalidHeight_FailsValidation(int height, CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeWindowParameters parameters = CreateValidParameters();
        parameters.Height = height;

        // Act
        ValidationResult? result = await Validator.ValidateAsync(parameters, ct);

        // Assert
        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(error => error.PropertyName == nameof(InfiniFrameNativeWindowParameters.Height))).IsTrue();
    }

    [Test]
    [Arguments(int.MaxValue, 10)]
    [Arguments(10, int.MaxValue)]
    [Arguments(int.MinValue, 10)]
    [Arguments(10, int.MinValue)]
    public async Task Validate_InvalidPosition_FailsValidation(int left, int top, CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeWindowParameters parameters = CreateValidParameters();
        parameters.Left = left;
        parameters.Top = top;

        // Act
        ValidationResult? result = await Validator.ValidateAsync(parameters, ct);

        // Assert
        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(error => error.PropertyName is nameof(InfiniFrameNativeWindowParameters.Left) or nameof(InfiniFrameNativeWindowParameters.Top))).IsTrue();
    }

    [Test]
    [Arguments(1, 1, 0, 0)]
    [Arguments(800, 600, -120, -80)]
    public async Task Validate_BoundaryValues_PassesWhenInRange(int width, int height, int left, int top, CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeWindowParameters parameters = CreateValidParameters();
        parameters.Width = width;
        parameters.Height = height;
        parameters.Left = left;
        parameters.Top = top;

        // Act
        ValidationResult? result = await Validator.ValidateAsync(parameters, ct);

        // Assert
        await Assert.That(result.IsValid).IsTrue();
    }

        private static InfiniFrameNativeWindowParameters CreateValidParameters() =>
        new() {
            StartUrl = "https://example.com",
            Width = 1024,
            Height = 768,
            Left = 120,
            Top = 80,
            UseOsDefaultSize = false,
            UseOsDefaultLocation = false,
            CustomSchemeNames = new IntPtr[16]
        };
}
