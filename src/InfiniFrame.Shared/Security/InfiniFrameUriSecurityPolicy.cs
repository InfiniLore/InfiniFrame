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
    IEnumerable<string> allowedExternalSchemes,
    IEnumerable<Uri>? trustedOrigins = null,
    bool trustAllOrigins = false
) {
    public static InfiniFrameUriSecurityPolicy Default { get; } = new(
        [Uri.UriSchemeHttps, Uri.UriSchemeHttp, "app"],
        [Uri.UriSchemeHttps, Uri.UriSchemeHttp, Uri.UriSchemeMailto]
    );

    public IReadOnlySet<string> AllowedNavigationSchemes { get; } = NormalizeSchemes(allowedNavigationSchemes);
    public IReadOnlySet<string> AllowedExternalSchemes { get; } = NormalizeSchemes(allowedExternalSchemes);
    public IReadOnlySet<Uri> TrustedOrigins { get; } = NormalizeTrustedOrigins(trustedOrigins ?? []);
    public bool TrustAllOrigins { get; } = trustAllOrigins;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public bool IsNavigationSchemeAllowed(string scheme)
        => AllowedNavigationSchemes.Contains(scheme);

    public bool IsExternalSchemeAllowed(string scheme)
        => AllowedExternalSchemes.Contains(scheme);

    public bool IsTrustedOrigin(Uri candidateOrigin) {
        ArgumentNullException.ThrowIfNull(candidateOrigin);

        return IsNavigationSchemeAllowed(candidateOrigin.Scheme)
            && (TrustAllOrigins || TrustedOrigins.Any(trustedOrigin => IsSameOrigin(candidateOrigin, trustedOrigin)));
    }

    public bool IsTrustedOrigin(Uri candidateOrigin, Uri trustedOrigin) {
        ArgumentNullException.ThrowIfNull(candidateOrigin);
        ArgumentNullException.ThrowIfNull(trustedOrigin);

        return IsNavigationSchemeAllowed(candidateOrigin.Scheme)
            && (TrustAllOrigins || IsSameOrigin(candidateOrigin, trustedOrigin));
    }

    public InfiniFrameUriSecurityPolicy WithTrustedOrigin(Uri trustedOrigin) {
        ArgumentNullException.ThrowIfNull(trustedOrigin);
        return WithTrustedOrigins([trustedOrigin]);
    }

    public InfiniFrameUriSecurityPolicy WithTrustedOrigins(IEnumerable<Uri> trustedOrigins) {
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
