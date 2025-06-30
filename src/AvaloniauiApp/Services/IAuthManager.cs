using System.Threading.Tasks;
using AvaloniauiApp.Models;

namespace AvaloniauiApp.Services
{
    public interface IAuthManager
    {
        Task<bool> TryAutoLoginAsync();
        Task<bool> RefreshTokenAsync();
        bool IsTokenValid();
        void Logout();
        Task LogoutAsync();
        string? GetCurrentToken();
        UserInfo? GetCurrentUser();
        void SetCurrentUser(UserInfo user);
        Task SetCurrentUserAsync(UserInfo user);
    }
} 