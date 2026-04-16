// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;

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
}
