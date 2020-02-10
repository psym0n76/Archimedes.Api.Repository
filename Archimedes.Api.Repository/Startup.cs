﻿
using Archimedes.Library.Domain;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archimedes.Api.Repository
{
    public class Startup
    {
        //https://www.youtube.com/watch?v=oXNslgIXIbQ logging
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.Configure<Config>(Configuration.GetSection("AppSettings"));
            services.AddSingleton(Configuration);

            var config = Configuration.GetSection("AppSettings").Get<Config>();

            services.AddAutoMapper(typeof(Startup));
            //services.AddControllers().AddNewtonsoftJson();

            services.AddDbContext<ArchimedesContext>(options =>
                options.UseSqlServer(config.DatabaseServerConnection));

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            //services.AddMvc()
             //   .AddNewtonsoftJson();
            services.AddMvc().SetCompatibilityVersion(version: CompatibilityVersion.Version_3_0);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILogger<Startup> logger)
        {
            logger.LogInformation("Started configure:");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}