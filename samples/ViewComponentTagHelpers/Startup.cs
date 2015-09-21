using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.DependencyInjection;

namespace ViewComponentTagHelpers
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseTagHelperGenerator();
            app.UseMvcWithDefaultRoute();
        }
    }
}
