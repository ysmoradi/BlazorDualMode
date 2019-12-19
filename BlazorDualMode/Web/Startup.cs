#if BlazorClient
using Microsoft.AspNetCore.Components.Builder;
#elif BlazorServer
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
#endif
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System;

namespace BlazorDualMode.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
#if BlazorServer
            services.AddHttpClient("DefaultHttpClient", (serviceProvider, httpClient) =>
            {
                httpClient.BaseAddress = new Uri(serviceProvider.GetRequiredService<IConfiguration>()["ApiServerAddress"]);
            });
            services.AddTransient(c => c.GetRequiredService<IHttpClientFactory>().CreateClient("DefaultHttpClient"));
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
                opts.Providers.Add<BrotliCompressionProvider>();
            }).Configure<BrotliCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Optimal);
#endif
        }

#if BlazorClient
        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
#endif

#if BlazorServer
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
#endif
    }
}
