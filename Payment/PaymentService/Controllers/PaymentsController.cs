using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PaymentService.Infrastructure.Contracts;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IApplicationLogger<PaymentsController> _logger;
        private readonly IConfiguration _configuration;

        public PaymentsController(IApplicationLogger<PaymentsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public string Get()
        {
            _logger.LogInformation($"The payment database connection string : { _configuration.GetConnectionString("PaymentConnection") }", ("Process", "Payment"), ("Data","123"));
            _logger.LogCritical($"The Elastic Search Endpoint : { _configuration["ElasticConfiguration:Uri"] }");
            _logger.LogCritical($"The Logstash Endpoint : { _configuration["LogstashConfiguration:Uri"] }");
            _logger.LogWarning($"The Current Environment : { Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") }");
            return "Payment Service Up ad running";
        }
    }


}
