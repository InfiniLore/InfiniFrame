// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameRootComponentListTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Add_Generic_ShouldAddComponentToList(CancellationToken ct = default) {
        // Arrange
        var list = new InfiniFrameRootComponentList();

        // Act
        list.Add<TestComponent>("#app");

        // Assert
        var items = list.ToList();
        await Assert.That(items.Count).IsEqualTo(1);
        await Assert.That(items[0].Item1).IsEqualTo(typeof(TestComponent));
        await Assert.That(items[0].Item2).IsEqualTo("#app");
    }

    [Test]
    public async Task Add_NonGeneric_WithValidComponentType_ShouldAddToList(CancellationToken ct = default) {
        // Arrange
        var list = new InfiniFrameRootComponentList();

        // Act
        list.Add(typeof(TestComponent), "#root");

        // Assert
        var items = list.ToList();
        await Assert.That(items.Count).IsEqualTo(1);
        await Assert.That(items[0].Item1).IsEqualTo(typeof(TestComponent));
        await Assert.That(items[0].Item2).IsEqualTo("#root");
    }

    [Test]
    public async Task Add_NonGeneric_WithInvalidComponentType_ShouldThrowArgumentException(CancellationToken ct = default) {
        // Arrange
        var list = new InfiniFrameRootComponentList();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Task.Run(() => {
            list.Add(typeof(string), "#root");
        }));

        // Assert
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.Message).Contains("IComponent");
    }

    [Test]
    public async Task Add_MultipleComponents_ShouldPreserveOrder(CancellationToken ct = default) {
        // Arrange
        var list = new InfiniFrameRootComponentList();

        // Act
        list.Add<TestComponent>("#first");
        list.Add<OtherComponent>("#second");

        // Assert
        var items = list.ToList();
        await Assert.That(items.Count).IsEqualTo(2);
        await Assert.That(items[0].Item2).IsEqualTo("#first");
        await Assert.That(items[1].Item2).IsEqualTo("#second");
    }

    [Test]
    public async Task JSComponents_ShouldNotBeNull(CancellationToken ct = default) {
        // Arrange

        // Act
        var list = new InfiniFrameRootComponentList();

        // Assert
        await Assert.That(list.JSComponents).IsNotNull();
    }

    [Test]
    public async Task GetEnumerator_NonGeneric_ShouldWork(CancellationToken ct = default) {
        // Arrange
        var list = new InfiniFrameRootComponentList();
        list.Add<TestComponent>("#app");

        // Act
        var enumerator = ((System.Collections.IEnumerable)list).GetEnumerator();
        bool moved = enumerator.MoveNext();

        // Assert
        await Assert.That(moved).IsTrue();
    }

    private sealed class TestComponent : IComponent {
        public void Attach(RenderHandle renderHandle) { }
        public Task SetParametersAsync(ParameterView parameters) => Task.CompletedTask;
    }

    private sealed class OtherComponent : IComponent {
        public void Attach(RenderHandle renderHandle) { }
        public Task SetParametersAsync(ParameterView parameters) => Task.CompletedTask;
    }
}
