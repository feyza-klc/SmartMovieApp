using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartMovieApp.Data;
using SmartMovieApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartMovieApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [HttpPost("register")]
        public IActionResult Register (RegisterDto dto)
        {
            if (_context.Users.Any(u=> u.Username == dto.Username || u.Email == dto.Email))
            {
                return BadRequest("Bu kullanıcı adı veya email zaten kayıtlı");
            }
            var user = new AppUser
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("Kayıt başarılı");
        }
        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.Users
                .FirstOrDefault(u=>
                (u.Username == dto.UsernameOrEmail|| u.Email==dto.UsernameOrEmail)
                && u.Password == dto.Password);

            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı veya şifre hatalı.");
            var token = GenerateJwtToken(user);

            return Ok(new UserDto
            {
                UserName = user.Username,
                Token = token,
            });
        }
        
        private string GenerateJwtToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes("bu-cok-gizli-ve-en-az-32-karakter-olan-bir-keydir!");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id.ToString())
            };
            var token = new JwtSecurityToken(
            issuer: "SmartMovieApp",
            audience: "SmartMovieUsers",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
