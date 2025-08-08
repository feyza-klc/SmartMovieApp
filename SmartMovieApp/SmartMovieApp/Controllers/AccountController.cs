using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SmartMovieApp.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
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
            if (!ModelState.IsValid)
                return View(model);

            var client = _clientFactory.CreateClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7168/api/Auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Kullanıcı adı, email veya şifre hatalı.";
                return View(model);
            }

            var result = await response.Content.ReadAsStringAsync();

            UserDto userDto;
            try
            {
                userDto = JsonSerializer.Deserialize<UserDto>(result, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                ViewBag.Error = "Sunucudan geçersiz yanıt alındı.";
                return View(model);
            }

            if (userDto == null)
            {
                ViewBag.Error = "Sunucudan boş yanıt alındı.";
                return View(model);
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, userDto.UserName),
       
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return RedirectToAction("Home", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
