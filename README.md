# InfiniFrame

A rework of
the [Photino.Net](https://github.com/tryphotino/photino.NET), [Photino.Net.Server](https://github.com/tryphotino/photino.NET.Server)
and [Photino.Blazor](https://github.com/tryphotino/Photino.Blazor)
and [Photino.Native](https://github.com/tryphotino/photino.Native) projects, to make them more modern and easier to use
within a DI container.

This project is mainly meant to be used as a dependency for Infinilore, so my goals are focused on that, but it should
be possible to use this as a replacement for the original `Photino.NET`, `Photino.NET.Server` and `Photino.Blazor`
projects, minding the breaking changes mentioned below.

This project is not affiliated with- or endorsed by the original authors of Photino.

---

## Status

This project is currently in a *very early stage of converting the old code base to more modern C# and .NET*.

Although all functionality should be working, it is not verified on all platforms if they are stable.
Currently a lot of the codebase is still untestsed on Linux and MacOS environments because setting up runners with the ability to have
properly working windows is still being looked for.

## Breaking changes

`InfiniFrame` is not meant to be a drop in replacement for `Photino`.
Although it uses much of the same API as Photino, it handles window initialization a whole different manner. 
For now, until proper documentation is setup, please refer to the examples and tests to see how to create windows and attach them to servers, etc...

*further list of all Breaking changes can be added at a later date*

---

## Building and running

For building and running the project, you will need to have the .NET 10 SDK installed.
You will also need to add the [nuget cli tool](https://www.nuget.org/downloads) to your PATH.

The solution is set up in such a way that you can build the entire solution without having to need any additional
dependencies first.

---

## Repo history

This repo was originally forked from [Photino.NET](https://github.com/tryphotino/photino.NET) and then the history of
the [Photino.Blazor](https://github.com/tryphotino/Photino.Blazor)
and [Photino.Net.Server](https://github.com/tryphotino/photino.NET.Server) repositories were merged into this.
By merging the histories, it was possible to ease development a lot, especially whilst also preserving the original
commit history and attribution from the contributors of Photino.

This was also done for the [Photino.Native](https://github.com/tryphotino/photino.Native) library, but given the
extensive work that had already been done, git was seemingly unable to fully merge the commit history without losing the
original commit history.

## License

Unlike the other projects in the InfiniLore ecosystem, which all follow GPL or LGPL, this repo follows the
same [Apache-2.0 license file](LICENSE) from [Photino.NET](https://github.com/tryphotino/photino.NET) to adhere to the
original licensing without the need for extra modifications to the license.
