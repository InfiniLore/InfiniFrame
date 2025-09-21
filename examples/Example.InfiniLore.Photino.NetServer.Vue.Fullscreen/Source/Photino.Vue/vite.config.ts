import {defineConfig} from 'vite'
import {fileURLToPath, URL} from 'node:url'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
    plugins: [vue()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    preview: {
        port: 7625,
        host: true,
    },
    build: {
        outDir: '../../wwwroot',
        emptyOutDir: true,
    }
})
