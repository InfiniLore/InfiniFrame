// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniTests;
using Microsoft.Playwright;

namespace InfiniAutomationTests.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class SharedWindowChromeTests : InfiniFramePlaywrightTestBase {

    private const string SetupInterceptHtml = """
        if (!window.__originalSendMessageToHost) {
            window.__testMessageLog = [];
            window.__originalSendMessageToHost = window.infiniframe.messaging.sendMessageToHost;
            window.infiniframe.messaging.sendMessageToHost = function(id, payload) {
                window.__testMessageLog.push({ id, payload });
            };
        }
        """;

    private const string CleanupHtml = """
        window.infiniframe.windowChrome.unregister();
        document.querySelectorAll('[id^="test-"]').forEach(el => el.remove());
        if (window.__originalSendMessageToHost) {
            window.infiniframe.messaging.sendMessageToHost = window.__originalSendMessageToHost;
            delete window.__originalSendMessageToHost;
        }
        delete window.__testMessageLog;
        """;

    private static async Task SetupTestAsync(IPage page, string createElementHtml) {
        await EvaluateWhenPageReadyAsync(page, createElementHtml);
        await EvaluateWhenPageReadyAsync(page, SetupInterceptHtml);
        await EvaluateWhenPageReadyAsync(page, "window.infiniframe.windowChrome.register({})");
    }

