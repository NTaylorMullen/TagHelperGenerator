using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using TagHelperGenerator;
using ViewComponentTagHelpers.ViewComponents;

namespace ViewComponentTagHelpers.TagHelpers
{
    public class StaticNumberTagHelper : GeneratedViewComponentTagHelperProxy
    {
        public StaticNumberTagHelper(IViewComponentHelper viewComponentHelper)
            : base(viewComponentHelper)
        {
        }

        public int Min { get; set; }

        public int Max { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            InvokeViewComponentTagHelper<NumberViewComponent>(output, Min, Max);
        }
    }
}
