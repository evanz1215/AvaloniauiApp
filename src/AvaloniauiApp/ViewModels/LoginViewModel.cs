using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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

        // 登入成功事件
        public event Action<UserInfo>? LoginSuccess;

        // 顯示註冊事件
        public event Action? ShowRegister;

        public LoginViewModel(IAuthService authService, IAuthManager authManager)
        {
            _authService = authService;
            _authManager = authManager;
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
                var request = new LoginRequest
                {
                    Username = Username,
                    Password = Password
                };

                var response = await _authService.LoginAsync(request);

                if (response?.User != null)
                {
                    // 設定當前用戶到 AuthManager
                    _authManager.SetCurrentUser(response.User);
                    
                    // 觸發登入成功事件
                    LoginSuccess?.Invoke(response.User);
                }
                else
                {
                    ErrorMessage = "登入失敗，請檢查帳號密碼。";
                }
            }
            catch (Exception ex)
            {
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