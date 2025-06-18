namespace MovieListingSystem.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieListingSystem.Application.DTOs;
using MovieListingSystem.Application.Interfaces;

[Authorize(AuthenticationSchemes = "MultiScheme")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/movies")]
public class MoviesV1Controller : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesV1Controller(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMovies()
    {
        var movies = await _movieService.GetAllAsync();
        return Ok(movies);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMovie(int id)
    {
        var movie = await _movieService.GetByIdAsync(id);
        if (movie == null) return NotFound();
        return Ok(movie);
    }

    [HttpPost]
    public async Task<IActionResult> AddMovie([FromBody] MovieDto movieDto)
    {
        var addedMovie = await _movieService.AddAsync(movieDto);
        return CreatedAtAction(nameof(GetMovie), new { id = addedMovie.Id }, addedMovie);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieDto movieDto)
    {
        var updated = await _movieService.UpdateAsync(id, movieDto);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var deleted = await _movieService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
