namespace AvaloniauiApp.Services
{
    public class TokenService : ITokenService
    {
        private string? _token;
        private string? _refreshToken;

        public string? GetToken()
        {
            return _token;
        }

        public void SetToken(string token)
        {
            _token = token;
        }

        public void ClearToken()
        {
            _token = null;
        }

        public string? GetRefreshToken()
        {
            return _refreshToken;
        }

        public void SetRefreshToken(string refreshToken)
        {
            _refreshToken = refreshToken;
        }

        public void ClearRefreshToken()
        {
            _refreshToken = null;
        }

        public void ClearAllTokens()
        {
            _token = null;
            _refreshToken = null;
        }
    }
} 