using Microsoft.AspNetCore.Mvc;
using SmartMovieApp.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SmartMovieApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly MovieService _movieService;
        public MoviesController(MovieService movieService)
        {
            _movieService = movieService;
        }

        // Tekil tam eşleşme (case-insensitive)
        [HttpGet("title/{title}")]
        public async Task<IActionResult> GetMovieByExactTitle(string title)
        {
            var movie = await _movieService.GetMovieByTitleAsync(title);
            if (movie == null)
                return NotFound("Film bulunamadı");

            return Ok(movie);
        }

        // Kısmi eşleşme (case-insensitive), çoklu sonuç
        [HttpGet("search/{keyword}")]
        public async Task<IActionResult> SearchMovies(string keyword)
        {
            var movies = await _movieService.SearchMoviesByTitleAsync(keyword);
            if (movies == null || !movies.Any())
                return NotFound("Eşleşen filmler bulunamadı");

            return Ok(movies);
        }
    }
}
