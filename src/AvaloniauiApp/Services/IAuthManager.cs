using System.Threading.Tasks;
using AvaloniauiApp.Models;

namespace AvaloniauiApp.Services
{
    public interface IAuthManager
    {
        Task<bool> RefreshTokenAsync();
        bool IsTokenValid();
        void Logout();
        string? GetCurrentToken();
        UserInfo? GetCurrentUser();
        void SetCurrentUser(UserInfo user);
    }
} 