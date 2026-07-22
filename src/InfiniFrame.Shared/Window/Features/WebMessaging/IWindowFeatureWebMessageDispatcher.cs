// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal interface IWindowFeatureWebMessageDispatcher {
    string FeatureName { get; }

    object? Get(IInfiniFrameWindow window, string command, JsonElement? args);

    void Post(IInfiniFrameWindow window, string command, JsonElement? args);
}
