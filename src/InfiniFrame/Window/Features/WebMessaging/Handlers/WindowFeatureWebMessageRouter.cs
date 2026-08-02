// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class WindowFeatureWebMessageRouter {
    private static readonly IWindowFeatureWebMessageDispatcher[] RegisteredDispatchers = [
        new BrowserWebMessageDispatcher(),
        new DebuggingWebMessageDispatcher(),
        new DecorationsWebMessageDispatcher(),
        new FilePickerDialogsWebMessageDispatcher(),
        new InvokeWebMessageDispatcher(),
        new LifecycleWebMessageDispatcher(),
        new MonitorsWebMessageDispatcher(),
        new NotificationsWebMessageDispatcher(),
        new PageNavigationWebMessageDispatcher(),
        new PositionWebMessageDispatcher(),
        new SizeWebMessageDispatcher(),
        new StateWebMessageDispatcher(),
        new WebMessagingWebMessageDispatcher()
    ];

    private static readonly Dictionary<string, IWindowFeatureWebMessageDispatcher> Dispatchers
        = RegisteredDispatchers.ToDictionary(keySelector: dispatcher => dispatcher.FeatureName, StringComparer.OrdinalIgnoreCase);

    internal static IReadOnlyList<string> RegisteredFeatureNames
        => RegisteredDispatchers.Select(dispatcher => dispatcher.FeatureName).ToArray();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal static string Get(
        IInfiniFrameWindow window,
        string featureName,
        string command,
        JsonElement? args
    ) => Serialize(Resolve(featureName).Get(window, command, args));

    internal static void Post(
        IInfiniFrameWindow window,
        string featureName,
        string command,
        JsonElement? args
    ) => Resolve(featureName).Post(window, command, args);

    private static IWindowFeatureWebMessageDispatcher Resolve(string featureName)
        => Dispatchers.TryGetValue(featureName, out IWindowFeatureWebMessageDispatcher? dispatcher)
            ? dispatcher
            : throw new InvalidOperationException($"Window feature '{featureName}' is not supported.");

    private static string Serialize(object? value) {
        if (value is null) return "null";

        JsonTypeInfo typeInfo = WindowFeatureWebMessageJsonContext.Default.GetTypeInfo(value.GetType())
            ?? throw new InvalidOperationException($"No JSON metadata is registered for '{value.GetType()}'.");
        return JsonSerializer.Serialize(value, typeInfo);
    }
}