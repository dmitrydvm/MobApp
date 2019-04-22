using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MobApps.Services;
using NSwag.AspNetCore;
using System.Collections.Generic;

namespace MobApps
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddJsonFormatters()
                .AddApiExplorer()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<Dictionary<string, string>>(Configuration.GetSection("ContentTypes"))
                .AddSwaggerDocument(config => config.Title = "MobApps Swagger Specification")
                .AddHttpContextAccessor()
                .AddSingleton<IFileService, FileService>()
                .AddSingleton(_env.ContentRootFileProvider);


            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(config =>
                {
                    config.Authority = Configuration.GetValue<string>("IdentitySettings:ApiAuthority");
                    config.ApiName = "ApiName";
                    config.ApiSecret = "ApiSecret";
                });

            services.AddAuthorization(config =>
                {
                    config.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();

                    config.AddPolicy("Downloader", cfg => cfg.RequireScope("mobapps.download"));
                });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Testing"))
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUi3();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseExceptionHandler(appError =>
            {
                appError.Run(context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>();
                    if (exception != null)
                    {
                        logger.LogError(exception.Error.ToString());
                    }

                    return null;
                });
            });
            app.UseMvc();
        }
    }
}
