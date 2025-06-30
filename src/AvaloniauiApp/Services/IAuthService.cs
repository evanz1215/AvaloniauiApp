using System.Threading.Tasks;
using AvaloniauiApp.Models;

namespace AvaloniauiApp.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<RegisterResult> RegisterAsync(RegisterRequest request);
        Task<RefreshTokenResponse?> RefreshTokenAsync(RefreshTokenRequest request);
    }
} 