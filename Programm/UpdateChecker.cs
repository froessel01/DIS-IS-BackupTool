using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BackupTool
{
    public sealed class UpdateInfo
    {
        public string Version { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public static class UpdateChecker
    {
        private static readonly HttpClient Http = new()
        {
            Timeout = TimeSpan.FromSeconds(6)
        };

        public static async Task<UpdateInfo?> FetchAsync(string updateInfoUrl)
        {
            if (string.IsNullOrWhiteSpace(updateInfoUrl))
                return null;

            var json = await Http.GetStringAsync(updateInfoUrl).ConfigureAwait(false);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<UpdateInfo>(json, options);
        }

        public static bool IsNewer(string currentVersion, string availableVersion)
        {
            if (!Version.TryParse(currentVersion, out var current))
                return false;
            if (!Version.TryParse(availableVersion, out var available))
                return false;
            return available > current;
        }
    }
}

