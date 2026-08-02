// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Security;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides extension methods on <see cref="IInfiniFrameWindowBuilder" /> for configuring URI security policies.
/// </summary>
public static class InfiniFrameUriSecurityPolicyBuilderExtensions {
    private static T ConfigureUriSecurityPolicy<T>(this T builder, Action<InfiniFrameUriSecurityPolicyBuilder> configure)
        where T : IInfiniFrameWindowBuilder {
        InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(builder, configure);
        return builder;
    }

    /// <summary>
    ///     Sets the allowed navigation schemes for the URI security policy of the window builder.
    /// </summary>
    /// <param name="builder">The window builder.</param>
    /// <param name="schemes">The URI schemes to allow for navigation.</param>
    /// <typeparam name="T">The type of the window builder.</typeparam>
    /// <returns>The window builder for chaining.</returns>
    public static T SetAllowedNavigationSchemes<T>(this T builder, params string[] schemes)
        where T : IInfiniFrameWindowBuilder {
        return builder.ConfigureUriSecurityPolicy(policy => policy.SetAllowedNavigationSchemes(schemes));
    }

    /// <summary>
    ///     Sets the allowed external schemes for the URI security policy of the window builder.
    /// </summary>
    /// <param name="builder">The window builder.</param>
    /// <param name="schemes">The URI schemes to allow for external content.</param>
    /// <typeparam name="T">The type of the window builder.</typeparam>
    /// <returns>The window builder for chaining.</returns>
    public static T SetAllowedExternalSchemes<T>(this T builder, params string[] schemes)
        where T : IInfiniFrameWindowBuilder {
        return builder.ConfigureUriSecurityPolicy(policy => policy.SetAllowedExternalSchemes(schemes));
    }

    /// <summary>
    ///     Sets the trusted origins for the URI security policy using string representations of absolute URIs.
    /// </summary>
    /// <param name="builder">The window builder.</param>
    /// <param name="origins">Absolute URI strings representing trusted origins.</param>
    /// <typeparam name="T">The type of the window builder.</typeparam>
    /// <returns>The window builder for chaining.</returns>
    public static T SetTrustedOrigins<T>(this T builder, params string[] origins)
        where T : IInfiniFrameWindowBuilder {
        return builder.ConfigureUriSecurityPolicy(policy => policy.SetTrustedOrigins(ParseOrigins(origins)));
    }

    /// <summary>
    ///     Adds a trusted origin to the URI security policy using a string representation of an absolute URI.
    /// </summary>
    /// <param name="builder">The window builder.</param>
    /// <param name="origin">An absolute URI string representing a trusted origin.</param>
    /// <typeparam name="T">The type of the window builder.</typeparam>
    /// <returns>The window builder for chaining.</returns>
    public static T AddTrustedOrigin<T>(this T builder, string origin)
        where T : IInfiniFrameWindowBuilder {
        return builder.ConfigureUriSecurityPolicy(policy => policy.AddTrustedOrigin(ParseOrigin(origin)));
    }

    /// <summary>
    ///     Sets the trusted origins for the URI security policy using <see cref="Uri" /> instances.
    /// </summary>
    /// <param name="builder">The window builder.</param>
    /// <param name="origins">Absolute URIs representing trusted origins.</param>
    /// <typeparam name="T">The type of the window builder.</typeparam>
    /// <returns>The window builder for chaining.</returns>
    public static T SetTrustedOrigins<T>(this T builder, params Uri[] origins)
        where T : IInfiniFrameWindowBuilder {
        return builder.ConfigureUriSecurityPolicy(policy => policy.SetTrustedOrigins(origins));
    }

    /// <summary>
    ///     Adds a trusted origin to the URI security policy using a <see cref="Uri" /> instance.
    /// </summary>
    /// <param name="builder">The window builder.</param>
    /// <param name="origin">An absolute URI representing a trusted origin.</param>
    /// <typeparam name="T">The type of the window builder.</typeparam>
    /// <returns>The window builder for chaining.</returns>
    public static T AddTrustedOrigin<T>(this T builder, Uri origin)
        where T : IInfiniFrameWindowBuilder {
        return builder.ConfigureUriSecurityPolicy(policy => policy.AddTrustedOrigin(origin));
    }

    /// <summary>
    ///     Configures whether all origins should be trusted for the URI security policy.
    /// </summary>
    /// <param name="builder">The window builder.</param>
    /// <param name="trustAllOrigins"><c>true</c> to trust all origins; otherwise <c>false</c>.</param>
    /// <typeparam name="T">The type of the window builder.</typeparam>
    /// <returns>The window builder for chaining.</returns>
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