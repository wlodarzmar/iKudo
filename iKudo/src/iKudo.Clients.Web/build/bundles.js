module.exports = {
    "bundles": {
        "dist/app-build": {
            "includes": [
                "[**/*.js]",
                "**/*.html!text",
                "**/*.css!text"
            ],
            "options": {
                "inject": true,
                "minify": true,
                "depCache": true,
                "rev": false
            }
        },
        "dist/aurelia": {
            "includes": [
                "aurelia-framework",
                "aurelia-bootstrapper",
                "aurelia-fetch-client",
                "aurelia-router",
                "aurelia-animator-css",
                "aurelia-templating-binding",
                "aurelia-polyfills",
                "aurelia-templating-resources",
                "aurelia-templating-router",
                "aurelia-loader-default",
                "aurelia-history-browser",
                "aurelia-logging-console",
                "aurelia-validation",
                "aurelia-dialog",
                "bootstrap",
                "bootstrap/css/bootstrap.min.css!text",
                "fetch",
                "jquery",
                "moment",
                "text",
                "urijs",
                "izitoast",
                "masonry-layout"
            ],
            "options": {
                "inject": true,
                "minify": true,
                "depCache": false,
                "rev": false
            }
        }
    }
};
