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
        "guides/core-window",
        "guides/blazor-webview",
        "guides/web-server",
        "guides/javascript-interop",
        "guides/custom-window-chrome",
      ],
    },
    {
      type: "category",
      label: "C# Articles",
      items: [
        "csharp/code-style",
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
        "migration/breaking-changes-from-photino",
        "migration/docfx-to-docusaurus",
      ],
    },
  ],
};

export default sidebars;
