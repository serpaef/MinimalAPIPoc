using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalAPIPoc.Domain.Entities;

namespace MinimalAPIPoc.Infrastructure.Db
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration? _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
        {
            _configuration = configuration;
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123",
                    Role = "Admin"
                });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if( optionsBuilder.IsConfigured || _configuration == null) return;

            var connectionString = _configuration.GetConnectionString("mysql")?.ToString();

            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                );
            }
        }

        public DbSet<Admin> Admins { get; set; } = default!;
        public DbSet<Vehicle> Vehicles { get; set; } = default!;
    }
}
