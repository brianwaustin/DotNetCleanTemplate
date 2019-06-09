using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using StructureMap;
using CleanTemplate.Infrastructure.Data;
using CleanTemplate.Core.Interfaces;
using CleanTemplate.Core.Intefaces;
using CleanTemplate.Infrastructure.DomainEvents;
using CleanTemplate.Core.SharedKernel;
using Swashbuckle.AspNetCore.Swagger;

namespace LoggingReferenceProject
{
    public class Startup
    {      

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration; 
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            /* Add DbContext */
            string dbName = Guid.NewGuid().ToString();
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(dbName));
            //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IRepository, EfRepository>();
            services.AddTransient<IDomainEventDispatcher, DomainEventDispatcher>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "CleanTemplate API - v1",
                    Version = "v1",
                    Description = "API documentation for endpoints for the CleanTemplate project"
                });               

                options.DescribeAllEnumsAsStrings();

                /* Set the comments path for the swagger json and ui. */
                //var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                //foreach (var name in Directory.GetFiles(basePath, "*.XML", SearchOption.AllDirectories))
                //{
                //    options.IncludeXmlComments(name);
                //}

            });

            services.ConfigureSwaggerGen(options =>
            {
                options.CustomSchemaIds(x => x.FullName);
            });

            var container = new Container();

            container.Configure(config =>
            {
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Startup)); // Web
                    _.AssemblyContainingType(typeof(BaseEntity)); // Core
                    _.Assembly("CleanTemplate.Infrastructure"); //Infrastructure
                    _.WithDefaultConventions();
                    _.ConnectImplementationsToTypesClosing(typeof(IHandle<>));
                });

                // TODO: Add Registry Classes to eliminate reference to Infrastructure

                // TODO: Move to Infrastucture Registry
                config.For<IRepository>().Add<EfRepository>();

                /* Populate the container using the service collection */
                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //_logger.LogInformation("In Development environment");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);

            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                //c.RoutePrefix = documentationSettings.URL;
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                //c.OAuthClientId("emporos_point-of-sale");
                //c.OAuthClientSecret("53cr375qu1rr3l");
            });

            app.UseMvc(routes => routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"));
        }
    }
}
