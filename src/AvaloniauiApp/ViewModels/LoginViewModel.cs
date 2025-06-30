using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Diagnostics;
using Avalonia.Controls;
using AvaloniauiApp.Models;
using AvaloniauiApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniauiApp.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IAuthManager _authManager;
        private readonly ITokenService _tokenService;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "請輸入使用者名稱")]
        private string _username = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "請輸入密碼")]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private bool _keepLoggedIn = true;

        // 登入成功事件
        public event Action<UserInfo>? LoginSuccess;

        // 顯示註冊事件
        public event Action? ShowRegister;

        public LoginViewModel(IAuthService authService, IAuthManager authManager, ITokenService tokenService)
        {
            _authService = authService;
            _authManager = authManager;
            _tokenService = tokenService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (HasErrors)
            {
                ErrorMessage = "請檢查輸入資料";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                Debug.WriteLine($"LoginViewModel: 開始登入，KeepLoggedIn: {KeepLoggedIn}");
                
                var request = new LoginRequest
                {
                    Username = Username,
                    Password = Password
                };

                var response = await _authService.LoginAsync(request);

                if (response?.User != null)
                {
                    Debug.WriteLine($"LoginViewModel: 登入成功，用戶: {response.User.UserName}");
                    Debug.WriteLine($"LoginViewModel: Token存在: {!string.IsNullOrEmpty(response.Token)}");
                    Debug.WriteLine($"LoginViewModel: RefreshToken存在: {!string.IsNullOrEmpty(response.RefreshToken)}");
                    
                    // 設定當前用戶到 AuthManager
                    if (KeepLoggedIn)
                    {
                        Debug.WriteLine("LoginViewModel: 開始保存token到持久化儲存");
                        // 儲存 token 到持久化儲存
                        await _tokenService.SetTokenAsync(response.Token);
                        await _tokenService.SetRefreshTokenAsync(response.RefreshToken);
                        await _authManager.SetCurrentUserAsync(response.User);
                        Debug.WriteLine("LoginViewModel: Token保存完成");
                    }
                    else
                    {
                        Debug.WriteLine("LoginViewModel: 不保存token，只設定記憶體中的用戶");
                        // 如果不保持登入，只設定記憶體中的用戶，不儲存到持久化儲存
                        _authManager.SetCurrentUser(response.User);
                    }
                    
                    // 觸發登入成功事件
                    LoginSuccess?.Invoke(response.User);
                }
                else
                {
                    Debug.WriteLine("LoginViewModel: 登入失敗，API回傳null");
                    ErrorMessage = "登入失敗，請檢查帳號密碼。";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoginViewModel: 登入時發生錯誤: {ex.Message}");
                ErrorMessage = $"登入時發生錯誤: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void ShowRegisterCommand()
        {
            ShowRegister?.Invoke();
        }

        [RelayCommand]
        private void ClearError()
        {
            ErrorMessage = string.Empty;
        }
    }
} 