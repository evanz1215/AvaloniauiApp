using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AvaloniauiApp.Models;
using AvaloniauiApp.Services;

namespace AvaloniauiApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _greeting = "歡迎使用系統！";

        [ObservableProperty]
        private string _userInfo = "您已成功登入";

        [ObservableProperty]
        private bool _showLoginView = true;

        [ObservableProperty]
        private bool _showMainView = false;

        [ObservableProperty]
        private LoginViewModel _loginViewModel;

        public MainWindowViewModel(INavigationService navigationService, LoginViewModel loginViewModel)
        {
            _navigationService = navigationService;
            _loginViewModel = loginViewModel;
            
            // 訂閱登入成功事件
            _loginViewModel.LoginSuccess += OnLoginSuccess;
        }

        private void OnLoginSuccess(UserInfo user)
        {
            SetUser(user);
            ShowLoginView = false;
            ShowMainView = true;
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
            // 清空登入資訊
            Greeting = "歡迎使用系統！";
            UserInfo = "您已成功登出";
            LoginViewModel.Username = string.Empty;
            LoginViewModel.Password = string.Empty;
            LoginViewModel.ErrorMessage = string.Empty;
            ShowMainView = false;
            ShowLoginView = true;
        }
    }
}
