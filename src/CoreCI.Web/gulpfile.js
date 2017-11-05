'use strict';

var gulp = require('gulp');
var webpack2 = require('webpack');
var webpackStream = require('webpack-stream');
var deletefile = require('gulp-delete-file');


gulp.task('webpack:dev' , function () {
    return webpackStream(require('./webpack.dev.config.js'))
        .pipe(gulp.dest('./wwwroot/dist'));
});

gulp.task('webpack:prod', function () {

    return webpackStream(require('./webpack.prod.config.js'))
        .pipe(gulp.dest('./wwwroot/dist'));
});

gulp.task('webpack:cleanup', function () {
    var regexp = /\w*(\-\w{8}\.js){1}$|\w*(\-\w{8}\.css){1}$/;
    gulp.src( [
        './wwwroot/dist/style.*'
    ] ).pipe( deletefile( {
        reg: regexp,
        deleteMatch: false
    } ) );
});

gulp.task('default', ['webpack:prod', 'webpack:cleanup']);