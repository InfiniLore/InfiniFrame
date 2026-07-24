using InfiniFrame;

namespace InfiniAutomationTests.BlazorWebView.MudBlazor;

public sealed record WindowTestState(string Title, bool IsFullScreen) {
    public static WindowTestState Default { get; } = new(
        Title: "InfiniFrame Playwright BlazorWebView",
        IsFullScreen: false
    );

    public void Restore(IInfiniFrameWindow window) {
        window.Features.State.SetFullScreen(IsFullScreen);
        window.Features.Decorations.SetTitle(Title);
    }
}

public sealed class WindowTestStateResetCoordinator {
    public event Func<Task>? Resetting;

    public async Task RestoreAsync(IInfiniFrameWindow window) {
        WindowTestState.Default.Restore(window);

        if (Resetting is null) return;

        foreach (Func<Task> reset in Resetting.GetInvocationList().Cast<Func<Task>>()) {
            await reset();
        }
    }
}
