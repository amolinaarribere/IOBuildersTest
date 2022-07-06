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

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Transfer> Transfers { get; set; }

    }
}
