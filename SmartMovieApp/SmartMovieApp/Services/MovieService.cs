using Microsoft.Extensions.Configuration;
using SmartMovieApp.DTOs;
using SmartMovieApp.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMovieApp.Services
{
    public class MovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public MovieService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("OMDbClient");
            _apiKey = configuration["OMDbApiKey"]; // appsettings.json'dan alacağız
        }

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

            if (movie.Response == "False")
            {
                return null;
            }

            return movie;
        }
    }

}
