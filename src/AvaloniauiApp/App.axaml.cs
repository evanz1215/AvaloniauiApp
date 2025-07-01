using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AvaloniauiApp.Services;
using AvaloniauiApp.ViewModels;
using AvaloniauiApp.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniauiApp
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;
        private string? _apiBaseUrl;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // 避免 Avalonia 和 CommunityToolkit 的重複驗證
                // 更多資訊: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();

                // 讀取API baseUrl
                _apiBaseUrl = LoadApiBaseUrl();

                // 設定依賴注入
                var services = new ServiceCollection();
                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();

                // 嘗試自動登入
                var authManager = _serviceProvider.GetRequiredService<IAuthManager>();
                var mainWindowViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
                
                // 在背景執行自動登入
                _ = Task.Run(async () =>
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("開始嘗試自動登入...");
                        var autoLoginSuccess = await authManager.TryAutoLoginAsync();
                        System.Diagnostics.Debug.WriteLine($"自動登入結果: {autoLoginSuccess}");
                        
                        if (autoLoginSuccess)
                        {
                            var currentUser = authManager.GetCurrentUser();
                            System.Diagnostics.Debug.WriteLine($"當前用戶: {currentUser?.UserName}");
                            
                            if (currentUser != null)
                            {
                                // 在主執行緒更新 UI
                                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    System.Diagnostics.Debug.WriteLine("更新UI到主畫面...");
                                    mainWindowViewModel.SetUser(currentUser);
                                    mainWindowViewModel.ShowLoginView = false;
                                    mainWindowViewModel.ShowRegisterView = false;
                                    mainWindowViewModel.ShowMainView = true;
                                    System.Diagnostics.Debug.WriteLine("UI更新完成");
                                });
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("自動登入失敗，保持登入畫面");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"自動登入發生錯誤: {ex.Message}");
                        // 自動登入失敗，保持登入畫面
                    }
                });

                // 啟動主視窗
                var mainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel
                };

                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // 註冊 HTTP 客戶端，帶入 baseUrl
            services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                if (!string.IsNullOrEmpty(_apiBaseUrl))
                {
                    client.BaseAddress = new System.Uri(_apiBaseUrl);
                }
            });

            // 註冊服務
            services.AddSingleton<ISecureStorage, FileSecureStorage>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IUserStorage, UserStorage>();
            services.AddSingleton<IAuthManager, AuthManager>();

            services.AddSingleton<INavigationService>(provider =>
            {
                var lifetime = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
                return new NavigationService(lifetime, provider);
            });

            // 註冊 ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<MainWindowViewModel>();
        }

        private string? LoadApiBaseUrl()
        {
            // 優先讀取環境變數
            var env = System.Environment.GetEnvironmentVariable("API_BASE_URL");
            if (!string.IsNullOrWhiteSpace(env))
                return env;

            // 讀取appsettings.json
            var configPath = Path.Combine(System.AppContext.BaseDirectory, "appsettings.json");
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("Api", out var apiElem) &&
                    apiElem.TryGetProperty("BaseUrl", out var urlElem))
                {
                    return urlElem.GetString();
                }
            }
            return null;
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // 取得要移除的插件陣列
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // 移除每個找到的項目
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}