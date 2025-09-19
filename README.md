# InfiniLore.Photino

A rework of the [Photino.Net](https://github.com/tryphotino/photino.NET), [Photino.Net.Server](https://github.com/tryphotino/photino.NET.Server) and [Photino.Blazor](https://github.com/tryphotino/photino.Blazor) projects, to make them more modern and easier to use within a DI container.

This project is mainly meant to be used as a dependency for Infinilore, so my goals are focused on that, but it should be possible to use this as a replacement for the original `Photino.NET`, `Photino.NET.Server` and `Photino.Blazor` projects, minding the breaking changes mentioned below.


This project is not affected with- or endorsed by the original authors of Photino.NET and Photino.Blazor.

---

## Status

This project is currently in a *very early stage of converting the old code base to more modern C# and .NET*.

## Breaking changes

Although I have been able to keep most of the original API so far, the following breaking changes have been made:

**Solution wide changes**
- Nullability: The entire project now has nullable enabled, this means that some of the return types of methods have been changed from `object` to `object?`, but most of the time this shouldn't be a problem given this now mimics what the actual code base was doing.

**Photino.NET changes**
- `PhotinoWindow.Log()` and `PhotinoWindow.Verbosity` have been removed from the codebase and have been replaced with a `ILogger` approach so proper logging handlers can be injected.
When a logger isn't defined in the DI container creating the `PhotinoWindow`, it will create a default console logger that will log to the console.
- `PhotinoWindow.Fullscreen` is not monitor aware and resizes to the original size of the window when you exit fullscreen.

- Full rework of the `PhotinoWindow` api to separate initializing logic from run time logic
  - `PhotinoWindow.Centered` property is now only available during usage of `PhotinoWindowBuilder`
  - `PhotinoWindow.Chromeless` setter property is now only available during usage of `PhotinoWindowBuilder`
  - `PhotinoWindow.Transparent` setter property is now only available during usage of `PhotinoWindowBuilder`

**Photino.NET.Server changes**
- `PhotinoServer(webRootFolder:...)` is no longer hard coded to depend on starting from the `Resources/` folder and is now fully configurable and has the default value of `wwwroot`. 
- `PhotinoServer.CreateStaticFileServer()` is completely replaced by a combination of `PhotinoServerBuilder.Create()` and `PhotinoServerBuilder.Build()` to create the static file server in a more fluent API way.
This also adds the `PhotinoServer.GetAttachedWindow()` method to immediately get an attached window to the server without further setup.

---

## Examples

## Repo history

This repo was originally forked from [Photino.NET](https://github.com/tryphotino/photino.NET) and then the history of
the [Photino.Blazor](https://github.com/tryphotino/photino.Blazor) and [Photino.Net.Server](https://github.com/tryphotino/photino.NET.Server) repositories were merged into this.
This means that GitHub will only show a direct reference to the original [Photino.NET](https://github.com/tryphotino/photino.NET), when trying to create pull requests.
By merging the histories, it was possible to ease development a lot, especially whilst also preserving the original commit history and attribution from the contributors of Photino.NET.

## License

Unlike the other projects in the InfiniLore ecosystem, which all follow GPL or LGPL, this repo follows the same [Apache-2.0 license file](LICENSE) from [Photino.NET](https://github.com/tryphotino/photino.NET) to adhere to the original licensing without the need for extra modifications to the license.