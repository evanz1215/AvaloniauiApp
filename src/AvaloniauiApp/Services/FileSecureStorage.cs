using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace AvaloniauiApp.Services
{
    public class FileSecureStorage : ISecureStorage
    {
        private readonly string _storagePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public FileSecureStorage()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AvaloniauiApp"
            );
            
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            
            _storagePath = Path.Combine(appDataPath, "secure_storage.json");
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            
            Debug.WriteLine($"FileSecureStorage: 儲存路徑: {_storagePath}");
        }

        public async Task<string?> GetAsync(string key)
        {
            try
            {
                Debug.WriteLine($"FileSecureStorage: 嘗試讀取key: {key}");
                
                if (!File.Exists(_storagePath))
                {
                    Debug.WriteLine("FileSecureStorage: 儲存檔案不存在");
                    return null;
                }

                var json = await File.ReadAllTextAsync(_storagePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json, _jsonOptions);
                
                var value = data?.GetValueOrDefault(key);
                Debug.WriteLine($"FileSecureStorage: 讀取key '{key}' 結果: {!string.IsNullOrEmpty(value)}");
                
                return value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FileSecureStorage: 讀取key '{key}' 時發生錯誤: {ex.Message}");
                return null;
            }
        }

        public async Task SetAsync(string key, string value)
        {
            try
            {
                Debug.WriteLine($"FileSecureStorage: 開始保存key: {key}, value存在: {!string.IsNullOrEmpty(value)}");
                
                Dictionary<string, string> data;
                
                if (File.Exists(_storagePath))
                {
                    var json = await File.ReadAllTextAsync(_storagePath);
                    data = JsonSerializer.Deserialize<Dictionary<string, string>>(json, _jsonOptions) ?? new Dictionary<string, string>();
                }
                else
                {
                    data = new Dictionary<string, string>();
                }

                data[key] = value;
                var updatedJson = JsonSerializer.Serialize(data, _jsonOptions);
                await File.WriteAllTextAsync(_storagePath, updatedJson);
                
                Debug.WriteLine($"FileSecureStorage: 成功保存key: {key}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FileSecureStorage: 保存key '{key}' 時發生錯誤: {ex.Message}");
                // 忽略儲存錯誤
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                Debug.WriteLine($"FileSecureStorage: 嘗試移除key: {key}");
                
                if (!File.Exists(_storagePath))
                    return;

                var json = await File.ReadAllTextAsync(_storagePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json, _jsonOptions);
                
                if (data != null && data.ContainsKey(key))
                {
                    data.Remove(key);
                    var updatedJson = JsonSerializer.Serialize(data, _jsonOptions);
                    await File.WriteAllTextAsync(_storagePath, updatedJson);
                    Debug.WriteLine($"FileSecureStorage: 成功移除key: {key}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FileSecureStorage: 移除key '{key}' 時發生錯誤: {ex.Message}");
                // 忽略移除錯誤
            }
        }

        public Task ClearAsync()
        {
            try
            {
                Debug.WriteLine("FileSecureStorage: 嘗試清除所有資料");
                
                if (File.Exists(_storagePath))
                {
                    File.Delete(_storagePath);
                    Debug.WriteLine("FileSecureStorage: 成功清除所有資料");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FileSecureStorage: 清除資料時發生錯誤: {ex.Message}");
                // 忽略清除錯誤
            }
            return Task.CompletedTask;
        }
    }
} 