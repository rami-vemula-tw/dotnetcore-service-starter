using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PaymentService.Infrastructure.Common;

namespace PaymentService.Infrastructure.Contracts
{
    public interface IApplicationLogger<T> where T : class
    {
        void LogInformation(string eventCode, string message, params (string Key, object Value)[] enrichedAttributes);
        void LogException(string eventCode, Exception ex, params (string Key, object Value)[] enrichedAttributes);
        void LogWarning(string eventCode, string message, params (string Key, object Value)[] enrichedAttributes);
        void LogCritical(string eventCode, string message, params (string Key, object Value)[] enrichedAttributes);
    }
}