using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System;
using AvaloniauiApp.Models;
using AvaloniauiApp.Services;

namespace AvaloniauiApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthManager _authManager;

        [ObservableProperty]
        private string _greeting = "歡迎使用系統！";

        [ObservableProperty]
        private string _userInfo = "您已成功登入";

        [ObservableProperty]
        private string _refreshTokenStatus = "未測試";

        [ObservableProperty]
        private bool _showLoginView = true;

        [ObservableProperty]
        private bool _showMainView = false;

        [ObservableProperty]
        private bool _showRegisterView = false;

        [ObservableProperty]
        private LoginViewModel _loginViewModel;

        [ObservableProperty]
        private RegisterViewModel _registerViewModel;

        public IRelayCommand RefreshTokenCommand { get; }

        public MainWindowViewModel(INavigationService navigationService, IAuthManager authManager, LoginViewModel loginViewModel, RegisterViewModel registerViewModel)
        {
            _navigationService = navigationService;
            _authManager = authManager;
            _loginViewModel = loginViewModel;
            _registerViewModel = registerViewModel;
            
            // 訂閱登入成功事件
            _loginViewModel.LoginSuccess += OnLoginSuccess;
            
            // 訂閱註冊成功事件
            _registerViewModel.RegisterSuccess += OnRegisterSuccess;

            // 訂閱View之間的導航事件
            _loginViewModel.ShowRegister += OnShowRegister;
            _registerViewModel.ShowLogin += OnShowLogin;

            RefreshTokenCommand = new AsyncRelayCommand(RefreshTokenAsync);
        }

        private void OnLoginSuccess(UserInfo user)
        {
            SetUser(user);
            ShowLoginView = false;
            ShowRegisterView = false;
            ShowMainView = true;
        }

        private void OnRegisterSuccess(UserInfo user)
        {
            SetUser(user);
            ShowLoginView = false;
            ShowRegisterView = false;
            ShowMainView = true;
        }

        private void OnShowRegister()
        {
            ShowLoginView = false;
            ShowRegisterView = true;
        }

        private void OnShowLogin()
        {
            ShowRegisterView = false;
            ShowLoginView = true;
        }

        public void SetUser(UserInfo? user)
        {
            if (user != null)
            {
                Greeting = $"歡迎，{user.FirstName}{user.LastName} ({user.UserName})！";
                UserInfo = $"Email: {user.Email}";
            }
        }

        [RelayCommand]
        private void Logout()
        {
            // 清除 AuthManager 中的 token 和用戶資訊
            _ = _authManager.LogoutAsync();
            
            // 清空登入資訊
            Greeting = "歡迎使用系統！";
            UserInfo = "您已成功登出";
            LoginViewModel.Username = string.Empty;
            LoginViewModel.Password = string.Empty;
            LoginViewModel.ErrorMessage = string.Empty;
            ShowMainView = false;
            ShowRegisterView = false;
            ShowLoginView = true;
        }

        public async Task RefreshTokenAsync()
        {
            RefreshTokenStatus = "正在刷新Token...";
            
            try
            {
                var success = await _authManager.RefreshTokenAsync();
                
                if (success)
                {
                    RefreshTokenStatus = "Token刷新成功！";
                    var currentUser = _authManager.GetCurrentUser();
                    if (currentUser != null)
                    {
                        SetUser(currentUser);
                    }
                }
                else
                {
                    RefreshTokenStatus = "Token刷新失敗，請重新登入";
                }
            }
            catch (Exception ex)
            {
                RefreshTokenStatus = $"刷新Token時發生錯誤: {ex.Message}";
            }
        }
    }
}
