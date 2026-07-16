// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Security;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameUriSecurityPolicy(
    IEnumerable<string> allowedNavigationSchemes,
    IEnumerable<string> allowedExternalSchemes,
    IEnumerable<Uri>? trustedOrigins = null,
    bool trustAllOrigins = false
) : IInfiniFrameUriSecurityPolicy {
    public static InfiniFrameUriSecurityPolicy Default { get; } = new(
        [Uri.UriSchemeHttps, Uri.UriSchemeHttp, "app"],
        [Uri.UriSchemeHttps, Uri.UriSchemeHttp, Uri.UriSchemeMailto]
    );

    /// <inheritdoc cref="IInfiniFrameUriSecurityPolicy.AllowedNavigationSchemes"/>
    public IReadOnlySet<string> AllowedNavigationSchemes { get; } = NormalizeSchemes(allowedNavigationSchemes);
    /// <inheritdoc cref="IInfiniFrameUriSecurityPolicy.AllowedExternalSchemes"/>
    public IReadOnlySet<string> AllowedExternalSchemes { get; } = NormalizeSchemes(allowedExternalSchemes);
    /// <inheritdoc cref="IInfiniFrameUriSecurityPolicy.TrustedOrigins"/>
    public IReadOnlySet<Uri> TrustedOrigins { get; } = NormalizeTrustedOrigins(trustedOrigins ?? []);
    /// <inheritdoc cref="IInfiniFrameUriSecurityPolicy.TrustAllOrigins"/>
    public bool TrustAllOrigins { get; } = trustAllOrigins;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameUriSecurityPolicy.IsNavigationSchemeAllowed(string)"/>
    public bool IsNavigationSchemeAllowed(string scheme)
        => AllowedNavigationSchemes.Contains(scheme);

    /// <inheritdoc cref="IInfiniFrameUriSecurityPolicy.IsExternalSchemeAllowed(string)"/>
    public bool IsExternalSchemeAllowed(string scheme)
        => AllowedExternalSchemes.Contains(scheme);

    /// <inheritdoc cref="IInfiniFrameUriSecurityPolicy.IsTrustedOrigin(Uri)"/>
    public bool IsTrustedOrigin(Uri candidateOrigin) {
        ArgumentNullException.ThrowIfNull(candidateOrigin);

        return IsNavigationSchemeAllowed(candidateOrigin.Scheme)
            && (TrustAllOrigins || TrustedOrigins.Any(trustedOrigin => IsSameOrigin(candidateOrigin, trustedOrigin)));
    }

    /// <inheritdoc cref="IInfiniFrameUriSecurityPolicy.IsTrustedOrigin(Uri, Uri)"/>
    public bool IsTrustedOrigin(Uri candidateOrigin, Uri trustedOrigin) {
        ArgumentNullException.ThrowIfNull(candidateOrigin);
        ArgumentNullException.ThrowIfNull(trustedOrigin);

        return IsNavigationSchemeAllowed(candidateOrigin.Scheme)
            && (TrustAllOrigins || IsSameOrigin(candidateOrigin, trustedOrigin));
    }

    /// <inheritdoc cref="IInfiniFrameUriSecurityPolicy.WithTrustedOrigin"/>
    public IInfiniFrameUriSecurityPolicy WithTrustedOrigin(Uri trustedOrigin) {
        ArgumentNullException.ThrowIfNull(trustedOrigin);
        return WithTrustedOrigins([trustedOrigin]);
    }

    /// <inheritdoc cref="IInfiniFrameUriSecurityPolicy.WithTrustedOrigins"/>
    public IInfiniFrameUriSecurityPolicy WithTrustedOrigins(IEnumerable<Uri> trustedOrigins) {
        ArgumentNullException.ThrowIfNull(trustedOrigins);

        var mergedTrustedOrigins = new HashSet<Uri>(TrustedOrigins, OriginComparer.Instance);
        foreach (Uri trustedOrigin in trustedOrigins) {
            mergedTrustedOrigins.Add(trustedOrigin);
        }

        return new InfiniFrameUriSecurityPolicy(
            AllowedNavigationSchemes,
            AllowedExternalSchemes,
            mergedTrustedOrigins,
            TrustAllOrigins
        );
    }

    private static HashSet<string> NormalizeSchemes(IEnumerable<string> schemes) {
        return schemes
            .Where(static scheme => !string.IsNullOrWhiteSpace(scheme))
            .Select(static scheme => scheme.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static HashSet<Uri> NormalizeTrustedOrigins(IEnumerable<Uri> trustedOrigins) {
        var normalized = new HashSet<Uri>(OriginComparer.Instance);
        foreach (Uri trustedOrigin in trustedOrigins.Where(static trustedOrigin => trustedOrigin.IsAbsoluteUri)) {
            normalized.Add(trustedOrigin);
        }

        return normalized;
    }

    private static bool IsSameOrigin(Uri left, Uri right)
        => string.Equals(left.Scheme, right.Scheme, StringComparison.OrdinalIgnoreCase)
        && string.Equals(left.Host, right.Host, StringComparison.OrdinalIgnoreCase)
        && left.Port == right.Port;

    private sealed class OriginComparer : IEqualityComparer<Uri> {
        public static OriginComparer Instance { get; } = new();

        public bool Equals(Uri? x, Uri? y) {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            return IsSameOrigin(x, y);
        }

        public int GetHashCode(Uri obj) =>
            HashCode.Combine(
                obj.Scheme.ToUpperInvariant(),
                obj.Host.ToUpperInvariant(),
                obj.Port
            );
    }
}
