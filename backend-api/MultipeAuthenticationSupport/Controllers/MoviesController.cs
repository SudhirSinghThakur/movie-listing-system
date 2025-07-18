﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultipeAuthenticationSupport.Data;
using MultipeAuthenticationSupport.Models;

namespace MultipeAuthenticationSupport.Controllers;

[Authorize(AuthenticationSchemes = "MultiScheme")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/movies")]
public class MoviesV1Controller : ControllerBase
{
    private readonly AppDbContext _context;

    public MoviesV1Controller(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetMovies() => Ok(_context.Movies.ToList());

    [HttpPost]
    public IActionResult AddMovie([FromBody] Movie movie)
    {
        _context.Movies.Add(movie);
        _context.SaveChanges();
        return Created($"api/v1/movies/{movie.Id}", movie);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateMovie(int id, [FromBody] Movie movie)
    {
        var existing = _context.Movies.Find(id);
        if (existing == null) return NotFound();

        existing.Title = movie.Title;
        existing.Genre = movie.Genre;
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMovie(int id)
    {
        var movie = _context.Movies.Find(id);
        if (movie == null) return NotFound();

        _context.Movies.Remove(movie);
        _context.SaveChanges();
        return NoContent();
    }
}
