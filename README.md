# Introduction 
[![Build status](https://ci.appveyor.com/api/projects/status/ls698amk4w687olj?svg=true)](https://ci.appveyor.com/project/kdcllc/weboptimizer-angulartemplatecache)

This ASP.NET Core Web Optimizer for AnuglarJs is based on [ASP.NET Core Web Optimizer]
(https://github.com/ligershark/WebOptimizer) engine designed by [Mads Kristensen](https://github.com/madskristensen).


# Using AngularJs $templateCache within ASP.NET Core

1. Create the script bundle
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddWebOptimizer(pipeline =>
    {

        //if html files are not located inside of the wwwroot folder add UseContentRoot();
        pipeline.AddHtmlTemplateBundle("/js/views.js", "app", "html/**/*.html")
                .UseContentRoot();

        pipeline.AddHtmlTemplateBundle("/js/templates.js", "templates-main", "app/views/**/*.html");

        //minify all of the jsfiles in the pipeline
        pipeline.MinifyJsFiles();
    )};
}
```
2. Inject the templates-main into the existing AngularJs application:
```
var app = angular.module("app", ["ngRoute", "templates-main"]);
```

