
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PaymentService.Data
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            using (var paymentContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>())
            {
                var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

                if (!environment.IsDevelopment())
                {
                    return host;
                }

                var logger = scope.ServiceProvider.GetRequiredService<ILogger<PaymentDbContext>>();
                try
                {
                    logger.LogInformation("DB Migration Started.");
                    paymentContext.Database.Migrate();
                    logger.LogInformation("DB Migration Completed.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    throw;
                }
            }

            return host;
        }
    }
}