using MovieListingSystem.Domain.Entities;

namespace MovieListingSystem.Persistence
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User { Email = "admin@test.com", Password = "admin123" },
                    new User { Email = "user@example.com", Password = "user123" }
                );
            }

            if (!context.Movies.Any())
            {
                context.Movies.AddRange(
                    new Movie { Title = "Inception", Genre = "Sci-Fi" },
                    new Movie { Title = "The Godfather", Genre = "Crime" },
                    new Movie { Title = "The Dark Knight", Genre = "Action" }
                );
            }

            context.SaveChanges();
        }
    }

}
