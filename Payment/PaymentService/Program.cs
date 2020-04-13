using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PaymentService.Data;
using PaymentService.Infrastructure.Logging;
using PaymentService.Infrastructure.Vault;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.Network;

namespace PaymentService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args)
                .Build()
                .MigrateDatabase()
                .Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);
                    config.AddEnvironmentVariables();

                    var baseConfig = new ConfigurationBuilder()
                                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                                        .AddEnvironmentVariables()
                                        .Build();

                    if (baseConfig.GetValue<bool>("Vault:Enabled"))
                    {
                        var url = baseConfig.GetValue<string>("Vault:URL");
                        var roleId = baseConfig.GetValue<string>("Vault:RoleId");
                        var secretId = baseConfig.GetValue<string>("Vault:SecretId");
                        config.AddVault(url, roleId, secretId);
                    }
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    var environment = hostingContext.Configuration["ASPNETCORE_ENVIRONMENT"];
                    var logConfiguration = new LoggerConfiguration()
                       .Enrich.FromLogContext()
                       .Enrich.WithExceptionDetails()
                       .Enrich.WithMachineName()
                       .Enrich.WithProperty("Environment", environment)
                       .Enrich.With(new SerilogPropertiesEnricher())
                       .WriteTo.Console(new CompactJsonFormatter())
                       .ReadFrom.Configuration(hostingContext.Configuration);

                    if (hostingContext.Configuration.GetValue<bool>("LogstashConfiguration:Enabled"))
                    {
                        logConfiguration.WriteTo.TCPSink(hostingContext.Configuration["LogstashConfiguration:Uri"], textFormatter: new CompactJsonFormatter());
                        //logConfiguration.WriteTo.Http(configuration["LogstashConfiguration:Uri"])
                    }

                    if (hostingContext.Configuration.GetValue<bool>("LogstashConfiguration:Enabled"))
                    {
                        logConfiguration.WriteTo.Elasticsearch(ConfigureElasticSink(hostingContext.Configuration, environment));
                    }

                    Log.Logger = logConfiguration.CreateLogger();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();

        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                CustomFormatter = new CompactJsonFormatter(),
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
    }
}
