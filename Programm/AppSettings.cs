using System;
using System.IO;
using System.Text.Json;

namespace BackupTool
{
    public sealed class AppSettings
    {
        public bool AutoUpdateEnabled { get; set; } = true;
    }

    public static class AppSettingsStore
    {
        public static AppSettings Load(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return new AppSettings();

                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public static void Save(string path, AppSettings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(path, json);
            }
            catch
            {
                // Ignore settings write errors.
            }
        }
    }
}

