using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentService.Model.WriteModel
{
    public class BankInfo
    {
        [Required]
        [StringLength(255)]
        public string BankCode { get; set; }

        [Required]
        [StringLength(255)]
        public string Url {get; set;}
    }
}