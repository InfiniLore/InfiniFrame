// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using InfiniFrame.NativeBridge.Delegates;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameEvents {
    private const int StreamCopyBufferSize = 81920;
    private static readonly CppReleaseCustomSchemeResponseDelegate ReleaseCustomSchemeResponse = ReleaseResponseStorage;
    private static long _activeCustomSchemeResponseAllocations;

    /// <inheritdoc cref="IInfiniFrameEvents.OnCustomScheme" />
    public int OnCustomScheme(string url, ref CustomSchemeResponse response) {
        // Native owns the descriptor and initializes it to zero. Never expose partially populated ownership state.
        response = default;

        try {
            ArgumentNullException.ThrowIfNull(Sender);
            ArgumentNullException.ThrowIfNull(url);

            Logger.LogDebug("Custom scheme request: {Url}", url);
            int colonPos = url.IndexOf(':');
            if (colonPos <= 0) {
                Logger.LogWarning("Ignoring malformed custom scheme URL: {Url}", url);
                return 0;
            }

            string scheme = url[..colonPos].ToLowerInvariant();
            if (!EventsStore.CustomScheme.TryInvoke(scheme, Sender, url, out (Stream? Data, string? ContentType) result)) {
                Logger.LogDebug("Custom scheme could not be found for `{Scheme}`", scheme);
                return 0;
            }

            if (result.Data is null) {
                Logger.LogDebug("Custom scheme handler returned null content for URL '{Url}'", url);
                return 0;
            }

            using (result.Data) {
                response = BufferResponse(result.Data, result.ContentType);
            }

            Logger.LogDebug(
                "Custom scheme response for {Url}. {NumBytes} bytes, ContentType={ContentType}",
                url, response.ContentLength, result.ContentType ?? "application/octet-stream"
            );
            return 1;
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            // Exceptions must never unwind through a reverse P/Invoke boundary.
            if (response.OwnerContext != IntPtr.Zero) ReleaseResponseStorage(response.OwnerContext);
            response = default;
            Logger.LogError(ex, "Custom scheme handler failed for URL '{Url}'.", url);
            return 0;
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private void ApplyCustomSchemeNames(ref InfiniFrameNativeParameters startupParameters) {
        var availableHandlers = new HashSet<string>(EventsStore.CustomScheme.Snapshot.Select(static item => item.Key), StringComparer.Ordinal);
        var seen = new HashSet<string>(StringComparer.Ordinal);

        IntPtr[] customSchemeNameArray = CustomSchemeNameMemory.Allocate(
            EventsStore.CustomScheme.Snapshot.Keys.Where(key => seen.Add(key) && availableHandlers.Contains(key))
        );

        CustomSchemeNameMemory.FreeAll(startupParameters.CustomSchemeNames);
        startupParameters.CustomSchemeNames = customSchemeNameArray;
    }

    private static CustomSchemeResponse BufferResponse(Stream source, string? contentType) {
        string normalizedContentType = string.IsNullOrWhiteSpace(contentType)
            ? "application/octet-stream"
            : contentType;
        if (normalizedContentType.IndexOfAny(['\r', '\n', '\0', '\t']) >= 0)
            throw new InvalidDataException("Custom scheme content type contains invalid control characters.");

        byte[] contentTypeBytes = Encoding.UTF8.GetBytes(normalizedContentType);
        if (contentTypeBytes.Length > CustomSchemeResponse.MaxContentTypeBytes)
            throw new InvalidDataException($"Custom scheme content type exceeds the {CustomSchemeResponse.MaxContentTypeBytes}-byte limit.");

        long? knownLength = TryGetRemainingLength(source);
        if (knownLength is < 0 || (ulong)(knownLength ?? 0) > CustomSchemeResponse.MaxBufferedBodyBytes)
            throw new InvalidDataException($"Custom scheme response exceeds the {CustomSchemeResponse.MaxBufferedBodyBytes}-byte limit.");

        return knownLength.HasValue
            ? BufferKnownLengthResponse(source, checked((int)knownLength.Value), contentTypeBytes)
            : BufferUnknownLengthResponse(source, contentTypeBytes);
    }

    private static long? TryGetRemainingLength(Stream source) {
        if (!source.CanSeek) return null;

        try {
            return checked(source.Length - source.Position);
        }
        catch (NotSupportedException) {
            return null;
        }
    }

    private static CustomSchemeResponse BufferKnownLengthResponse(
        Stream source,
        int reservedBodyLength,
        byte[] contentTypeBytes
    ) {
        IntPtr storage = AllocateResponseStorage(reservedBodyLength, contentTypeBytes.Length);
        byte[] copyBuffer = ArrayPool<byte>.Shared.Rent(Math.Min(StreamCopyBufferSize, Math.Max(reservedBodyLength, 1)));
        int written = 0;
        try {
            while (written < reservedBodyLength) {
                int read = source.Read(copyBuffer, 0, Math.Min(copyBuffer.Length, reservedBodyLength - written));
                if (read == 0) break;

                Marshal.Copy(copyBuffer, 0, IntPtr.Add(storage, written), read);
                written = checked(written + read);
            }

            if (written == reservedBodyLength && source.ReadByte() != -1)
                throw new InvalidDataException("Custom scheme stream grew while it was being read.");

            IntPtr contentTypePointer = IntPtr.Add(storage, reservedBodyLength);
            WriteContentType(contentTypePointer, contentTypeBytes);
            return CreateOwnedResponse(storage, contentTypePointer, written);
        }
        catch {
            ReleaseResponseStorage(storage);
            throw;
        }
        finally {
            ArrayPool<byte>.Shared.Return(copyBuffer);
        }
    }

    private static CustomSchemeResponse BufferUnknownLengthResponse(Stream source, byte[] contentTypeBytes) {
        using var buffered = new MemoryStream();
        byte[] copyBuffer = ArrayPool<byte>.Shared.Rent(StreamCopyBufferSize);
        try {
            while (true) {
                int read = source.Read(copyBuffer, 0, copyBuffer.Length);
                if (read == 0) break;

                if ((ulong)buffered.Length + (uint)read > CustomSchemeResponse.MaxBufferedBodyBytes)
                    throw new InvalidDataException($"Custom scheme response exceeds the {CustomSchemeResponse.MaxBufferedBodyBytes}-byte limit.");

                buffered.Write(copyBuffer, 0, read);
            }
        }
        finally {
            ArrayPool<byte>.Shared.Return(copyBuffer);
        }

        int bodyLength = checked((int)buffered.Length);
        IntPtr storage = AllocateResponseStorage(bodyLength, contentTypeBytes.Length);
        try {
            if (bodyLength > 0) {
                ArraySegment<byte> body = buffered.TryGetBuffer(out ArraySegment<byte> segment)
                    ? segment
                    : throw new InvalidOperationException("Could not access the buffered custom scheme response.");
                Marshal.Copy(body.Array!, body.Offset, storage, bodyLength);
            }

            IntPtr contentTypePointer = IntPtr.Add(storage, bodyLength);
            WriteContentType(contentTypePointer, contentTypeBytes);
            return CreateOwnedResponse(storage, contentTypePointer, bodyLength);
        }
        catch {
            ReleaseResponseStorage(storage);
            throw;
        }
    }

    private static IntPtr AllocateResponseStorage(int bodyLength, int contentTypeLength) {
        int allocationSize = checked(bodyLength + contentTypeLength + 1);
        IntPtr storage = Marshal.AllocCoTaskMem(allocationSize);
        if (storage == IntPtr.Zero)
            throw new OutOfMemoryException($"Failed to allocate {allocationSize} bytes for custom scheme response.");

        Interlocked.Increment(ref _activeCustomSchemeResponseAllocations);
        return storage;
    }

    private static void WriteContentType(IntPtr destination, byte[] contentTypeBytes) {
        if (contentTypeBytes.Length > 0) Marshal.Copy(contentTypeBytes, 0, destination, contentTypeBytes.Length);
        Marshal.WriteByte(destination, contentTypeBytes.Length, 0);
    }

    private static CustomSchemeResponse CreateOwnedResponse(IntPtr storage, IntPtr contentType, int bodyLength) => new() {
        StructSize = checked((uint)Marshal.SizeOf<CustomSchemeResponse>()),
        AbiVersion = CustomSchemeResponse.CurrentAbiVersion,
        StatusCode = 200,
        BodyKind = CustomSchemeResponse.BufferedBodyKind,
        ContentLength = checked((ulong)bodyLength),
        Body = bodyLength == 0 ? IntPtr.Zero : storage,
        ContentTypeUtf8 = contentType,
        OwnerContext = storage,
        Release = Marshal.GetFunctionPointerForDelegate(ReleaseCustomSchemeResponse)
    };

    private static void ReleaseResponseStorage(IntPtr ownerContext) {
        if (ownerContext == IntPtr.Zero) return;

        Marshal.FreeCoTaskMem(ownerContext);
        Interlocked.Decrement(ref _activeCustomSchemeResponseAllocations);
    }
}
