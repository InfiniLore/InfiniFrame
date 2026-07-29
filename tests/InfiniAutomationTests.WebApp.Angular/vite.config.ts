import {defineConfig} from 'vite'
import angular from '@analogjs/vite-plugin-angular'

export default defineConfig({
    plugins: [angular()],
    preview: {port: 7627, host: true},
    build: {
        outDir: 'wwwroot',
        emptyOutDir: false,
        rollupOptions: {
            output: {
                entryFileNames: 'assets/index.js',
                chunkFileNames: 'assets/[name].js',
                assetFileNames: 'assets/[name][extname]'
            }
        }
    }
})
