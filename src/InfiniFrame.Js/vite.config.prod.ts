import { defineConfig } from "vite";
import { resolve } from "node:path";

const entry = resolve(__dirname, "TypeScript/Index.ts");

export default defineConfig({
    build: {
        emptyOutDir: true,
        outDir: "wwwroot",
        minify: "terser",
        sourcemap: false,

        lib: {
            entry,
            formats: ["iife"],
            name: "InfiniFrameJs",
            fileName: () => "InfiniFrame.js"
        },

        rollupOptions: {
            output: {
                entryFileNames: "InfiniFrame.js",
                extend: true,
                codeSplitting: false
            }
        },

        terserOptions: {
            compress: {
                drop_console: true,
                drop_debugger: true
            },
            format: {
                comments: false
            },
            mangle: true
        }
    }
});