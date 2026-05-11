// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Security;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameUriSecurityPolicy {
   IReadOnlySet<string> AllowedNavigationSchemes { get; }
   IReadOnlySet<string> AllowedExternalSchemes { get; }
   IReadOnlySet<Uri> TrustedOrigins { get; }
   bool TrustAllOrigins { get; }
   
   bool IsNavigationSchemeAllowed(string scheme);
   bool IsExternalSchemeAllowed(string scheme);
   bool IsTrustedOrigin(Uri candidateOrigin);
   bool IsTrustedOrigin(Uri candidateOrigin, Uri trustedOrigin);
   IInfiniFrameUriSecurityPolicy WithTrustedOrigin(Uri trustedOrigin);
   IInfiniFrameUriSecurityPolicy WithTrustedOrigins(IEnumerable<Uri> trustedOrigins);
}
