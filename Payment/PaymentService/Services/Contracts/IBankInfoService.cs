using System.Threading.Tasks;
using Read = PaymentService.Model.ReadModel;
using Write = PaymentService.Model.WriteModel;

namespace PaymentService.Services.Contracts
{
    public interface IBankInfoService
    {
        Task<Read.BankInfo> CreateBankInfoAsync(Write.BankInfo bankInfo);
        Task<Read.BankInfo> GetBankInfoByIdAsync(int id);
    }
}