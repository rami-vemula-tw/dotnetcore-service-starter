using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentService.Model.ReadModel
{
    public class BankInfo
    {
        public int Id { get; set; }
        public string BankCode { get; set; }
        public string Url {get; set;}
    }
}