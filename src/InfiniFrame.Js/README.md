# InfiniFrame.Js

JavaScript/TypeScript interop library for InfiniFrame Blazor applications. Provides the client-side bridge between C# and the browser, including pointer capture utilities, built-in window management message handlers, and the envelope protocol for web messaging.

## Package

This is an internal build package (`infinilore.infiniframe.js-build`). It is not published to npm. The compiled output is consumed by `InfiniLore.InfiniFrame.Js` (the .NET package) as embedded static assets.

## Architecture

```
TypeScript/
  Index.ts                    # Entry point — exports the InfiniFrame namespace
  InfiniFrame.ts              # Main InfiniFrame class (window features, messaging, window chrome)
  InfiniFrameHostMessaging.ts # Low-level messaging bridge to the native host
  InfiniFrameUtils.ts         # Utility helpers
  Contracts/                  # TypeScript interfaces and type definitions
    EnvelopeProtocol.ts       # Web messaging envelope v2 contract
    InfiniFrame.ts            # Global InfiniFrame namespace contract
    InfiniFrameHostMessaging.ts # Host messaging message ID constants
    Window/                   # Window feature type contracts
  Window/
    WindowChrome.ts           # Custom window chrome registration (drag, resize, controls)
    InfiniFrameWindow.ts      # Window facade class
    InfiniFrameWindowFeature.ts # Abstract base for window features
    InfiniFrameWindowFeatures.ts # Feature collection (Browser, Size, State, etc.)
    Features/                 # 14 window feature implementations
  Utils/
    Observers.ts              # MutationObserver helpers
    BlankTargetHandler.ts     # External link handler
  Interop/
    NativeInterop/            # Native bridge setup, custom elements, Blazor patches
    EnvelopeProtocol/         # Envelope creation and parsing
```

## Development

### Prerequisites

- Node.js 24+
- npm

### Install

```bash
npm ci
```

### Build

```bash
npm run build
```

Produces two bundles:
- `wwwroot/InfiniFrame.js` — production (minified)
- `wwwroot/InfiniFrame.dev.js` — development (with source maps)

### Test

```bash
npm test              # Run once
npm run test:watch    # Watch mode
npm run test:coverage # With coverage
```

### Type Check

```bash
npm run typecheck
```

## Integration

Include the script in your HTML host page:

```html
<script src="_content/InfiniLore.InfiniFrame.Js/InfiniFrame.js"></script>
```

The script registers itself on `window.infiniframe` and provides:
- `window.infiniframe.host` — messaging bridge to C#
- `window.infiniframe.window.features` — runtime window feature access
- `window.infiniframe.windowChrome` — custom window chrome registration
- `window.infiniframe.messaging` — convenience messaging helpers
