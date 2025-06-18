namespace MovieListingSystem.Application.Services;

using Microsoft.EntityFrameworkCore;
using MovieListingSystem.Application.DTOs;
using MovieListingSystem.Application.Interfaces;
using MovieListingSystem.Domain.Entities;
using MovieListingSystem.Persistence;

public class MovieService : IMovieService
{
    private readonly AppDbContext _context;

    public MovieService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MovieDto>> GetAllAsync()
    {
        return await _context.Movies
            .Select(m => new MovieDto
            {
                Id = m.Id,
                Title = m.Title,
                Genre = m.Genre
            })
            .ToListAsync();
    }

    public async Task<MovieDto?> GetByIdAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        return movie == null ? null : new MovieDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Genre = movie.Genre
        };
    }

    public async Task<MovieDto> AddAsync(MovieDto movieDto)
    {
        var movie = new Movie
        {
            Title = movieDto.Title,
            Genre = movieDto.Genre
        };

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        movieDto.Id = movie.Id;
        return movieDto;
    }

    public async Task<bool> UpdateAsync(int id, MovieDto movieDto)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return false;

        movie.Title = movieDto.Title;
        movie.Genre = movieDto.Genre;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return false;

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        return true;
    }
}
