using Microsoft.Extensions.Configuration;
using SmartMovieApp.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace SmartMovieApp.Services
{
    public class MovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public MovieService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("OMDbClient");
            _apiKey = configuration["OMDbApiKey"]; // appsettings.json'dan alınır
        }

        // Tekil film (tam eşleşme, case-insensitive)
        public async Task<MovieDto> GetMovieByTitleAsync(string title)
        {
            var response = await _httpClient.GetAsync($"?apikey={_apiKey}&t={title}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            var movie = JsonSerializer.Deserialize<MovieDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (movie == null || movie.Response == "False")
            {
                return null;
            }

            return movie;
        }

        // Kısmi eşleşme için API desteklemiyorsa local filtre yapılır.
        // Burada örnek olarak lokal önbellekte (liste) arama varsayımı var.
        // Eğer API destekliyorsa burada HTTP isteği yapmalısın.

        public async Task<List<MovieDto>> SearchMoviesByTitleAsync(string keyword)
        {
            // Örnek: API 's' parametresi ile arama yapıyor olabilir.
            // Eğer OMDb API kullanıyorsan 's' parametresi arama için:
            // e.g. http://www.omdbapi.com/?apikey=xxx&s=keyword

            var response = await _httpClient.GetAsync($"?apikey={_apiKey}&s={keyword}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            // OMDb 'Search' sonucu farklı yapıda gelir, DTO'nu buna göre oluşturman gerek.
            var searchResult = JsonSerializer.Deserialize<OmdbSearchResultDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (searchResult == null || searchResult.Response == "False" || searchResult.Search == null)
            {
                return new List<MovieDto>();
            }

            return searchResult.Search;
        }
    }
}
