using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System;
using AvaloniauiApp.Models;
using AvaloniauiApp.Services;
using System.Collections.ObjectModel;
using AvaloniauiApp.ViewModels;

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

        // 新增的屬性用於Tab功能
        [ObservableProperty]
        private int _selectedTabIndex = 0;

        [ObservableProperty]
        private string _feedbackMessage = "";

        // 新增：Tab集合
        public ObservableCollection<TabItemModel> Tabs { get; } = new ObservableCollection<TabItemModel>();

        public IRelayCommand RefreshTokenCommand { get; }
        public IRelayCommand EditProfileCommand { get; }
        public IRelayCommand ChangePasswordCommand { get; }
        public IRelayCommand SaveSettingsCommand { get; }
        public IRelayCommand SubmitFeedbackCommand { get; }

        public MainWindowViewModel(INavigationService navigationService, IAuthManager authManager, LoginViewModel loginViewModel, RegisterViewModel registerViewModel)
        {
            _navigationService = navigationService;
            _authManager = authManager;
            _loginViewModel = loginViewModel;
            _registerViewModel = registerViewModel;
            
            // 初始化Tabs
            Tabs.Add(new TabItemModel { Header = "首頁", View = new AvaloniauiApp.Views.HomeTabView() });
            Tabs.Add(new TabItemModel { Header = "個人資料", View = new AvaloniauiApp.Views.ProfileTabView() });
            Tabs.Add(new TabItemModel { Header = "設定", View = new AvaloniauiApp.Views.SettingsTabView() });
            Tabs.Add(new TabItemModel { Header = "幫助", View = new AvaloniauiApp.Views.HelpTabView() });
            
            // 訂閱登入成功事件
            _loginViewModel.LoginSuccess += OnLoginSuccess;
            
            // 訂閱註冊成功事件
            _registerViewModel.RegisterSuccess += OnRegisterSuccess;

            // 訂閱View之間的導航事件
            _loginViewModel.ShowRegister += OnShowRegister;
            _registerViewModel.ShowLogin += OnShowLogin;

            RefreshTokenCommand = new AsyncRelayCommand(RefreshTokenAsync);
            EditProfileCommand = new RelayCommand(EditProfile);
            ChangePasswordCommand = new RelayCommand(ChangePassword);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            SubmitFeedbackCommand = new RelayCommand(SubmitFeedback);
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

        // 新增的方法用於Tab功能
        private void EditProfile()
        {
            // 切換到個人資料Tab
            SelectedTabIndex = 1;
            // 這裡可以添加編輯個人資料的邏輯
        }

        private void ChangePassword()
        {
            // 這裡可以添加修改密碼的邏輯
            // 例如顯示修改密碼的對話框或切換到修改密碼頁面
        }

        private void SaveSettings()
        {
            // 這裡可以添加儲存設定的邏輯
            // 例如將設定保存到本地或發送到伺服器
        }

        private void SubmitFeedback()
        {
            // 這裡可以添加送出意見回饋的邏輯
            // 例如將回饋內容發送到伺服器
            FeedbackMessage = "感謝您的意見回饋！我們會盡快處理。";
        }
    }
}
