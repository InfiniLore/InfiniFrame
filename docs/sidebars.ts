import type { SidebarsConfig } from "@docusaurus/plugin-content-docs";

const sidebars: SidebarsConfig = {
  docsSidebar: [
    "intro",
    {
      type: "category",
      label: "Guides",
      items: [
        "guides/getting-started",
        "guides/pack-tool",
        "guides/trim-aot-compatibility",
        {
          type: "category",
          label: "Core Window",
          items: [
            "guides/core-window",
            "guides/window-features-architecture",
          ],
        },
        {
          type: "category",
          label: "Window Features",
          items: [
            "guides/size-feature",
            "guides/position-feature",
            "guides/state-feature",
            "guides/decorations-feature",
            "guides/browser-feature",
            "guides/debugging-feature",
            "guides/lifecycle-feature",
            "guides/page-navigation-feature",
          ],
        },
        {
          type: "category",
          label: "Dialogs and System",
          items: [
            "guides/file-dialogs-feature",
            "guides/notifications",
            "guides/native-menu",
            "guides/monitors-feature",
          ],
        },
        {
          type: "category",
          label: "Input and Messaging",
          items: [
            "guides/drag-drop-feature",
            "guides/javascript-execution-feature",
            "guides/javascript-interop",
            "guides/invoke-feature",
          ],
        },
        {
          type: "category",
          label: "Integrations",
          items: [
            "guides/blazor-webview",
            "guides/web-server",
            "guides/custom-window-chrome",
            "guides/javascript-window-features",
          ],
        },
        "guides/instance-arbitration",
        "guides/scripts",
      ],
    },
    {
      type: "category",
      label: "C# Articles",
      items: [
        "csharp/code-style",
        "csharp/async-window-contract-design",
      ],
    },
    {
      type: "category",
      label: "C++ Articles",
      items: [
        "cpp/native-cpp-api",
        "cpp/code-style",
      ],
    },
    "api",
    {
      type: "category",
      label: "Migration",
      items: [
        "migration/photino-breaking-changes",
          "migration/photino-backlog"
      ],
    },
  ],
};

export default sidebars;
