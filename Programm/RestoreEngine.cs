using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace BackupTool
{
    public sealed class RestoreRunResult
    {
        public bool Aborted { get; set; }
        public int SuccessCount { get; set; }
        public int TotalPrograms { get; set; }
        public string? BackupZipPath { get; set; }
        public string? PreBackupPath { get; set; }
        public string? PreBackupZipPath { get; set; }
        public List<string> FailedPrograms { get; set; } = new();
    }

    public static class RestoreEngine
    {
        public static RestoreRunResult Run(
            BackupConfig config,
            IReadOnlyList<RestoreSelection> selections,
            string backupZipPath,
            bool createPreBackup,
            Action<string, string> log,
            Action<int> setProgressMax,
            Action incrementProgress)
        {
            var result = new RestoreRunResult
            {
                BackupZipPath = backupZipPath,
                TotalPrograms = selections.Count
            };

            if (!File.Exists(backupZipPath))
            {
                log("FEHLER: Backup-ZIP nicht gefunden.", "ERROR");
                result.Aborted = true;
                return result;
            }

            string preBackupPath = string.Empty;
            if (createPreBackup)
            {
                var root = Environment.ExpandEnvironmentVariables(config.BackupRootPath ?? string.Empty);
                if (string.IsNullOrWhiteSpace(root))
                {
                    log("FEHLER: BackupRootPath ist leer (Pre-Backup).", "ERROR");
                    result.Aborted = true;
                    return result;
                }

                var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                preBackupPath = Path.Combine(root, $"Backuptool_restore_before_{timestamp}");
                try
                {
                    Directory.CreateDirectory(preBackupPath);
                }
                catch (Exception ex)
                {
                    log($"FEHLER: Pre-Backup Ordner konnte nicht erstellt werden: {ex.Message}", "ERROR");
                    result.Aborted = true;
                    return result;
                }

                result.PreBackupPath = preBackupPath;
                WriteBackupConfig(preBackupPath, config, log);
            }

            setProgressMax(selections.Count);

            var successCount = 0;
            var failed = new List<string>();

            using var archive = ZipFile.OpenRead(backupZipPath);

            foreach (var selection in selections)
            {
                var program = selection.Program;
                var programName = program.Name ?? selection.FolderName;
                log($"Restore: {programName}", "INFO");

                var targetPath = ExpandPath(program.Path);
                if (string.IsNullOrWhiteSpace(targetPath))
                {
                    log($"  FEHLER: Zielpfad fehlt für {programName}.", "ERROR");
                    failed.Add(programName);
                    incrementProgress();
                    continue;
                }

                if (createPreBackup && (Directory.Exists(targetPath) || File.Exists(targetPath)))
                {
                    var preDest = Path.Combine(preBackupPath, SanitizeName(programName));
                    if (!BackupCurrentData(program, targetPath, preDest, log))
                    {
                        log($"  FEHLER: Pre-Backup fehlgeschlagen für {programName}.", "ERROR");
                        failed.Add(programName);
                        incrementProgress();
                        continue;
                    }
                }

                if (!RestoreProgramFromZip(archive, selection.FolderName, targetPath, log))
                {
                    failed.Add(programName);
                    incrementProgress();
                    continue;
                }

                successCount++;
                incrementProgress();
            }

            result.SuccessCount = successCount;
            result.FailedPrograms = failed;
            result.Aborted = failed.Count > 0 && successCount == 0;

            if (createPreBackup && !string.IsNullOrWhiteSpace(preBackupPath))
            {
                var preBackupZip = preBackupPath + ".zip";
                if (TryCompress(preBackupPath, preBackupZip, log))
                    result.PreBackupZipPath = preBackupZip;
            }

            return result;
        }

        private static bool RestoreProgramFromZip(ZipArchive archive, string folderName, string targetPath, Action<string, string> log)
        {
            var prefix = folderName.TrimEnd('/') + "/";
            var entries = new List<ZipArchiveEntry>();

            foreach (var entry in archive.Entries)
            {
                if (entry.FullName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    entries.Add(entry);
            }

            if (entries.Count == 0)
            {
                log($"  FEHLER: Keine Daten im Backup für '{folderName}'.", "ERROR");
                return false;
            }

            Directory.CreateDirectory(targetPath);

            try
            {
                foreach (var entry in entries)
                {
                    var relative = entry.FullName.Substring(prefix.Length);
                    if (string.IsNullOrWhiteSpace(relative))
                        continue;

                    var normalized = relative.Replace('/', Path.DirectorySeparatorChar);
                    var destination = Path.Combine(targetPath, normalized);

                    if (string.IsNullOrWhiteSpace(entry.Name))
                    {
                        Directory.CreateDirectory(destination);
                        continue;
                    }

                    var destDir = Path.GetDirectoryName(destination);
                    if (!string.IsNullOrWhiteSpace(destDir))
                        Directory.CreateDirectory(destDir);

                    entry.ExtractToFile(destination, true);
                }
            }
            catch (Exception ex)
            {
                log($"  FEHLER: Restore fehlgeschlagen: {ex.Message}", "ERROR");
                return false;
            }

            log("  OK: Restore abgeschlossen.", "SUCCESS");
            return true;
        }

        private static bool BackupCurrentData(ProgramEntry program, string sourcePath, string destinationPath, Action<string, string> log)
        {
            try
            {
                var isSelective = string.Equals(program.Type, "Selective", StringComparison.OrdinalIgnoreCase)
                                  && program.Items != null
                                  && program.Items.Count > 0;

                if (isSelective)
                {
                    return BackupSelective(sourcePath, destinationPath, program.Items!, log);
                }

                CopyDirectoryContents(sourcePath, destinationPath);
                log("  Pre-Backup erstellt.", "SUCCESS");
                return true;
            }
            catch (Exception ex)
            {
                log($"  FEHLER: Pre-Backup fehlgeschlagen: {ex.Message}", "ERROR");
                return false;
            }
        }

        private static void WriteBackupConfig(string backupPath, BackupConfig config, Action<string, string> log)
        {
            try
            {
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                var configPath = Path.Combine(backupPath, "config.json");
                File.WriteAllText(configPath, json, Encoding.UTF8);
                log("config.json im Pre-Backup gespeichert.", "INFO");
            }
            catch (Exception ex)
            {
                log($"Warnung: config.json konnte nicht im Pre-Backup gespeichert werden: {ex.Message}", "WARNING");
            }
        }

        private static bool TryCompress(string sourcePath, string zipPath, Action<string, string> log)
        {
            try
            {
                if (File.Exists(zipPath))
                    File.Delete(zipPath);
                ZipFile.CreateFromDirectory(sourcePath, zipPath, CompressionLevel.Optimal, false);
                log($"Pre-Backup ZIP erstellt: {zipPath}", "SUCCESS");
                return true;
            }
            catch (Exception ex)
            {
                log($"Warnung: Pre-Backup ZIP konnte nicht erstellt werden: {ex.Message}", "WARNING");
                return false;
            }
        }

        private static bool BackupSelective(string sourceBasePath, string destinationPath, List<string> items, Action<string, string> log)
        {
            Directory.CreateDirectory(destinationPath);

            var copiedItems = 0;
            foreach (var item in items)
            {
                try
                {
                    if (ContainsWildcard(item))
                    {
                        var matches = FindWildcardMatches(sourceBasePath, item);
                        foreach (var found in matches)
                        {
                            var relativePath = Path.GetRelativePath(sourceBasePath, found);
                            var destPath = Path.Combine(destinationPath, relativePath);
                            CopyItem(found, destPath);
                        }
                        if (matches.Count > 0)
                            copiedItems++;
                    }
                    else
                    {
                        var sourcePath = Path.Combine(sourceBasePath, item);
                        if (File.Exists(sourcePath) || Directory.Exists(sourcePath))
                        {
                            var destPath = Path.Combine(destinationPath, item);
                            CopyItem(sourcePath, destPath);
                            copiedItems++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log($"  Warnung: Pre-Backup Item '{item}' fehlgeschlagen: {ex.Message}", "WARNING");
                }
            }

            if (copiedItems > 0)
            {
                log("  Pre-Backup erstellt.", "SUCCESS");
                return true;
            }

            log("  Warnung: Pre-Backup hat keine Dateien gesichert.", "WARNING");
            return true;
        }

        private static bool ContainsWildcard(string value)
        {
            return value.Contains('*') || value.Contains('?');
        }

        private static string ExpandPath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;
            return Environment.ExpandEnvironmentVariables(path.Trim());
        }

        private static string SanitizeName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var builder = new StringBuilder(name.Length);
            foreach (var ch in name)
            {
                builder.Append(Array.IndexOf(invalidChars, ch) >= 0 ? '-' : ch);
            }
            return builder.ToString();
        }

        private static void CopyItem(string sourcePath, string destinationPath)
        {
            if (File.Exists(sourcePath))
            {
                var destDir = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrWhiteSpace(destDir))
                    Directory.CreateDirectory(destDir);
                File.Copy(sourcePath, destinationPath, true);
                return;
            }

            if (Directory.Exists(sourcePath))
            {
                CopyDirectoryContents(sourcePath, destinationPath);
            }
        }

        private static void CopyDirectoryContents(string sourcePath, string destinationPath)
        {
            Directory.CreateDirectory(destinationPath);

            foreach (var dir in Directory.EnumerateDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(sourcePath, dir);
                Directory.CreateDirectory(Path.Combine(destinationPath, relative));
            }

            foreach (var file in Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(sourcePath, file);
                var dest = Path.Combine(destinationPath, relative);
                var destDir = Path.GetDirectoryName(dest);
                if (!string.IsNullOrWhiteSpace(destDir))
                    Directory.CreateDirectory(destDir);
                File.Copy(file, dest, true);
            }
        }

        private static List<string> FindWildcardMatches(string basePath, string pattern)
        {
            var matches = new List<string>();
            var normalizedPattern = pattern.Replace('/', '\\');
            var regex = WildcardToRegex(normalizedPattern);

            foreach (var entry in Directory.EnumerateFileSystemEntries(basePath, "*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(basePath, entry).Replace('/', '\\');
                if (regex.IsMatch(relative))
                    matches.Add(entry);
            }

            return matches;
        }

        private static System.Text.RegularExpressions.Regex WildcardToRegex(string pattern)
        {
            var escaped = System.Text.RegularExpressions.Regex.Escape(pattern)
                .Replace(@"\*", "[^\\\\/]*")
                .Replace(@"\?", "[^\\\\/]");
            return new System.Text.RegularExpressions.Regex("^" + escaped + "$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
    }

    public sealed class RestoreSelection
    {
        public string FolderName { get; }
        public ProgramEntry Program { get; }

        public RestoreSelection(string folderName, ProgramEntry program)
        {
            FolderName = folderName;
            Program = program;
        }
    }
}
