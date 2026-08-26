// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using TUnit.Core.Interfaces;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class MacOsMainThreadTestExecutorAttribute : Attribute, ITestExecutor {
    private static readonly MacOsMainThreadExecutor Executor = new();

    public async ValueTask ExecuteTest(TestContext context, Func<ValueTask> action) {
        await Executor.ExecuteTest(context, action);
    }
}
