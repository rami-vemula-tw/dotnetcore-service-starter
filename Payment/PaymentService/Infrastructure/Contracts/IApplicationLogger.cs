using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PaymentService.Infrastructure.Common;

namespace PaymentService.Infrastructure.Contracts
{
    public interface IApplicationLogger<T> where T : class
    {
        void LogInformation(string message, Dictionary<string, object> attributes = null);
        void LogException(Exception ex, Dictionary<string, object> attributes = null);
        void LogWarning(string message, Dictionary<string, object> attributes = null);
        void LogCritical(string message, Dictionary<string, object> attributes = null);
    }
}