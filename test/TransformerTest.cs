using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebOptimizer;
using WebOptimizer.AngularTemplateCache;
using Xunit;

namespace xUnitTest
{
    public class CompilerTest
    {
        [Fact]
        public async Task Compile_SuccessAsync()
        {
            var processor = new Transformer("templates-main");
            var pipeline = new Mock<IAssetPipeline>().SetupAllProperties();
            var context = new Mock<IAssetContext>().SetupAllProperties();
            var asset = new Mock<IAsset>().SetupAllProperties();
            var env = new Mock<IHostingEnvironment>();
            var fileProvider = new Mock<IFileProvider>();

            context.Object.Content = new Dictionary<string, byte[]> {
                { "/other.html", @"<form ng-submit=ctrl.submitForm(ctrl.formData)>Name:<input type=text ng-model=ctrl.formData.name required><br> Last name:<input type=text ng-model=ctrl.formData.lastname required><br><input type=submit value=Submit></form><pre>{{ctrl.formData}}</pre>'}".AsByteArray() },
            };

            context.Setup(s => s.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment)))
                   .Returns(env.Object);

            string temp = Path.GetTempPath();
            var inputFile = new PhysicalFileInfo(new FileInfo("other.html"));

            context.SetupGet(s => s.Asset)
                          .Returns(asset.Object);

            env.SetupGet(e => e.WebRootFileProvider)
                 .Returns(fileProvider.Object);

            fileProvider.Setup(f => f.GetFileInfo(It.IsAny<string>()))
                   .Returns(inputFile);

            await processor.ExecuteAsync(context.Object);
            var result = context.Object.Content.First().Value;
            Assert.Equal(@"angular.module('templates-main',[]).run(['$templateCache',function($templateCache){$templateCache.put('other.html','<form ng-submit=ctrl.submitForm(ctrl.formData)>Name:<input type=text ng-model=ctrl.formData.name required><br> Last name:<input type=text ng-model=ctrl.formData.lastname required><br><input type=submit value=Submit></form><pre>{{ctrl.formData}}</pre>\'}');}]);", result.AsString());
        }
    }
}
