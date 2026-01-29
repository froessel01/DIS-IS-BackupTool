using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BackupTool
{
    public partial class MainForm : Form
    {
        private static readonly string CurrentVersion = typeof(MainForm).Assembly.GetName().Version?.ToString() ?? "0.0.0";
        private static readonly string DisplayVersion = FormatVersion(CurrentVersion);
        private const string UpdateInfoUrl = "http://localhost:3001/tools/backuptool/download/version.json";
        private string _logFile = string.Empty;
        private string _backupRootPath = string.Empty;
        private string _backupPath = string.Empty;
        private string _zipPath = string.Empty;
        private string _baseDir = string.Empty;
        private string _configPath = string.Empty;
        private bool _hasError = false;
        private bool _isBackupMode = true;
        private int _currentStep = 0;
        private readonly Dictionary<string, bool> _programSelection = new(StringComparer.OrdinalIgnoreCase);
        private Color _accentColor = Color.FromArgb(140, 92, 255);
        private Color _cardBorderColor = Color.FromArgb(52, 54, 62);
        private Color _textColor = Color.FromArgb(230, 230, 235);
        private Color _subtleTextColor = Color.FromArgb(170, 170, 180);
        private AppSettings _settings = new();
        private string _settingsPath = string.Empty;

        public MainForm()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? Icon;
            _baseDir = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            _configPath = Path.Combine(_baseDir, "config.json");
            _settingsPath = Path.Combine(_baseDir, "settings.json");
            _settings = AppSettingsStore.Load(_settingsPath);
            labelBanner.Text = $"Experimentelles Tool (v{DisplayVersion}) – Bitte prüfe die generierten Dateien vor der Verwendung und erstelle Testbackups.";
            labelVersion.Text = $"Version: {DisplayVersion}";
            labelCreator.Text = "Ersteller: DIS IS LIFE - froessel";
            checkBoxAutoUpdate.Checked = _settings.AutoUpdateEnabled;
            ApplyTheme();
            LoadConfigForUi();
            SetMode(true);
            ShowStep(0);
            UpdateActionButtons();
            _ = CheckForUpdatesAsync(false);
        }

        private async void buttonBackup_Click(object sender, EventArgs e)
        {
            buttonBackup.Enabled = false;
            progressBar.Value = 0;
            textBoxLog.Clear();
            _hasError = false;

            await Task.Run(() =>
            {
                try
                {
                    RunBackup();
                }
                catch (Exception ex)
                {
                    WriteLog($"Unerwarteter Fehler: {ex.Message}", "ERROR");
                }
            });

            buttonBackup.Enabled = true;
        }

        private void RunBackup()
        {
            if (!_isBackupMode)
            {
                WriteLog("Restore-Modus ist noch nicht implementiert.", "WARNING");
                ShowError("Restore-Modus ist noch nicht implementiert.");
                return;
            }

            var logsDir = Path.Combine(_baseDir, "logs");
            Directory.CreateDirectory(logsDir);
            _logFile = Path.Combine(logsDir, $"backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log");

            LogBanner();
            WriteLog("=== Backup-Prozess gestartet ===", "INFO");

            if (!File.Exists(_configPath))
            {
                WriteLog("FEHLER: config.json nicht gefunden!", "ERROR");
                WriteLog($"Pfad: {_configPath}", "ERROR");
                ShowError("config.json nicht gefunden. Bitte erstelle die Datei oder verwende die Vorlage.");
                return;
            }

            if (!ConfigLoader.TryLoad(_configPath, out var config, out var errorMessage))
            {
                WriteLog($"FEHLER beim Laden der Konfiguration: {errorMessage}", "ERROR");
                WriteLog($"Datei: {_configPath}", "ERROR");
                ShowError("Fehler beim Laden der Konfiguration.");
                return;
            }

            if (config.ProgramsToBackup == null || config.ProgramsToBackup.Count == 0)
            {
                WriteLog("FEHLER: Keine Programme in der Konfiguration gefunden.", "ERROR");
                ShowError("Keine Programme in der Konfiguration gefunden.");
                return;
            }

            var selectedPrograms = GetSelectedPrograms(config.ProgramsToBackup);
            if (selectedPrograms.Count == 0)
            {
                WriteLog("FEHLER: Kein Programm ausgewählt.", "ERROR");
                ShowError("Kein Programm ausgewählt.");
                return;
            }

            var result = BackupEngine.Run(
                config,
                selectedPrograms,
                WriteLog,
                SetProgressMaximum,
                IncrementProgress);

            _backupRootPath = result.BackupRootPath ?? _backupRootPath;
            _backupPath = result.BackupPath ?? _backupPath;
            _zipPath = result.ZipPath ?? _zipPath;

            if (result.Aborted)
            {
                ResetProgress();
                ShowError("Backup abgebrochen, weil ein Fehler aufgetreten ist.");
                WriteSummary(result.SuccessCount, result.TotalPrograms, result.FailedPrograms);
                return;
            }

            WriteSummary(result.SuccessCount, result.TotalPrograms, result.FailedPrograms);
            UpdateActionButtons();
        }

        private void LogBanner()
        {
            WriteLog(string.Empty, "INFO");
            WriteLog("========================================", "INFO");
            WriteLog("  Backup Tool by DIS IS LIFE - froessel", "INFO");
            WriteLog($"  Version: {DisplayVersion}", "INFO");
            WriteLog("========================================", "INFO");
            WriteLog(string.Empty, "INFO");
        }

        private void WriteLog(string message, string level)
        {
            var logMessage = $"[{level}] {message}";

            try
            {
                File.AppendAllText(_logFile, logMessage + Environment.NewLine, Encoding.UTF8);
            }
            catch
            {
                // Ignore log file errors to keep UI responsive.
            }

            AppendLogToUi(logMessage, level);
            SetStatus(message, level);
            if (string.Equals(level, "ERROR", StringComparison.OrdinalIgnoreCase))
                _hasError = true;
        }

        private void AppendLogToUi(string message, string level)
        {
            if (textBoxLog.InvokeRequired)
            {
                textBoxLog.BeginInvoke(new Action<string, string>(AppendLogToUi), message, level);
                return;
            }

            var color = GetStatusColor(level);
            var start = textBoxLog.TextLength;
            textBoxLog.AppendText(message + Environment.NewLine);
            textBoxLog.Select(start, message.Length);
            textBoxLog.SelectionColor = color;
            textBoxLog.SelectionLength = 0;
            textBoxLog.ScrollToCaret();
        }

        private void SetProgressMaximum(int maximum)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.BeginInvoke(new Action<int>(SetProgressMaximum), maximum);
                return;
            }

            progressBar.Maximum = Math.Max(1, maximum);
            progressBar.Value = 0;
        }

        private void IncrementProgress()
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.BeginInvoke(new Action(IncrementProgress));
                return;
            }

            if (progressBar.Value < progressBar.Maximum)
                progressBar.Value += 1;
        }

        private void ResetProgress()
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.BeginInvoke(new Action(ResetProgress));
                return;
            }

            progressBar.Value = 0;
        }

        private void ShowError(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(ShowError), message);
                return;
            }

            MessageBox.Show(this, message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetStatus(string message, string level)
        {
            if (labelStatus.InvokeRequired)
            {
                labelStatus.BeginInvoke(new Action<string, string>(SetStatus), message, level);
                return;
            }

            labelStatus.Text = message;
            labelStatus.ForeColor = GetStatusColor(level);
        }

        private Color GetStatusColor(string level)
        {
            return level switch
            {
                "ERROR" => Color.FromArgb(220, 53, 69),
                "WARNING" => Color.FromArgb(255, 193, 7),
                "SUCCESS" => Color.FromArgb(25, 135, 84),
                _ => labelStatus.ForeColor
            };
        }

        private void WriteSummary(int successCount, int totalPrograms, List<string> failedPrograms)
        {
            if (_hasError)
                WriteLog("Backup abgebrochen.", "ERROR");
            else if (successCount == totalPrograms)
                WriteLog("Backup abgeschlossen.", "SUCCESS");
            else if (successCount > 0)
                WriteLog("Backup teilweise abgeschlossen.", "WARNING");
            else
                WriteLog("Backup fehlgeschlagen.", "ERROR");

            WriteLog($"Ergebnis: {successCount}/{totalPrograms} Programme gesichert.", "INFO");

            if (failedPrograms.Count > 0)
                WriteLog($"Fehlgeschlagen: {string.Join(", ", failedPrograms)}", "WARNING");

            if (File.Exists(_zipPath))
                WriteLog($"ZIP: {_zipPath}", "SUCCESS");
            else if (!string.IsNullOrWhiteSpace(_backupPath))
                WriteLog($"Ordner: {_backupPath}", "WARNING");

            WriteLog("QUACK!", "INFO");
        }

        private void buttonOpenBackupFolder_Click(object sender, EventArgs e)
        {
            var path = _backupRootPath;
            if (string.IsNullOrWhiteSpace(path) && ConfigLoader.TryLoad(_configPath, out var config, out _))
                path = Environment.ExpandEnvironmentVariables(config.BackupRootPath ?? string.Empty);

            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                ShowError("Backup-Ordner nicht gefunden.");
                return;
            }

            OpenPath(path);
        }

        private void buttonOpenZip_Click(object sender, EventArgs e)
        {
            var zipPath = _zipPath;
            if (string.IsNullOrWhiteSpace(zipPath) || !File.Exists(zipPath))
                zipPath = FindLatestZip();

            if (string.IsNullOrWhiteSpace(zipPath) || !File.Exists(zipPath))
            {
                ShowError("Keine ZIP-Datei gefunden.");
                return;
            }

            OpenPath(zipPath);
        }

        private void buttonOpenLogs_Click(object sender, EventArgs e)
        {
            var logsDir = Path.Combine(_baseDir, "logs");
            if (!Directory.Exists(logsDir))
            {
                ShowError("Logs-Ordner nicht gefunden.");
                return;
            }

            OpenPath(logsDir);
        }

        private void buttonOpenConfig_Click(object sender, EventArgs e)
        {
            if (!File.Exists(_configPath))
            {
                ShowError("config.json nicht gefunden.");
                return;
            }

            OpenPath(_configPath);
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (_currentStep > 0)
                ShowStep(_currentStep - 1);
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (_currentStep < 2)
                ShowStep(_currentStep + 1);
        }

        private void buttonModeBackup_Click(object sender, EventArgs e)
        {
            SetMode(true);
        }

        private void buttonModeRestore_Click(object sender, EventArgs e)
        {
            SetMode(false);
            ShowError("Restore-Modus ist noch nicht implementiert.");
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            if (_programSelection.Count == 0)
                return;

            var selectAll = _programSelection.Values.Any(v => !v);
            var keys = _programSelection.Keys.ToList();
            foreach (var key in keys)
                _programSelection[key] = selectAll;

            UpdateProgramButtons();
        }

        private void buttonCopyLog_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxLog.Text))
                return;
            Clipboard.SetText(textBoxLog.Text);
            SetStatus("Log in die Zwischenablage kopiert.", "SUCCESS");
        }

        private void UpdateActionButtons()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateActionButtons));
                return;
            }

            buttonBackup.Enabled = _isBackupMode;
            buttonBackup.Visible = _currentStep == 2;
            buttonBack.Enabled = _currentStep > 0;
            buttonNext.Enabled = _currentStep < 2;
            buttonOpenZip.Enabled = File.Exists(_zipPath) || !string.IsNullOrWhiteSpace(FindLatestZip());
            buttonOpenBackupFolder.Enabled = Directory.Exists(_backupRootPath);
            buttonOpenConfig.Enabled = File.Exists(_configPath);
        }

        private void ShowStep(int step)
        {
            _currentStep = Math.Clamp(step, 0, 2);
            panelStep1.Visible = _currentStep == 0;
            panelStep2.Visible = _currentStep == 1;
            panelStep3.Visible = _currentStep == 2;

            UpdateStep2Mode();
            UpdateActionButtons();
        }

        private void UpdateStep2Mode()
        {
            if (_isBackupMode)
            {
                labelPrograms.Visible = true;
                flowPrograms.Visible = true;
                buttonSelectAll.Visible = true;
                labelRestorePlaceholder.Visible = false;
            }
            else
            {
                labelPrograms.Visible = false;
                flowPrograms.Visible = false;
                buttonSelectAll.Visible = false;
                labelRestorePlaceholder.Visible = true;
            }
        }

        private string FindLatestZip()
        {
            if (string.IsNullOrWhiteSpace(_backupRootPath) || !Directory.Exists(_backupRootPath))
                return string.Empty;

            string? latest = null;
            DateTime latestTime = DateTime.MinValue;

            foreach (var file in Directory.EnumerateFiles(_backupRootPath, "Backuptool_by_froessel_*.zip"))
            {
                var info = new FileInfo(file);
                if (info.LastWriteTime > latestTime)
                {
                    latestTime = info.LastWriteTime;
                    latest = file;
                }
            }

            return latest ?? string.Empty;
        }

        private static void OpenPath(string path)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }

        private static string FormatVersion(string version)
        {
            if (!Version.TryParse(version, out var parsed))
                return version;
            return $"{parsed.Major}.{parsed.Minor}.{parsed.Build}";
        }

        private async Task CheckForUpdatesAsync(bool userInitiated)
        {
            try
            {
                if (!userInitiated && !_settings.AutoUpdateEnabled)
                    return;

                var info = await UpdateChecker.FetchAsync(UpdateInfoUrl);
                if (info == null || string.IsNullOrWhiteSpace(info.Version) || string.IsNullOrWhiteSpace(info.Url))
                {
                    if (userInitiated)
                        MessageBox.Show(this, "Keine Update-Informationen gefunden.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!UpdateChecker.IsNewer(CurrentVersion, info.Version))
                {
                    if (userInitiated)
                        MessageBox.Show(this, "Du hast bereits die aktuelle Version.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var message = $"Es ist ein Update verfügbar (v{info.Version}).\n\n" +
                              "Möchtest du die neue Version herunterladen?";
                if (!string.IsNullOrWhiteSpace(info.Notes))
                    message += $"\n\nHinweise:\n{info.Notes}";

                var result = MessageBox.Show(this, message, "Update verfügbar", MessageBoxButtons.YesNo, MessageBoxIcon.None);
                if (result == DialogResult.Yes)
                    OpenPath(info.Url);
            }
            catch
            {
                if (userInitiated)
                    MessageBox.Show(this, "Update konnte nicht geprüft werden.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // Ignore update errors to keep startup smooth.
            }
        }

        private void buttonCheckUpdates_Click(object sender, EventArgs e)
        {
            _ = CheckForUpdatesAsync(true);
        }

        private void checkBoxAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            _settings.AutoUpdateEnabled = checkBoxAutoUpdate.Checked;
            AppSettingsStore.Save(_settingsPath, _settings);
        }

        private void ApplyTheme()
        {
            var background = Color.FromArgb(25, 26, 30);
            var card = Color.FromArgb(34, 36, 42);
            var cardBorder = Color.FromArgb(52, 54, 62);
            var banner = Color.FromArgb(46, 38, 24);
            var bannerBorder = Color.FromArgb(126, 98, 42);
            var text = Color.FromArgb(230, 230, 235);
            var subtle = Color.FromArgb(170, 170, 180);
            var accent = Color.FromArgb(140, 92, 255);
            var logBg = Color.FromArgb(30, 32, 38);

            BackColor = background;
            ForeColor = text;
            _accentColor = accent;
            _cardBorderColor = cardBorder;
            _textColor = text;
            _subtleTextColor = subtle;

            panelBanner.BackColor = banner;
            panelBanner.Padding = new Padding(8);
            panelHeader.BackColor = card;
            panelWizard.BackColor = card;
            panelStep1.BackColor = card;
            panelStep2.BackColor = card;
            panelStep3.BackColor = card;
            panelMode.BackColor = card;
            flowPrograms.BackColor = logBg;

            ApplyPanelStyle(panelBanner, bannerBorder, 6);
            ApplyPanelStyle(panelHeader, cardBorder, 6);
            ApplyPanelStyle(panelWizard, cardBorder, 8);
            ApplyPanelStyle(panelStep1, cardBorder, 6);
            ApplyPanelStyle(panelStep2, cardBorder, 6);
            ApplyPanelStyle(panelStep3, cardBorder, 6);
            ApplyPanelStyle(panelMode, cardBorder, 6);

            labelBanner.ForeColor = Color.FromArgb(240, 232, 205);
            labelTitle.ForeColor = accent;
            labelSubtitle.ForeColor = subtle;
            labelVersion.ForeColor = subtle;
            labelCreator.ForeColor = subtle;
            labelStep1.ForeColor = subtle;
            labelStep1Desc.ForeColor = _subtleTextColor;
            labelStep2.ForeColor = subtle;
            labelStep2Desc.ForeColor = _subtleTextColor;
            labelStep3.ForeColor = subtle;
            labelStep3Desc.ForeColor = _subtleTextColor;
            labelStatus.ForeColor = subtle;
            labelMode.ForeColor = subtle;
            labelPrograms.ForeColor = subtle;
            labelRestorePlaceholder.ForeColor = subtle;
            checkBoxAutoUpdate.ForeColor = subtle;

            labelTitle.Font = new Font("Bahnschrift SemiBold", 18F);
            labelSubtitle.Font = new Font("Bahnschrift", 11F);
            labelVersion.Font = new Font("Bahnschrift", 10F);
            labelCreator.Font = new Font("Bahnschrift", 10F);
            labelStep1.Font = new Font("Bahnschrift SemiBold", 11F);
            labelStep2.Font = new Font("Bahnschrift SemiBold", 11F);
            labelStep3.Font = new Font("Bahnschrift SemiBold", 11F);

            textBoxLog.BackColor = logBg;
            textBoxLog.ForeColor = text;
            textBoxLog.BorderStyle = BorderStyle.FixedSingle;
            textBoxLog.Font = new Font("Consolas", 11F);

            ApplyThemeToPrimaryButton(buttonBackup, accent);
            ApplyThemeToSecondaryButton(buttonBack, cardBorder, text);
            ApplyThemeToSecondaryButton(buttonNext, cardBorder, text);
            ApplyThemeToSecondaryButton(buttonOpenBackupFolder, cardBorder, text);
            ApplyThemeToSecondaryButton(buttonOpenZip, cardBorder, text);
            ApplyThemeToSecondaryButton(buttonOpenLogs, cardBorder, text);
            ApplyThemeToSecondaryButton(buttonOpenConfig, cardBorder, text);
            ApplyThemeToSecondaryButton(buttonCopyLog, cardBorder, text);
            ApplyThemeToSecondaryButton(buttonSelectAll, cardBorder, text);
            ApplyThemeToSecondaryButton(buttonModeBackup, cardBorder, text);
            ApplyThemeToSecondaryButton(buttonModeRestore, cardBorder, text);
            ApplyThemeToSecondaryButton(buttonCheckUpdates, cardBorder, text);

            ApplyRoundedCorners(flowPrograms, 10);
            ApplyRoundedCorners(buttonBack, 10);
            ApplyRoundedCorners(buttonNext, 10);
            ApplyRoundedCorners(buttonBackup, 10);
            ApplyRoundedCorners(buttonSelectAll, 10);
            ApplyRoundedCorners(buttonModeBackup, 10);
            ApplyRoundedCorners(buttonModeRestore, 10);
            ApplyRoundedCorners(buttonCheckUpdates, 10);

            UpdateModeButtonStyles();
            UpdateProgramButtons();
        }

        private static void ApplyThemeToPrimaryButton(Button button, Color accent)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = accent;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = accent;
            button.ForeColor = Color.White;
        }

        private static void ApplyThemeToSecondaryButton(Button button, Color border, Color text)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = border;
            button.FlatAppearance.BorderSize = 1;
            button.BackColor = Color.FromArgb(30, 32, 38);
            button.ForeColor = text;
        }

        private static void ApplyPanelStyle(RoundedPanel panel, Color border, int radius)
        {
            panel.BorderColor = border;
            panel.BorderThickness = 1;
            panel.BorderRadius = radius;
        }

        private static void ApplyRoundedCorners(Control control, int radius)
        {
            if (control.Width <= 0 || control.Height <= 0)
                return;

            var rect = new Rectangle(0, 0, control.Width, control.Height);
            using var path = new GraphicsPath();
            var diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }

        private void LoadConfigForUi()
        {
            flowPrograms.Controls.Clear();
            _programSelection.Clear();

            if (!File.Exists(_configPath))
            {
                flowPrograms.Controls.Add(CreateInfoLabel("Keine config.json gefunden."));
                return;
            }

            if (!ConfigLoader.TryLoad(_configPath, out var config, out _))
            {
                flowPrograms.Controls.Add(CreateInfoLabel("Config konnte nicht gelesen werden."));
                return;
            }

            if (config.ProgramsToBackup == null || config.ProgramsToBackup.Count == 0)
            {
                flowPrograms.Controls.Add(CreateInfoLabel("Keine Programme in der Config."));
                return;
            }

            foreach (var program in config.ProgramsToBackup)
            {
                var name = program.Name ?? "(Unbekannt)";
                if (!_programSelection.ContainsKey(name))
                    _programSelection[name] = true;

                var btn = new Button
                {
                    Text = name,
                    Tag = name,
                    Width = 300,
                    Height = 64,
                    Margin = new Padding(8)
                };
                btn.Click += ProgramToggle_Click;
                flowPrograms.Controls.Add(btn);
            }

            UpdateProgramButtons();
        }

        private void ProgramToggle_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not string key)
                return;

            if (_programSelection.ContainsKey(key))
                _programSelection[key] = !_programSelection[key];

            UpdateProgramButtons();
        }

        private void UpdateProgramButtons()
        {
            foreach (Control control in flowPrograms.Controls)
            {
                if (control is Button btn && btn.Tag is string key)
                {
                    var selected = _programSelection.TryGetValue(key, out var isSelected) && isSelected;
                    if (selected)
                    {
                        ApplyThemeToPrimaryButton(btn, _accentColor);
                    }
                    else
                    {
                        ApplyThemeToSecondaryButton(btn, _cardBorderColor, _textColor);
                    }
                    ApplyRoundedCorners(btn, 10);
                }
                else if (control is Label lbl)
                {
                    lbl.ForeColor = _subtleTextColor;
                }
            }

            if (_programSelection.Count > 0)
                buttonSelectAll.Text = _programSelection.Values.All(v => v) ? "Alle abwählen" : "Alle auswählen";
        }

        private Label CreateInfoLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = _subtleTextColor,
                Margin = new Padding(4, 6, 4, 6)
            };
        }

        private void SetMode(bool isBackup)
        {
            _isBackupMode = isBackup;
            labelSubtitle.Text = _isBackupMode
                ? "Backup-Modus: Programme sichern (Restore folgt später)"
                : "Restore-Modus: Platzhalter (kommt bald)";
            labelStep3.Text = _isBackupMode
                ? "Schritt 3: Backup durchführen"
                : "Schritt 3: Restore (Platzhalter)";

            UpdateModeButtonStyles();
            UpdateStep2Mode();
            UpdateActionButtons();
        }

        private void UpdateModeButtonStyles()
        {
            if (_isBackupMode)
            {
                ApplyThemeToPrimaryButton(buttonModeBackup, _accentColor);
                ApplyThemeToSecondaryButton(buttonModeRestore, _cardBorderColor, _textColor);
            }
            else
            {
                ApplyThemeToSecondaryButton(buttonModeBackup, _cardBorderColor, _textColor);
                ApplyThemeToPrimaryButton(buttonModeRestore, _accentColor);
            }
        }

        private List<ProgramEntry> GetSelectedPrograms(List<ProgramEntry> programs)
        {
            var selected = new List<ProgramEntry>();
            foreach (var program in programs)
            {
                var name = program.Name ?? string.Empty;
                if (_programSelection.TryGetValue(name, out var isSelected) && isSelected)
                    selected.Add(program);
            }
            return selected;
        }

    }
}
