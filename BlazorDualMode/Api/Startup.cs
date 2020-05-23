using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;

namespace BlazorDualMode.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient("ApiHttpClient", (serviceProvider, httpClient) => /*This HtmlClient is being used in PreRendering of BlazorClient only. See Pages\_Host.cshtml */
            {
                Uri requestUrl = new Uri(serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext.Request.GetDisplayUrl());
                string baseUrl = requestUrl.GetLeftPart(UriPartial.Authority); // something like https://localhost:5001/ (The address of your host api)
                httpClient.BaseAddress = new Uri(baseUrl);
            });
            services.AddTransient(c => c.GetRequiredService<IHttpClientFactory>().CreateClient("ApiHttpClient"));
            services.AddMvcCore();
            services.AddRazorPages();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
                opts.Providers.Add<BrotliCompressionProvider>();
                opts.Providers.Add<GzipCompressionProvider>();
            })
                .Configure<BrotliCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest)
                .Configure<GzipCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
#if BlazorClient
                app.UseWebAssemblyDebugging();
#endif
            }

            app.UseStaticFiles();
#if BlazorClient
            app.UseBlazorFrameworkFiles();
#endif

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
#if BlazorClient
                endpoints.MapFallbackToPage("/_Host");
#endif
            });
        }
    }
}
