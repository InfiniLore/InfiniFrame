// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once CheckNamespace
namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameUriSecurityPolicy(
    IEnumerable<string> allowedNavigationSchemes,
    IEnumerable<string> allowedExternalSchemes
) {
    public static InfiniFrameUriSecurityPolicy Default { get; } = new(
        allowedNavigationSchemes: [Uri.UriSchemeHttps, Uri.UriSchemeHttp, "app"],
        allowedExternalSchemes: [Uri.UriSchemeHttps, Uri.UriSchemeHttp, Uri.UriSchemeMailto]
    );

    public IReadOnlySet<string> AllowedNavigationSchemes { get; } = NormalizeSchemes(allowedNavigationSchemes);
    public IReadOnlySet<string> AllowedExternalSchemes { get; } = NormalizeSchemes(allowedExternalSchemes);

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public bool IsNavigationSchemeAllowed(string scheme)
        => AllowedNavigationSchemes.Contains(scheme);

    public bool IsExternalSchemeAllowed(string scheme)
        => AllowedExternalSchemes.Contains(scheme);

    public bool IsTrustedOrigin(Uri candidateOrigin, Uri trustedOrigin) {
        ArgumentNullException.ThrowIfNull(candidateOrigin);
        ArgumentNullException.ThrowIfNull(trustedOrigin);

        return IsNavigationSchemeAllowed(candidateOrigin.Scheme)
               && string.Equals(candidateOrigin.Scheme, trustedOrigin.Scheme, StringComparison.OrdinalIgnoreCase)
               && string.Equals(candidateOrigin.Host, trustedOrigin.Host, StringComparison.OrdinalIgnoreCase)
               && candidateOrigin.Port == trustedOrigin.Port;
    }

    private static HashSet<string> NormalizeSchemes(IEnumerable<string> schemes) {
        return schemes
            .Where(static scheme => !string.IsNullOrWhiteSpace(scheme))
            .Select(static scheme => scheme.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
