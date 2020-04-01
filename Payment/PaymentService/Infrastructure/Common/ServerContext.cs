using System;
using PaymentService.Infrastructure.Contracts;

namespace PaymentService.Infrastructure.Common
{
    public class ServerContext : IServerContext
    {
        public Guid CorrelationId { get; set; }

        public IClientContext ClientContext { get; set; }
    }
}