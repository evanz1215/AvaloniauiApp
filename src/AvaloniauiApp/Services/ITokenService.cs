namespace AvaloniauiApp.Services
{
    public interface ITokenService
    {
        string? GetToken();
        void SetToken(string token);
        void ClearToken();
        
        string? GetRefreshToken();
        void SetRefreshToken(string refreshToken);
        void ClearRefreshToken();
        
        void ClearAllTokens();
    }
} 