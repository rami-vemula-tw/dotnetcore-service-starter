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

        public void LogException(string eventCode, Exception ex, params (string Key, object Value)[] enrichedAttributes)
        {
            Log(eventCode, LogLevel.Error, ex.Message, ex, enrichedAttributes);
        }

        public void LogWarning(string eventCode, string message, params (string Key, object Value)[] enrichedAttributes)
        {
            Log(eventCode, LogLevel.Warning, message, enrichedAttributes: enrichedAttributes);
        }

        public void LogCritical(string eventCode, string message, params (string Key, object Value)[] enrichedAttributes)
        {
            Log(eventCode, LogLevel.Critical, message, enrichedAttributes: enrichedAttributes);
        }

        public void LogInformation(string eventCode, string message, params (string Key, object Value)[] enrichedAttributes)
        {
            Log(eventCode, LogLevel.Information, message, enrichedAttributes: enrichedAttributes);
        }

        private void Log(string eventCode, LogLevel logLevel, string message, Exception exception = null, params (string Key, object Value)[] enrichedAttributes)
        {
            using (this._logger.BeginScope(GetRequestInformation(eventCode, enrichedAttributes)))
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

        private Dictionary<string, object> GetRequestInformation(string eventCode, params (string Key, object Value)[] enrichedAttributes)
        {
            var attributes = new Dictionary<string, object>();
            foreach (var enrichedAttribute in enrichedAttributes) {
                attributes.Add(enrichedAttribute.Key, enrichedAttribute.Value);
            }

            attributes.Add("eventCode", eventCode);
            attributes.Add("ApplicationEventId", Guid.NewGuid());
            attributes.Add("CorrelationId", serverContext.CorrelationId);
            attributes.Add("ClientInfo", SerializeObject(serverContext.ClientContext));
            attributes.Add("Source", this.appSettings.Source);
            return attributes;
        }
        private string SerializeObject(object o) => JsonSerializer.Serialize(o);
    }
}