// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class FilePickerDialogsWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<IFilePickerDialogsInfiniFrameWindowFeature> {
    public override string FeatureName => "filePickerDialogs";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected override IFilePickerDialogsInfiniFrameWindowFeature SelectFeature(IInfiniFrameWindowFeatures features)
        => features.FilePickerDialogs;

    protected override object? Get(IFilePickerDialogsInfiniFrameWindowFeature feature, string command, JsonElement? args) {
        string? defaultPath = Arg<string?>(args, "defaultPath", null);
        WindowFeatureFilePickerFilter[]? filterDtos = Arg<WindowFeatureFilePickerFilter[]?>(args, "filters", null);
        (string Name, string[] Extensions)[]? filters = filterDtos?.Select(filter => (filter.Name, filter.Extensions)).ToArray();
        string? defaultFileName = Arg<string?>(args, "defaultFileName", null);

        return command switch {
            "showOpenFile" => feature.ShowOpenFile(Arg(args, "title", "Choose file"), defaultPath, Arg(args, "multiSelect", false), filters),
            "showOpenFolder" => feature.ShowOpenFolder(Arg(args, "title", "Select folder"), defaultPath, Arg(args, "multiSelect", false)),
            "showSaveFile" => feature.ShowSaveFile(Arg(args, "title", "Save file"), defaultPath, filters, defaultFileName),
            _ => throw Unsupported(command)
        };
    }
}