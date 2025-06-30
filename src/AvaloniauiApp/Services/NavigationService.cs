using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniauiApp.ViewModels;
using AvaloniauiApp.Views;

namespace AvaloniauiApp.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IClassicDesktopStyleApplicationLifetime _applicationLifetime;
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IClassicDesktopStyleApplicationLifetime applicationLifetime, IServiceProvider serviceProvider)
        {
            _applicationLifetime = applicationLifetime;
            _serviceProvider = serviceProvider;
        }

        public Task NavigateToMainAsync()
        {
            var mainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetService(typeof(MainWindowViewModel))
            };

            _applicationLifetime.MainWindow = mainWindow;
            mainWindow.Show();
            return Task.CompletedTask;
        }

        public Task NavigateToLoginAsync()
        {
            // 由於現在使用單一視窗架構，這個方法不再需要
            // 登入畫面會在主視窗中顯示
            return Task.CompletedTask;
        }
    }
} 