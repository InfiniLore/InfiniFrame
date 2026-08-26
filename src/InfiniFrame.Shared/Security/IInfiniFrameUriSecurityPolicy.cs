// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Security;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Defines the URI security policy for validating trusted origins.
/// </summary>
public interface IInfiniFrameUriSecurityPolicy {
    /// <summary>
    ///     Gets the set of URI schemes that are allowed for navigation.
    /// </summary>
    IReadOnlySet<string> AllowedNavigationSchemes { get; }

    /// <summary>
    ///     Gets the set of URI schemes that are allowed for external content.
    /// </summary>
    IReadOnlySet<string> AllowedExternalSchemes { get; }

    /// <summary>
    ///     Gets the set of trusted origins.
    /// </summary>
    IReadOnlySet<Uri> TrustedOrigins { get; }

    /// <summary>
    ///     Gets a value indicating whether all origins are trusted.
    /// </summary>
    bool TrustAllOrigins { get; }

    /// <summary>
    ///     Determines whether the specified URI scheme is allowed for navigation.
    /// </summary>
    /// <param name="scheme">The URI scheme to check.</param>
    /// <returns><c>true</c> if the scheme is allowed for navigation; otherwise, <c>false</c>.</returns>
    bool IsNavigationSchemeAllowed(string scheme);

    /// <summary>
    ///     Determines whether the specified URI scheme is allowed for external content.
    /// </summary>
    /// <param name="scheme">The URI scheme to check.</param>
    /// <returns><c>true</c> if the scheme is allowed for external content; otherwise, <c>false</c>.</returns>
    bool IsExternalSchemeAllowed(string scheme);

    /// <summary>
    ///     Determines whether the specified origin is a trusted origin.
    /// </summary>
    /// <param name="candidateOrigin">The origin URI to validate.</param>
    /// <returns><c>true</c> if the origin is trusted; otherwise, <c>false</c>.</returns>
    bool IsTrustedOrigin(Uri candidateOrigin);

    /// <summary>
    ///     Determines whether the specified candidate origin matches the given trusted origin.
    /// </summary>
    /// <param name="candidateOrigin">The origin URI to validate.</param>
    /// <param name="trustedOrigin">The trusted origin URI to compare against.</param>
    /// <returns><c>true</c> if the candidate origin is trusted; otherwise, <c>false</c>.</returns>
    bool IsTrustedOrigin(Uri candidateOrigin, Uri trustedOrigin);

    /// <summary>
    ///     Creates a new security policy with the specified origin added to the trusted origins collection.
    /// </summary>
    /// <param name="trustedOrigin">The origin URI to trust.</param>
    /// <returns>A new <see cref="IInfiniFrameUriSecurityPolicy" /> instance with the added trusted origin.</returns>
    IInfiniFrameUriSecurityPolicy WithTrustedOrigin(Uri trustedOrigin);

    /// <summary>
    ///     Creates a new security policy with the specified origins added to the trusted origins collection.
    /// </summary>
    /// <param name="trustedOrigins">The origin URIs to trust.</param>
    /// <returns>A new <see cref="IInfiniFrameUriSecurityPolicy" /> instance with the added trusted origins.</returns>
    IInfiniFrameUriSecurityPolicy WithTrustedOrigins(IEnumerable<Uri> trustedOrigins);
}
