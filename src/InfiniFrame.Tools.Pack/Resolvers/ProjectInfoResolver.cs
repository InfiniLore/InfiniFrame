// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Resolvers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class ProjectInfoResolver {
    /// <summary>
    /// Resolves the target framework from evaluated MSBuild properties.
    /// </summary>
    /// <param name="projectPath">Path to the project file.</param>
    /// <param name="timeout">
    /// An optional timeout specifying the maximum duration for resolving properties.
    /// If not provided, the default timeout is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that allows the operation to be cancelled.
    /// </param>
    /// <returns>
    /// The value of <c>TargetFramework</c>, or the first framework from <c>TargetFrameworks</c> when multi-targeted.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no framework can be resolved from the evaluated project properties.
    /// </exception>
    public static async Task<string> ResolveFrameworkAsync(
        string projectPath,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default
    ) {
        string? targetFramework = await MsBuildPropertyResolver.TryGetPropertyAsync(
            projectPath,
            "TargetFramework",
            timeout,
            cancellationToken);
        if (!string.IsNullOrWhiteSpace(targetFramework)) return targetFramework;

        string? targetFrameworks = await MsBuildPropertyResolver.TryGetPropertyAsync(
            projectPath,
            "TargetFrameworks",
            timeout,
            cancellationToken);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (string.IsNullOrWhiteSpace(targetFrameworks)) {
            throw new InvalidOperationException("Could not resolve target framework from project evaluation. Use --framework.");
        }

        return targetFrameworks.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).First();
    }

    /// <summary>
    /// Resolves the assembly name using evaluated MSBuild properties.
    /// </summary>
    /// <param name="projectPath">Path to a project file.</param>
    /// <param name="timeout">
    /// Optional timeout value for the operation. If null, a default timeout is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// The <c>AssemblyName</c> value when present; otherwise the project file name without extension.
    /// </returns>
    public static async Task<string> ResolveAssemblyNameAsync(
        string projectPath,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default
    ) {
        string? assemblyName = await MsBuildPropertyResolver.TryGetPropertyAsync(
            projectPath,
            "AssemblyName",
            timeout,
            cancellationToken);
        return string.IsNullOrWhiteSpace(assemblyName) ? Path.GetFileNameWithoutExtension(projectPath) : assemblyName;
    }
}
