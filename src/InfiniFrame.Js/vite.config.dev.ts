import {resolve} from "node:path";
import {defineConfig} from "vite";

const entry = resolve(import.meta.dirname, "TypeScript/Index.ts");

export default defineConfig({
    build: {
        emptyOutDir: false,
        outDir: "wwwroot",
        minify: false,
        sourcemap: true,

        lib: {
            entry,
            formats: ["iife"],
            name: "InfiniFrameJs",
            fileName: () => "InfiniFrame.dev.js",
        },
    },
});
