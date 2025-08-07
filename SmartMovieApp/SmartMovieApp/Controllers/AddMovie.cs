using Microsoft.AspNetCore.Mvc;
using SmartMovieApp.Data;
using SmartMovieApp.DTOs;
using SmartMovieApp.Models;
using System.Text.Json;

namespace SmartMovieApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddMovie:ControllerBase
    {
        private readonly AppDbContext _context;
        public AddMovie(AppDbContext context)
        {
            _context = context;
        }
        [Route("import-movies")]
        public async Task<IActionResult> ImportMovies()
        {
            var movieTitles = new List<string>
    {
        "Inception", "The Dark Knight", "Interstellar", "Fight Club", "Pulp Fiction",
        "The Godfather", "The Shawshank Redemption", "Avengers: Endgame", "Titanic", "Whiplash",
        "Joker", "The Prestige", "The Lord of the Rings", "The Social Network", "Parasite",
        "Coco", "Up", "WALL·E", "Inside Out", "La La Land",
        "1917", "The Truman Show", "The Green Mile", "Django Unchained", "Good Will Hunting",
        "Catch Me If You Can", "Her", "The Imitation Game", "Black Swan", "The Departed"
    };

            using var httpClient = new HttpClient();

            foreach (var title in movieTitles)
            {
                var url = $"https://www.omdbapi.com/?apikey=7bb2b731&t={Uri.EscapeDataString(title)}";
                var response = await httpClient.GetStringAsync(url);
                var movieData = JsonSerializer.Deserialize<OmdbMovieResponse>(response);

                if (movieData != null && movieData.Response == "True")
                {
                    // Duplicate kontrolü
                    if (!_context.Movies.Any(m => m.Title == movieData.Title))
                    {
                        // Released tarihini DateTime olarak parse et
                        DateTime releaseDate = DateTime.MinValue;
                        DateTime.TryParse(movieData.Released, out releaseDate);

                        double.TryParse(movieData.imdbRating, out double imdb);

                        var movie = new Movie
                        {
                            Title = movieData.Title,
                            Description = movieData.Plot,
                            PosterUrl = movieData.Poster,
                            ReleaseDate = releaseDate,
                            ImdbRating = imdb
                        };

                        _context.Movies.Add(movie);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok("30 film başarıyla kaydedildi.");
        }

    }
}
