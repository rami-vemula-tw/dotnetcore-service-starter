using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentService.Model;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(ILogger<PaymentsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<BankInfo> Get()
        {
            _logger.LogInformation("Get Action");
            return new List<BankInfo>();
        }
    }
}
