// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using InfiniFrame.NativeBridge.Dialogs;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    Converters = [
        typeof(PointWebMessageJsonConverter), typeof(SizeWebMessageJsonConverter), typeof(RectangleWebMessageJsonConverter),
        typeof(CamelCaseEnumWebMessageJsonConverter<ResizeOrigin>),
        typeof(CamelCaseEnumWebMessageJsonConverter<InfiniFrameDialogButtons>),
        typeof(CamelCaseEnumWebMessageJsonConverter<InfiniFrameDialogIcon>),
        typeof(CamelCaseEnumWebMessageJsonConverter<InfiniFrameDialogResult>),
        typeof(CamelCaseEnumWebMessageJsonConverter<InfiniFrameWindowLifecycleState>),
        typeof(CamelCaseEnumWebMessageJsonConverter<InfiniFrameDebugEndpointStatus>)
    ]
)]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(InfiniMonitor))]
[JsonSerializable(typeof(InfiniMonitor[]))]
[JsonSerializable(typeof(Point))]
[JsonSerializable(typeof(Size))]
[JsonSerializable(typeof(Rectangle))]
[JsonSerializable(typeof(InfiniFrameDebugCapabilities))]
[JsonSerializable(typeof(InfiniFrameDebugDiagnostics))]
[JsonSerializable(typeof(DebugEndpointResult))]
[JsonSerializable(typeof(WindowFeatureFilePickerFilter[]))]
[JsonSerializable(typeof(ResizeOrigin))]
[JsonSerializable(typeof(InfiniFrameDialogButtons))]
[JsonSerializable(typeof(InfiniFrameDialogIcon))]
[JsonSerializable(typeof(InfiniFrameDialogResult))]
[JsonSerializable(typeof(InfiniFrameWindowLifecycleState))]
internal partial class WindowFeatureWebMessageJsonContext : JsonSerializerContext;
