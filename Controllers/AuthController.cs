using System.Text;
using LibraryAPI.DTOs.Auth;
using LibraryAPI.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace LibraryAPI.Controllers {

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase {
        private readonly IUsersRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IUsersRepository repo, IConfiguration config) {
            _repo = repo;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequestDTO dto) {

            var user = _repo.Login(dto.Login, dto.Password);
            if (user == null)
                return Unauthorized("Invalid login or password.");

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Name, user.name),
                new Claim(ClaimTypes.Email, user.email)
            };

            var jwtKey = _config["Jwt:Key"];

            if (string.IsNullOrEmpty(jwtKey)) {
                throw new InvalidOperationException("The JWT key was not configured.");
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new LoginResponseDTO {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresIn = token.ValidTo,
                User = new {
                    user.id,
                    user.name,
                    user.email
                }
            });
        }
    }
}