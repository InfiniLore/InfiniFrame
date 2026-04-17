// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once CheckNamespace
namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameUriSecurityPolicyBuilder {
    private readonly HashSet<string> _allowedNavigationSchemes;
    private readonly HashSet<string> _allowedExternalSchemes;
    private readonly HashSet<Uri> _trustedOrigins;
    private bool _trustAllOrigins;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public InfiniFrameUriSecurityPolicyBuilder(InfiniFrameUriSecurityPolicy? basePolicy = null) {
        InfiniFrameUriSecurityPolicy initialPolicy = basePolicy ?? InfiniFrameUriSecurityPolicy.Default;
        _allowedNavigationSchemes = new HashSet<string>(initialPolicy.AllowedNavigationSchemes, StringComparer.OrdinalIgnoreCase);
        _allowedExternalSchemes = new HashSet<string>(initialPolicy.AllowedExternalSchemes, StringComparer.OrdinalIgnoreCase);
        _trustedOrigins = new HashSet<Uri>();
        _trustAllOrigins = initialPolicy.TrustAllOrigins;
        foreach (Uri trustedOrigin in initialPolicy.TrustedOrigins) {
            AddTrustedOrigin(trustedOrigin);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public InfiniFrameUriSecurityPolicyBuilder SetAllowedNavigationSchemes(IEnumerable<string> schemes) {
        _allowedNavigationSchemes.Clear();
        foreach (string scheme in schemes) {
            AddScheme(_allowedNavigationSchemes, scheme);
        }

        return this;
    }

    public InfiniFrameUriSecurityPolicyBuilder SetAllowedExternalSchemes(IEnumerable<string> schemes) {
        _allowedExternalSchemes.Clear();
        foreach (string scheme in schemes) {
            AddScheme(_allowedExternalSchemes, scheme);
        }

        return this;
    }

    public InfiniFrameUriSecurityPolicyBuilder AllowNavigationScheme(string scheme) {
        AddScheme(_allowedNavigationSchemes, scheme);
        return this;
    }

    public InfiniFrameUriSecurityPolicyBuilder AllowExternalScheme(string scheme) {
        AddScheme(_allowedExternalSchemes, scheme);
        return this;
    }

    public InfiniFrameUriSecurityPolicyBuilder SetTrustedOrigins(IEnumerable<Uri> origins) {
        _trustedOrigins.Clear();
        foreach (Uri origin in origins) {
            AddTrustedOrigin(origin);
        }

        return this;
    }

    public InfiniFrameUriSecurityPolicyBuilder AddTrustedOrigin(Uri origin) {
        if (!origin.IsAbsoluteUri) return this;
        _trustedOrigins.Add(origin);
        return this;
    }

    public InfiniFrameUriSecurityPolicyBuilder SetTrustAllOrigins(bool trustAllOrigins = true) {
        _trustAllOrigins = trustAllOrigins;
        return this;
    }

    public InfiniFrameUriSecurityPolicy Build()
        => new(_allowedNavigationSchemes, _allowedExternalSchemes, _trustedOrigins, _trustAllOrigins);

    private static void AddScheme(HashSet<string> target, string scheme) {
        if (string.IsNullOrWhiteSpace(scheme)) return;
        target.Add(scheme.Trim());
    }
}
