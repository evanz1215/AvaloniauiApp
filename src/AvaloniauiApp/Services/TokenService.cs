using System.Threading.Tasks;
using System.Diagnostics;

namespace AvaloniauiApp.Services
{
    public class TokenService : ITokenService
    {
        private readonly ISecureStorage _secureStorage;
        private string? _token;
        private string? _refreshToken;

        public TokenService(ISecureStorage secureStorage)
        {
            _secureStorage = secureStorage;
        }

        public string? GetToken()
        {
            return _token;
        }

        public async Task<string?> GetTokenAsync()
        {
            if (_token == null)
            {
                _token = await _secureStorage.GetAsync("access_token");
            }
            return _token;
        }

        public void SetToken(string token)
        {
            _token = token;
            Debug.WriteLine($"TokenService: 設定Token到記憶體: {!string.IsNullOrEmpty(token)}");
            // 使用非同步方法但不等待，這可能導致問題
            _ = SetTokenAsync(token);
        }

        public async Task SetTokenAsync(string token)
        {
            _token = token;
            Debug.WriteLine($"TokenService: 開始保存Token到持久化儲存: {!string.IsNullOrEmpty(token)}");
            await _secureStorage.SetAsync("access_token", token);
            Debug.WriteLine("TokenService: Token保存完成");
        }

        public void ClearToken()
        {
            _token = null;
            _ = _secureStorage.RemoveAsync("access_token");
        }

        public async Task ClearTokenAsync()
        {
            _token = null;
            await _secureStorage.RemoveAsync("access_token");
        }

        public string? GetRefreshToken()
        {
            return _refreshToken;
        }

        public async Task<string?> GetRefreshTokenAsync()
        {
            if (_refreshToken == null)
            {
                _refreshToken = await _secureStorage.GetAsync("refresh_token");
            }
            return _refreshToken;
        }

        public void SetRefreshToken(string refreshToken)
        {
            _refreshToken = refreshToken;
            Debug.WriteLine($"TokenService: 設定RefreshToken到記憶體: {!string.IsNullOrEmpty(refreshToken)}");
            // 使用非同步方法但不等待，這可能導致問題
            _ = SetRefreshTokenAsync(refreshToken);
        }

        public async Task SetRefreshTokenAsync(string refreshToken)
        {
            _refreshToken = refreshToken;
            Debug.WriteLine($"TokenService: 開始保存RefreshToken到持久化儲存: {!string.IsNullOrEmpty(refreshToken)}");
            await _secureStorage.SetAsync("refresh_token", refreshToken);
            Debug.WriteLine("TokenService: RefreshToken保存完成");
        }

        public void ClearRefreshToken()
        {
            _refreshToken = null;
            _ = _secureStorage.RemoveAsync("refresh_token");
        }

        public async Task ClearRefreshTokenAsync()
        {
            _refreshToken = null;
            await _secureStorage.RemoveAsync("refresh_token");
        }

        public void ClearAllTokens()
        {
            _token = null;
            _refreshToken = null;
            _ = _secureStorage.ClearAsync();
        }

        public async Task ClearAllTokensAsync()
        {
            _token = null;
            _refreshToken = null;
            await _secureStorage.ClearAsync();
        }
    }
} 