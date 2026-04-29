// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json.Serialization;

namespace InfiniFrame.HostMessaging;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(GetMessageSuccessResponse))]
[JsonSerializable(typeof(GetMessageErrorResponse))]
internal partial class InfiniFrameWindowMessagesJsonContext : JsonSerializerContext;