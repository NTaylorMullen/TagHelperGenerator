using System.Linq;
using Microsoft.AspNet.Mvc.Actions;
using Microsoft.AspNet.Mvc.ViewComponents;
using Microsoft.Dnx.Runtime;

namespace TagHelperGenerator
{
    public class Program
    {
        private readonly ILibraryManager _libraryManager;

        public Program(ILibraryManager libraryManager)
        {
            _libraryManager = libraryManager;
        }

        public void Main(string[] args)
        {
            var assemblyProvider = new DefaultAssemblyProvider(_libraryManager);
            var viewComponentDescriptorProvider = new DefaultViewComponentDescriptorProvider(assemblyProvider);
            var viewComponentDescriptors = viewComponentDescriptorProvider.GetViewComponents();

            if (viewComponentDescriptors.Any())
            {
                var moduleBuilder = new TagHelperModuleBuilder();
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
