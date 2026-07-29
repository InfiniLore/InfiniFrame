import {copyFileSync, mkdirSync} from 'node:fs'
import path from 'node:path'
import {fileURLToPath} from 'node:url'

const projectDirectory = path.dirname(fileURLToPath(import.meta.url))
const testsDirectory = path.dirname(projectDirectory)
const copies = [
  ['react', 'InfiniAutomationTests.WebApp.React/Sources/infiniframe-playwright-react/src/generated'],
  ['vue', 'InfiniAutomationTests.WebApp.Vue/Sources/infiniframe-playwright-vue/src/generated'],
  ['angular', 'InfiniAutomationTests.WebApp.Angular/Sources/infiniframe-playwright-angular/src/generated']
]

for (const [target, destinationRelativePath] of copies) {
  const sourceDirectory = path.join(projectDirectory, 'obj', 'generated', target, 'src')
  const destinationDirectory = path.join(testsDirectory, destinationRelativePath)
  mkdirSync(destinationDirectory, {recursive: true})

  for (const component of ['InputDataProbe', 'OutputDataProbe', 'WindowFeatureTestPanel']) {
    const extension = target === 'vue' ? '.vue' : target === 'react' ? '.tsx' : '.ts'
    copyFileSync(path.join(sourceDirectory, component + extension), path.join(destinationDirectory, component + extension))
  }
}
