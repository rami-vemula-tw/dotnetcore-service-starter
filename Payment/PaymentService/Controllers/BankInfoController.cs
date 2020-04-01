using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Model.ReadModel;
using PaymentService.Infrastructure.Contracts;
using PaymentService.Services.Contracts;
using Read = PaymentService.Model.ReadModel;
using Write = PaymentService.Model.WriteModel;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankInfoController : ControllerBase
    {
        private readonly IApplicationLogger<BankInfoController> _logger;
        private readonly IBankInfoService _bankInfoService;

        public BankInfoController(IApplicationLogger<BankInfoController> logger, IBankInfoService bankInfoService)
        {
            _logger = logger;
            _bankInfoService = bankInfoService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BankInfo>> PostBankInfoAsync(Write.BankInfo bankInfo)
        {
            var bankInfoReadModel = await _bankInfoService.CreateBankInfoAsync(bankInfo);
            return CreatedAtAction(nameof(GetBankInfoAsync), new { id = bankInfoReadModel.Id }, bankInfoReadModel);
        }

        [HttpGet("{id}")]
        [ActionName("GetBankInfoAsync")]
        public async Task<ActionResult<BankInfo>> GetBankInfoAsync(int id)
        {
            var bankInfoReadModel =  await _bankInfoService.GetBankInfoByIdAsync(id);
            if(bankInfoReadModel == null) return NotFound();
            return bankInfoReadModel;
        }
    }
}
