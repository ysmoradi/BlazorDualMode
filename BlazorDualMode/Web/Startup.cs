#if BlazorServer
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
#endif
using Microsoft.Extensions.DependencyInjection;
using Bit.Http.Contracts;
using Bit.Http.Implementations;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorDualMode.Web.Implementations;

namespace BlazorDualMode.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorizationCore();

            services.AddScoped<ISecurityService, DefaultSecurityService>();
            services.AddTransient<ITokenProvider, DefaultTokenProvider>();

            services.AddScoped<AuthenticationStateProvider, BlazorDualModeAuthenticationStateProvider>();
            services.AddTransient(serviceProvider => (BlazorDualModeAuthenticationStateProvider)serviceProvider.GetRequiredService<AuthenticationStateProvider>());

#if BlazorServer
            services.AddHttpClient("ApiHttpClient", (serviceProvider, httpClient) =>
            {
                httpClient.BaseAddress = new Uri(serviceProvider.GetRequiredService<IConfiguration>()["ApiServerAddress"]);
                Token? token = serviceProvider.GetRequiredService<ITokenProvider>().GetToken();
                if (token != null)
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);
            });
            services.AddTransient(c => c.GetRequiredService<IHttpClientFactory>().CreateClient("ApiHttpClient"));
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
                opts.Providers.Add<BrotliCompressionProvider>();
                opts.Providers.Add<GzipCompressionProvider>();
            })
                .Configure<BrotliCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest)
                .Configure<GzipCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest);
#endif
        }

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
