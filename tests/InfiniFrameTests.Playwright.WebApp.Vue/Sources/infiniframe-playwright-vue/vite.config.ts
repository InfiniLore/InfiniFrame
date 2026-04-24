import {defineConfig} from 'vite'
import vue from '@vitejs/plugin-vue'
import {fileURLToPath, URL} from "node:url";

// https://vite.dev/config/
export default defineConfig({
    plugins: [vue()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    preview: {
        port: 9100,
        host: true,
    },
    build: {
        outDir: '../../wwwroot',
        // Multi-target dotnet builds can evaluate static web assets while another target is rebuilding frontend files.
        // Keep output stable and avoid directory wipes to prevent transient "asset file does not exist" failures.
        emptyOutDir: false,
        rolldownOptions: {
            output: {
                entryFileNames: 'assets/index.js',
                chunkFileNames: 'assets/[name].js',
                assetFileNames: 'assets/[name][extname]',
            },
        },
    }
})
