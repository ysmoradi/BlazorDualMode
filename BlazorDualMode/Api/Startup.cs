using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO.Compression;
using System.Linq;

namespace BlazorDualMode.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
                opts.Providers.Add<BrotliCompressionProvider>();
            }).Configure<BrotliCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Optimal);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBlazorDebugging();
            }

            app.UseStaticFiles();
#if BlazorClient
            app.UseClientSideBlazorFiles<Web.Startup>();
#endif

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
#if BlazorClient
                endpoints.MapFallbackToClientSideBlazor<Web.Startup>("index.html");
#endif
            });
        }
    }
}
