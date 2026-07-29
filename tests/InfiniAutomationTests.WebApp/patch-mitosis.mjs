import { readFile, writeFile } from 'node:fs/promises'

const files = [
  'node_modules/@builder.io/mitosis/dist/src/helpers/babel-transform.js',
  'node_modules/@builder.io/mitosis/dist/src/generators/vue/helpers.js',
  'node_modules/@builder.io/mitosis/dist/src/parsers/context.js',
  'node_modules/@builder.io/mitosis/dist/src/parsers/jsx/helpers.js',
]

for (const file of files) {
  const source = await readFile(file, 'utf8')
  let patched = source.replaceAll('babel.transform(', 'babel.transformSync(')
  patched = patched.replaceAll('{ legacy: true }', "{ version: 'legacy' }")
  patched = patched.replaceAll(
    '{ isTSX: true, allExtensions: true }',
    '{ ignoreExtensions: true }',
  )
  patched = patched.replaceAll(
    'parserOpts: { allowReturnOutsideFunction: true },',
    "parserOpts: { allowReturnOutsideFunction: true, plugins: ['jsx'] },",
  )

  if (file.endsWith('/parsers/jsx/helpers.js')) {
    patched = patched.replace(
      /comments: false,\r?\n    plugins:/,
      "comments: false,\n    parserOpts: { plugins: ['jsx'] },\n    plugins:",
    )
  }

  if (file.endsWith('/generators/vue/helpers.js')) {
    patched = patched.replace(
      "path.replaceWith(api === 'composition' ? core_1.types.memberExpression(core_1.types.identifier(name), core_1.types.identifier('value')) : core_1.types.identifier(newValue));",
      "path.replaceWith(api === 'composition' ? core_1.types.memberExpression(core_1.types.identifier(name), core_1.types.identifier('value')) : core_1.types.identifier(newValue)); path.skip();",
    )
    patched = patched.replace(
      "path.replaceWith(core_1.types.identifier(newValue));",
      "path.replaceWith(api === 'composition' ? core_1.types.memberExpression(core_1.types.identifier(name), core_1.types.identifier('value')) : core_1.types.identifier(newValue)); path.skip();",
    )
  }

  await writeFile(file, patched)
}
