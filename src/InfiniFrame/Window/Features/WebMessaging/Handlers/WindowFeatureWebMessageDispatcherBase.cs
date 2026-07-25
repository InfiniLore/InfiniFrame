// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal abstract class WindowFeatureWebMessageDispatcherBase<TFeature> : IWindowFeatureWebMessageDispatcher {
    public abstract string FeatureName { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public object? Get(IInfiniFrameWindow window, string command, JsonElement? args)
        => Get(SelectFeature(window.Features), command, args);

    public void Post(IInfiniFrameWindow window, string command, JsonElement? args)
        => Post(SelectFeature(window.Features), command, args);

    protected abstract TFeature SelectFeature(IInfiniFrameWindowFeatures features);

    protected virtual object? Get(TFeature feature, string command, JsonElement? args)
        => throw Unsupported(command);

    protected virtual void Post(TFeature feature, string command, JsonElement? args)
        => throw Unsupported(command);

    protected static T Required<T>(JsonElement? args, string name) {
        if (args is not { ValueKind: JsonValueKind.Object } value || !value.TryGetProperty(name, out JsonElement property))
            throw new ArgumentException($"Argument '{name}' is required.");

        if (property.ValueKind == JsonValueKind.Null)
            throw new ArgumentException($"Argument '{name}' cannot be null.");

        return Deserialize<T>(property, name)
            ?? throw new ArgumentException($"Argument '{name}' cannot be null.");
    }

    protected static T Arg<T>(JsonElement? args, string name, T fallback) {
        if (args is not { ValueKind: JsonValueKind.Object } value
            || !value.TryGetProperty(name, out JsonElement property)
            || property.ValueKind == JsonValueKind.Null) {
            return fallback;
        }

        return Deserialize<T>(property, name) ?? fallback;
    }

    private static T? Deserialize<T>(JsonElement property, string name) {
        JsonTypeInfo typeInfo = WindowFeatureWebMessageJsonContext.Default.GetTypeInfo(typeof(T))
            ?? throw new InvalidOperationException($"No JSON metadata is registered for '{typeof(T)}'.");
        try {
            return (T?)property.Deserialize(typeInfo);
        }
        catch (JsonException exception) {
            throw new ArgumentException($"Argument '{name}' is invalid.", name, exception);
        }
    }

    protected InvalidOperationException Unsupported(string command)
        => new($"Window feature command '{FeatureName}:{command}' is not supported.");
}
