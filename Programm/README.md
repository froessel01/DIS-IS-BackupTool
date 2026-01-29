# BackupTool (WinForms)

Kleines WinForms-Backup-Tool. Die Backup-Logik ist aus dem PowerShell-Skript nach C# portiert.

Voraussetzung: .NET 8 SDK auf Windows.

Build:
```powershell
dotnet build
```

Publish (self-contained, Single-File, win-x64, Ausgabe in `../exe`):
```powershell
dotnet publish -c Release
```

Hinweis: Der Publish lÃ¶scht den Inhalt des `exe`-Ordners vor dem Build, damit dort nur `BackupTool.exe` und `config.json` liegen.

Konfiguration:
- Lege eine `config.json` im selben Ordner wie die `.exe` ab (siehe Beispiel `Programm/config.json`).
- Felder: `BackupRootPath` und `ProgramsToBackup`.

