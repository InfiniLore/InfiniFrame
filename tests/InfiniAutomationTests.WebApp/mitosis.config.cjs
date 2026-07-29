module.exports = {
  files: 'src/**/*.lite.tsx',
  targets: ['react', 'vue', 'angular'],
  dest: 'obj/generated',
  commonOptions: {
    typescript: true
  },
  options: {
    angular: {
      standalone: true,
      plugins: [() => ({
        name: 'align-angular-callback-outputs',
        code: {
          post: code => code
            .replace('@Output() onDataChanged', '@Output("dataChanged") onDataChanged')
            .replace('@Output() onSubmitted', '@Output("submitted") onSubmitted')
            .replace('@Output() onReadRequested', '@Output("readRequested") onReadRequested')
        }
      })]
    },
    react: {
      plugins: [() => ({
        name: 'generated-react-type-boundary',
        code: { post: code => `// @ts-nocheck\n${code}` }
      })]
    },
    vue: {
      api: 'composition',
      plugins: [() => ({
        name: 'generated-vue-type-boundary',
        code: {
          post: code => code.replace('<script setup lang="ts">', '<script setup lang="ts">\n// @ts-nocheck')
        }
      })]
    }
  }
}
