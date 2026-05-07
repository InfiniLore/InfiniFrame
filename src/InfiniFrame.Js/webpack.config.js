const path = require('path');
const TerserPlugin = require("terser-webpack-plugin");

module.exports = (env, _) => {
    const isProduction = env?.production === true
        || env?.production === "true"
        || process.env.NODE_ENV === "production";

    return {
        mode: isProduction ? 'production' : 'development',
        devtool: isProduction ? false : 'inline-source-map',

        optimization: isProduction ? {
            concatenateModules: true,
            minimize: true,
            minimizer: [
                new TerserPlugin({
                    terserOptions: {
                        compress: {
                            passes: 3,
                            drop_console: true,
                            drop_debugger: true,
                            pure_funcs: ["console.log"]
                        },
                        mangle: true,
                        format: {
                            comments: false
                        }
                    },
                    extractComments: false
                })
            ]
        } : {},
        
        entry: {
            main: "./TypeScript/Index.ts",
        },
        output: {
            path: path.resolve(__dirname, './wwwroot'),
            filename: "InfiniFrame.js",
        },
        resolve: {
            extensions: [".ts", ".tsx", ".js"],
        },
        module: {
            rules: [
                {
                    test: /\.tsx?$/,
                    loader: "ts-loader",
                    options: {
                        configFile: path.resolve(__dirname, "tsconfig.json"),
                        compilerOptions: {
                            sourceMap: false
                        }
                    },
                    exclude: /node_modules/
                }
            ]
        }
    }
};
