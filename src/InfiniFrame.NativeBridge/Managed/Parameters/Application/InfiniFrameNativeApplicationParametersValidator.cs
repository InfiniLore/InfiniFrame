// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using FluentValidation;

namespace InfiniFrame.NativeBridge.Parameters.Application;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
/// Validates <see cref="InfiniFrameNativeApplicationParameters" /> instances.
/// </summary>
public sealed class InfiniFrameNativeApplicationParametersValidator
    : AbstractValidator<InfiniFrameNativeApplicationParameters> {
    public InfiniFrameNativeApplicationParametersValidator() {
        RuleFor(parameters => parameters.Size)
            .Equal(Marshal.SizeOf<InfiniFrameNativeApplicationParameters>());
    }
}
