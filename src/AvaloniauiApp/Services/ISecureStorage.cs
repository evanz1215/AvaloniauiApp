using System.Threading.Tasks;

namespace AvaloniauiApp.Services
{
    public interface ISecureStorage
    {
        Task<string?> GetAsync(string key);
        Task SetAsync(string key, string value);
        Task RemoveAsync(string key);
        Task ClearAsync();
    }
} 