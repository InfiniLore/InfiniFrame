// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InfiniFrame.Window.Features.Menu;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(InfiniFrameMenuBar))]
[JsonSerializable(typeof(InfiniFrameMenuItem))]
[JsonSerializable(typeof(InfiniFrameMenuItemType))]
internal partial class MenuJsonContext : JsonSerializerContext;
