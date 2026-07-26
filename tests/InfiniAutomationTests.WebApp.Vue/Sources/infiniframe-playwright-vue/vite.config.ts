import {defineConfig} from 'vite'
import vue from '@vitejs/plugin-vue'
import vuetify from 'vite-plugin-vuetify'
import {fileURLToPath, URL} from "node:url";

// https://vite.dev/config/
export default defineConfig({
    plugins: [vue(), vuetify({autoImport: true})],
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
        rollupOptions: {
            output: {
                entryFileNames: 'assets/index.js',
                chunkFileNames: 'assets/[name].js',
                assetFileNames: 'assets/[name][extname]',
            },
        },
    }
})
