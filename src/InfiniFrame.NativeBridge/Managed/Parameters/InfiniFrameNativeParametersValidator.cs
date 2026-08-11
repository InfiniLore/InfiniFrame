// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using FluentValidation;

namespace InfiniFrame.NativeBridge.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Validates <see cref="InfiniFrameNativeParameters" /> instances using FluentValidation rules.
/// </summary>
public sealed class InfiniFrameNativeParametersValidator
    : AbstractValidator<InfiniFrameNativeParameters> {

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Initializes a new instance of <see cref="InfiniFrameNativeParametersValidator" />
    ///     and configures all validation rules.
    /// </summary>
    public InfiniFrameNativeParametersValidator() {
        RuleFor(p => p.Size)
            .Equal(Marshal.SizeOf<InfiniFrameNativeParameters>());

        RuleFor(p => p)
            .Must(p =>
                !string.IsNullOrWhiteSpace(p.StartUrl)
                || !string.IsNullOrWhiteSpace(p.StartString)
            )
            .WithMessage("No initial URL or HTML string was supplied in StartUrl or StartString for the browser control to navigate to.");

        RuleFor(p => p.Width)
            .GreaterThan(0)
            .When(p => !p.UseOsDefaultSize)
            .WithMessage("Width must be greater than zero when UseOsDefaultSize is false.");

        RuleFor(p => p.Height)
            .GreaterThan(0)
            .When(p => !p.UseOsDefaultSize)
            .WithMessage("Height must be greater than zero when UseOsDefaultSize is false.");

        RuleFor(p => p.Width)
            .LessThan(int.MaxValue)
            .When(p => !p.UseOsDefaultSize)
            .WithMessage("Width must be less than Int32.MaxValue.");

        RuleFor(p => p.Height)
            .LessThan(int.MaxValue)
            .When(p => !p.UseOsDefaultSize)
            .WithMessage("Height must be less than Int32.MaxValue.");

        RuleFor(p => p.Left)
            .Must(value => value is not int.MinValue and not int.MaxValue)
            .When(p => !p.UseOsDefaultLocation)
            .WithMessage("Left must be a finite coordinate value.");

        RuleFor(p => p.Top)
            .Must(value => value is not int.MinValue and not int.MaxValue)
            .When(p => !p.UseOsDefaultLocation)
            .WithMessage("Top must be a finite coordinate value.");

        RuleFor(p => p)
            .Must(p => p is not { Maximized: true, Minimized: true })
            .WithMessage("Maximized and Minimized cannot be set to true at the same time.");

        RuleFor(p => p)
            .Must(p => !(p.FullScreen && (p.Maximized || p.Minimized)))
            .WithMessage("FullScreen cannot be set to true at the same time as Maximized or Minimized.");

        RuleFor(p => p)
            .Must(p =>
                !p.Chromeless
                || p is { UseOsDefaultLocation: false, UseOsDefaultSize: false }
            )
            .When(_ => RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            .WithMessage("Chromeless cannot be used with UseOsDefaultLocation or UseOsDefaultSize on Windows. Size and location must be specified.");

        RuleFor(p => p.TemporaryFilesPath)
            .Must(path => IsPathAccessible(path))
            .When(p => !string.IsNullOrWhiteSpace(p.TemporaryFilesPath))
            .WithMessage(p => $"TemporaryFilesPath '{p.TemporaryFilesPath}' is not writable.");

        RuleFor(p => p.CustomSchemeNames)
            .NotNull().WithMessage("CustomSchemeNames must be specified.")
            .Must(names => names.Length <= 16).WithMessage("CustomSchemeNames must contain at most 16 names.");

        RuleFor(p => p.WindowsAppUserModelId)
            .NotEmpty()
            .MaximumLength(128)
            .Must(value => value is null || !value.Any(char.IsWhiteSpace))
            .When(p => p.WindowsAppUserModelId is not null)
            .WithMessage("WindowsAppUserModelId must contain 1 to 128 characters and cannot contain whitespace.");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Checks whether the given path is writable by creating and deleting a temporary probe file.
    /// </summary>
    /// <param name="path">The directory path to check.</param>
    /// <returns><c>true</c> if the path is writable; otherwise, <c>false</c>.</returns>
    private static bool IsPathAccessible(string? path) {
        if (string.IsNullOrWhiteSpace(path)) return true;

        string? probeFile = null;
        try {
            if (!Directory.Exists(path)) return false;

            probeFile = System.IO.Path.Join(path, $".infiniframe-write-check-{Guid.NewGuid():N}.tmp");

            File.WriteAllText(probeFile, "ok");
            File.Delete(probeFile);

            return true;
        }
        catch (Exception ex) when (
            ex is IOException
                or UnauthorizedAccessException
                or ArgumentException
                or NotSupportedException
                or PathTooLongException
        ) {
            return false;
        }
        finally {
            if (probeFile is not null) {
                File.Delete(probeFile);
            }
        }
    }

    /// <summary>
    ///     Ensures the temporary files path exists and is writable.
    ///     Call this before validation when the path should be created automatically.
    /// </summary>
    /// <param name="path">The directory path to ensure.</param>
    /// <returns><c>true</c> if the path was successfully created or already accessible; otherwise, <c>false</c>.</returns>
    public static bool EnsureTemporaryFilesPath(string? path) {
        if (string.IsNullOrWhiteSpace(path)) return true;

        try {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            return IsPathAccessible(path);
        }
        catch {
            return false;
        }
    }
}