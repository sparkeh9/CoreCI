const webpack = require( 'webpack' );
const path = require( 'path' );
const { AureliaPlugin, ModuleDependenciesPlugin } = require( 'aurelia-webpack-plugin' );

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
            { test: /\.json$/, loader: 'json-loader' },
            { test: /\.html$/i, loaders: 'html-loader' },
            { test: /\.ts$/i, loaders: 'ts-loader' },
            { test: /\.svg$/, use: [ { loader: 'url-loader', options: { limit: 4096, name: '[name].[ext]' } } ] },
            {
                test: /\.(eot|woff|woff2|ttf|png|jpe?g|gif)$/,
                use: [
                    {
                        loader: 'url-loader',
                        options:
                        {
                            limit: 4096,
                            name: '[name].[ext]'
                        }
                    }
                ]
            },
        ]
    },
    resolve: {
        extensions: [ '.ts', '.js' ],
        modules: [ 'App', 'node_modules' ].map( x => path.resolve( x ) )
    },
    devtool: 'sourcemap',
    plugins: [
        new webpack.optimize.CommonsChunkPlugin( {
            name: 'vendor',
            minChunks: function ( module )
            {
                return isExternal( module );
            }
        } ),
        new AureliaPlugin(),
        new ModuleDependenciesPlugin( {
        } )
    ]
}

function isExternal( module )
{
    var context = module.context;

    if( typeof context !== 'string' )
    {
        return false;
    }

    return context.indexOf( 'node_modules' ) !== -1;
}
