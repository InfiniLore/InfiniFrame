// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json.Serialization;

namespace InfiniFrame.HostMessaging;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[JsonSerializable(typeof(GetMessageSuccessResponse))]
[JsonSerializable(typeof(GetMessageErrorResponse))]
internal partial class InfiniFrameWindowMessagesJsonContext : JsonSerializerContext;