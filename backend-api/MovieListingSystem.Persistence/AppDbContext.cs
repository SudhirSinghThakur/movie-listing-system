using Microsoft.EntityFrameworkCore;
using MovieListingSystem.Domain.Entities;

namespace MovieListingSystem.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Movie> Movies => Set<Movie>();
    }
}
