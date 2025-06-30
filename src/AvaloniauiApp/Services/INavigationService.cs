using System.Threading.Tasks;

namespace AvaloniauiApp.Services
{
    public interface INavigationService
    {
        Task NavigateToMainAsync();
        Task NavigateToLoginAsync();
    }
} 