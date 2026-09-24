import {defineConfig} from "vite";
import {resolve} from "node:path";

const entry = resolve(import.meta.dirname, "TypeScript/Index.ts");

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

        terserOptions: {
            compress: {
                drop_debugger: true
            },
            format: {
                comments: false
            },
            mangle: true
        }
    }
});
