using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using NUglify;
using NUglify.Html;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebOptimizer.AngularTemplateCache
{
    /// <summary>
    /// Compiles Html files into JavaScript $templateCache
    /// </summary>
    /// <seealso cref="WebOptimizer.IProcessor" />
    public class Transformer : IProcessor
    {
        private string _moduleName;
        private string _path;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleName"></param>
        public Transformer(string moduleName) : this(moduleName, string.Empty ,new HtmlSettings()) { }
        /// <summary>
        /// AngularJs main module name for the files
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="path"></param>
        /// <param name="settings"></param>
        public Transformer(string moduleName, string path, HtmlSettings settings)
        {
            _moduleName = moduleName;
            _path = path;
            Settings = settings;
        }
        /// <summary>
        /// Gets the custom key that should be used when calculating the memory cache key.
        /// </summary>
        public string CacheKey(HttpContext context) => string.Empty;

        public HtmlSettings Settings { get; set; }

        /// <summary>
        /// Executes the processor on the specified configuration.
        /// </summary>
        public Task ExecuteAsync(IAssetContext context)
        {
            var content = new Dictionary<string, byte[]>();
            var env = (IHostingEnvironment)context.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment));
            IFileProvider fileProvider = context.Asset.GetFileProvider(env);

            var builder = new StringBuilder();
            // Javascript module for Angular that uses templateCache 
            builder.AppendFormat(@"angular.module('{0}',[]).run(['$templateCache',function($templateCache){{",
                                 _moduleName);
            foreach (string route in context.Content.Keys)
            {
                IFileInfo file = fileProvider.GetFileInfo(route);
                string inputFile = file.PhysicalPath;

                string input = context.Content[route].AsString().Replace("\r\n", "").Replace("'", "\\'");
                UglifyResult result = Uglify.Html(input, Settings);
                string minified = result.Code;

                if (result.HasErrors)
                {
                    minified = $"<!-- {string.Join("\r\n", result.Errors)} -->\r\n" + input;
                }
                builder.AppendFormat(@"$templateCache.put('{0}{1}','{2}');", _path, file.Name, minified);
              
                //content[route] = cx.AsByteArray();
            }
            builder.Append(@"}]);");
            var cx = builder.ToString();
            content.Add(Guid.NewGuid().ToString(), cx.AsByteArray());
            context.Content = content;

            return Task.CompletedTask;
        }
    }

}

