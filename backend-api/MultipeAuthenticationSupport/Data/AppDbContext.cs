using Microsoft.EntityFrameworkCore;
using MultipeAuthenticationSupport.Models;

namespace MultipeAuthenticationSupport.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Movie> Movies => Set<Movie>();
    }
}
