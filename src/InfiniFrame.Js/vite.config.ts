import {defineConfig} from "vite";
import {resolve} from "node:path";

export default defineConfig(({mode}) => {
    const isProduction = mode !== "development";

    return {
        build: {
            emptyOutDir: false,
            minify: isProduction ? "terser" : false,
            outDir: "wwwroot",
            sourcemap: false,
            lib: {
                entry: resolve(__dirname, "TypeScript/Index.ts"),
                formats: ["iife"],
                name: "InfiniFrameJs",
                fileName: () => "InfiniFrame.js"
            },
            rollupOptions: {
                treeshake: "safest",
                output: {
                    compact: isProduction,
                    entryFileNames: "InfiniFrame.js",
                    extend: true,
                    inlineDynamicImports: true,
                    manualChunks: undefined
                }
            },
            terserOptions: {
                compress: {
                    defaults: true,
                    drop_console: true,
                    drop_debugger: true,
                    passes: 2
                },
                format: {
                    comments: false
                },
                mangle: true
            }
        }
    };
});
