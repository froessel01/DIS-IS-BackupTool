using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BackupTool
{
    public sealed class BackupRunResult
    {
        public bool Aborted { get; set; }
        public string? BackupRootPath { get; set; }
        public string? BackupPath { get; set; }
        public string? ZipPath { get; set; }
        public int SuccessCount { get; set; }
        public int TotalPrograms { get; set; }
        public List<string> FailedPrograms { get; set; } = new();
    }

    public static class BackupEngine
    {
        public static BackupRunResult Run(
            BackupConfig config,
            IReadOnlyList<ProgramEntry> programs,
            Action<string, string> log,
            Action<int> setProgressMax,
            Action incrementProgress)
        {
            var result = new BackupRunResult
            {
                TotalPrograms = programs.Count
            };

            var backupRootPath = Environment.ExpandEnvironmentVariables(config.BackupRootPath ?? string.Empty);
            if (string.IsNullOrWhiteSpace(backupRootPath))
            {
                log("FEHLER: BackupRootPath ist leer.", "ERROR");
                result.Aborted = true;
                return result;
            }

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var backupFolderName = $"Backuptool_by_froessel_{timestamp}";
            var backupPath = Path.Combine(backupRootPath, backupFolderName);
            var zipPath = backupPath + ".zip";

            result.BackupRootPath = backupRootPath;
            result.BackupPath = backupPath;
            result.ZipPath = zipPath;

            if (!EnsureDirectory(backupRootPath, "Backup-Ordner", log))
            {
                result.Aborted = true;
                return result;
            }

            if (!EnsureDirectory(backupPath, "Temporaerer Ordner", log))
            {
                result.Aborted = true;
                return result;
            }

            log("Backup gestartet.", "INFO");
            log($"Ziel: {backupPath}", "INFO");

            var successCount = 0;
            var failedPrograms = new List<string>();

            setProgressMax(programs.Count);
            log($"Starte Sicherung von {programs.Count} Programmen.", "INFO");

            foreach (var program in programs)
            {
                var programName = program.Name ?? "(Unbekannt)";
                string? foundPath = null;

                log($"Verarbeite: {programName}", "INFO");

                var expandedPath = ExpandPath(program.Path);
                if (!string.IsNullOrWhiteSpace(expandedPath) && Directory.Exists(expandedPath))
                {
                    foundPath = expandedPath;
                    log($"  Pfad ok: {foundPath}", "INFO");
                }
                else if (program.AlternatePaths != null && program.AlternatePaths.Count > 0)
                {
                    log("  Hauptpfad fehlt, pruefe Alternativen...", "WARNING");
                    foreach (var altPath in program.AlternatePaths)
                    {
                        var expandedAltPath = ExpandPath(altPath);
                        if (!string.IsNullOrWhiteSpace(expandedAltPath) && Directory.Exists(expandedAltPath))
                        {
                            foundPath = expandedAltPath;
                        log($"  Alternativ ok: {foundPath}", "INFO");
                            break;
                        }
                    }
                }

                if (foundPath == null)
                {
                    log($"  {programName} nicht gefunden.", "ERROR");
                    if (!string.IsNullOrWhiteSpace(program.Path))
                        log($"    Gesuchter Pfad: {program.Path}", "ERROR");
                    if (program.AlternatePaths != null && program.AlternatePaths.Count > 0)
                        log($"    Alternative Pfade: {string.Join(", ", program.AlternatePaths)}", "ERROR");
                    failedPrograms.Add(programName);
                    incrementProgress();
                    CleanupTempBackup(backupPath);
                    result.Aborted = true;
                    result.SuccessCount = successCount;
                    result.FailedPrograms = failedPrograms;
                    return result;
                }

                try
                {
                    var destName = SanitizeName(programName);
                    var isSelective = string.Equals(program.Type, "Selective", StringComparison.OrdinalIgnoreCase)
                                      && program.Items != null
                                      && program.Items.Count > 0;

                    if (isSelective)
                    {
                    log($"  Selektiv ({program.Items!.Count} Items)", "INFO");
                        if (BackupSelective(foundPath, destName, programName, program.Items!, backupPath, log))
                        {
                            successCount++;
                        }
                        else
                        {
                            failedPrograms.Add(programName);
                            incrementProgress();
                            CleanupTempBackup(backupPath);
                            result.Aborted = true;
                            result.SuccessCount = successCount;
                            result.FailedPrograms = failedPrograms;
                            return result;
                        }
                    }
                    else
                    {
                    log("  Vollstaendig", "INFO");
                        if (BackupFolder(foundPath, destName, programName, backupPath, log))
                        {
                            successCount++;
                        }
                        else
                        {
                            failedPrograms.Add(programName);
                            incrementProgress();
                            CleanupTempBackup(backupPath);
                            result.Aborted = true;
                            result.SuccessCount = successCount;
                            result.FailedPrograms = failedPrograms;
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log($"  [FEHLER] Unerwarteter Fehler bei {programName}: {ex.Message}", "ERROR");
                    failedPrograms.Add(programName);
                    incrementProgress();
                    CleanupTempBackup(backupPath);
                    result.Aborted = true;
                    result.SuccessCount = successCount;
                    result.FailedPrograms = failedPrograms;
                    return result;
                }

                incrementProgress();
            }

            if (!CompressBackup(backupPath, zipPath, log))
            {
                CleanupTempBackup(backupPath);
                if (File.Exists(zipPath))
                {
                    try
                    {
                        File.Delete(zipPath);
                    }
                    catch
                    {
                        // Ignore cleanup errors.
                    }
                }

                result.Aborted = true;
                result.SuccessCount = successCount;
                result.FailedPrograms = failedPrograms;
                return result;
            }

            result.Aborted = false;
            result.SuccessCount = successCount;
            result.FailedPrograms = failedPrograms;
            return result;
        }

        private static bool EnsureDirectory(string path, string name, Action<string, string> log)
        {
            try
            {
                Directory.CreateDirectory(path);
                log($"{name} erstellt: {path}", "SUCCESS");
                return true;
            }
            catch (Exception ex)
            {
                log($"FEHLER: {name} konnte nicht erstellt werden: {ex.Message}", "ERROR");
                log($"Pfad: {path}", "ERROR");
                return false;
            }
        }

        private static bool BackupSelective(string sourceBasePath, string destinationName, string programName, List<string> items, string backupPath, Action<string, string> log)
        {
            if (!Directory.Exists(sourceBasePath))
            {
                log($"  [FEHLT] {programName} nicht gefunden: {sourceBasePath}", "ERROR");
                return false;
            }

            var destination = Path.Combine(backupPath, destinationName);
            Directory.CreateDirectory(destination);
            log($"Sichere {programName}...", "INFO");

            var copiedItems = 0;
            var failedItems = new List<string>();
            long totalSize = 0;

            foreach (var item in items)
            {
                try
                {
                    if (ContainsWildcard(item))
                    {
                        var matches = FindWildcardMatches(sourceBasePath, item);
                        if (matches.Count > 0)
                        {
                            foreach (var foundItem in matches)
                            {
                                var relativePath = Path.GetRelativePath(sourceBasePath, foundItem);
                                var destPath = Path.Combine(destination, relativePath);
                                CopyItem(foundItem, destPath);
                                totalSize += CalculateSize(destPath);
                            }

                            copiedItems++;
                            log($"    OK: '{item}' ({matches.Count} Treffer)", "SUCCESS");
                        }
                        else
                        {
                            log($"    Warnung: Keine Treffer fuer '{item}'", "WARNING");
                            failedItems.Add(item);
                        }
                    }
                    else
                    {
                        var sourcePath = Path.Combine(sourceBasePath, item);
                        if (File.Exists(sourcePath) || Directory.Exists(sourcePath))
                        {
                            var destPath = Path.Combine(destination, item);
                            CopyItem(sourcePath, destPath);
                            copiedItems++;
                            totalSize += CalculateSize(destPath);
                            log($"    OK: '{item}'", "SUCCESS");
                        }
                        else
                        {
                            log($"    Warnung: Item fehlt: {item}", "WARNING");
                            failedItems.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log($"    Warnung: Fehler bei '{item}': {ex.Message}", "WARNING");
                    failedItems.Add(item);
                }
            }

            if (copiedItems > 0)
            {
                var sizeMb = Math.Round(totalSize / 1024d / 1024d, 2);
                log($"  OK: {programName} ({copiedItems}/{items.Count} Items, {sizeMb} MB)", "SUCCESS");
                if (failedItems.Count > 0)
                    log($"    Fehlgeschlagen: {string.Join(", ", failedItems)}", "WARNING");
                return true;
            }

            log("  Fehler: Keine Dateien gesichert", "ERROR");
            if (failedItems.Count > 0)
                log($"    Fehlgeschlagen: {string.Join(", ", failedItems)}", "ERROR");
            return false;
        }

        private static bool BackupFolder(string sourcePath, string destinationName, string programName, string backupPath, Action<string, string> log)
        {
            if (!Directory.Exists(sourcePath))
            {
                log($"  [FEHLT] {programName} nicht gefunden: {sourcePath}", "ERROR");
                return false;
            }

            var destination = Path.Combine(backupPath, destinationName);
            log($"Sichere {programName} (vollstaendig)...", "INFO");

            try
            {
                CopyDirectoryContents(sourcePath, destination);
                var size = CalculateSize(destination);
                var fileCount = 0;
                foreach (var _ in Directory.EnumerateFiles(destination, "*", SearchOption.AllDirectories))
                    fileCount++;
                var sizeMb = Math.Round(size / 1024d / 1024d, 2);
                log($"  OK: {programName} ({fileCount} Dateien, {sizeMb} MB)", "SUCCESS");
                return true;
            }
            catch (Exception ex)
            {
                log($"  [FEHLER] Fehler beim Sichern von {programName}: {ex.Message}", "ERROR");
                log($"    Quellpfad: {sourcePath}", "ERROR");
                return false;
            }
        }

        private static bool CompressBackup(string backupPath, string zipPath, Action<string, string> log)
        {
            log("Komprimiere Backup...", "INFO");

            try
            {
                if (File.Exists(zipPath))
                    File.Delete(zipPath);

                ZipFile.CreateFromDirectory(backupPath, zipPath, CompressionLevel.Optimal, false);

                if (File.Exists(zipPath))
                {
                    var zipSize = new FileInfo(zipPath).Length / 1024d / 1024d;
                    log($"ZIP erstellt: {zipPath} ({Math.Round(zipSize, 2)} MB)", "SUCCESS");

                    try
                    {
                        Directory.Delete(backupPath, true);
                        log("Temporaerer Ordner geloescht.", "SUCCESS");
                    }
                    catch (Exception ex)
                    {
                        log($"[WARNUNG] Temporaerer Ordner konnte nicht geloescht werden: {ex.Message}", "WARNING");
                    }

                    return true;
                }

                log("[FEHLER] ZIP-Datei wurde nicht erstellt!", "ERROR");
                return false;
            }
            catch (Exception ex)
            {
                log($"[FEHLER] Fehler beim Komprimieren: {ex.Message}", "ERROR");
                log($"  Backup-Ordner bleibt erhalten: {backupPath}", "WARNING");
                return false;
            }
        }

        private static void CleanupTempBackup(string backupPath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(backupPath) && Directory.Exists(backupPath))
                    Directory.Delete(backupPath, true);
            }
            catch
            {
                // Ignore cleanup errors.
            }
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

        private static long CalculateSize(string path)
        {
            if (File.Exists(path))
                return new FileInfo(path).Length;

            if (!Directory.Exists(path))
                return 0;

            long size = 0;
            foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                try
                {
                    size += new FileInfo(file).Length;
                }
                catch
                {
                    // Ignore size errors.
                }
            }

            return size;
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
}
