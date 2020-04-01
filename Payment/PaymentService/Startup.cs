using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentService.Data;
using PaymentService.Infrastructure.Common;
using PaymentService.Infrastructure.Configuration;
using PaymentService.Infrastructure.Contracts;
using PaymentService.Infrastructure.Logging;
using PaymentService.Infrastructure.Middleware;
using PaymentService.Services;
using PaymentService.Services.Contracts;

namespace PaymentService
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
         public IWebHostEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddConsole();
                config.Services.AddApplicationInsightsTelemetry();
            });

            services.AddControllers();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IServerContext, ServerContext>();
            services.AddScoped(typeof(IApplicationLogger<>), typeof(ApplicationLogger<>));
            services.AddEntityFrameworkNpgsql().AddDbContext<PaymentDbContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("PaymentConnection")));

            services.AddScoped<IBankInfoService, BankInfoService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ServerContextMiddleware>();
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
