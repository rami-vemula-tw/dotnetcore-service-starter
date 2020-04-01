using System.Threading.Tasks;
using PaymentService.Data;
using PaymentService.Model;
using PaymentService.Services.Contracts;
using Read = PaymentService.Model.ReadModel;
using Write = PaymentService.Model.WriteModel;
using DataModel = PaymentService.Data.Model;

namespace PaymentService.Services
{
    public class BankInfoService : IBankInfoService
    {
        private readonly PaymentDbContext _context;
        public BankInfoService(PaymentDbContext context)
        {
            _context = context;
        }
        public async Task<Read.BankInfo> CreateBankInfoAsync(Write.BankInfo bankInfo)
        {
            var bankDataModel = new Data.Model.BankInfo { BankCode = bankInfo.BankCode, Url = bankInfo.Url};
            _context.BankInfos.Add(bankDataModel);
            await _context.SaveChangesAsync();
            
            return ConvertBankInfoDataModelToReadModel(bankDataModel);
        }

        public async Task<Read.BankInfo> GetBankInfoByIdAsync(int id)
        {
            var bankDataModel = await _context.BankInfos.FindAsync(id);
            if (bankDataModel == null) return null;

            return ConvertBankInfoDataModelToReadModel(bankDataModel);
        }

        private Read.BankInfo ConvertBankInfoDataModelToReadModel(DataModel.BankInfo bankDataModel)
        {
            return new Read.BankInfo{ Id = bankDataModel.Id, BankCode = bankDataModel.BankCode, Url = bankDataModel.Url };
        }
    }

}