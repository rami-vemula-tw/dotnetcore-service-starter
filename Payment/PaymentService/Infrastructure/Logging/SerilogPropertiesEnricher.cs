using Serilog.Core;
using Serilog.Events;

namespace PaymentService.Infrastructure.Logging
{
    class SerilogPropertiesEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory logEventPropertyFactory)
        {
            logEvent.RemovePropertyIfPresent("SourceContext");
            logEvent.RemovePropertyIfPresent("ActionId");
            logEvent.RemovePropertyIfPresent("RequestId");
            logEvent.RemovePropertyIfPresent("ConnectionId");
            logEvent.RemovePropertyIfPresent("ParentId");
            logEvent.RemovePropertyIfPresent("SpanId");
        }
    }
}