    private static async Task<bool> HasMessageAsync(IPage page, string predicate) {
        return await EvaluateWhenPageReadyAsync<bool>(page,
            $"() => window.__testMessageLog.some(m => {predicate})");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task WindowChromeModule_Exists(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        bool exists = await EvaluateWhenPageReadyAsync<bool>(
            page,
            "() => window.infiniframe?.windowChrome !== undefined && window.infiniframe?.windowChrome !== null"
        );

        await Assert.That(exists).IsTrue();
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task WindowChrome_DataAttributeDragRegion_IsDetected(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        try {
            await SetupTestAsync(page, """
                const el = document.createElement('div');
                el.setAttribute('data-infiniframe-drag-region', '');
                el.id = 'test-drag-region';
                document.body.appendChild(el);
                """);

            await EvaluateWhenPageReadyAsync(page, """
                document.getElementById('test-drag-region')
                    .dispatchEvent(new PointerEvent('pointerdown', { button: 0, pointerId: 1, bubbles: true }));
                """);

            await Assert.That(await HasMessageAsync(page, "true")).IsTrue();
        }
        finally {
            await EvaluateWhenPageReadyAsync(page, CleanupHtml);
        }
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task WindowChrome_DataAttributeResize_IsDetected(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        try {
            await SetupTestAsync(page, """
                const el = document.createElement('div');
                el.setAttribute('data-infiniframe-resize', 'top');
                el.id = 'test-resize-top';
                document.body.appendChild(el);
                """);

            await EvaluateWhenPageReadyAsync(page, """
                const el = document.getElementById('test-resize-top');
                el.dispatchEvent(new PointerEvent('pointerdown', { button: 0, pointerId: 1, bubbles: true }));
                el.dispatchEvent(new PointerEvent('pointermove', { button: 0, pointerId: 1, movementX: 10, movementY: 5, bubbles: true }));
                el.dispatchEvent(new PointerEvent('pointerup', { button: 0, pointerId: 1, bubbles: true }));
                """);

            await Assert.That(await HasMessageAsync(page, "m.payload?.command?.includes('resize')")).IsTrue();
        }
        finally {
            await EvaluateWhenPageReadyAsync(page, CleanupHtml);
        }
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task WindowChrome_MinimizeButton_SendsMinimizeMessage(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        try {
            await SetupTestAsync(page, """
                const el = document.createElement('div');
                el.setAttribute('data-infiniframe-window-action', 'minimize');
                el.id = 'test-minimize-btn';
                document.body.appendChild(el);
                """);

            await EvaluateWhenPageReadyAsync(page,
                "document.getElementById('test-minimize-btn').click()");

            await Assert.That(await HasMessageAsync(page, "m.payload?.command?.includes('minimize')")).IsTrue();
        }
        finally {
            await EvaluateWhenPageReadyAsync(page, CleanupHtml);
        }
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task WindowChrome_MaximizeButton_SendsToggleMaximizeMessage(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        try {
            await SetupTestAsync(page, """
                const el = document.createElement('div');
                el.setAttribute('data-infiniframe-window-action', 'maximize');
                el.id = 'test-maximize-btn';
                document.body.appendChild(el);
                """);

            await EvaluateWhenPageReadyAsync(page,
                "document.getElementById('test-maximize-btn').click()");

            await Assert.That(await HasMessageAsync(page, "m.payload?.command?.includes('toggleMaximize')")).IsTrue();
        }
        finally {
            await EvaluateWhenPageReadyAsync(page, CleanupHtml);
        }
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task WindowChrome_CloseButton_SendsCloseMessage(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        try {
            await SetupTestAsync(page, """
                const el = document.createElement('div');
                el.setAttribute('data-infiniframe-window-action', 'close');
                el.id = 'test-close-btn';
                document.body.appendChild(el);
                """);

            await EvaluateWhenPageReadyAsync(page,
                "document.getElementById('test-close-btn').click()");

            await Assert.That(await HasMessageAsync(page, "m.payload?.command?.includes(':close')")).IsTrue();
        }
        finally {
            await EvaluateWhenPageReadyAsync(page, CleanupHtml);
        }
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task WindowChrome_DoubleClickDragRegion_SendsToggleMaximizeMessage(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        try {
            await SetupTestAsync(page, """
                const el = document.createElement('div');
                el.setAttribute('data-infiniframe-drag-region', '');
                el.id = 'test-drag-region';
                document.body.appendChild(el);
                """);

            await EvaluateWhenPageReadyAsync(page, """
                const el = document.getElementById('test-drag-region');
                el.dispatchEvent(new MouseEvent('dblclick', { bubbles: true }));
                """);

            await Assert.That(await HasMessageAsync(page, "m.payload?.command?.includes('toggleMaximize')")).IsTrue();
        }
        finally {
            await EvaluateWhenPageReadyAsync(page, CleanupHtml);
        }
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task WindowChrome_Register_WithExplicitConfig_AttachesToMatchingElements(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        try {
            await EvaluateWhenPageReadyAsync(page, """
                const el = document.createElement('div');
                el.setAttribute('data-infiniframe-window-action', 'minimize');
                el.id = 'test-minimize-btn';
                document.body.appendChild(el);
                """);
            await EvaluateWhenPageReadyAsync(page, SetupInterceptHtml);
            await EvaluateWhenPageReadyAsync(page, """
                window.infiniframe.windowChrome.register({
                    controls: { minimize: '#test-minimize-btn' }
                })
                """);

            await EvaluateWhenPageReadyAsync(page,
                "document.getElementById('test-minimize-btn').click()");

            await Assert.That(await HasMessageAsync(page, "m.payload?.command?.includes('minimize')")).IsTrue();
        }
        finally {
            await EvaluateWhenPageReadyAsync(page, CleanupHtml);
        }
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task WindowChrome_Unregister_DetachesAllListeners(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        try {
            await SetupTestAsync(page, """
                const el = document.createElement('div');
                el.setAttribute('data-infiniframe-window-action', 'minimize');
                el.id = 'test-minimize-btn';
                document.body.appendChild(el);
                """);

            await EvaluateWhenPageReadyAsync(page, "window.infiniframe.windowChrome.unregister()");
            await EvaluateWhenPageReadyAsync(page, "window.__testMessageLog = []");

            await EvaluateWhenPageReadyAsync(page,
                "document.getElementById('test-minimize-btn').click()");

            int messageCount = await EvaluateWhenPageReadyAsync<int>(
                page,
                "() => window.__testMessageLog.length"
            );

            await Assert.That(messageCount).IsEqualTo(0);
        }
        finally {
            await EvaluateWhenPageReadyAsync(page, CleanupHtml);
        }
    }
}
