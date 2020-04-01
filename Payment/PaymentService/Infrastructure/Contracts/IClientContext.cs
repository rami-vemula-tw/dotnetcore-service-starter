using System;

namespace PaymentService.Infrastructure.Contracts
{
    public interface IClientContext
    {
        string IPAddress { get; set; }
    }
}