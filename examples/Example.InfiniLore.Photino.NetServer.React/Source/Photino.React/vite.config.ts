import { defineConfig } from 'vite'
import { fileURLToPath, URL } from 'node:url'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
    plugins: [react()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    preview: {
        port: 7626,
        host: true,
    },
    build : {
        outDir: '../../wwwroot',
        emptyOutDir: true,
    }
})
