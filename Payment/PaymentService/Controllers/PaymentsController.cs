using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
            _logger.LogInformation(_configuration.GetConnectionString("PaymentConnection"));
            return "This is Payment Service Up ad running";
        }
    }
}
