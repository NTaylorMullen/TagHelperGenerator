using Microsoft.Dnx.Runtime;
using TagHelperGenerator;

namespace Microsoft.AspNet.Builder
{
    public static class TagHelperGeneratorAppBuilderExtensions
    {
        public static IApplicationBuilder UseTagHelperGenerator(this IApplicationBuilder app)
        {
            var applicationEnvironment = app.ApplicationServices.GetService(typeof(IApplicationEnvironment)) as IApplicationEnvironment;
            var libraryManager = app.ApplicationServices.GetService(typeof(ILibraryManager)) as ILibraryManager;

            Program.GenerateTagHelpers(applicationEnvironment, libraryManager);

            return app;
        }
    }
}
