using System;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PaymentService.Data;
using PaymentService.Infrastructure.Logging;
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
            ConfigureLogging();

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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();

        private static void ConfigureLogging()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Environment", environment)
                .Enrich.With(new SerilogPropertiesEnricher())
                .WriteTo.Console(new CompactJsonFormatter())
                .WriteTo.TCPSink(configuration["LogstashConfiguration:Uri"], textFormatter : new CompactJsonFormatter())
                //.WriteTo.Http(configuration["LogstashConfiguration:Uri"])
                //.WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
    }
}
