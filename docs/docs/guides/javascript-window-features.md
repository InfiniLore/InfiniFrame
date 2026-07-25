# JavaScript window feature parity

`window.infiniframe.window.features` exposes all runtime window-feature operations that have a safe web-message representation. Synchronous and asynchronous C# overload pairs intentionally map to one idiomatic JavaScript method; getters and result-returning operations return `Promise` values.

The parity boundary is the 13 interfaces reachable through `IInfiniFrameWindowFeatures`. Native handles, `IServiceProvider`, configuration, managed events, and the other members of `IInfiniFrameWindow` are not part of this contract. They have no generally safe or useful serialized representation.

| Feature             | JavaScript representation                                              | Intentional exclusions                                                                                     |
|---------------------|------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------|
| Browser             | All getters and mutations                                              | None                                                                                                       |
| Debugging           | Capabilities, diagnostics, endpoint results, and DevTools control      | None                                                                                                       |
| Decorations         | All getters and mutations                                              | None                                                                                                       |
| File picker dialogs | Open file/folder and save file, including filters and nullable results | Sync/async C# pairs share one Promise-returning method                                                     |
| Invoke              | Feature namespace only                                                 | `Invoke(Action)` and `DispatchAsync(Action)` cannot transport managed delegates through web messaging      |
| Lifecycle           | State, closed/closing query, and close                                 | `WaitForClose` must not block the web-message/UI thread; a future wait API must be an event-backed Promise |
| Monitors            | Monitor list, main monitor, and DPI                                    | None                                                                                                       |
| Notifications       | Native notification and message dialog/result                          | None                                                                                                       |
| Page navigation     | URI/path/raw loading and `tryLoad` results                             | C# overloads share idiomatic JS methods                                                                    |
| Position            | All getters and mutations                                              | Point and numeric overloads share idiomatic JS methods                                                     |
| Size                | All getters and mutations                                              | Size and numeric overloads share idiomatic JS methods                                                      |
| State               | All getters and mutations, including both cached-bounds setters        | None                                                                                                       |
| Web messaging       | Raw string web messages                                                | Sync/async C# pair shares one method                                                                       |

Feature names are matched case-insensitively by the native router. Command and argument names are case-sensitive. DTO properties and enum values use exact camel-case web JSON shapes. The native parity test inventories every public member (including overload counts), so a new C# API member must be represented or added as a documented exclusion.
