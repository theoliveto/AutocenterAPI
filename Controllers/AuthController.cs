using Google.Apis.Auth;
using AutocenterAPI.DTOs;
using AutocenterAPI.DTOs.Auth;
using AutocenterAPI.Helpers;
using AutocenterAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutocenterAPI.Controllers {

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase {
        private readonly IUsersRepository _repo;
        private readonly IConfiguration _config;
        private readonly IEmailService _email;

        public AuthController(IUsersRepository repo, IConfiguration config, IEmailService email) {
            _repo = repo;
            _config = config;
            _email = email;
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

        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody] GoogleLoginRequestDTO dto) {
            if (string.IsNullOrWhiteSpace(dto.IdToken)) { 
                return BadRequest(new { message = "Missing idToken." });
            }

            var googleClientId = _config["Google:ClientId"];
            if (string.IsNullOrWhiteSpace(googleClientId)) { 
                throw new InvalidOperationException("Google ClientId not configured.");
            }

            GoogleJsonWebSignature.Payload payload;
            try {
                payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, new GoogleJsonWebSignature.ValidationSettings {
                    Audience = new[] { googleClientId }
                });
            } catch {
                return Unauthorized(new { message = "Invalid Google token." });
            }

            if (payload.EmailVerified != true) { 
                return Unauthorized(new { message = "Google account email not verified." });
            }

            var email = (payload.Email ?? "").Trim().ToLowerInvariant();
            var googleSub = payload.Subject;

            var user = _repo.GetByGoogleSub(googleSub) ?? _repo.GetByEmail(email);

            if (user != null && string.IsNullOrWhiteSpace(user.googleSub)) {
                _repo.SetGoogleSub(user.id, googleSub);
            }

            if (user == null) {
                user = _repo.CreateGoogleUser(new UsersDTO {
                    name = payload.Name ?? payload.Email ?? "User",
                    login = email, 
                    email = email,
                    role = "USER",
                    active = true,
                    password = HashHelper.CreateSha256(Guid.NewGuid().ToString("N") + "!G"),
                    googleSub = googleSub
                });
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Name, user.name),
                new Claim(ClaimTypes.Email, user.email)
            };

            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey)) { 
                throw new InvalidOperationException("The JWT key was not configured.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
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
                User = new { user.id, user.name, user.email }
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO dto) {
            var email = (dto.Email ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email))
                return Ok(new { message = "If the account exists, we sent a reset link." });

            var user = _repo.GetByEmail(email);
            if (user == null)
                return Ok(new { message = "If the account exists, we sent a reset link." });

            var token = ResetTokenHelper.GenerateToken();
            var tokenHash = ResetTokenHelper.Sha256(token);

            var expiresAtUtc = DateTime.UtcNow.AddMinutes(30);
            _repo.CreatePasswordResetToken(user.id, tokenHash, expiresAtUtc);

            var frontend = _config["App:FrontendBaseUrl"]?.TrimEnd('/') ?? "";
            var resetUrl = $"{frontend}/reset-password?email={Uri.EscapeDataString(user.email)}&token={Uri.EscapeDataString(token)}";

            var subject = "Reset your password";
            var body = $@"
                <p>You requested a password reset.</p>
                <p><a href=""{resetUrl}"">Click here to reset your password</a> (valid for 30 minutes).</p>
                <p>If you did not request this, you can ignore this email.</p>
            ";

            try {
                await _email.SendAsync(user.email, subject, body);
            } catch {

            }

            return Ok(new { message = "If the account exists, we sent a reset link." });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequestDTO dto) {
            var email = (dto.Email ?? "").Trim().ToLowerInvariant();
            var token = (dto.Token ?? "").Trim();
            var newPassword = dto.NewPassword ?? "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword)) { 
                return BadRequest(new { message = "Invalid request." });
            }

            if (newPassword.Length < 8) { 
                return BadRequest(new { message = "Password must be at least 8 characters." });
            }

            var user = _repo.GetByEmail(email);
            if (user == null) { 
                return BadRequest(new { message = "Invalid token or expired." });
            }

            var tokenHash = ResetTokenHelper.Sha256(token);
            var nowUtc = DateTime.UtcNow;

            var ok = _repo.TryUsePasswordResetToken(user.id, tokenHash, nowUtc);
            if (!ok) { 
                return BadRequest(new { message = "Invalid token or expired." });
            }

            var passwordHash = HashHelper.CreateSha256(newPassword);
            _repo.UpdatePassword(user.id, passwordHash);

            return Ok(new { message = "Password updated successfully." });
        }
    }
}