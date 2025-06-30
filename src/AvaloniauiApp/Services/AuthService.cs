using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AvaloniauiApp.Models;

namespace AvaloniauiApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ITokenService _tokenService;

        public AuthService(HttpClient httpClient, ITokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/v1/Auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, _jsonOptions);
                    if (loginResponse != null)
                    {
                        _tokenService.SetToken(loginResponse.Token);
                        _tokenService.SetRefreshToken(loginResponse.RefreshToken);
                    }
                    return loginResponse;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<RegisterResult> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/v1/Auth/register", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var registerResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, _jsonOptions);
                    if (registerResponse != null)
                    {
                        _tokenService.SetToken(registerResponse.Token);
                        _tokenService.SetRefreshToken(registerResponse.RefreshToken);
                    }
                    return new RegisterResult { User = registerResponse?.User };
                }
                else
                {
                    // 嘗試解析API回傳的錯誤訊息
                    string? errorMsg = null;
                    try
                    {
                        using var doc = JsonDocument.Parse(responseContent);
                        if (doc.RootElement.TryGetProperty("message", out var msgProp))
                            errorMsg = msgProp.GetString();
                        else if (doc.RootElement.TryGetProperty("error", out var errProp))
                            errorMsg = errProp.GetString();
                        else
                            errorMsg = responseContent;
                    }
                    catch
                    {
                        errorMsg = responseContent;
                    }
                    return new RegisterResult { ErrorMessage = errorMsg };
                }
            }
            catch (Exception ex)
            {
                return new RegisterResult { ErrorMessage = ex.Message };
            }
        }

        public async Task<RefreshTokenResponse?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/v1/Auth/refresh-token", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var refreshResponse = JsonSerializer.Deserialize<RefreshTokenResponse>(responseContent, _jsonOptions);
                    if (refreshResponse != null)
                    {
                        _tokenService.SetToken(refreshResponse.Token);
                        _tokenService.SetRefreshToken(refreshResponse.RefreshToken);
                    }
                    return refreshResponse;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
} 