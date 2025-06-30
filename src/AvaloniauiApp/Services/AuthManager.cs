using System;
using System.Threading.Tasks;
using AvaloniauiApp.Models;

namespace AvaloniauiApp.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private UserInfo? _currentUser;

        public AuthManager(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            var currentToken = _tokenService.GetToken();
            var refreshToken = _tokenService.GetRefreshToken();

            if (string.IsNullOrEmpty(currentToken) || string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }

            try
            {
                var request = new RefreshTokenRequest
                {
                    Token = currentToken,
                    RefreshToken = refreshToken
                };

                var response = await _authService.RefreshTokenAsync(request);
                
                if (response != null)
                {
                    _currentUser = response.User;
                    return true;
                }
            }
            catch (Exception)
            {
                // Token 刷新失敗，清除所有 token
                Logout();
            }

            return false;
        }

        public bool IsTokenValid()
        {
            var token = _tokenService.GetToken();
            return !string.IsNullOrEmpty(token);
        }

        public void Logout()
        {
            _tokenService.ClearAllTokens();
            _currentUser = null;
        }

        public string? GetCurrentToken()
        {
            return _tokenService.GetToken();
        }

        public UserInfo? GetCurrentUser()
        {
            return _currentUser;
        }

        public void SetCurrentUser(UserInfo user)
        {
            _currentUser = user;
        }
    }
} 