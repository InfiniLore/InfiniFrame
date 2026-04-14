// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Resolvers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class ProjectInfoResolver {
    /// <summary>
    ///     Resolves the target framework from evaluated MSBuild properties.
    /// </summary>
    /// <param name="projectPath">Path to a project file.</param>
    /// <returns>
    ///     The value of <c>TargetFramework</c>, or the first framework from <c>TargetFrameworks</c> when multi-targeted.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when no framework can be resolved from evaluated project properties.
    /// </exception>
    public static string ResolveFramework(string projectPath) {
        string? targetFramework = MsBuildPropertyResolver.TryGetProperty(projectPath, "TargetFramework");
        if (!string.IsNullOrWhiteSpace(targetFramework)) return targetFramework;

        string? targetFrameworks = MsBuildPropertyResolver.TryGetProperty(projectPath, "TargetFrameworks");

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (string.IsNullOrWhiteSpace(targetFrameworks)) {
            throw new InvalidOperationException("Could not resolve target framework from project evaluation. Use --framework.");
        }

        return targetFrameworks.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).First();
    }

    /// <summary>
    ///     Resolves the assembly name using evaluated MSBuild properties.
    /// </summary>
    /// <param name="projectPath">Path to a project file.</param>
    /// <returns>
    ///     The <c>AssemblyName</c> value when present; otherwise the project file name without extension.
    /// </returns>
    public static string ResolveAssemblyName(string projectPath) {
        string? assemblyName = MsBuildPropertyResolver.TryGetProperty(projectPath, "AssemblyName");
        return string.IsNullOrWhiteSpace(assemblyName) ? Path.GetFileNameWithoutExtension(projectPath) : assemblyName;
    }
}
