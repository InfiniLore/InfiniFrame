// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Xml.Linq;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class ProjectInfoResolver {
    public static string ResolveFramework(string projectPath) {
        XElement root = LoadProjectRoot(projectPath);

        string? targetFramework = FindSingleValue(root, "TargetFramework");
        if (!string.IsNullOrWhiteSpace(targetFramework)) return targetFramework;

        string? targetFrameworks = FindSingleValue(root, "TargetFrameworks");
        if (string.IsNullOrWhiteSpace(targetFrameworks)) {
            throw new InvalidOperationException("Could not resolve target framework from project file. Use --framework.");
        }

        return targetFrameworks.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).First();
    }

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
