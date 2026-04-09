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
      label: "Concepts",
      items: [
        "concepts/breaking-changes-from-photino",
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
    "migration/docfx-to-docusaurus",
  ],
};

export default sidebars;
