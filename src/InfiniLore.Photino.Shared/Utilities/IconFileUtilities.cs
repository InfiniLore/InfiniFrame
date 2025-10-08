using System.Diagnostics.CodeAnalysis;

namespace InfiniLore.Photino.Utilities;

public static class IconFileUtilities {
    public static bool IsValidIconFile([NotNullWhen(true)] string? filePath) {
        if (string.IsNullOrWhiteSpace(filePath)) return false;
        if (File.Exists(filePath)) return true;

        string absolutePath = $"{AppContext.BaseDirectory}{filePath}";
        return File.Exists(absolutePath);
    }
}
