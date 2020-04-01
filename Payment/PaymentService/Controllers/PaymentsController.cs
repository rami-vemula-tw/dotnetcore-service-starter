using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentService.Infrastructure.Contracts;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IApplicationLogger<PaymentsController> _logger;

        public PaymentsController(IApplicationLogger<PaymentsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            _logger.LogInformation("This is information");
            _logger.LogWarning("This is Warning");
            _logger.LogException(new Exception("This is exception"));

            return "This is Payment Service";
        }
    }
}
