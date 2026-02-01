using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace BackupTool
{
    public static class ConfigLoader
    {
        public static bool TryLoad(string configPath, out BackupConfig config, out string errorMessage)
        {
            config = new BackupConfig();
            errorMessage = string.Empty;

            try
            {
                var json = File.ReadAllText(configPath, Encoding.UTF8);
                return TryParse(json, out config, out errorMessage);
            }
            catch (JsonException ex)
            {
                errorMessage = $"Konfiguration konnte nicht geparst werden: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public static bool TryParse(string json, out BackupConfig config, out string errorMessage)
        {
            config = new BackupConfig();
            errorMessage = string.Empty;

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                config = JsonSerializer.Deserialize<BackupConfig>(json, options) ?? new BackupConfig();
                return true;
            }
            catch (JsonException ex)
            {
                errorMessage = $"Konfiguration konnte nicht geparst werden: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
