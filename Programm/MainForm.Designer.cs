using System.Windows.Forms;

namespace BackupTool
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private RoundedPanel panelBanner;
        private Label labelBanner;
        private RoundedPanel panelHeader;
        private Label labelTitle;
        private Label labelSubtitle;
        private Label labelVersion;
        private Label labelCreator;
        private Button buttonCheckUpdates;
        private CheckBox checkBoxAutoUpdate;
        private RoundedPanel panelWizard;
        private RoundedPanel panelStep1;
        private RoundedPanel panelStep2;
        private RoundedPanel panelStep3;
        private Label labelStep1;
        private Label labelStep1Desc;
        private Label labelStep2;
        private Label labelStep2Desc;
        private Label labelStep3;
        private Label labelStep3Desc;
        private Label labelMode;
        private RoundedPanel panelMode;
        private Button buttonModeBackup;
        private Button buttonModeRestore;
        private Label labelPrograms;
        private FlowLayoutPanel flowPrograms;
        private Button buttonSelectAll;
        private Label labelRestorePlaceholder;
        private Label labelRestoreBackups;
        private ListBox listBoxRestoreBackups;
        private Label labelRestorePrograms;
        private FlowLayoutPanel flowRestorePrograms;
        private Button buttonRestoreSelectAll;
        private CheckBox checkBoxRestoreBackupBefore;
        private Label labelRestoreWarning;
        private ProgressBar progressBar;
        private Label labelStatus;
        private RichTextBox textBoxLog;
        private Button buttonCopyLog;
        private Button buttonOpenConfig;
        private Button buttonOpenLogs;
        private Button buttonOpenZip;
        private Button buttonOpenBackupFolder;
        private Button buttonBack;
        private Button buttonNext;
        private Button buttonBackup;
        private Button buttonCancel;
        private ToolTip toolTip;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelBanner = new RoundedPanel();
            this.labelBanner = new Label();
            this.panelHeader = new RoundedPanel();
            this.labelTitle = new Label();
            this.labelSubtitle = new Label();
            this.labelVersion = new Label();
            this.labelCreator = new Label();
            this.buttonCheckUpdates = new Button();
            this.checkBoxAutoUpdate = new CheckBox();
            this.panelWizard = new RoundedPanel();
            this.panelStep1 = new RoundedPanel();
            this.panelStep2 = new RoundedPanel();
            this.panelStep3 = new RoundedPanel();
            this.labelStep1 = new Label();
            this.labelStep1Desc = new Label();
            this.labelStep2 = new Label();
            this.labelStep2Desc = new Label();
            this.labelStep3 = new Label();
            this.labelStep3Desc = new Label();
            this.labelMode = new Label();
            this.panelMode = new RoundedPanel();
            this.buttonModeBackup = new Button();
            this.buttonModeRestore = new Button();
            this.labelPrograms = new Label();
            this.flowPrograms = new FlowLayoutPanel();
            this.buttonSelectAll = new Button();
            this.labelRestorePlaceholder = new Label();
            this.labelRestoreBackups = new Label();
            this.listBoxRestoreBackups = new ListBox();
            this.labelRestorePrograms = new Label();
            this.flowRestorePrograms = new FlowLayoutPanel();
            this.buttonRestoreSelectAll = new Button();
            this.checkBoxRestoreBackupBefore = new CheckBox();
            this.labelRestoreWarning = new Label();
            this.progressBar = new ProgressBar();
            this.labelStatus = new Label();
            this.textBoxLog = new RichTextBox();
            this.buttonCopyLog = new Button();
            this.buttonOpenConfig = new Button();
            this.buttonOpenLogs = new Button();
            this.buttonOpenZip = new Button();
            this.buttonOpenBackupFolder = new Button();
            this.buttonBack = new Button();
            this.buttonNext = new Button();
            this.buttonBackup = new Button();
            this.buttonCancel = new Button();
            this.toolTip = new ToolTip(this.components);
            this.panelBanner.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelWizard.SuspendLayout();
            this.panelStep1.SuspendLayout();
            this.panelStep2.SuspendLayout();
            this.panelStep3.SuspendLayout();
            this.panelMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBanner
            // 
            this.panelBanner.Controls.Add(this.labelBanner);
            this.panelBanner.Location = new System.Drawing.Point(18, 16);
            this.panelBanner.Name = "panelBanner";
            this.panelBanner.Size = new System.Drawing.Size(1060, 58);
            this.panelBanner.TabIndex = 0;
            // 
            // labelBanner
            // 
            this.labelBanner.AutoSize = true;
            this.labelBanner.Location = new System.Drawing.Point(18, 20);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Size = new System.Drawing.Size(680, 17);
            this.labelBanner.TabIndex = 0;
            this.labelBanner.Text = "Experimentelles Tool (v2.0.0) – Bitte prüfe die generierten Dateien vor der Verwendung und erstelle Testbackups.";
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.buttonCheckUpdates);
            this.panelHeader.Controls.Add(this.checkBoxAutoUpdate);
            this.panelHeader.Controls.Add(this.labelCreator);
            this.panelHeader.Controls.Add(this.labelVersion);
            this.panelHeader.Controls.Add(this.labelSubtitle);
            this.panelHeader.Controls.Add(this.labelTitle);
            this.panelHeader.Location = new System.Drawing.Point(18, 86);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1060, 120);
            this.panelHeader.TabIndex = 1;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Bahnschrift SemiBold", 18F);
            this.labelTitle.Location = new System.Drawing.Point(18, 12);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(321, 29);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Willkommen beim Backup Tool";
            // 
            // labelSubtitle
            // 
            this.labelSubtitle.AutoSize = true;
            this.labelSubtitle.Font = new System.Drawing.Font("Bahnschrift", 11F);
            this.labelSubtitle.Location = new System.Drawing.Point(20, 46);
            this.labelSubtitle.Name = "labelSubtitle";
            this.labelSubtitle.Size = new System.Drawing.Size(486, 18);
            this.labelSubtitle.TabIndex = 1;
            this.labelSubtitle.Text = "Backup-Modus: Programme sichern (Restore folgt später)";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(700, 12);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(109, 17);
            this.labelVersion.TabIndex = 2;
            this.labelVersion.Text = "Version: 0.0.0";
            // 
            // labelCreator
            // 
            this.labelCreator.AutoSize = true;
            this.labelCreator.Location = new System.Drawing.Point(700, 36);
            this.labelCreator.Name = "labelCreator";
            this.labelCreator.Size = new System.Drawing.Size(231, 17);
            this.labelCreator.TabIndex = 3;
            this.labelCreator.Text = "Ersteller: froessel | Gruppe: DIS IS LIFE";
            // 
            // buttonCheckUpdates
            // 
            this.buttonCheckUpdates.Location = new System.Drawing.Point(860, 68);
            this.buttonCheckUpdates.Name = "buttonCheckUpdates";
            this.buttonCheckUpdates.Size = new System.Drawing.Size(180, 32);
            this.buttonCheckUpdates.TabIndex = 5;
            this.buttonCheckUpdates.Text = "Updates prüfen";
            this.toolTip.SetToolTip(this.buttonCheckUpdates, "Jetzt nach Updates suchen");
            this.buttonCheckUpdates.UseVisualStyleBackColor = true;
            this.buttonCheckUpdates.Click += new System.EventHandler(this.buttonCheckUpdates_Click);
            // 
            // checkBoxAutoUpdate
            // 
            this.checkBoxAutoUpdate.AutoSize = true;
            this.checkBoxAutoUpdate.Location = new System.Drawing.Point(700, 72);
            this.checkBoxAutoUpdate.Name = "checkBoxAutoUpdate";
            this.checkBoxAutoUpdate.Size = new System.Drawing.Size(145, 21);
            this.checkBoxAutoUpdate.TabIndex = 4;
            this.checkBoxAutoUpdate.Text = "Beim Start prüfen";
            this.checkBoxAutoUpdate.UseVisualStyleBackColor = true;
            this.checkBoxAutoUpdate.CheckedChanged += new System.EventHandler(this.checkBoxAutoUpdate_CheckedChanged);
            // 
            // panelWizard
            // 
            this.panelWizard.Controls.Add(this.panelStep1);
            this.panelWizard.Controls.Add(this.panelStep2);
            this.panelWizard.Controls.Add(this.panelStep3);
            this.panelWizard.Controls.Add(this.buttonBack);
            this.panelWizard.Controls.Add(this.buttonNext);
            this.panelWizard.Controls.Add(this.buttonBackup);
            this.panelWizard.Controls.Add(this.buttonCancel);
            this.panelWizard.Location = new System.Drawing.Point(18, 216);
            this.panelWizard.Name = "panelWizard";
            this.panelWizard.Size = new System.Drawing.Size(1060, 540);
            this.panelWizard.TabIndex = 2;
            // 
            // panelStep1
            // 
            this.panelStep1.Controls.Add(this.labelStep1Desc);
            this.panelStep1.Controls.Add(this.labelStep1);
            this.panelStep1.Controls.Add(this.labelMode);
            this.panelStep1.Controls.Add(this.panelMode);
            this.panelStep1.Location = new System.Drawing.Point(16, 16);
            this.panelStep1.Name = "panelStep1";
            this.panelStep1.Size = new System.Drawing.Size(1028, 460);
            this.panelStep1.TabIndex = 0;
            // 
            // labelStep1
            // 
            this.labelStep1.AutoSize = true;
            this.labelStep1.Location = new System.Drawing.Point(12, 12);
            this.labelStep1.Name = "labelStep1";
            this.labelStep1.Size = new System.Drawing.Size(151, 17);
            this.labelStep1.TabIndex = 0;
            this.labelStep1.Text = "Schritt 1: Modus wählen";
            // 
            // labelStep1Desc
            // 
            this.labelStep1Desc.AutoSize = true;
            this.labelStep1Desc.Location = new System.Drawing.Point(12, 36);
            this.labelStep1Desc.Name = "labelStep1Desc";
            this.labelStep1Desc.Size = new System.Drawing.Size(318, 17);
            this.labelStep1Desc.TabIndex = 1;
            this.labelStep1Desc.Text = "Wähle ob du ein Backup erstellen oder restaurieren möchtest.";
            // 
            // labelMode
            // 
            this.labelMode.AutoSize = true;
            this.labelMode.Location = new System.Drawing.Point(12, 70);
            this.labelMode.Name = "labelMode";
            this.labelMode.Size = new System.Drawing.Size(49, 17);
            this.labelMode.TabIndex = 1;
            this.labelMode.Text = "Modus";
            // 
            // panelMode
            // 
            this.panelMode.Controls.Add(this.buttonModeBackup);
            this.panelMode.Controls.Add(this.buttonModeRestore);
            this.panelMode.Location = new System.Drawing.Point(12, 96);
            this.panelMode.Name = "panelMode";
            this.panelMode.Size = new System.Drawing.Size(700, 100);
            this.panelMode.TabIndex = 2;
            // 
            // buttonModeBackup
            // 
            this.buttonModeBackup.Location = new System.Drawing.Point(0, 0);
            this.buttonModeBackup.Name = "buttonModeBackup";
            this.buttonModeBackup.Size = new System.Drawing.Size(320, 80);
            this.buttonModeBackup.TabIndex = 0;
            this.buttonModeBackup.Text = "Backup";
            this.toolTip.SetToolTip(this.buttonModeBackup, "Backup-Modus auswählen");
            this.buttonModeBackup.UseVisualStyleBackColor = true;
            this.buttonModeBackup.Click += new System.EventHandler(this.buttonModeBackup_Click);
            // 
            // buttonModeRestore
            // 
            this.buttonModeRestore.Location = new System.Drawing.Point(340, 0);
            this.buttonModeRestore.Name = "buttonModeRestore";
            this.buttonModeRestore.Size = new System.Drawing.Size(320, 80);
            this.buttonModeRestore.TabIndex = 1;
            this.buttonModeRestore.Text = "Restore";
            this.toolTip.SetToolTip(this.buttonModeRestore, "Restore-Modus (Platzhalter)");
            this.buttonModeRestore.UseVisualStyleBackColor = true;
            this.buttonModeRestore.Click += new System.EventHandler(this.buttonModeRestore_Click);
            // 
            // panelStep2
            // 
            this.panelStep2.Controls.Add(this.labelStep2Desc);
            this.panelStep2.Controls.Add(this.labelStep2);
            this.panelStep2.Controls.Add(this.labelPrograms);
            this.panelStep2.Controls.Add(this.flowPrograms);
            this.panelStep2.Controls.Add(this.buttonSelectAll);
            this.panelStep2.Controls.Add(this.labelRestorePlaceholder);
            this.panelStep2.Controls.Add(this.labelRestoreBackups);
            this.panelStep2.Controls.Add(this.listBoxRestoreBackups);
            this.panelStep2.Controls.Add(this.labelRestorePrograms);
            this.panelStep2.Controls.Add(this.flowRestorePrograms);
            this.panelStep2.Controls.Add(this.buttonRestoreSelectAll);
            this.panelStep2.Controls.Add(this.checkBoxRestoreBackupBefore);
            this.panelStep2.Controls.Add(this.labelRestoreWarning);
            this.panelStep2.Location = new System.Drawing.Point(16, 16);
            this.panelStep2.Name = "panelStep2";
            this.panelStep2.Size = new System.Drawing.Size(1028, 460);
            this.panelStep2.TabIndex = 1;
            // 
            // labelStep2
            // 
            this.labelStep2.AutoSize = true;
            this.labelStep2.Location = new System.Drawing.Point(12, 12);
            this.labelStep2.Name = "labelStep2";
            this.labelStep2.Size = new System.Drawing.Size(206, 17);
            this.labelStep2.TabIndex = 0;
            this.labelStep2.Text = "Schritt 2: Programme auswählen";
            // 
            // labelStep2Desc
            // 
            this.labelStep2Desc.AutoSize = true;
            this.labelStep2Desc.Location = new System.Drawing.Point(12, 36);
            this.labelStep2Desc.Name = "labelStep2Desc";
            this.labelStep2Desc.Size = new System.Drawing.Size(374, 17);
            this.labelStep2Desc.TabIndex = 1;
            this.labelStep2Desc.Text = "Wähle die Programme, die du im aktuellen Lauf sichern möchtest.";
            // 
            // labelPrograms
            // 
            this.labelPrograms.AutoSize = true;
            this.labelPrograms.Location = new System.Drawing.Point(12, 70);
            this.labelPrograms.Name = "labelPrograms";
            this.labelPrograms.Size = new System.Drawing.Size(83, 17);
            this.labelPrograms.TabIndex = 1;
            this.labelPrograms.Text = "Programme";
            // 
            // flowPrograms
            // 
            this.flowPrograms.AutoScroll = true;
            this.flowPrograms.FlowDirection = FlowDirection.LeftToRight;
            this.flowPrograms.Location = new System.Drawing.Point(12, 96);
            this.flowPrograms.Name = "flowPrograms";
            this.flowPrograms.Size = new System.Drawing.Size(1000, 320);
            this.flowPrograms.TabIndex = 2;
            this.flowPrograms.WrapContents = true;
            // 
            // buttonSelectAll
            // 
            this.buttonSelectAll.Location = new System.Drawing.Point(12, 430);
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.Size = new System.Drawing.Size(320, 36);
            this.buttonSelectAll.TabIndex = 3;
            this.buttonSelectAll.Text = "Alle auswählen";
            this.toolTip.SetToolTip(this.buttonSelectAll, "Alle Programme auswählen oder abwählen");
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
            // 
            // labelRestorePlaceholder
            // 
            this.labelRestorePlaceholder.AutoSize = true;
            this.labelRestorePlaceholder.Location = new System.Drawing.Point(12, 96);
            this.labelRestorePlaceholder.Name = "labelRestorePlaceholder";
            this.labelRestorePlaceholder.Size = new System.Drawing.Size(364, 17);
            this.labelRestorePlaceholder.TabIndex = 4;
            this.labelRestorePlaceholder.Text = "Restore ist noch nicht implementiert. (Platzhalter-Ansicht)";
            // 
            // labelRestoreBackups
            // 
            this.labelRestoreBackups.AutoSize = true;
            this.labelRestoreBackups.Location = new System.Drawing.Point(12, 70);
            this.labelRestoreBackups.Name = "labelRestoreBackups";
            this.labelRestoreBackups.Size = new System.Drawing.Size(149, 17);
            this.labelRestoreBackups.TabIndex = 5;
            this.labelRestoreBackups.Text = "Backups (ZIP-Dateien)";
            // 
            // listBoxRestoreBackups
            // 
            this.listBoxRestoreBackups.DrawMode = DrawMode.OwnerDrawFixed;
            this.listBoxRestoreBackups.FormattingEnabled = true;
            this.listBoxRestoreBackups.ItemHeight = 17;
            this.listBoxRestoreBackups.Location = new System.Drawing.Point(12, 96);
            this.listBoxRestoreBackups.Name = "listBoxRestoreBackups";
            this.listBoxRestoreBackups.Size = new System.Drawing.Size(1000, 140);
            this.listBoxRestoreBackups.TabIndex = 6;
            this.listBoxRestoreBackups.SelectedIndexChanged += new System.EventHandler(this.listBoxRestoreBackups_SelectedIndexChanged);
            this.listBoxRestoreBackups.DrawItem += new DrawItemEventHandler(this.listBoxRestoreBackups_DrawItem);
            // 
            // labelRestorePrograms
            // 
            this.labelRestorePrograms.AutoSize = true;
            this.labelRestorePrograms.Location = new System.Drawing.Point(12, 244);
            this.labelRestorePrograms.Name = "labelRestorePrograms";
            this.labelRestorePrograms.Size = new System.Drawing.Size(153, 17);
            this.labelRestorePrograms.TabIndex = 7;
            this.labelRestorePrograms.Text = "Programme im Backup";
            // 
            // flowRestorePrograms
            // 
            this.flowRestorePrograms.AutoScroll = true;
            this.flowRestorePrograms.FlowDirection = FlowDirection.LeftToRight;
            this.flowRestorePrograms.Location = new System.Drawing.Point(12, 270);
            this.flowRestorePrograms.Name = "flowRestorePrograms";
            this.flowRestorePrograms.Size = new System.Drawing.Size(1000, 150);
            this.flowRestorePrograms.TabIndex = 8;
            this.flowRestorePrograms.WrapContents = true;
            // 
            // buttonRestoreSelectAll
            // 
            this.buttonRestoreSelectAll.Location = new System.Drawing.Point(12, 430);
            this.buttonRestoreSelectAll.Name = "buttonRestoreSelectAll";
            this.buttonRestoreSelectAll.Size = new System.Drawing.Size(320, 36);
            this.buttonRestoreSelectAll.TabIndex = 9;
            this.buttonRestoreSelectAll.Text = "Alle auswählen";
            this.toolTip.SetToolTip(this.buttonRestoreSelectAll, "Alle Programme auswählen oder abwählen");
            this.buttonRestoreSelectAll.UseVisualStyleBackColor = true;
            this.buttonRestoreSelectAll.Click += new System.EventHandler(this.buttonRestoreSelectAll_Click);
            // 
            // checkBoxRestoreBackupBefore
            // 
            this.checkBoxRestoreBackupBefore.AutoSize = true;
            this.checkBoxRestoreBackupBefore.Location = new System.Drawing.Point(350, 436);
            this.checkBoxRestoreBackupBefore.Name = "checkBoxRestoreBackupBefore";
            this.checkBoxRestoreBackupBefore.Size = new System.Drawing.Size(358, 21);
            this.checkBoxRestoreBackupBefore.TabIndex = 10;
            this.checkBoxRestoreBackupBefore.Text = "Vorher Backup der bestehenden Daten erstellen";
            this.checkBoxRestoreBackupBefore.UseVisualStyleBackColor = true;
            this.checkBoxRestoreBackupBefore.Checked = true;
            // 
            // labelRestoreWarning
            // 
            this.labelRestoreWarning.AutoSize = true;
            this.labelRestoreWarning.Location = new System.Drawing.Point(12, 52);
            this.labelRestoreWarning.Name = "labelRestoreWarning";
            this.labelRestoreWarning.Size = new System.Drawing.Size(474, 17);
            this.labelRestoreWarning.TabIndex = 11;
            this.labelRestoreWarning.Text = "Hinweis: Beim Restore werden Dateien überschrieben. Bitte vorher prüfen.";
            // 
            // panelStep3
            // 
            this.panelStep3.Controls.Add(this.labelStep3Desc);
            this.panelStep3.Controls.Add(this.labelStep3);
            this.panelStep3.Controls.Add(this.progressBar);
            this.panelStep3.Controls.Add(this.labelStatus);
            this.panelStep3.Controls.Add(this.textBoxLog);
            this.panelStep3.Controls.Add(this.buttonCopyLog);
            this.panelStep3.Controls.Add(this.buttonOpenConfig);
            this.panelStep3.Controls.Add(this.buttonOpenLogs);
            this.panelStep3.Controls.Add(this.buttonOpenZip);
            this.panelStep3.Controls.Add(this.buttonOpenBackupFolder);
            this.panelStep3.Location = new System.Drawing.Point(16, 16);
            this.panelStep3.Name = "panelStep3";
            this.panelStep3.Size = new System.Drawing.Size(1028, 460);
            this.panelStep3.TabIndex = 2;
            // 
            // labelStep3
            // 
            this.labelStep3.AutoSize = true;
            this.labelStep3.Location = new System.Drawing.Point(12, 12);
            this.labelStep3.Name = "labelStep3";
            this.labelStep3.Size = new System.Drawing.Size(188, 17);
            this.labelStep3.TabIndex = 0;
            this.labelStep3.Text = "Schritt 3: Backup durchführen";
            // 
            // labelStep3Desc
            // 
            this.labelStep3Desc.AutoSize = true;
            this.labelStep3Desc.Location = new System.Drawing.Point(12, 36);
            this.labelStep3Desc.Name = "labelStep3Desc";
            this.labelStep3Desc.Size = new System.Drawing.Size(401, 17);
            this.labelStep3Desc.TabIndex = 1;
            this.labelStep3Desc.Text = "Starte den Prozess und verfolge Fortschritt und Log-Ausgaben.";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 64);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1000, 20);
            this.progressBar.TabIndex = 1;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(12, 90);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(45, 17);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Text = "Bereit";
            // 
            // textBoxLog
            // 
            this.textBoxLog.Location = new System.Drawing.Point(12, 116);
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(1000, 280);
            this.textBoxLog.TabIndex = 3;
            // 
            // buttonCopyLog
            // 
            this.buttonCopyLog.Location = new System.Drawing.Point(12, 408);
            this.buttonCopyLog.Name = "buttonCopyLog";
            this.buttonCopyLog.Size = new System.Drawing.Size(180, 40);
            this.buttonCopyLog.TabIndex = 4;
            this.buttonCopyLog.Text = "Log kopieren";
            this.buttonCopyLog.UseVisualStyleBackColor = true;
            this.buttonCopyLog.Click += new System.EventHandler(this.buttonCopyLog_Click);
            // 
            // buttonOpenConfig
            // 
            this.buttonOpenConfig.Location = new System.Drawing.Point(832, 408);
            this.buttonOpenConfig.Name = "buttonOpenConfig";
            this.buttonOpenConfig.Size = new System.Drawing.Size(180, 40);
            this.buttonOpenConfig.TabIndex = 8;
            this.buttonOpenConfig.Text = "Config öffnen";
            this.buttonOpenConfig.UseVisualStyleBackColor = true;
            this.buttonOpenConfig.Click += new System.EventHandler(this.buttonOpenConfig_Click);
            // 
            // buttonOpenLogs
            // 
            this.buttonOpenLogs.Location = new System.Drawing.Point(644, 408);
            this.buttonOpenLogs.Name = "buttonOpenLogs";
            this.buttonOpenLogs.Size = new System.Drawing.Size(180, 40);
            this.buttonOpenLogs.TabIndex = 7;
            this.buttonOpenLogs.Text = "Logs öffnen";
            this.buttonOpenLogs.UseVisualStyleBackColor = true;
            this.buttonOpenLogs.Click += new System.EventHandler(this.buttonOpenLogs_Click);
            // 
            // buttonOpenZip
            // 
            this.buttonOpenZip.Location = new System.Drawing.Point(456, 408);
            this.buttonOpenZip.Name = "buttonOpenZip";
            this.buttonOpenZip.Size = new System.Drawing.Size(180, 40);
            this.buttonOpenZip.TabIndex = 6;
            this.buttonOpenZip.Text = "Letztes ZIP öffnen";
            this.buttonOpenZip.UseVisualStyleBackColor = true;
            this.buttonOpenZip.Click += new System.EventHandler(this.buttonOpenZip_Click);
            // 
            // buttonOpenBackupFolder
            // 
            this.buttonOpenBackupFolder.Location = new System.Drawing.Point(268, 408);
            this.buttonOpenBackupFolder.Name = "buttonOpenBackupFolder";
            this.buttonOpenBackupFolder.Size = new System.Drawing.Size(180, 40);
            this.buttonOpenBackupFolder.TabIndex = 5;
            this.buttonOpenBackupFolder.Text = "Backup-Ordner öffnen";
            this.buttonOpenBackupFolder.UseVisualStyleBackColor = true;
            this.buttonOpenBackupFolder.Click += new System.EventHandler(this.buttonOpenBackupFolder_Click);
            // 
            // buttonBack
            // 
            this.buttonBack.Location = new System.Drawing.Point(16, 492);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(150, 40);
            this.buttonBack.TabIndex = 3;
            this.buttonBack.Text = "Zurück";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Location = new System.Drawing.Point(172, 492);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(150, 40);
            this.buttonNext.TabIndex = 4;
            this.buttonNext.Text = "Weiter";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonBackup
            // 
            this.buttonBackup.Location = new System.Drawing.Point(894, 492);
            this.buttonBackup.Name = "buttonBackup";
            this.buttonBackup.Size = new System.Drawing.Size(150, 40);
            this.buttonBackup.TabIndex = 5;
            this.buttonBackup.Text = "Start";
            this.buttonBackup.UseVisualStyleBackColor = true;
            this.buttonBackup.Click += new System.EventHandler(this.buttonBackup_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(738, 492);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(150, 40);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Abbrechen";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1096, 780);
            this.Controls.Add(this.panelWizard);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelBanner);
            this.Font = new System.Drawing.Font("Bahnschrift", 10F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Backup Tool";
            this.panelBanner.ResumeLayout(false);
            this.panelBanner.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelWizard.ResumeLayout(false);
            this.panelStep1.ResumeLayout(false);
            this.panelStep1.PerformLayout();
            this.panelStep2.ResumeLayout(false);
            this.panelStep2.PerformLayout();
            this.panelStep3.ResumeLayout(false);
            this.panelStep3.PerformLayout();
            this.panelMode.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}

