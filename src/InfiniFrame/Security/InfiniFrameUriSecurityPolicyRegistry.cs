// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;

namespace InfiniFrame.Security;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameUriSecurityPolicyRegistry {
    private static readonly ConditionalWeakTable<IInfiniFrameWindowBuilder, PolicyHolder> BuilderPolicies = new();
    private static readonly ConditionalWeakTable<IInfiniFrameWindow, PolicyHolder> WindowPolicies = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static IInfiniFrameUriSecurityPolicy GetForBuilder(IInfiniFrameWindowBuilder builder) {
        ArgumentNullException.ThrowIfNull(builder);
        return BuilderPolicies.GetValue(builder, createValueCallback: static _ => new PolicyHolder()).Policy;
    }

    public static void ConfigureForBuilder(IInfiniFrameWindowBuilder builder, Action<InfiniFrameUriSecurityPolicyBuilder> configure) {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        PolicyHolder holder = BuilderPolicies.GetValue(builder, createValueCallback: static _ => new PolicyHolder());
        var policyBuilder = new InfiniFrameUriSecurityPolicyBuilder(holder.Policy);
        configure(policyBuilder);
        holder.Policy = policyBuilder.Build();
    }

    public static void BindToWindow(IInfiniFrameWindow window, IInfiniFrameUriSecurityPolicy policy) {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(policy);

        WindowPolicies.AddOrUpdate(window, new PolicyHolder(policy));
    }

    public static IInfiniFrameUriSecurityPolicy GetForWindow(IInfiniFrameWindow window) {
        ArgumentNullException.ThrowIfNull(window);
        return WindowPolicies.TryGetValue(window, out PolicyHolder? holder)
            ? holder.Policy
            : InfiniFrameUriSecurityPolicy.Default;
    }

    private sealed class PolicyHolder(IInfiniFrameUriSecurityPolicy policy) {
        public IInfiniFrameUriSecurityPolicy Policy { get; set; } = policy;
        
        public PolicyHolder() : this(InfiniFrameUriSecurityPolicy.Default) {}
    }
}
