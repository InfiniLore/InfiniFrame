// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;

namespace InfiniFrame.Security;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides a central registry for associating URI security policies with window builders and windows.
/// </summary>
public static class InfiniFrameUriSecurityPolicyRegistry {
    private static readonly ConditionalWeakTable<IInfiniFrameWindowBuilder, PolicyHolder> BuilderPolicies = new();
    private static readonly ConditionalWeakTable<IInfiniFrameWindow, PolicyHolder> WindowPolicies = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Gets the current URI security policy for the specified window builder.
    /// </summary>
    /// <param name="builder">The window builder.</param>
    /// <returns>The current <see cref="IInfiniFrameUriSecurityPolicy" /> for the builder.</returns>
    public static IInfiniFrameUriSecurityPolicy GetForBuilder(IInfiniFrameWindowBuilder builder) {
        ArgumentNullException.ThrowIfNull(builder);
        return BuilderPolicies.GetValue(builder, createValueCallback: static _ => new PolicyHolder()).Policy;
    }

    /// <summary>
    ///     Configures the URI security policy for the specified window builder using a configuration delegate.
    /// </summary>
    /// <param name="builder">The window builder.</param>
    /// <param name="configure">A delegate that configures the <see cref="InfiniFrameUriSecurityPolicyBuilder" />.</param>
    public static void ConfigureForBuilder(IInfiniFrameWindowBuilder builder, Action<InfiniFrameUriSecurityPolicyBuilder> configure) {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        PolicyHolder holder = BuilderPolicies.GetValue(builder, createValueCallback: static _ => new PolicyHolder());
        var policyBuilder = new InfiniFrameUriSecurityPolicyBuilder(holder.Policy);
        configure(policyBuilder);
        holder.Policy = policyBuilder.Build();
    }

    /// <summary>
    ///     Binds a URI security policy to a specific window instance.
    /// </summary>
    /// <param name="window">The window to bind the policy to.</param>
    /// <param name="policy">The security policy to associate with the window.</param>
    public static void BindToWindow(IInfiniFrameWindow window, IInfiniFrameUriSecurityPolicy policy) {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(policy);

        WindowPolicies.AddOrUpdate(window, new PolicyHolder(policy));
    }

    /// <summary>
    ///     Gets the URI security policy for the specified window, or the default policy if none is bound.
    /// </summary>
    /// <param name="window">The window whose policy to retrieve.</param>
    /// <returns>The <see cref="IInfiniFrameUriSecurityPolicy" /> associated with the window.</returns>
    public static IInfiniFrameUriSecurityPolicy GetForWindow(IInfiniFrameWindow window) {
        ArgumentNullException.ThrowIfNull(window);
        return WindowPolicies.TryGetValue(window, out PolicyHolder? holder)
            ? holder.Policy
            : InfiniFrameUriSecurityPolicy.Default;
    }

    private sealed class PolicyHolder(IInfiniFrameUriSecurityPolicy policy) {
        public IInfiniFrameUriSecurityPolicy Policy { get; set; } = policy;

        public PolicyHolder() : this(InfiniFrameUriSecurityPolicy.Default) { }
    }
}