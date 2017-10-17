namespace WebOptimizer.AngularTemplateCache
{
    /// <summary>
    /// Settings for the AngularJs settings
    /// </summary>
    public class AngularTemplateOptions
    {
        /// <summary>
        /// angular.module(moduleName)
        /// </summary>
        public string moduleName { get; set; }

        /// <summary>
        /// App/Views
        /// </summary>
        public string templatePath { get; set; } = string.Empty;
    }
}
