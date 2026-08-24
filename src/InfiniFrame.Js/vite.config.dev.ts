import {defineConfig} from "vite";
import {resolve} from "node:path";

const entry = resolve(__dirname, "TypeScript/Index.ts");

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
            fileName: () => "InfiniFrame.dev.js"
        }
    }
});
