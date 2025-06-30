using System.Threading.Tasks;
using System.Text.Json;
using AvaloniauiApp.Models;

namespace AvaloniauiApp.Services
{
    public class UserStorage : IUserStorage
    {
        private readonly ISecureStorage _secureStorage;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserStorage(ISecureStorage secureStorage)
        {
            _secureStorage = secureStorage;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<UserInfo?> GetCurrentUserAsync()
        {
            try
            {
                var userJson = await _secureStorage.GetAsync("current_user");
                if (string.IsNullOrEmpty(userJson))
                    return null;

                return JsonSerializer.Deserialize<UserInfo>(userJson, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task SetCurrentUserAsync(UserInfo user)
        {
            try
            {
                var userJson = JsonSerializer.Serialize(user, _jsonOptions);
                await _secureStorage.SetAsync("current_user", userJson);
            }
            catch
            {
                // 忽略儲存錯誤
            }
        }

        public async Task ClearCurrentUserAsync()
        {
            try
            {
                await _secureStorage.RemoveAsync("current_user");
            }
            catch
            {
                // 忽略清除錯誤
            }
        }
    }
} 