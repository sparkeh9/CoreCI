const webpack = require('webpack');
const merge = require('webpack-merge');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const UglifyJSPlugin = require('uglifyjs-webpack-plugin');
const OptimizeCssAssetsPlugin = require( 'optimize-css-assets-webpack-plugin' );
const baseConfig = require( './webpack.config.js' );

module.exports = merge(baseConfig, {
    module: {
        rules: [
            {
                test: /\.scss$/,
                use: ExtractTextPlugin.extract( {
                    use: ['css-loader', 'sass-loader'],
                    fallback: 'style-loader'
                } )
            }
        ]
    },
    plugins: [
        new ExtractTextPlugin( {
            filename: '[name].css',
            allChunks: true
        }),
        new UglifyJSPlugin( {
            sourceMap: true
        }), 
        new OptimizeCssAssetsPlugin({
            assetNameRegExp: /\.css$/,
            cssProcessorOptions: { discardComments: { removeAll: true } }
        })
    ]
} );
