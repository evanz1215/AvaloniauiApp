using System;

namespace AvaloniauiApp.Models
{
    public class RefreshTokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserInfo User { get; set; } = new UserInfo();
    }
} 