using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMovieApp.Data;
using SmartMovieApp.Models;
using SmartMovieApp.Services;

namespace SmartMovieApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly MovieService _movieService;

        public HomeController(MovieService movieService, AppDbContext context)
        {
            _movieService = movieService;
            _context = context;
        }

        // Giriþ yapmamýþ kullanýcýlar için ana sayfa
        public async Task<IActionResult> Index(string query)
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                var apiMovies = await _movieService.SearchMoviesByTitleAsync(query);
                ViewBag.SearchQuery = query;

                return View("IndexApi", apiMovies);  // Arama sonuçlarý için view
            }

            var movies = _context.Movies
                .OrderBy(x => Guid.NewGuid())
                .Take(15)
                .ToList();

            return View(movies);  // Index.cshtml
        }

        // Giriþ yapmýþ kullanýcýlar için ana
        [Authorize]
        public async Task<IActionResult> Home(string query)
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                var apiMovies = await _movieService.SearchMoviesByTitleAsync(query);
                ViewBag.SearchQuery = query;

                return View("IndexApi", apiMovies);  // Arama sonuçlarý için view (ayný IndexApi.cshtml)
            }

            var movies = _context.Movies
                .OrderBy(x => Guid.NewGuid())
                .Take(15)
                .ToList();

            return View(movies);  // Home.cshtml — giriþli kullanýcý anasayfasý
        }
    }
}
