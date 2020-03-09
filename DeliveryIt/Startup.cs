using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DeliverIt.Data;
using DeliverIt.Services;

namespace DeliverIt
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));


            services.AddDbContext<DeliverItContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DeliveryItContext")));
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IPartnerService, PartnerService>();
            services.AddTransient<IDeliveryService, DeliveryService>();
            services.AddSwaggerDocument(config =>
                {
                    config.PostProcess = document =>
                    {
                        document.Info.Version = "v1";
                        document.Info.Title = "DeliverIt API";
                        document.Info.Description = "A simple ASP.NET Core web API";
                        document.Info.TermsOfService = "Basic Terms";
                        document.Info.Contact = new NSwag.OpenApiContact
                        {
                            Name = "Mudasar Rauf",
                            Email = string.Empty,
                            Url = "https://github.com/mudasar"
                        };
                        document.Info.License = new NSwag.OpenApiLicense
                        {
                            Name = "Open License",
                            Url = "https://mudasarrauf.wordpress.com/license"
                        };
                    };
                });

            services.AddControllers();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
