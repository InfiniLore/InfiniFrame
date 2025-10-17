//webpack.config.js
const path = require('path');

module.exports = (env, args) => {
    const isProduction = env?.production === true;

    return {
        mode: isProduction ? 'production' : 'development',
        devtool: isProduction ? false : 'inline-source-map',
        entry: {
            main: "./src/InfiniLore.InfiniFrame.Js/TsSource/Index.ts",
        },
        output: {
            path: path.resolve(__dirname, './src/InfiniLore.InfiniFrame.Js/wwwroot'),
            filename: "InfiniLore.InfiniFrame.js", // <--- Will be compiled to this single file
        },
        resolve: {
            extensions: [".ts", ".tsx", ".js"],
        },
        module: {
            rules: [
                {
                    test: /\.tsx?$/,
                    loader: "ts-loader",
                    exclude: /node_modules/
                }
            ]
        }
    }
};