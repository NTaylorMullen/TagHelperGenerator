using Microsoft.AspNet.Mvc.ViewComponents;

namespace TagHelperGenerator
{
    public class ViewComponentTagHelperProxyGenerator
    {
        private readonly TagHelperModuleBuilder _moduleBuilder;

        public ViewComponentTagHelperProxyGenerator(TagHelperModuleBuilder moduleBuilder)
        {
            _moduleBuilder = moduleBuilder;
        }

        public void CreateTagHelper(ViewComponentDescriptor viewComponentDescriptor)
        {
            _moduleBuilder.CreateTagHelperType(viewComponentDescriptor.ShortName + "TagHelper");
        }
    }
}
