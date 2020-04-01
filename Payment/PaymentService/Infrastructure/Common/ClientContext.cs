using System;
using PaymentService.Infrastructure.Contracts;

namespace PaymentService.Infrastructure.Common
{
    public class ClientContext : IClientContext
    {
        public string IPAddress { get; set; }
    }
}