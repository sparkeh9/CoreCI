﻿const webpack = require('webpack');
const path = require('path');
const { AureliaPlugin } = require( 'aurelia-webpack-plugin' );

module.exports = {
    entry: {
        'main': [ 'aurelia-bootstrapper' ],
        'style': 'sass/main.scss'
    },
    output: {
        path: path.join( __dirname, 'wwwroot', 'dist' ),
        publicPath: '/dist/',
        filename: '[name].js',
        chunkFilename: '[chunkhash].[name].js'
    },
    module: {
        rules: [
            { test: /\.html$/i, loaders: 'html-loader' },
            { test: /\.ts$/i, loaders: 'ts-loader' }
        ]
    },
    resolve: {
        extensions: [ '.ts', '.js' ],
        modules: [ 'App', 'node_modules' ].map( x => path.resolve( x ) )
    },
    devtool: 'sourcemap',
    plugins: [
        new webpack.optimize.CommonsChunkPlugin({
            name: 'vendor',
            minChunks: function (module) {
                return isExternal(module);
            }
        }),
        new AureliaPlugin(),
    ]
}

function isExternal(module) {
    var context = module.context;

    if (typeof context !== 'string') {
        return false;
    }

    return context.indexOf('node_modules') !== -1;
}