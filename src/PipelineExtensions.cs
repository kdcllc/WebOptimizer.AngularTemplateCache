using System.Collections.Generic;
using WebOptimizer;
using WebOptimizer.AngularTemplateCache;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions methods for registering the AngularJs $templateCache transformer on the Asset Pipeline.
    /// </summary>
    public static class PipelineExtensions
    {
        /// <summary>
        /// Transforms HTML AngularJs partial files to Javascript file that can be cached.
        /// </summary>
        /// <param name="asset"></param>
        ///<param name="moduleName"></param>
        public static IAsset TransformHtml(this IAsset asset, string moduleName)
        {
            asset.Processors.Add(new Transformer(moduleName));
            return asset;
        }

        /// <summary>
        /// Build a transformation with path and module name
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="templateSettings"></param>
        /// <returns></returns>
        public static IAsset TransformHtml(this IAsset asset, AngularTemplateOptions templateSettings)
        {
            asset.Processors.Add(new Transformer(templateSettings));
            return asset;
        }

        /// <summary>
        ///  files on the asset pipeline.
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="moduleName"></param>
        public static IEnumerable<IAsset> TransformHtml(this IEnumerable<IAsset> assets, string moduleName)
        {
            var list = new List<IAsset>();

            foreach (IAsset asset in assets)
            {
                list.Add(asset.TransformHtml(moduleName));
            }

            return list;
        }

        /// <summary>
        ///  Creates AngularJs bundle from HTML templates.
        /// </summary>
        /// <param name="pipeline">The asset pipeline.</param>
        /// <param name="route">The route where the compiled .html file will be available from.</param>
        /// <param name="moduleSettings"></param>
        /// <param name="sourceFiles">The path to the .html source files to compile.</param>
        public static IAsset AddHtmlTemplateBundle(this IAssetPipeline pipeline, string route, 
                                                   AngularTemplateOptions moduleSettings,
                                                   params string[] sourceFiles)
        {
            Guard.ArgumentIsNotNull(moduleSettings, "Can't be null");

            return pipeline.AddBundle(route, "text/javascript; charset=UTF-8", sourceFiles)
                            .AdjustRelativePaths()
                            .TransformHtml(moduleSettings);
        }
        
        /// <summary>
        /// Default folder is app
        /// </summary>
        /// <param name="pipeline">The asset pipeline.</param>
        /// <param name="moduleName"></param>
        public static IEnumerable<IAsset> TransformHtmlFiles(this IAssetPipeline pipeline, string moduleName)
        {
            return pipeline.AddFiles("text/javascript; charset=UTF-8", "**/*.html")
                           //.Concatenate()
                           .TransformHtml(moduleName);
        }

        /// <summary>
        ///  and makes them servable in the browser.
        /// </summary>
        /// <param name="pipeline">The pipeline object.</param>
        /// <param name="moduleName"></param>
        /// <param name="sourceFiles">A list of relative file names of the sources to compile.</param>
        public static IEnumerable<IAsset> TransformHtmlFiles(this IAssetPipeline pipeline, string moduleName,
                                                            params string[] sourceFiles)
        {
            return pipeline.AddFiles("text/javascript; charset=UFT-8", sourceFiles)
                           .TransformHtml(moduleName);
        }
    }
}