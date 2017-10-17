using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WebOptimizer.AngularTemplateCache;

namespace WebOptimizerDemo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddMvc();
            services.AddWebOptimizer(pipeline =>
            {
                //pipeline.CompileLessFiles();
                pipeline.AddLessBundle("/css/ab.css", "less/**/*.less");

                pipeline.AddTypeScriptBundle("/js/ts-bundle.js", "ts/**/*.ts");

                //if html files are not located inside of the wwwroot folder add UseContentRoot();
                pipeline.AddHtmlTemplateBundle("/js/views.js", new AngularTemplateOptions { moduleName = "app"}
                                              , "html/**/*.html")
                                            .UseContentRoot();

                pipeline.AddHtmlTemplateBundle("/js/templates.js", new AngularTemplateOptions { moduleName = "templates-main" } 
                                            , "app/views/**/*.html");

                pipeline.AddHtmlTemplateBundle("/js/templates-app.js",
                                                new AngularTemplateOptions { moduleName = "templates", templatePath = "app/others/" },
                                               "html/others/*.html")
                                            .UseContentRoot();

                // Creates a CSS and a JS bundle. Globbing patterns supported.
                pipeline.AddCssBundle("/css/bundle.css", "css/*.css");
                pipeline.AddJavaScriptBundle("/js/bundle.js", "js/plus.js", "js/minus.js");

                // This bundle uses source files from the Content Root and uses a custom PrependHeader extension
                pipeline.AddJavaScriptBundle("/js/scripts.js", "scripts/a.js", "wwwroot/js/plus.js")
                        .UseContentRoot()
                        .PrependHeader("My custom header");

                // This will minify any JS and CSS file that isn't part of any bundle
                pipeline.MinifyCssFiles();
                pipeline.MinifyJsFiles();

                // This will automatically compile any referenced .scss files
                pipeline.CompileScssFiles();

                // AddFiles/AddBundle allow for custom pipelines
                pipeline.AddBundle("/text.txt", "text/plain", "random/*.txt")
                        .AdjustRelativePaths()
                        .Concatenate()
                        .FingerprintUrls()
                        .MinifyCss();

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            app.UseWebOptimizer();

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
