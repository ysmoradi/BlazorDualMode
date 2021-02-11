using Bit.Core;
using Bit.Core.Contracts;
using Bit.Model.Implementations;
using Bit.OData.ActionFilters;
using Bit.OData.Contracts;
using Bit.Owin;
using Bit.Owin.Contracts;
using Bit.Owin.Implementations;
using BlazorDualMode.Api.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Reflection;

[assembly: ODataModule("BlazorDualMode")]

namespace BlazorDualMode.Api
{
    public class AppStartup : AutofacAspNetCoreAppStartup, IAppModule, IAppModulesProvider
    {
        public AppStartup(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            AspNetCoreAppEnvironmentsProvider.Current.Init();
        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            DefaultAppModulesProvider.Current = this;

            return base.ConfigureServices(services);
        }

        public IEnumerable<IAppModule> GetAppModules()
        {
            yield return this;
        }

        public virtual void ConfigureDependencies(IServiceCollection services, IDependencyManager dependencyManager)
        {
            AssemblyContainer.Current.Init();

            #region Configure services

            dependencyManager.RegisterMinimalDependencies();

            dependencyManager.RegisterDefaultLogger(AspNetCoreAppEnvironmentsProvider.Current.WebHostEnvironment.IsDevelopment() ? new[] { typeof(DebugLogStore).GetTypeInfo(), typeof(ConsoleLogStore).GetTypeInfo() } : Array.Empty<TypeInfo>());

            dependencyManager.RegisterDefaultAspNetCoreApp();

            dependencyManager.RegisterDefaultWebApiAndODataConfiguration();

            dependencyManager.RegisterDtoEntityMapper();
            dependencyManager.RegisterMapperConfiguration<DefaultMapperConfiguration>();

            services.AddRazorPages();
            services.AddControllers();
            services.AddResponseCompression(opts =>
            {
                opts.Providers.Add<BrotliCompressionProvider>();
                opts.Providers.Add<GzipCompressionProvider>();
            })
                .Configure<BrotliCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest)
                .Configure<GzipCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlazorDualMode", Version = "v1" });
            });

            #endregion

            #region Configure middlewares

            dependencyManager.RegisterAspNetCoreMiddlewareUsing(aspNetCoreApp =>
            {
#if BlazorClient
                if (AspNetCoreAppEnvironmentsProvider.Current.WebHostEnvironment.IsDevelopment())
                    aspNetCoreApp.UseWebAssemblyDebugging();
                aspNetCoreApp.UseBlazorFrameworkFiles();
#endif
                aspNetCoreApp.UseResponseCompression();
                aspNetCoreApp.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                        {
                            MaxAge = TimeSpan.FromDays(365),
                            Public = true
                        };
                    }
                });

                aspNetCoreApp.UseRouting();

                aspNetCoreApp.UseSwagger();
                aspNetCoreApp.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlazorDualMode v1"));
            });

            dependencyManager.RegisterAspNetCoreSingleSignOnClient();

            dependencyManager.RegisterMinimalAspNetCoreMiddlewares();

            dependencyManager.RegisterODataMiddleware(odataDependencyManager =>
            {
                odataDependencyManager.RegisterGlobalWebApiCustomizerUsing(httpConfiguration =>
                {
                    httpConfiguration.Filters.Add(new DefaultODataAuthorizeAttribute());
                    httpConfiguration.EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", $"Swagger-Api");
                        c.ApplyDefaultODataConfig(httpConfiguration);
                    }).EnableBitSwaggerUi();
                });

                odataDependencyManager.RegisterWebApiODataMiddlewareUsingDefaultConfiguration();
            });

            dependencyManager.RegisterSingleSignOnServer<BlazorDualModeUserService, BlazorDualModeOAuthClientsProvider>();

            dependencyManager.RegisterAspNetCoreMiddlewareUsing(aspNetCoreApp =>
            {
                aspNetCoreApp.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers().RequireAuthorization();
#if BlazorClient
                    endpoints.MapFallbackToPage("/_Host");
#endif
                });
            }, MiddlewarePosition.AfterOwinMiddlewares);

            #endregion
        }
    }
}
