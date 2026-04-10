import type { Config } from "@docusaurus/types";
import type * as Preset from "@docusaurus/preset-classic";

const config: Config = {
  title: "InfiniFrame Documentation",
  tagline: "Guides, concepts, and API references",
  favicon: "favicon.ico",
  url: "https://docs.infiniframe.dev",
  baseUrl: "/",
  onBrokenLinks: "throw",
  markdown: {
    hooks: {
      onBrokenMarkdownLinks: "warn",
    },
  },
  trailingSlash: false,
  i18n: {
    defaultLocale: "en",
    locales: ["en"],
  },
  presets: [
    [
      "classic",
      {
        docs: {
          path: "docs",
          routeBasePath: "/",
          sidebarPath: "./sidebars.ts",
          editUrl: "https://github.com/InfiniLore/InfiniFrame/tree/core/docs/",
        },
        blog: false,
        pages: false,
        theme: {
          customCss: "./src/css/custom.css",
        },
        sitemap: {
          changefreq: "weekly",
          priority: 0.5,
        },
      } satisfies Preset.Options,
    ],
  ],
  plugins: [
    [
      "@docusaurus/plugin-client-redirects",
      {
        redirects: [
          { from: ["/index.html"], to: "/" },
          { from: ["/articles/guides"], to: "/guides/getting-started" },
          { from: ["/articles/concepts"], to: "/migration/breaking-changes-from-photino" },
          { from: ["/articles/csharp"], to: "/csharp/code-style" },
          { from: ["/articles/cpp"], to: "/cpp/native-cpp-api" },
          { from: ["/articles/guides/getting-started", "/articles/guides/getting-started.html"], to: "/guides/getting-started" },
          { from: ["/articles/guides/pack-tool", "/articles/guides/pack-tool.html"], to: "/guides/pack-tool" },
          { from: ["/articles/guides/core-window", "/articles/guides/core-window.html"], to: "/guides/core-window" },
          { from: ["/articles/guides/blazor-webview", "/articles/guides/blazor-webview.html"], to: "/guides/blazor-webview" },
          { from: ["/articles/guides/web-server", "/articles/guides/web-server.html"], to: "/guides/web-server" },
          { from: ["/articles/guides/javascript-interop", "/articles/guides/javascript-interop.html"], to: "/guides/javascript-interop" },
          { from: ["/articles/guides/custom-window-chrome", "/articles/guides/custom-window-chrome.html"], to: "/guides/custom-window-chrome" },
          { from: ["/articles/concepts/breaking-changes-from-photino", "/articles/concepts/breaking-changes-from-photino.html"], to: "/migration/breaking-changes-from-photino" },
          { from: ["/concepts/breaking-changes-from-photino"], to: "/migration/breaking-changes-from-photino" },
          { from: ["/articles/csharp/code-style", "/articles/csharp/code-style.html"], to: "/csharp/code-style" },
          { from: ["/articles/cpp/native-cpp-api", "/articles/cpp/native-cpp-api.html"], to: "/cpp/native-cpp-api" },
          { from: ["/articles/cpp/code-style", "/articles/cpp/code-style.html"], to: "/cpp/code-style" },
          { from: ["/api/cpp"], to: "/api" }
        ]
      }
    ]
  ],
  themeConfig: {
    navbar: {
      title: "InfiniFrame Docs",
      items: [
        { type: "docSidebar", sidebarId: "docsSidebar", position: "left", label: "Documentation" },
        { href: "https://github.com/InfiniLore/InfiniFrame", label: "GitHub", position: "right" }
      ],
    },
    footer: {
      style: "dark",
      links: [
        {
          title: "Docs",
          items: [
            { label: "Getting Started", to: "/guides/getting-started" },
            { label: "API Strategy", to: "/api" }
          ],
        },
        {
          title: "Project",
          items: [
            { label: "GitHub", href: "https://github.com/InfiniLore/InfiniFrame" },
          ],
        }
      ],
      copyright: `Copyright ${new Date().getFullYear()} InfiniLore`,
    },
    prism: {
      additionalLanguages: ["csharp", "bash", "powershell"],
    },
  } satisfies Preset.ThemeConfig,
};

export default config;
