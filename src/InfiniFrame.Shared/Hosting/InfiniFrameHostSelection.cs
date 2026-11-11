// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>
///     Process-wide, engine-neutral latch: a process uses either the default native backend or one explicitly selected
///     external host provider, chosen before native initialization. No hot switching and no mixed engines.
/// </summary>
internal static class InfiniFrameHostSelection {
    internal enum Kind {
        None = 0,
        Default = 1,
        Hosted = 2
    }

    private static int _selected;

    internal static Kind Current => (Kind)Volatile.Read(ref _selected);

    /// <summary>Latches <paramref name="kind" />; throws before any native work when the other kind is already selected.</summary>
    internal static void Select(Kind kind) {
        if (kind == Kind.None) throw new ArgumentOutOfRangeException(nameof(kind));
        int previous = Interlocked.CompareExchange(ref _selected, (int)kind, (int)Kind.None);
        if (previous == (int)Kind.None || previous == (int)kind) return;
        throw new InvalidOperationException(
            $"This process already uses {Describe((Kind)previous)}; it cannot also use {Describe(kind)}. " +
            "InfiniFrame's window host is selected once per process, before native initialization. " +
            "Mixing or switching between the default native backend and an external host provider is not supported.");
    }

    /// <summary>Test support only: the latch is process-wide, so isolated tests must reset it.</summary>
    internal static void ResetForTests() => Volatile.Write(ref _selected, (int)Kind.None);

    private static string Describe(Kind kind) => kind == Kind.Hosted ? "an external host provider" : "the default native backend";
}
