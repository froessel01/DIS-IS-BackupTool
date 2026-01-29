using System.Collections.Generic;

namespace BackupTool
{
    public sealed class BackupConfig
    {
        public string? BackupRootPath { get; set; }
        public List<ProgramEntry> ProgramsToBackup { get; set; } = new();
    }

    public sealed class ProgramEntry
    {
        public string? Name { get; set; }
        public string? Path { get; set; }
        public string? Type { get; set; }
        public List<string>? Items { get; set; }
        public List<string>? AlternatePaths { get; set; }
    }
}
