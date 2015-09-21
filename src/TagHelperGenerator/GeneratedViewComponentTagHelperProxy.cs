using Microsoft.AspNet.Html.Abstractions;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.AspNet.Mvc.ViewFeatures.Internal;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;

namespace TagHelperGenerator
{
    public abstract class GeneratedViewComponentTagHelperProxy : TagHelper
    {
        private readonly IViewComponentHelper _viewComponentHelper;
        private bool _contextualized;

        public GeneratedViewComponentTagHelperProxy(IViewComponentHelper viewComponentHelper)
        {
            _viewComponentHelper = viewComponentHelper;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public void InvokeViewComponentTagHelper<TViewComponent>(TagHelperOutput output, params object[] args)
        {
            if (!_contextualized)
            {
                // Only try to contextualize once, even if the helper isn't contextualizable.
                _contextualized = true;

                var contextualizable = _viewComponentHelper as ICanHasViewContext;
                if (contextualizable != null)
                {
                    contextualizable.Contextualize(ViewContext);
                }
            }

            var viewComponentOutput = _viewComponentHelper.Invoke(typeof(TViewComponent), args);

            output.Content.SetContent(viewComponentOutput);
            output.TagName = null;
        }
    }
}
