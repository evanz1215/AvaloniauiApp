using System.Threading.Tasks;
using AvaloniauiApp.Models;

namespace AvaloniauiApp.Services
{
    public interface IUserStorage
    {
        Task<UserInfo?> GetCurrentUserAsync();
        Task SetCurrentUserAsync(UserInfo user);
        Task ClearCurrentUserAsync();
    }
} 