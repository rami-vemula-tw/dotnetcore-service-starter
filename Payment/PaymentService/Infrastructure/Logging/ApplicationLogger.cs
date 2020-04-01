using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentService.Infrastructure.Common;
using PaymentService.Infrastructure.Configuration;
using PaymentService.Infrastructure.Contracts;

namespace PaymentService.Infrastructure.Logging
{
    public class ApplicationLogger<T> : IApplicationLogger<T> where T : class
    {
        private readonly Microsoft.Extensions.Logging.ILogger<T> logger;
        private readonly IServerContext serverContext;
        private readonly AppSettings appSettings;
        public ApplicationLogger(Microsoft.Extensions.Logging.ILogger<T> logger, IOptions<AppSettings> appSettings, IServerContext serverContext)
        {
            this.logger = logger;
            this.appSettings = appSettings.Value;
            this.serverContext = serverContext;
        }

        public void LogException(Exception ex, Dictionary<string, object> attributes = null)
        {
            Log(LogLevel.Error, ex.Message, ex, attributes);
        }

        public void LogWarning(string message, Dictionary<string, object> attributes = null)
        {
            Log(LogLevel.Information, message, attributes: attributes);
        }

        public void LogCritical(string message, Dictionary<string, object> attributes = null)
        {
            Log(LogLevel.Information, message, attributes: attributes);
        }

        public void LogInformation(string message, Dictionary<string, object> attributes = null)
        {
            Log(LogLevel.Information, message, attributes: attributes);
        }

        private void Log(LogLevel logLevel, string message, Exception exception = null, Dictionary<string, object> attributes = null)
        {
            var attrs = GetRequestInformation(attributes);
            using (this.logger.BeginScope(attrs))
            {
                switch (logLevel)
                {
                    case LogLevel.Error:
                        this.logger.LogError(exception, message, attrs);
                        break;
                    case LogLevel.Warning:
                        this.logger.LogWarning(message, attrs);
                        break;
                    case LogLevel.Critical:
                        this.logger.LogCritical(message, attrs);
                        break;
                    case LogLevel.Information:
                        this.logger.LogInformation(message, attrs);
                        break;
                    default:
                        this.logger.LogInformation(message, attrs);
                        break;
                }
            }
        }

        private Dictionary<string, object> GetRequestInformation(Dictionary<string, object> attributes)
        {
            if (attributes == null)
                attributes = new Dictionary<string, object>();

            attributes.Add("EventId", Guid.NewGuid());
            attributes.Add("CorrelationId", serverContext.CorrelationId);
            attributes.Add("ClientInfo", SerializeObject(serverContext.ClientContext));
            attributes.Add("TypeName", typeof(T).Name);
            attributes.Add("Timestamp", DateTime.UtcNow);
            attributes.Add("Source", this.appSettings.Source);
            return attributes;
        }

        private string SerializeObject(object o) => JsonSerializer.Serialize(o);
    }
}