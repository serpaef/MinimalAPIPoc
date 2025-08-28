using Microsoft.EntityFrameworkCore;
using MinimalAPIPoc.Domain.Entities;

namespace MinimalAPIPoc.Infrastructure.Db
{
    public class ApplicationDbContext : DbContext
    {

        private readonly IConfiguration _configuration;

        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DbSet<Admin> Admins { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if( optionsBuilder.IsConfigured ) return;

            var connectionString = _configuration.GetConnectionString("mysql")?.ToString();

            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                );
            }
        }
    }
}
