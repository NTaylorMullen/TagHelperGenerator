using System.Linq;
using Microsoft.AspNet.Mvc.Actions;
using Microsoft.AspNet.Mvc.ViewComponents;
using Microsoft.Dnx.Runtime;

namespace TagHelperGenerator
{
    public class Program
    {
        private readonly ILibraryManager _libraryManager;
        private readonly IApplicationEnvironment _applicationEnvironment;

        public Program(IApplicationEnvironment applicationEnvironment, ILibraryManager libraryManager)
        {
            _applicationEnvironment = applicationEnvironment;
            _libraryManager = libraryManager;
        }

        public void Main(string[] args)
        {
            GenerateTagHelpers(_applicationEnvironment, _libraryManager);
        }

        internal static void GenerateTagHelpers(
            IApplicationEnvironment applicationEnvironment,
            ILibraryManager libraryManager)
        {
            var assemblyProvider = new DefaultAssemblyProvider(libraryManager);
            var viewComponentDescriptorProvider = new DefaultViewComponentDescriptorProvider(assemblyProvider);
            var viewComponentDescriptors = viewComponentDescriptorProvider.GetViewComponents();

            if (viewComponentDescriptors.Any())
            {
                var moduleBuilder = new TagHelperModuleBuilder(applicationEnvironment);
                var proxyGenerator = new ViewComponentTagHelperProxyGenerator(moduleBuilder);

                foreach (var viewComponentDescriptor in viewComponentDescriptors)
                {
                    proxyGenerator.CreateTagHelper(viewComponentDescriptor);
                }

                moduleBuilder.Build();
            }
        }
    }
}
