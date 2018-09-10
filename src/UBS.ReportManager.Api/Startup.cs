namespace UBS.ReportManager.Api
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Abstractions.Repository;
    using Abstractions.Service;
    using LendFoundry.Foundation.Logging;
    using LendFoundry.Foundation.Persistence.Mongo;
    using LendFoundry.Security.Tokens;
    using LendFoundry.Tenant.Client;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Persistence;
    using Service;
    using Swashbuckle.AspNetCore.Swagger;
    using Contact = Swashbuckle.AspNetCore.Swagger.Contact;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTokenHandler();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpServiceLogging(Settings.ServiceName);

            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IReportRepository, ReportRepository>();

            services.AddTenantService(new Uri(Settings.Tenant_URL));
            
            services.AddSingleton<IMongoConfiguration>(p => new MongoConfiguration(
                Settings.MongoConnectionString, Settings.MongoDatabaseName));

            services.AddMvc();

            ConfigureSwashbuckleService(services);
        }

        private static void ConfigureSwashbuckleService(IServiceCollection services)
        {
            var appVersion = typeof(Program).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Report Manager API",
                    Version = appVersion,
                    Description = "A micro-service that provides report management and generation services for R4",
                    TermsOfService = "UBS Confidential",
                    Contact = new Contact
                    {
                        Name = "Unique Business Systems",
                        Email = "to-be-updated@in.unibiz.com",
                        Url = "http://www.unibiz.com",
                    },
                    License = new License
                    {
                        Name = "LicenceTerms (To be updated)",
                        Url = " "
                    }
                });

                var xmlPath = AppDomain.CurrentDomain.BaseDirectory + @"UBS.ReportManager.Api.xml";
                c.IncludeXmlComments(xmlPath);

                var modelPath = AppDomain.CurrentDomain.BaseDirectory + @"UBS.ReportManager.Abstractions.xml";
                c.IncludeXmlComments(modelPath);

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Type = "apiKey",
                    Name = "Authorization",
                    Description = "For accessing the API a valid JWT token must be passed in all the " +
                                  "queries in the 'Authorization' header. The syntax used in the 'Authorization' " +
                                  "header should be Bearer xxxxx.yyyyyy.zzzz",
                    In = "header",
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", Array.Empty<string>()}
                });
                c.DescribeAllEnumsAsStrings();
                c.IgnoreObsoleteProperties();
                c.DescribeStringEnumsInCamelCase();
                c.IgnoreObsoleteActions();
                c.CustomSchemaIds(t => t.FullName);

                c.TagActionsBy(apiDesc => apiDesc.HttpMethod);
                c.OrderActionsBy(apiDesc => apiDesc.RelativePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ConfigureSwashbuckleUse(app, env);
            app.UseRequestLogging();
            app.UseMvc();
        }

        private static void ConfigureSwashbuckleUse(IApplicationBuilder app, IHostingEnvironment env)
        {
            var virtualBasePathSegment = Environment.GetEnvironmentVariable("SWAGGER_VIRTUAL_BASEPATH_SEGMENT");

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api-docs/currency-converter/{documentName}/swagger.json";
                c.PreSerializeFilters.Add((swagger, httpReq) => { swagger.BasePath = virtualBasePathSegment; });
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../api-docs/currency-converter/v1/swagger.json", "Currency-converter API v1");
                c.RoutePrefix = "api-docs"; // default is 'swagger'
                c.DocumentTitle = "Currency Converter API Documentation";
            });
        }
    }
}