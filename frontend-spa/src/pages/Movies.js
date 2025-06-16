import React, { useState, useEffect } from 'react';
import './Movies.css';
import LogoutButton from "./Logout";


function Movies() {
  const [movies, setMovies] = useState([]);
  const [title, setTitle] = useState('');
  const [genre, setGenre] = useState('');

  const loadMovies = async () => {
    const token = localStorage.getItem("token");
    const res = await fetch("http://localhost:5091/api/v1/movies", {
      headers: { Authorization: `Bearer ${token}` }
    });
    const data = await res.json();
    setMovies(data);
  };

  const addMovie = async () => {
    const token = localStorage.getItem("token");
    await fetch("http://localhost:5091/api/v1/movies", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ title, genre })
    });
    setTitle('');
    setGenre('');
    loadMovies();
  };

  useEffect(() => {
    loadMovies();
  }, []);

  return (
    <div className="movies-container">
      <div className="movies-header">
        <h1>🎬 Movies</h1>
        <LogoutButton />
      </div>
      <div className="add-movie-form">
        <input
          value={title}
          onChange={e => setTitle(e.target.value)}
          placeholder="Title"
        />
        <input
          value={genre}
          onChange={e => setGenre(e.target.value)}
          placeholder="Genre"
        />
        <button onClick={addMovie} disabled={!title || !genre}>
          Add Movie
        </button>
      </div>
      <div className="movies-list">
        {movies.length === 0 ? (
          <div className="empty-list">No movies found.</div>
        ) : (
          movies.map(m => (
            <div className="movie-card" key={m.id}>
              <div className="movie-title">{m.title}</div>
              <div className="movie-genre">{m.genre}</div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}

export default Movies;
