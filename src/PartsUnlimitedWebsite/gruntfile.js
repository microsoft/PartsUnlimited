/// <binding BeforeBuild='default' />

/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
module.exports = function (grunt) {
    var scriptsList = ["Scripts/*.js", "!Scripts/_references.js"];

    grunt.initConfig({
        bower: {
            install: {
                options: {
                    targetDir: "wwwroot/lib",
                    install: true,
                    copy: true,
                    layout: "byComponent",
                    cleanTargetDir: "true"
                }
            }
        },

        copy: {
            main: {
                files: [
                {
                    expand: true,
                    src: scriptsList,
                    dest: "wwwroot"
                },
                {
                    expand: true,
                    src: ["Content/*.css"],
                    dest: "wwwroot"
                },
                {
                    expand: true,
                    src: ["images/*"],
                    dest: "wwwroot"
                }
                ]
            }
        },

        sass: {
            options: {
                sourceMap: true
            },
            dist: {
                files: [
                    {
                        "expand": true,
                        "cwd": "Content",
                        "src": "**/*.scss",
                        "dest": "wwwroot/Content",
                        "ext": ".css"
                    }
                ]
            }
        },

        concat: {
            options: {
                separator: ';'
            },
            dist: {
                src: scriptsList,
                dest: 'wwwroot/Scripts/site.js'
            }
        },

        uglify: {
            build: {
                src: "wwwroot/Scripts/site.js",
                dest: "wwwroot/Scripts/site.min.js"
            }
        }
    });

    grunt.loadNpmTasks("grunt-bower-task");
    grunt.loadNpmTasks("grunt-contrib-copy");
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-sass');

    grunt.registerTask("default", ["bower:install", "copy", "sass", "concat", "uglify"]);
    grunt.registerTask("copy-all", ["copy", "sass", "concat", "uglify"]);
};