'use strict';

var gulp = require('gulp');
var sass = require('gulp-sass');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var replace = require('gulp-replace');

sass.compiler = require('node-sass');

gulp.task('sass:chartwell', function () {
  return gulp.src('./Assets/sass/chartwell.scss')
    .pipe(sass({ outputStyle: 'compressed' }).on('error', sass.logError))
    .pipe(gulp.dest('./Assets/Styles/'));
});

gulp.task('sass:vendor', function () {
  return gulp.src('./Assets/sass/vendor.scss')
    .pipe(sass({ outputStyle: 'compressed' }).on('error', sass.logError))
    .pipe(gulp.dest('./Assets/Styles/'));
});

gulp.task('sass:watch', function () {
  gulp.watch('./Assets/sass/**/*.scss', gulp.series('sass:vendor', 'sass:chartwell'));
});

gulp.task('sass:chartwell:watch', function () {
  gulp.watch('./Assets/sass/chartwell/**/*.scss', gulp.series('sass:chartwell'));
});


gulp.task('js:all', done => {
  gulp.series("js:chartwell", "js:vendor")(done());
});

/*
 * IMPORTANT: UPDATE THIS MANIFEST WHENEVER THE CONTENTS OF VENDOR.JS CHANGES
 * 
 * FOLDER: Assets/js/vendor/
 * 
 * autocomplete.js has variables that are specific to different environments.
 * 
 * GET RID OF JQUERYUI <-- to-do 
 * 
    jquery-3.3.1.js
    jquery-ui-1.12.1.js
    jquery.validate.1.18.0.min.js 
    jquery.unobtrusive-ajax.js
    jquery.maskedinput.js

    pvdatepicker.js
    ExtractReportFromToDate.js
    autocomplete.js
    jquery.validate.js
    jquery.validate.unobtrusive.js
    jquery.dynamicmaxheight.js
    bootstrap.min.js
    maskedinput-binder.js
    isotope.pkgd.min.js
    jquery.bootstrap-responsive-tabs.min.js
    StarRating.js
    jquery.fancybox.js
    bootstrap.bundle.js
    moment-with-locales.min.js
    FeedEk.js

*/
gulp.task('js:vendor', function () {
  return gulp.src(
    [
      'Assets/js/vendor/jquery-3.3.1.js',
      'Assets/js/vendor/jquery-ui-1.12.1.js',
      'Assets/js/vendor/jquery.validate.1.18.0.min.js',
      'Assets/js/vendor/jquery.unobtrusive-ajax.js',
      'Assets/js/vendor/jquery.maskedinput.js',
      'Assets/js/vendor/pvdatepicker.js',
      'Assets/js/vendor/ExtractReportFromToDate.js',
      'Assets/js/vendor/autocomplete.js',
      'Assets/js/vendor/jquery.validate.js',
      'Assets/js/vendor/jquery.validate.unobtrusive.js',
      'Assets/js/vendor/jquery.dynamicmaxheight.js',
      'Assets/js/vendor/maskedinput-binder.js',
      'Assets/js/vendor/isotope.pkgd.min.js',
      'Assets/js/vendor/StarRating.js',
      'Assets/js/vendor/jquery.fancybox.js',
      'Assets/js/vendor/bootstrap.bundle.js',
      'Assets/js/vendor/moment-with-locales.min.js',
      'Assets/js/vendor/FeedEk.js',
      'Assets/js/vendor/jquery.bootstrap-responsive-tabs.min.js'
    ]
  ).pipe(concat('vendor.js'))
    .pipe(gulp.dest('Assets/js/'))
    .pipe(uglify())
    .pipe(gulp.dest('Assets/js/'))
});

/*
 * IMPORTANT: UPDATE THIS MANIFEST WHENEVER THE CONTENTS OF CHARTWELL.JS CHANGES
 * 
 * FOLDER "Assets/js/chartwell"
    datepicker-fr.js,
    datepicker-en.js,
    layout.js,
    LocationBasedSearch.js,
    self-executing-scripts.js
*/

gulp.task('js:chartwell', function () {
  return gulp.src(
    [
    'Assets/js/chartwell/datepicker-fr.js',
    'Assets/js/chartwell/datepicker-en.js',
    'Assets/js/chartwell/layout.js',
    'Assets/js/chartwell/LocationBasedSearch.js',
    'Assets/js/chartwell/self-executing-scripts.js',
    ]
  ).pipe(concat('chartwell.js'))
    .pipe(gulp.dest('Assets/js/'))
    .pipe(uglify())
    .pipe(gulp.dest('Assets/js/'));
});
gulp.task('js:chartwell:watch', function () {
  gulp.watch('./Assets/js/chartwell/**/*.js', gulp.series('js:chartwell'));
});