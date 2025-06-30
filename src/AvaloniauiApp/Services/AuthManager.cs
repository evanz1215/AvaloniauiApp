using System;
using System.Threading.Tasks;
using AvaloniauiApp.Models;

namespace AvaloniauiApp.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IUserStorage _userStorage;
        private UserInfo? _currentUser;

        public AuthManager(IAuthService authService, ITokenService tokenService, IUserStorage userStorage)
        {
            _authService = authService;
            _tokenService = tokenService;
            _userStorage = userStorage;
        }

        public async Task<bool> TryAutoLoginAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("AuthManager: 開始嘗試自動登入");
                
                // 檢查是否有儲存的 token
                var token = await _tokenService.GetTokenAsync();
                var refreshToken = await _tokenService.GetRefreshTokenAsync();

                System.Diagnostics.Debug.WriteLine($"AuthManager: Token存在: {!string.IsNullOrEmpty(token)}");
                System.Diagnostics.Debug.WriteLine($"AuthManager: RefreshToken存在: {!string.IsNullOrEmpty(refreshToken)}");

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
                {
                    System.Diagnostics.Debug.WriteLine("AuthManager: Token或RefreshToken為空，自動登入失敗");
                    return false;
                }

                // 嘗試使用 refresh token 更新 token
                System.Diagnostics.Debug.WriteLine("AuthManager: 嘗試刷新Token");
                var success = await RefreshTokenAsync();
                System.Diagnostics.Debug.WriteLine($"AuthManager: Token刷新結果: {success}");
                
                if (success)
                {
                    // 如果刷新成功，從持久化儲存載入用戶資訊
                    var savedUser = await _userStorage.GetCurrentUserAsync();
                    System.Diagnostics.Debug.WriteLine($"AuthManager: 載入用戶資訊: {savedUser?.UserName}");
                    
                    if (savedUser != null)
                    {
                        _currentUser = savedUser;
                    }
                    System.Diagnostics.Debug.WriteLine("AuthManager: 自動登入成功");
                    return true;
                }

                // 如果 refresh token 也失效，清除所有儲存的資料
                System.Diagnostics.Debug.WriteLine("AuthManager: Token刷新失敗，清除所有資料");
                await LogoutAsync();
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AuthManager: 自動登入發生錯誤: {ex.Message}");
                await LogoutAsync();
                return false;
            }
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
                    await _userStorage.SetCurrentUserAsync(response.User);
                    return true;
                }
            }
            catch (Exception)
            {
                // Token 刷新失敗，清除所有 token
                await LogoutAsync();
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
            _ = _userStorage.ClearCurrentUserAsync();
        }

        public async Task LogoutAsync()
        {
            await _tokenService.ClearAllTokensAsync();
            _currentUser = null;
            await _userStorage.ClearCurrentUserAsync();
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
            _ = _userStorage.SetCurrentUserAsync(user);
        }

        public async Task SetCurrentUserAsync(UserInfo user)
        {
            _currentUser = user;
            await _userStorage.SetCurrentUserAsync(user);
        }
    }
} 