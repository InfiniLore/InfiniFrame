// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Security;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Builds a custom <see cref="IInfiniFrameUriSecurityPolicy" /> by configuring allowed schemes, trusted origins, and
///     trust-all settings.
/// </summary>
public sealed class InfiniFrameUriSecurityPolicyBuilder {
    private readonly HashSet<string> _allowedExternalSchemes;
    private readonly HashSet<string> _allowedNavigationSchemes;
    private readonly HashSet<Uri> _trustedOrigins;
    private bool _trustAllOrigins;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Initializes a new instance of the <see cref="InfiniFrameUriSecurityPolicyBuilder" /> class.
    /// </summary>
    /// <param name="basePolicy">An optional base policy whose settings are used as defaults.</param>
    public InfiniFrameUriSecurityPolicyBuilder(IInfiniFrameUriSecurityPolicy? basePolicy = null) {
        IInfiniFrameUriSecurityPolicy initialPolicy = basePolicy ?? InfiniFrameUriSecurityPolicy.Default;
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
    /// <summary>
    ///     Replaces the set of allowed navigation schemes with the specified collection.
    /// </summary>
    /// <param name="schemes">The URI schemes to allow for navigation.</param>
    /// <returns>The builder instance for chaining.</returns>
    public InfiniFrameUriSecurityPolicyBuilder SetAllowedNavigationSchemes(IEnumerable<string> schemes) {
        _allowedNavigationSchemes.Clear();
        foreach (string scheme in schemes) {
            AddScheme(_allowedNavigationSchemes, scheme);
        }

        return this;
    }

    /// <summary>
    ///     Replaces the set of allowed external schemes with the specified collection.
    /// </summary>
    /// <param name="schemes">The URI schemes to allow for external content.</param>
    /// <returns>The builder instance for chaining.</returns>
    public InfiniFrameUriSecurityPolicyBuilder SetAllowedExternalSchemes(IEnumerable<string> schemes) {
        _allowedExternalSchemes.Clear();
        foreach (string scheme in schemes) {
            AddScheme(_allowedExternalSchemes, scheme);
        }

        return this;
    }

    /// <summary>
    ///     Adds a URI scheme to the set of allowed navigation schemes.
    /// </summary>
    /// <param name="scheme">The scheme to allow (e.g., "https").</param>
    /// <returns>The builder instance for chaining.</returns>
    public InfiniFrameUriSecurityPolicyBuilder AllowNavigationScheme(string scheme) {
        AddScheme(_allowedNavigationSchemes, scheme);
        return this;
    }

    /// <summary>
    ///     Adds a URI scheme to the set of allowed external schemes.
    /// </summary>
    /// <param name="scheme">The scheme to allow (e.g., "mailto").</param>
    /// <returns>The builder instance for chaining.</returns>
    public InfiniFrameUriSecurityPolicyBuilder AllowExternalScheme(string scheme) {
        AddScheme(_allowedExternalSchemes, scheme);
        return this;
    }

    /// <summary>
    ///     Replaces the set of trusted origins with the specified collection.
    /// </summary>
    /// <param name="origins">The absolute URIs representing trusted origins.</param>
    /// <returns>The builder instance for chaining.</returns>
    public InfiniFrameUriSecurityPolicyBuilder SetTrustedOrigins(IEnumerable<Uri> origins) {
        _trustedOrigins.Clear();
        foreach (Uri origin in origins) {
            AddTrustedOrigin(origin);
        }

        return this;
    }

    /// <summary>
    ///     Adds a single trusted origin to the policy.
    /// </summary>
    /// <param name="origin">An absolute URI representing a trusted origin. Relative URIs are ignored.</param>
    /// <returns>The builder instance for chaining.</returns>
    public InfiniFrameUriSecurityPolicyBuilder AddTrustedOrigin(Uri origin) {
        if (!origin.IsAbsoluteUri) return this;

        _trustedOrigins.Add(origin);
        return this;
    }

    /// <summary>
    ///     Configures whether all origins should be treated as trusted.
    /// </summary>
    /// <param name="trustAllOrigins"><c>true</c> to trust all origins; otherwise <c>false</c>.</param>
    /// <returns>The builder instance for chaining.</returns>
    public InfiniFrameUriSecurityPolicyBuilder SetTrustAllOrigins(bool trustAllOrigins = true) {
        _trustAllOrigins = trustAllOrigins;
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="IInfiniFrameUriSecurityPolicy" /> with the configured settings.
    /// </summary>
    /// <returns>The constructed security policy.</returns>
    public InfiniFrameUriSecurityPolicy Build()
        => new(_allowedNavigationSchemes, _allowedExternalSchemes, _trustedOrigins, _trustAllOrigins);

    private static void AddScheme(HashSet<string> target, string scheme) {
        if (string.IsNullOrWhiteSpace(scheme)) return;

        target.Add(scheme.Trim());
    }
}
