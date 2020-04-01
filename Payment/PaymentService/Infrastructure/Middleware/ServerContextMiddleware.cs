using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using PaymentService.Infrastructure.Common;
using PaymentService.Infrastructure.Contracts;

namespace PaymentService.Infrastructure.Middleware    
{
    public class ServerContextMiddleware
    {
        private readonly RequestDelegate _next;

        public ServerContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IServerContext serverContext)
        {
            var correlationId = context.Request.Headers["CorrelationId"];
            serverContext.CorrelationId = correlationId == StringValues.Empty ? Guid.NewGuid() : Guid.Parse(correlationId);
            serverContext.ClientContext = new ClientContext{ IPAddress = context.Connection.RemoteIpAddress.ToString() };
            await _next.Invoke(context);
        }
    }
}