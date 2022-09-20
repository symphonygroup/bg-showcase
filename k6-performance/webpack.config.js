const path = require("path");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const CopyPlugin = require("copy-webpack-plugin");
const GlobEntries = require("webpack-glob-entries");

module.exports = {
    mode: "production",
    entry: "./index.ts",
    output: {
        path: path.resolve(__dirname, "dist"),
        libraryTarget: "commonjs",
        filename: "index.js",
    },
    resolve: {
        extensions: [".ts", ".js"],
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                use: "babel-loader",
                exclude: /node_modules/,
            },
        ],
    },
    target: "web",
    externals: /^(k6|https?\:\/\/)(\/.*)?/,
    // Generate map files for compiled scripts
    devtool: "source-map",
    stats: {
        colors: true,
    },
    plugins: [
        new CleanWebpackPlugin(),
        // Copy assets to the destination folder
        // see `tests/post-file.test.ts`
        new CopyPlugin({
            patterns: [
                {
                    from: path.resolve(__dirname, "assets"),
                    noErrorOnMissing: true,
                },
            ],
        }),
    ],
    optimization: {
        // Don't minimize, as it's not used in the browser
        minimize: false,
    },
};
