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

    public InfiniFrameUriSecurityPolicyBuilder(InfiniFrameUriSecurityPolicy? basePolicy = null) {
        InfiniFrameUriSecurityPolicy initialPolicy = basePolicy ?? InfiniFrameUriSecurityPolicy.Default;
        _allowedNavigationSchemes = new HashSet<string>(initialPolicy.AllowedNavigationSchemes, StringComparer.OrdinalIgnoreCase);
        _allowedExternalSchemes = new HashSet<string>(initialPolicy.AllowedExternalSchemes, StringComparer.OrdinalIgnoreCase);
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

    public InfiniFrameUriSecurityPolicy Build()
        => new(_allowedNavigationSchemes, _allowedExternalSchemes);

    private static void AddScheme(HashSet<string> target, string scheme) {
        if (string.IsNullOrWhiteSpace(scheme)) return;
        target.Add(scheme.Trim());
    }
}
