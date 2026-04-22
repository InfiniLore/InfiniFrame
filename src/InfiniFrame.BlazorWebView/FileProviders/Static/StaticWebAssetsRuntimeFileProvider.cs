// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace InfiniFrame.BlazorWebView.FileProviders.Static;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class StaticWebAssetsRuntimeFileProvider(string[] contentRoots, StaticWebAssetNode root) : IFileProvider {
    private const RegexOptions PatternRegexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

    private IFileProvider[] ContentRootProviders { get; } = contentRoots
        .Select(static rootPath => {
            string normalizedRoot = rootPath;
            if (!Path.IsPathRooted(normalizedRoot)) {
                normalizedRoot = Path.GetFullPath(normalizedRoot);
            }

            return Directory.Exists(normalizedRoot)
                ? (IFileProvider)new PhysicalFileProvider(normalizedRoot)
                : new NullFileProvider();
        })
        .ToArray();

    private StaticWebAssetNode Root { get; } = root;
    private readonly ConcurrentDictionary<string, Regex> _patternRegexCache = new(StringComparer.Ordinal);

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static IFileProvider? TryCreate(string baseDirectory) {
        if (string.IsNullOrWhiteSpace(baseDirectory)) return null;

        ManifestCandidate[] candidates = GetManifestCandidates(baseDirectory).ToArray();
        if (candidates.Length == 0) return null;

        ScoredManifestCandidate? bestCandidate = null;
        foreach (ManifestCandidate candidate in candidates) {
            if (!TryLoadManifest(candidate.ManifestPath, out StaticWebAssetManifest? manifest)) continue;
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
            string[] contentRoots = bestCandidate.Manifest.ContentRoots!
                .Select(contentRoot => Path.IsPathRooted(contentRoot)
                    ? contentRoot
                    : Path.GetFullPath(Path.Join(baseDirectory, contentRoot)))
                .ToArray();

            return new StaticWebAssetsRuntimeFileProvider(contentRoots, bestCandidate.Manifest.Root!);
        }
        catch {
            return null;
        }
    }

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

    public IChangeToken Watch(string filter) {
        return NullChangeToken.Singleton;
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

    private static bool TryLoadManifest(string manifestPath, out StaticWebAssetManifest? manifest) {
        manifest = null;
        try {
            string json = File.ReadAllText(manifestPath);
            manifest = JsonSerializer.Deserialize(json, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);
            return manifest is not null;
        }
        catch {
            return false;
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
                return new ManifestDirectoryContents(Array.Empty<IFileInfo>());
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

    private IFileInfo GetAssetFileInfo(StaticWebAsset asset) {
        return GetContentRootFileInfo(asset.ContentRootIndex, NormalizeSubPath(asset.SubPath));
    }

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
        return _patternRegexCache.GetOrAdd(pattern, static key => {
            string normalizedPattern = NormalizeSubPath(key);
            if (string.IsNullOrEmpty(normalizedPattern)) {
                normalizedPattern = "**";
            }

            string regexPattern = GlobToRegex(normalizedPattern);
            return new Regex(regexPattern, PatternRegexOptions);
        });
    }

    private static string GlobToRegex(string pattern) {
        var regex = new System.Text.StringBuilder("^");
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
}
