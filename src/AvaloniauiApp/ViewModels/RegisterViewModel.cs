using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AvaloniauiApp.Models;
using AvaloniauiApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniauiApp.ViewModels
{
    public partial class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IAuthManager _authManager;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "請輸入使用者名稱")]
        private string _username = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "請輸入Email")]
        [EmailAddress(ErrorMessage = "請輸入有效的Email格式")]
        private string _email = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "請輸入密碼")]
        [MinLength(6, ErrorMessage = "密碼至少需要6個字元")]
        private string _password = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "請確認密碼")]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "請輸入名字")]
        private string _firstName = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "請輸入姓氏")]
        private string _lastName = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isLoading = false;

        // 註冊成功事件
        public event Action<UserInfo>? RegisterSuccess;

        // 顯示登入事件
        public event Action? ShowLogin;

        public RegisterViewModel(IAuthService authService, IAuthManager authManager)
        {
            _authService = authService;
            _authManager = authManager;
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (HasErrors)
            {
                ErrorMessage = "請檢查輸入資料";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "密碼與確認密碼不符";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var request = new RegisterRequest
                {
                    Username = Username,
                    Email = Email,
                    Password = Password,
                    ConfirmPassword = ConfirmPassword,
                    FirstName = FirstName,
                    LastName = LastName
                };

                var result = await _authService.RegisterAsync(request);

                if (result.User != null)
                {
                    // 設定當前用戶到 AuthManager
                    _authManager.SetCurrentUser(result.User);
                    
                    // 觸發註冊成功事件
                    RegisterSuccess?.Invoke(result.User);
                }
                else
                {
                    ErrorMessage = string.IsNullOrWhiteSpace(result.ErrorMessage)
                        ? "註冊失敗，請檢查輸入資料。"
                        : result.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"註冊時發生錯誤: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void ShowLoginCommand()
        {
            ShowLogin?.Invoke();
        }

        [RelayCommand]
        private void ClearError()
        {
            ErrorMessage = string.Empty;
        }
    }
} 