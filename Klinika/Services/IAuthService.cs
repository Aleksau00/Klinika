using Klinika.Models;
using Klinika.Models.DTOs;

namespace Klinika.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        string GenerateJwtToken(Worker worker);
    }
}
