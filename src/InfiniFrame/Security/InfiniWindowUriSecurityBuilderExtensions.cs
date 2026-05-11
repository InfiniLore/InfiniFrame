// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Security;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniWindowUriSecurityBuilderExtensions {
    private static T ConfigureUriSecurityPolicy<T>(this T builder, Action<InfiniFrameUriSecurityPolicyBuilder> configure)
        where T : IInfiniFrameWindowBuilder {
        InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(builder, configure);
        return builder;
    }

    public static T SetAllowedNavigationSchemes<T>(this T builder, params string[] schemes)
        where T : IInfiniFrameWindowBuilder {
        return builder.ConfigureUriSecurityPolicy(policy => policy.SetAllowedNavigationSchemes(schemes));
    }

    public static T SetAllowedExternalSchemes<T>(this T builder, params string[] schemes)
        where T : IInfiniFrameWindowBuilder {
        return builder.ConfigureUriSecurityPolicy(policy => policy.SetAllowedExternalSchemes(schemes));
    }

    public static T SetTrustedOrigins<T>(this T builder, params string[] origins)
        where T : IInfiniFrameWindowBuilder {
        return builder.ConfigureUriSecurityPolicy(policy => policy.SetTrustedOrigins(ParseOrigins(origins)));
    }

    public static T AddTrustedOrigin<T>(this T builder, string origin)
        where T : IInfiniFrameWindowBuilder {
        return builder.ConfigureUriSecurityPolicy(policy => policy.AddTrustedOrigin(ParseOrigin(origin)));
    }

    public static T SetTrustedOrigins<T>(this T builder, params Uri[] origins)
        where T : IInfiniFrameWindowBuilder {
        return builder.ConfigureUriSecurityPolicy(policy => policy.SetTrustedOrigins(origins));
    }

    public static T AddTrustedOrigin<T>(this T builder, Uri origin)
        where T : IInfiniFrameWindowBuilder {
        return builder.ConfigureUriSecurityPolicy(policy => policy.AddTrustedOrigin(origin));
    }

    public static T SetTrustAllOrigins<T>(this T builder, bool trustAllOrigins = true)
        where T : IInfiniFrameWindowBuilder {
        return builder.ConfigureUriSecurityPolicy(policy => policy.SetTrustAllOrigins(trustAllOrigins));
    }

    private static IEnumerable<Uri> ParseOrigins(IEnumerable<string> origins) => origins.Select(ParseOrigin);

    private static Uri ParseOrigin(string origin) {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!Uri.TryCreate(origin, UriKind.Absolute, out Uri? uri)) throw new ArgumentException($"Invalid trusted origin URI: '{origin}'", nameof(origin));

        return uri;
    }
}
