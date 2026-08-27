// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace InfiniFrame.BlazorWebView.FileProviders;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class StaticWebAssetsRuntimeFileProvider(string baseDirectory, string[] contentRoots, StaticWebAssetNode root, Assembly? embeddedAssembly = null) : IFileProvider {
    private const RegexOptions PatternRegexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
    private readonly ConcurrentDictionary<string, Regex> _patternRegexCache = new(StringComparer.Ordinal);

    private IFileProvider[] ContentRootProviders { get; } = [
        .. contentRoots
            .Select(IFileProvider (rootPath) => {
                string normalizedRoot = rootPath;
                if (!Path.IsPathRooted(normalizedRoot)) {
                    normalizedRoot = Path.GetFullPath(normalizedRoot);
                }

                if (!Directory.Exists(normalizedRoot) && Path.IsPathRooted(rootPath)) {
                    string? fallback = TryResolveRelativeContentRoot(baseDirectory, rootPath);
                    if (fallback is not null) {
                        normalizedRoot = fallback;
                    }
                }

                if (Directory.Exists(normalizedRoot)) return new PhysicalFileProvider(normalizedRoot);
                if (embeddedAssembly is not null) return new EmbeddedFileProvider(embeddedAssembly, "publish");

                return new NullFileProvider();
            })
    ];

    private StaticWebAssetNode Root { get; } = root;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IFileInfo GetFileInfo(string subpath) {
        string normalizedPath = NormalizeSubPath(subpath);
        if (string.IsNullOrEmpty(normalizedPath)) return new NotFoundFileInfo(subpath);

        string[] segments = normalizedPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0 || segments.Any(static segment => segment == "..")) return new NotFoundFileInfo(subpath);

        var traversal = new List<NodeTraversalState>(capacity: segments.Length + 1) {
            new(Root, 0, string.Empty)
        };

        StaticWebAssetNode current = Root;
        int consumedSegments = 0;
        for (; consumedSegments < segments.Length; consumedSegments++) {
            string segment = segments[consumedSegments];
            StaticWebAssetNode? next = TryGetChild(current, segment);
            if (next is null) break;

            string pathPrefix = string.IsNullOrEmpty(traversal[^1].PathPrefix)
                ? segment
                : $"{traversal[^1].PathPrefix}/{segment}";

            current = next;
            traversal.Add(new NodeTraversalState(current, consumedSegments + 1, pathPrefix));
        }

        bool fullyMatched = consumedSegments == segments.Length;
        if (fullyMatched && current.Asset is not null) {
            IFileInfo fileFromAsset = GetAssetFileInfo(current.Asset);
            if (fileFromAsset.Exists) return fileFromAsset;
        }

        for (int i = traversal.Count - 1; i >= 0; i--) {
            NodeTraversalState state = traversal[i];
            if (state.Node.Patterns is null || state.Node.Patterns.Count == 0) continue;
            if (state.ConsumedSegments >= segments.Length) continue;

            string remainingPath = string.Join('/', segments[state.ConsumedSegments..]);
            string requestedRelativePath = string.IsNullOrEmpty(state.PathPrefix)
                ? remainingPath
                : $"{state.PathPrefix}/{remainingPath}";

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (StaticWebAssetPattern pattern in state.Node.Patterns) {
                if (!MatchesPattern(pattern.Pattern, remainingPath)) continue;

                IFileInfo fileFromPattern = GetContentRootFileInfo(pattern.ContentRootIndex, requestedRelativePath);
                if (fileFromPattern.Exists) return fileFromPattern;
            }
        }

        return new NotFoundFileInfo(subpath);
    }

    public IDirectoryContents GetDirectoryContents(string subpath) {
        string normalizedPath = NormalizeSubPath(subpath);
        if (string.IsNullOrEmpty(normalizedPath)) {
            return BuildDirectoryContents(Root);
        }

        string[] segments = normalizedPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0 || segments.Any(static segment => segment == "..")) return NotFoundDirectoryContents.Singleton;

        StaticWebAssetNode? node = Root;
        foreach (string segment in segments) {
            if (node is null) break;

            node = TryGetChild(node, segment);
        }

        if (node is null) return NotFoundDirectoryContents.Singleton;

        return BuildDirectoryContents(node);
    }

    public IChangeToken Watch(string filter) => NullChangeToken.Singleton;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static IFileProvider? TryCreate(string baseDirectory, Assembly? embeddedAssembly = null) {
        if (string.IsNullOrWhiteSpace(baseDirectory)) return null;

        ManifestCandidate[] candidates = [
            .. GetManifestCandidates(baseDirectory),
            .. GetManifestCandidatesFromResources(embeddedAssembly)
        ];
        if (candidates.Length == 0) return null;

        ScoredManifestCandidate? bestCandidate = null;
        foreach (ManifestCandidate candidate in candidates) {
            if (!TryLoadManifest(candidate.ManifestPath, candidate.ResourceStream, out StaticWebAssetManifest? manifest)) continue;
            if (manifest?.ContentRoots is null || manifest.ContentRoots.Length == 0 || manifest.Root is null) continue;

            int score = candidate.BaseScore;
            if (ContainsTopLevelNode(manifest.Root, "index.html")) score += 100;
            if (ContainsTopLevelNode(manifest.Root, "_framework")) score += 50;
            if (ContainsTopLevelNode(manifest.Root, "js")) score += 10;

            if (bestCandidate is null
                || score > bestCandidate.Score
                || score == bestCandidate.Score
                && string.Compare(candidate.ManifestPath, bestCandidate.ManifestPath, StringComparison.OrdinalIgnoreCase) < 0) {
                bestCandidate = new ScoredManifestCandidate(manifest, score, candidate.ManifestPath);
            }
        }

        if (bestCandidate is null) return null;

        try {
            string[] contentRoots = [
                .. bestCandidate.Manifest.ContentRoots!
                    .Select(contentRoot => Path.IsPathRooted(contentRoot)
                        ? contentRoot
                        : Path.GetFullPath(Path.Join(baseDirectory, contentRoot)))
            ];

            return new StaticWebAssetsRuntimeFileProvider(baseDirectory, contentRoots, bestCandidate.Manifest.Root!, embeddedAssembly);
        }
        catch (ArgumentException) {
            return null;
        }
        catch (IOException) {
            return null;
        }
        catch (UnauthorizedAccessException) {
            return null;
        }
        catch (NotSupportedException) {
            return null;
        }
    }

    private static IEnumerable<ManifestCandidate> GetManifestCandidates(string baseDirectory) {
        string[] runtimeManifests = Directory
            .GetFiles(baseDirectory, "*.staticwebassets.runtime.json", SearchOption.TopDirectoryOnly);
        if (runtimeManifests.Length == 0) yield break;

        string? entryAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
        string friendlyName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
        string? processName = Environment.ProcessPath is { Length: > 0 }
            ? Path.GetFileNameWithoutExtension(Environment.ProcessPath)
            : null;

        foreach (string manifestPath in runtimeManifests) {
            string manifestName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(manifestPath)));
            int baseScore = 0;

            if (!string.IsNullOrWhiteSpace(entryAssemblyName)
                && string.Equals(manifestName, entryAssemblyName, StringComparison.OrdinalIgnoreCase)) {
                baseScore += 1000;
            }

            if (!string.IsNullOrWhiteSpace(friendlyName)
                && string.Equals(manifestName, friendlyName, StringComparison.OrdinalIgnoreCase)) {
                baseScore += 500;
            }

            if (!string.IsNullOrWhiteSpace(processName)
                && string.Equals(manifestName, processName, StringComparison.OrdinalIgnoreCase)) {
                baseScore += 250;
            }

            yield return new ManifestCandidate(manifestPath, baseScore);
        }
    }

    private static bool TryLoadManifest(string manifestPath, Stream? resourceStream, out StaticWebAssetManifest? manifest) {
        manifest = null;
        try {
            string json;
            if (resourceStream is not null) {
                using var reader = new StreamReader(resourceStream);
                json = reader.ReadToEnd();
            }
            else {
                json = File.ReadAllText(manifestPath);
            }

            manifest = JsonSerializer.Deserialize(json, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);
            return manifest is not null;
        }
        catch {
            return false;
        }
    }

    private static IEnumerable<ManifestCandidate> GetManifestCandidatesFromResources(Assembly? embeddedAssembly) {
        if (embeddedAssembly is null) yield break;

        string? entryAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
        string friendlyName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
        string? processName = Environment.ProcessPath is { Length: > 0 }
            ? Path.GetFileNameWithoutExtension(Environment.ProcessPath)
            : null;

        string[] resourceNames;
        try {
            resourceNames = embeddedAssembly.GetManifestResourceNames();
        }
        catch {
            yield break;
        }

        foreach (string resourceName in resourceNames) {
            if (!resourceName.EndsWith(".staticwebassets.runtime.json", StringComparison.OrdinalIgnoreCase)) continue;

            // Match disk behavior: strip ".staticwebassets.runtime.json" suffix to get the manifest name
            string manifestName = resourceName[..^".staticwebassets.runtime.json".Length];

            int baseScore = 0;

            if (!string.IsNullOrWhiteSpace(entryAssemblyName)
                && string.Equals(manifestName, entryAssemblyName, StringComparison.OrdinalIgnoreCase)) {
                baseScore += 1000;
            }

            if (!string.IsNullOrWhiteSpace(friendlyName)
                && string.Equals(manifestName, friendlyName, StringComparison.OrdinalIgnoreCase)) {
                baseScore += 500;
            }

            if (!string.IsNullOrWhiteSpace(processName)
                && string.Equals(manifestName, processName, StringComparison.OrdinalIgnoreCase)) {
                baseScore += 250;
            }

            Stream? stream = null;
            try {
                stream = embeddedAssembly.GetManifestResourceStream(resourceName);
            }
            catch {
                // Skip resources that can't be opened
            }

            if (stream is not null) {
                yield return new ManifestCandidate(resourceName, baseScore, stream);
            }
        }
    }

    private static bool ContainsTopLevelNode(StaticWebAssetNode root, string name) {
        if (root.Children is null || root.Children.Count == 0) return false;

        return root.Children.ContainsKey(name)
            || root.Children.Keys.Any(key => string.Equals(key, name, StringComparison.OrdinalIgnoreCase));
    }

    private IDirectoryContents BuildDirectoryContents(StaticWebAssetNode node) {
        if (node.Children is null || node.Children.Count == 0) {
            if (node.Patterns is not null && node.Patterns.Count > 0) {
                return new ManifestDirectoryContents([]);
            }

            return NotFoundDirectoryContents.Singleton;
        }

        var children = new List<IFileInfo>(node.Children.Count);
        foreach ((string name, StaticWebAssetNode childNode) in node.Children) {
            if (childNode.Asset is not null) {
                IFileInfo fileInfo = GetAssetFileInfo(childNode.Asset);
                children.Add(fileInfo.Exists ? fileInfo : new NotFoundFileInfo(name));
                continue;
            }

            children.Add(new ManifestDirectoryFileInfo(name));
        }

        return new ManifestDirectoryContents(children);
    }

    private IFileInfo GetAssetFileInfo(StaticWebAsset asset) => GetContentRootFileInfo(asset.ContentRootIndex, NormalizeSubPath(asset.SubPath));

    private IFileInfo GetContentRootFileInfo(int contentRootIndex, string? subPath) {
        if (contentRootIndex < 0 || contentRootIndex >= ContentRootProviders.Length || string.IsNullOrEmpty(subPath)) {
            return new NotFoundFileInfo(subPath ?? string.Empty);
        }

        return ContentRootProviders[contentRootIndex].GetFileInfo(subPath);
    }

    private static StaticWebAssetNode? TryGetChild(StaticWebAssetNode node, string segment) {
        if (node.Children is null) return null;
        if (node.Children.TryGetValue(segment, out StaticWebAssetNode? exact)) return exact;

        foreach ((string childName, StaticWebAssetNode childNode) in node.Children) {
            if (string.Equals(childName, segment, StringComparison.OrdinalIgnoreCase)) {
                return childNode;
            }
        }

        return null;
    }

    private bool MatchesPattern(string? pattern, string relativePath) {
        if (string.IsNullOrWhiteSpace(pattern) || string.IsNullOrWhiteSpace(relativePath)) return false;

        Regex regex = GetOrCreatePatternRegex(pattern);
        return regex.IsMatch(relativePath);
    }

    private Regex GetOrCreatePatternRegex(string pattern) {
        return _patternRegexCache.GetOrAdd(pattern, valueFactory: static key => {
            string normalizedPattern = NormalizeSubPath(key);
            if (string.IsNullOrEmpty(normalizedPattern)) {
                normalizedPattern = "**";
            }

            string regexPattern = GlobToRegex(normalizedPattern);
            return new Regex(regexPattern, PatternRegexOptions);
        });
    }

    private static string GlobToRegex(string pattern) {
        var regex = new StringBuilder("^");
        for (int i = 0; i < pattern.Length; i++) {
            char c = pattern[i];
            switch (c) {
                case '*': {
                    bool isDoubleStar = i + 1 < pattern.Length && pattern[i + 1] == '*';
                    if (isDoubleStar) {
                        regex.Append(".*");
                        i++;
                    }
                    else {
                        regex.Append("[^/]*");
                    }

                    break;
                }

                case '?':
                    regex.Append("[^/]");
                    break;
                default:
                    regex.Append(Regex.Escape(c.ToString()));
                    break;
            }
        }

        regex.Append('$');
        return regex.ToString();
    }

    private static string NormalizeSubPath(string? subPath) {
        if (string.IsNullOrWhiteSpace(subPath)) return string.Empty;

        return subPath
            .Trim()
            .TrimStart('~')
            .TrimStart('/', '\\')
            .Replace('\\', '/');
    }

    private static string? TryResolveRelativeContentRoot(string baseDirectory, string originalPath) {
        string fileName = Path.GetFileName(originalPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        if (string.IsNullOrWhiteSpace(fileName)) return null;

        string searchPattern = fileName;
        string? directory = baseDirectory;
        while (!string.IsNullOrEmpty(directory)) {
            string candidate = Path.Join(directory, searchPattern);
            if (Directory.Exists(candidate)) return candidate;

            string[] children = [];
            try {
                children = Directory.GetDirectories(directory, searchPattern, SearchOption.TopDirectoryOnly);
            }
            catch (IOException) {
                // Ignore
            }
            catch (UnauthorizedAccessException) {
                // Ignore
            }

            if (children.Length > 0) return children[0];

            directory = Path.GetDirectoryName(directory);
        }

        return null;
    }
}
