using Microsoft.EntityFrameworkCore;

namespace GammStore.Models
{
    public class GammStoreContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderBody> OrderBodies { get; set; }

        public GammStoreContext(DbContextOptions<GammStoreContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
