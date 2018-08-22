using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtMuseWeb.Models;
using VirtMuseWeb.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace VirtMuseWeb
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

            services.AddDbContext<VirtMuseWebContext>(options => { 
                    options.UseSqlServer(Configuration.GetConnectionString("VirtMuseWebContext")); options.EnableSensitiveDataLogging(true);
            });

            services.AddMvc();

            services.AddLogging();

            services.AddScoped<IResourceService, ResourceService>((serv) => {
                    return new ResourceService(serv.GetService<IHostingEnvironment>(), serv.GetService<VirtMuseWebContext>(), serv.GetService<ILogger<Program>>()
                       );
                });

            services.AddScoped<IMuseumService, MuseumService>((serv) =>
            {
                return new MuseumService(serv.GetService<IHostingEnvironment>(), serv.GetService<VirtMuseWebContext>(), serv.GetService<ILogger<Program>>()
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
