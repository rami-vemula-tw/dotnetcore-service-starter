using System;

namespace PaymentService.Infrastructure.Contracts
{
    public interface IServerContext
    {
        Guid CorrelationId { get; set; }
        IClientContext ClientContext { get; set; }
    }
}