using Microsoft.EntityFrameworkCore;

namespace Bank.Models
{
    public class BankContext : DbContext
    {
        public BankContext(DbContextOptions<BankContext> options)
           : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<CustodialWallet> Wallets { get; set; }

        public DbSet<BankTransaction> Transactions { get; set; }

    }
}
