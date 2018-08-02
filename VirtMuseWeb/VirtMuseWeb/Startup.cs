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
            services.AddDbContext<VirtMuseWebContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            VirtMuseWebContext context = services.First(serv =>
            {
                return serv.ServiceType == typeof(VirtMuseWebContext);
            }).ImplementationInstance as VirtMuseWebContext;

            services.AddMvc();

            services.AddLogging();

            services.AddTransient<IMailService, MailService>();

            services.AddTransient<IResourceService, ResourceService>(
                (serv) => {
                    return new ResourceService(serv.GetService<IHostingEnvironment>(), serv.GetService<VirtMuseWebContext>()
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
