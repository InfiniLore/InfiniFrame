# InfiniLore.Photino

A rework of the [`Photino.Net`](https://github.com/tryphotino/photino.NET) and [`Photino.Blazor`](https://github.com/tryphotino/photino.Blazor) projects, to make them more modern and easier to use within a DI container.

This project is mainly meant to be used as a dependency for Infinilore, so my goals are focused on that, but it should be possible to use this as a replacement for the original `Photino.NET` and `Photino.Blazor` projects, minding the breaking changes mentioned below.

---

## Status

This project is currently in a *very early stage of converting the old code base to more modern C# and .NET*.

Although I have been able to keep most of the original API so far, the following breaking changes have been made:
- Nullability: The entire project now has nullable enabled, this means that some of the return types of methods have been changed from `object` to `object?`, but most of the time this shouldn't be a problem given this now mimics what the actual code base was doing.
- `PhotinoWindow.Log()` and `PhotinoWindow.Verbosity` have been removed from the codebase and have been replaced with a `ILogger` approach so proper logging handlers can be injected.
When a logger isn't defined in the DI container creating the `PhotinoWindow`, it will create a default console logger that will log to the console.

---

## Repo history

This repo was originally forked from [`Photino.NET`](https://github.com/tryphotino/photino.NET) and then the history of
the [`Photino.Blazor`](https://github.com/tryphotino/photino.Blazor) repo was merged into this.
This means that GitHub will only show a reference to the original [
`Photino.NET`](https://github.com/tryphotino/photino.NET), though the git history and thus proper attribution of both
repos is preserved.

## License

Unlike the other projects in the InfiniLore ecosystem, which all follow GPL or LGPL, this repo follows the
same [ Apache-2.0 license file](LICENSE) from [`Photino.NET`](https://github.com/tryphotino/photino.NET) to adhere to
the original licensing without the need for extra modifications to the license.