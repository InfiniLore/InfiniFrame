// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Xml.Linq;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class ProjectInfoResolver {
    /// <summary>
    /// Resolves the target framework from a project file.
    /// </summary>
    /// <param name="projectPath">Path to a project file.</param>
    /// <returns>
    /// The value of <c>TargetFramework</c>, or the first framework from <c>TargetFrameworks</c> when multi-targeted.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no framework can be resolved from the project file.
    /// </exception>
    public static string ResolveFramework(string projectPath) {
        XElement root = LoadProjectRoot(projectPath);

        string? targetFramework = FindSingleValue(root, "TargetFramework");
        if (!string.IsNullOrWhiteSpace(targetFramework)) return targetFramework;

        string? targetFrameworks = FindSingleValue(root, "TargetFrameworks");
        
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (string.IsNullOrWhiteSpace(targetFrameworks)) throw new InvalidOperationException("Could not resolve target framework from project file. Use --framework.");

        return targetFrameworks.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).First();
    }

    /// <summary>
    /// Resolves the assembly name used to determine the expected single-file output name.
    /// </summary>
    /// <param name="projectPath">Path to a project file.</param>
    /// <returns>
    /// The <c>AssemblyName</c> value when present; otherwise the project file name without extension.
    /// </returns>
    public static string ResolveAssemblyName(string projectPath) {
        XElement root = LoadProjectRoot(projectPath);
        string? assemblyName = FindSingleValue(root, "AssemblyName");
        return string.IsNullOrWhiteSpace(assemblyName) ? Path.GetFileNameWithoutExtension(projectPath) : assemblyName;
    }

    private static XElement LoadProjectRoot(string projectPath) {
        XDocument document = XDocument.Load(projectPath);
        return document.Root ?? throw new InvalidOperationException("Invalid project file.");
    }

    private static string? FindSingleValue(XElement root, string localName) {
        return root.Descendants().FirstOrDefault(x => x.Name.LocalName == localName)?.Value.Trim();
    }
}
