using Microsoft.AspNetCore.Mvc;
using SmartMovieApp.Services;

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
        [HttpGet("title/{title}")]
        public async Task<IActionResult> GetMovieByTitle(string title)
        {
            var movie = await _movieService.GetMovieByTitleAsync(title);
            if (movie == null) 
                return NotFound("Film bulunamadı");

            return Ok(movie);
        }
    }
}
