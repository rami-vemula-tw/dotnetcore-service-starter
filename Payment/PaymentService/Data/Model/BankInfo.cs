using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentService.Data.Model
{
    [Table("bank_info")]
    public class BankInfo
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("bank_code", TypeName="varchar(255)")]
        public string BankCode { get; set; }

        [Column("url", TypeName="varchar(255)")]
        public string Url {get; set;}
    }
}