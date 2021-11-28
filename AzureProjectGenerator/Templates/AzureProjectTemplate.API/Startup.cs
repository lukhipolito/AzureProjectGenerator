using AutoMapper;
using HealthChecks.UI.Client;
using HealthChecks.UI.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Logging;
using Microsoft.Net.Http.Headers;
using NSwag;
using NSwag.Generation.Processors.Security;
using Polly;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using AzureProjectTemplate.API.Extensions;
using AzureProjectTemplate.API.Filters;
using AzureProjectTemplate.API.Middlewares;
using AzureProjectTemplate.API.Services;
using AzureProjectTemplate.API.Services.Interfaces;
using AzureProjectTemplate.API.Settings;
using AzureProjectTemplate.Domain.Interfaces.Identity;
using AzureProjectTemplate.Domain.Interfaces.Notifications;
using AzureProjectTemplate.Domain.Interfaces.Repository;
using AzureProjectTemplate.Domain.Interfaces.Services;
using AzureProjectTemplate.Domain.Interfaces.UoW;
using AzureProjectTemplate.Domain.Notifications;
using AzureProjectTemplate.Infra.Context;
using AzureProjectTemplate.Infra.Identity;
using AzureProjectTemplate.Infra.Repository;
using AzureProjectTemplate.Infra.Services;

namespace AzureProjectTemplate.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;            
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddControllers();
            services.AddMvc(options =>
            {
                options.Filters.Add<DomainNotificationFilter>();
                options.EnableEndpointRouting = false;
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = Configuration["AppID:Authority"];
                options.Audience = Configuration["AppID:Audience"];
                options.RequireHttpsMetadata = false;

                if (PlatformServices.Default.Application.ApplicationName == "testhost")
                {
                    options.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration();
                }

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async ctx =>
                    {
                        var jwtClaimScope = ctx.Principal.Claims.FirstOrDefault(x => x.Type == "scope")?.Value;

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.System, jwtClaimScope),
                            new Claim(ClaimTypes.Authentication, ((JwtSecurityToken)ctx.SecurityToken).RawData)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims);
                        ctx.Principal.AddIdentity(claimsIdentity);
                        ctx.Success();
                    }
                };
            });

            services.Configure<GzipCompressionProviderOptions>(x => x.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(x =>
            {
                x.Providers.Add<GzipCompressionProvider>();
            });

            this.RegisterHttpClient(services);


            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1";
                document.Version = "v1";
                document.Title = "AzureProjectTemplate API";
                document.Description = "API de AzureProjectTemplate";

                document.PostProcess = (configure) =>
                {
                    configure.Info.TermsOfService = "None";
                    configure.Info.Contact = new OpenApiContact()
                    {
                        Name = "Squad",
                        Email = "squad@xyz.com",
                        Url = "exemplo.xyz.com"
                    };
                    configure.Info.License = new OpenApiLicense()
                    {
                        Name = "Exemplo",
                        Url = "exemplo.xyz.com"
                    };
                };

            });

            //services.AddSwaggerDocument();

            services.AddAutoMapper(typeof(Startup));
            services.AddHealthChecks();
            services.AddHttpContextAccessor();

            this.RegisterServices(services);
            this.RegisterDatabaseServices(services);
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<ApplicationInsightsSettings> options)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseResponseCompression();


            app.UseOpenApi();
            app.UseSwaggerUi3();


            app.UseAuthorization();
            app.UseAuthentication();
            app.UseLogMiddleware();

            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new ErrorHandlerMiddleware(options).Invoke
            });

            app.UseEndpoints(endpoints =>
            {
                if (PlatformServices.Default.Application.ApplicationName != "testhost")
                {
                    endpoints.MapHealthChecks("/health", new HealthCheckOptions
                    {
                        Predicate = r => r.Tags.Contains("self"),
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                    endpoints.MapHealthChecks("/ready", new HealthCheckOptions
                    {
                        Predicate = r => r.Tags.Contains("services"),
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                    endpoints.MapHealthChecksUI(setup =>
                    {
                        setup.UIPath = "/health-ui";
                    });
                }

                endpoints.MapControllers();
            });
        }


        private void RegisterHttpClient(IServiceCollection services)
        {
            services.AddHttpClient<IViaCEPService, ViaCEPService>((s, c) =>
                {
                    c.BaseAddress = new Uri(Configuration["API:ViaCEP"]);
                    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                });
        }

        protected virtual void RegisterServices(IServiceCollection services)
        {
            services.Configure<ApplicationInsightsSettings>(Configuration.GetSection("ApplicationInsights"));

            #region Service
            services.AddScoped<ICustomerService, CustomerService>();
            #endregion

            #region Domain
            services.AddScoped<IDomainNotification, DomainNotification>();
            services.AddScoped<IViaCEPService, ViaCEPService>();
            #endregion

            #region Infra
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IIdentityService, IdentityService>();
            #endregion
        }

        protected virtual void RegisterDatabaseServices(IServiceCollection services)
        {
            // if (PlatformServices.Default.Application.ApplicationName != "testhost")
            // {
                services.AddSingleton<DbConnection>(conn => new SqlConnection(Configuration.GetConnectionString("CustomerDB")));
                services.AddScoped<DapperContext>();
            // }
        }
    }
}
