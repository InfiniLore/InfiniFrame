const path = require('path');

module.exports = (env, _) => {
    const isProduction = env?.production === true
        || env?.production === "true"
        || process.env.NODE_ENV === "production";

    return {
        mode: isProduction ? 'production' : 'development',
        devtool: isProduction ? false : 'inline-source-map',
        entry: {
            main: "./TypeScript/Index.ts",
        },
        output: {
            path: path.resolve(__dirname, './wwwroot'),
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
                    options: {
                        configFile: path.resolve(__dirname, "tsconfig.json"),
                    },
                    exclude: /node_modules/
                }
            ]
        }
    }
};
