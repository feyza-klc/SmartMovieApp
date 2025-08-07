using Microsoft.AspNetCore.Mvc;
using SmartMovieApp.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMovieApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public AccountController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7168/api/Auth/register", dto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Error = await response.Content.ReadAsStringAsync();
                return View(dto);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _clientFactory.CreateClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7168/api/Auth/login", content); // PORT'u düzelt

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var userDto = JsonSerializer.Deserialize<UserDto>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Login başarılı → token vs. saklayabilirsin
                HttpContext.Session.SetString("username", userDto.UserName);
                HttpContext.Session.SetString("token", userDto.Token);

                return RedirectToAction("Home", "Home");
            }
            else
            {
                ViewBag.Error = "Kullanıcı adı, email veya şifre hatalı.";
                return View(model);
            }
        }
    }
}
