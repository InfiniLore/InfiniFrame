//webpack.config.js
const path = require('path');

module.exports = (env, args) => {
    const isProduction = env?.production === true;

    return {
        mode: isProduction ? 'production' : 'development',
        devtool: isProduction ? false : 'inline-source-map',
        entry: {
            main: "./src/InfiniFrame.Js/TsSource/Index.ts",
        },
        output: {
            path: path.resolve(__dirname, './src/InfiniFrame.Js/wwwroot'),
            filename: "InfiniFrame.js", // <--- Will be compiled to this single file
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