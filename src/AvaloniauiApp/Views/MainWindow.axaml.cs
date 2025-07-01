using Avalonia.Controls;
using Avalonia.Input;
using AvaloniauiApp.ViewModels;

namespace AvaloniauiApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                // F1~F8 對應Tab
                if (e.Key >= Key.F1 && e.Key <= Key.F8)
                {
                    int idx = (int)e.Key - (int)Key.F1;
                    if (idx < vm.Tabs.Count)
                    {
                        vm.SelectedTabIndex = idx;
                        e.Handled = true;
                    }
                }
            }
        }
    }
}