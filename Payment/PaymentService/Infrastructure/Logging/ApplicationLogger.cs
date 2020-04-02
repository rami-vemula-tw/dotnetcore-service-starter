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
        private readonly Microsoft.Extensions.Logging.ILogger<T> _logger;
        private readonly IServerContext serverContext;
        private readonly AppSettings appSettings;
        public ApplicationLogger(Microsoft.Extensions.Logging.ILogger<T> logger, IOptions<AppSettings> appSettings, IServerContext serverContext)
        {
            this._logger = logger;
            this.appSettings = appSettings.Value;
            this.serverContext = serverContext;
        }

        public void LogException(Exception ex, Dictionary<string, object> attributes = null)
        {
            Log(LogLevel.Error, ex.Message, ex, attributes);
        }

        public void LogWarning(string message, Dictionary<string, object> attributes = null)
        {
            Log(LogLevel.Warning, message, attributes: attributes);
        }

        public void LogCritical(string message, Dictionary<string, object> attributes = null)
        {
            Log(LogLevel.Critical, message, attributes: attributes);
        }

        public void LogInformation(string message, Dictionary<string, object> attributes = null)
        {
            Log(LogLevel.Information, message, attributes: attributes);
        }

        private void Log(LogLevel logLevel, string message, Exception exception = null, Dictionary<string, object> attributes = null)
        {
            using (this._logger.BeginScope(GetRequestInformation(attributes)))
            {
                switch (logLevel)
                {
                    case LogLevel.Error:
                        this._logger.LogError(exception, message);
                        break;
                    case LogLevel.Warning:
                        this._logger.LogWarning(message);
                        break;
                    case LogLevel.Critical:
                        this._logger.LogCritical(message);
                        break;
                    case LogLevel.Information:
                        this._logger.LogInformation(message);
                        break;
                    default:
                        this._logger.LogInformation(message);
                        break;
                }
            }
        }

        private Dictionary<string, object> GetRequestInformation(Dictionary<string, object> attributes)
        {
            if (attributes == null)
                attributes = new Dictionary<string, object>();

            attributes.Add("ApplicationEventId", Guid.NewGuid());
            attributes.Add("CorrelationId", serverContext.CorrelationId);
            attributes.Add("ClientInfo", SerializeObject(serverContext.ClientContext));
            attributes.Add("Source", this.appSettings.Source);
            return attributes;
        }
        private string SerializeObject(object o) => JsonSerializer.Serialize(o);
    }
}