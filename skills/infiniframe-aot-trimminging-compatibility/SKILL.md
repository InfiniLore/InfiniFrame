---
name: infiniframe-aot-trimminging-compatibility
description: Publishing InfiniFrame apps with NativeAOT and trimming enabled. Handling trim warnings, rooting types, and CI validation.
---
# InfiniFrame AOT/Trimming Compatibility

> Skill for publishing InfiniFrame apps with NativeAOT and trimming enabled.

## When to Use This Skill

- Publishing with NativeAOT
- Publishing with trimming enabled
- Reducing app binary size
- Improving startup performance
- Understanding trim/AOT annotations

## Compatibility Guarantees

- Public APIs relying on runtime reflection or dynamic code generation are annotated with:
  - `RequiresUnreferencedCode`
  - `RequiresDynamicCode`
- Trim/AOT compatibility checks run in CI and must pass before release
- `InfiniFrame.Tools.Pack` validated with NativeAOT smoke publish using:
  - `PublishTrimmed=true`
  - `PublishAot=true`

## Consumer Guidance

### Treat Warnings as Actionable

Trim/AOT warnings from annotated APIs should be addressed:
- Keep required types/members rooted
- OR avoid those APIs in fully trimmed flows

### Validate Final Publish Profile in CI

```bash
dotnet publish -c Release -r <RID> -p:PublishTrimmed=true -p:PublishAot=true
```

### Framework Features Requiring Reflection

If app uses framework features depending on reflection:
- Configuration binding
- Runtime component activation
- Etc.

Account for their requirements explicitly in trimming strategy.

## Publishing with Trimming

### Basic Trimmed Publish

```bash
dotnet publish -c Release -r win-x64 -p:PublishTrimmed=true
```

### Trimmed Publish with Analysis

```bash
dotnet publish -c Release -r win-x64 -p:PublishTrimmed=true -p:TrimMode=link
```

`TrimMode=link` — more aggressive, removes unused members

## Publishing with NativeAOT

### Basic NativeAOT Publish

```bash
dotnet publish -c Release -r win-x64 -p:PublishAot=true
```

### NativeAOT + Trimming

```bash
dotnet publish -c Release -r win-x64 -p:PublishAot=true -p:PublishTrimmed=true
```

## Handling Trim Warnings

### Rooting Types with ILLink Descriptors

Create `ILLink.Descriptors.xml` in project:

```xml
<linker>
  <assembly fullname="MyAssembly">
    <type fullname="MyNamespace.MyType" preserve="all" />
  </assembly>
</linker>
```

Reference in `.csproj`:

```xml
<ItemGroup>
  <TrimmerRootDescriptor Include="ILLink.Descriptors.xml" />
</ItemGroup>
```

### Using DynamicDependency Attribute

```csharp
[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(MyType))]
static void MyMethod() {
    // Type preserved by trimmer
}
```

### Avoiding Reflection-Heavy APIs

Instead of:
```csharp
// May cause trim warnings
var obj = Activator.CreateInstance(typeName);
```

Use:
```csharp
// Trim-safe factory pattern
var obj = type switch {
    "A" => new TypeA(),
    "B" => new TypeB(),
    _ => throw new ArgumentException()
};
```

## Configuration Binding with Trimming

### Problem

```csharp
// Requires reflection — will warn with trimming
builder.Configuration.GetSection("Settings").Get<MySettings>();
```

### Solutions

#### Option 1: Options Pattern with Binding

```csharp
builder.Services.Configure<MySettings>(builder.Configuration.GetSection("Settings"));
```

#### Option 2: Manual Binding

```csharp
var settings = new MySettings {
    Title = builder.Configuration["InfiniFrame:Title"],
    Width = int.Parse(builder.Configuration["InfiniFrame:Width"] ?? "800")
};
```

## CI Validation

InfiniFrame includes CI validation lanes for:
- Trimming compatibility (net8.0, net9.0, net10.0)
- NativeAOT compatibility (net8.0, net9.0, net10.0)

### Example CI Check

```yaml
- name: Verify Trim Compatibility
  run: dotnet publish -c Release -r ${{ matrix.rid }} -p:PublishTrimmed=true
  
- name: Verify NativeAOT
  run: dotnet publish -c Release -r ${{ matrix.rid }} -p:PublishAot=true
```

## Common Patterns

### Minimal Trimmed App

```bash
dotnet publish -c Release -r win-x64 \
  -p:PublishTrimmed=true \
  -p:PublishSingleFile=true \
  -p:TrimMode=link
```

### Minimal NativeAOT App

```bash
dotnet publish -c Release -r win-x64 -p:PublishAot=true
```

### Combined (Smallest Binary)

```bash
dotnet publish -c Release -r win-x64 \
  -p:PublishAot=true \
  -p:PublishTrimmed=true \
  -p:TrimMode=link
```

## Packaging Tool Integration

`InfiniFrame.Tools.Pack` validated with NativeAOT smoke publish. To use with packaging:

```bash
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj \
  --rid win-x64 \
  --configuration Release
```

Ensure your `.csproj` has:

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <PublishTrimmed>true</PublishTrimmed>
</PropertyGroup>
```

## Anti-Patterns

❌ **Ignore trim warnings**:
```
warning IL2026: Using member 'System.Reflection.Assembly.GetType(string)' which has 'RequiresUnreferencedCodeAttribute'
```

✅ **Address warnings**:
- Root the type via descriptor
- OR refactor to avoid reflection

❌ **Expect runtime reflection to work with NativeAOT**:
```csharp
// WRONG with AOT — will fail at runtime
var type = Type.GetType("MyType");
var instance = Activator.CreateInstance(type);
```

✅ **Use compile-time patterns with AOT**:
```csharp
var instance = new MyType();  // Explicit construction
```

❌ **Mix trimming with heavy reflection**:
```csharp
// WRONG — trimmer can't track runtime type discovery
foreach (var type in assembly.GetTypes()) {
    if (typeof(IPlugin).IsAssignableFrom(type)) { ... }
}
```

✅ **Use source generation or explicit registration**:
```csharp
// Register plugins explicitly
builder.RegisterPlugin(new PluginA());
builder.RegisterPlugin(new PluginB());
```

## Supported Runtimes

Trim/AOT checks run against:
- `net8.0`
- `net9.0`
- `net10.0`

## Platform Support

| Platform | NativeAOT | Trimming |
|----------|-----------|----------|
| Windows (x64, arm64) | ✓ | ✓ |
| Linux (x64, arm64) | ✓ | ✓ |
| macOS (x64, arm64) | ✓ | ✓ |

Check .NET documentation for latest platform support matrix.
