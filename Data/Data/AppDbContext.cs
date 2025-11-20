using Microsoft.EntityFrameworkCore;
using Repository.Entities;



namespace Repository.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<ItemEntity> Items { get; set; }
        public DbSet<RentalBookingEntity> RentalBookings { get; set; }
    }
}
