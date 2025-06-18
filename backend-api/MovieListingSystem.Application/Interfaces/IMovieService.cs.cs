using MovieListingSystem.Application.DTOs;

namespace MovieListingSystem.Application.Interfaces;

public interface IMovieService
{
    Task<IEnumerable<MovieDto>> GetAllAsync();
    Task<MovieDto?> GetByIdAsync(int id);
    Task<MovieDto> AddAsync(MovieDto movie);
    Task<bool> UpdateAsync(int id, MovieDto movie);
    Task<bool> DeleteAsync(int id);
}
