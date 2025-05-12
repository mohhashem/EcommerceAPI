using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class EcommerceDBContext : DbContext
    {
        public EcommerceDBContext(DbContextOptions<EcommerceDBContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<Brand> Brand { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Product> Product{ get; set; }
    }
}
