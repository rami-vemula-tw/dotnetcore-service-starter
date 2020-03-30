using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PaymentService.Model;

namespace PaymentService.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options):base(options) {  }
        public DbSet<BankInfo> BankInfos { get; set; }
    }
}