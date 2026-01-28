using Klinika.Models;
using Klinika.Models.DTOs;
using Klinika.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Klinika.Services
{
    internal class AuthService : IAuthService
    {
        private readonly IWorkerRepository _workerRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IWorkerRepository workerRepository, IConfiguration configuration)
        {
            _workerRepository = workerRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var worker = await _workerRepository.GetByEmailAsync(request.Email);

            if (worker == null || !VerifyPassword(request.Password, worker.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            if (!worker.IsActive)
            {
                throw new UnauthorizedAccessException("Account is inactive");
            }

            var token = GenerateJwtToken(worker);

            return new LoginResponse
            {
                Token = token,
                Email = worker.Email,
                Role = GetRoleName(worker)
            };
        }

        private string GetRoleName(Worker worker)
        {
            return worker switch
            {
                Administrator => "Administrator",
                Doctor => "Doctor",
                Secretary => "Secretary",
                _ => throw new InvalidOperationException("Unknown worker type")
            };
        }

        public string GenerateJwtToken(Worker worker)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, worker.Id.ToString()),
            new Claim(ClaimTypes.Email, worker.Email),
            new Claim(ClaimTypes.Role, GetRoleName(worker))
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
