using System.Threading.Tasks;

namespace AvaloniauiApp.Services
{
    public interface ITokenService
    {
        string? GetToken();
        Task<string?> GetTokenAsync();
        void SetToken(string token);
        Task SetTokenAsync(string token);
        void ClearToken();
        Task ClearTokenAsync();
        
        string? GetRefreshToken();
        Task<string?> GetRefreshTokenAsync();
        void SetRefreshToken(string refreshToken);
        Task SetRefreshTokenAsync(string refreshToken);
        void ClearRefreshToken();
        Task ClearRefreshTokenAsync();
        
        void ClearAllTokens();
        Task ClearAllTokensAsync();
    }
} 