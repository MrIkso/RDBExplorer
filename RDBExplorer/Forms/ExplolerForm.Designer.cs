namespace RDBExplorer.Forms
{
    partial class ExplolerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            extractAllToolStripMenuItem = new ToolStripMenuItem();
            grabNamesToolStripMenuItem = new ToolStripMenuItem();
            grabAllMagicHeadersToolStripMenuItem = new ToolStripMenuItem();
            upackBinArchiveToolStripMenuItem = new ToolStripMenuItem();
            packBinArchiveToolStripMenuItem = new ToolStripMenuItem();
            g1TTexureToolToolStripMenuItem = new ToolStripMenuItem();
            localeToolStripMenuItem = new ToolStripMenuItem();
            unpackLocalesToolStripMenuItem = new ToolStripMenuItem();
            packLocalesToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            infoToolStripMenuItem = new ToolStripMenuItem();
            tableLayoutPanel1 = new TableLayoutPanel();
            archiveList = new ListView();
            toolStripStatusLabel = new Label();
            progressBarOperation = new ProgressBar();
            typeFilterComboBox = new RDBExplorer.Controls.CheckedComboBox.CheckedComboBox();
            filterBox = new TextBox();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            exportWitchNameToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, toolsToolStripMenuItem, localeToolStripMenuItem, settingsToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(882, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(128, 26);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(128, 26);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { extractAllToolStripMenuItem, grabNamesToolStripMenuItem, grabAllMagicHeadersToolStripMenuItem, upackBinArchiveToolStripMenuItem, packBinArchiveToolStripMenuItem, g1TTexureToolToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(58, 24);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // extractAllToolStripMenuItem
            // 
            extractAllToolStripMenuItem.Enabled = false;
            extractAllToolStripMenuItem.Name = "extractAllToolStripMenuItem";
            extractAllToolStripMenuItem.Size = new Size(250, 26);
            extractAllToolStripMenuItem.Text = "Extract All";
            extractAllToolStripMenuItem.Click += extractAllToolStripMenuItem_Click;
            // 
            // grabNamesToolStripMenuItem
            // 
            grabNamesToolStripMenuItem.Enabled = false;
            grabNamesToolStripMenuItem.Name = "grabNamesToolStripMenuItem";
            grabNamesToolStripMenuItem.Size = new Size(250, 26);
            grabNamesToolStripMenuItem.Text = "Grab Names";
            grabNamesToolStripMenuItem.Click += grabNamesToolStripMenuItem_Click;
            // 
            // grabAllMagicHeadersToolStripMenuItem
            // 
            grabAllMagicHeadersToolStripMenuItem.Enabled = false;
            grabAllMagicHeadersToolStripMenuItem.Name = "grabAllMagicHeadersToolStripMenuItem";
            grabAllMagicHeadersToolStripMenuItem.Size = new Size(250, 26);
            grabAllMagicHeadersToolStripMenuItem.Text = "Grab All Magic Headers";
            grabAllMagicHeadersToolStripMenuItem.Click += grabAllMagicHeadersToolStripMenuItem_Click;
            // 
            // upackBinArchiveToolStripMenuItem
            // 
            upackBinArchiveToolStripMenuItem.Name = "upackBinArchiveToolStripMenuItem";
            upackBinArchiveToolStripMenuItem.Size = new Size(250, 26);
            upackBinArchiveToolStripMenuItem.Text = "Upack Bin Archive";
            upackBinArchiveToolStripMenuItem.Click += upackBinArchiveToolStripMenuItem_Click;
            // 
            // packBinArchiveToolStripMenuItem
            // 
            packBinArchiveToolStripMenuItem.Name = "packBinArchiveToolStripMenuItem";
            packBinArchiveToolStripMenuItem.Size = new Size(250, 26);
            packBinArchiveToolStripMenuItem.Text = "Pack Bin Archive";
            packBinArchiveToolStripMenuItem.Click += packBinArchiveToolStripMenuItem_Click;
            // 
            // g1TTexureToolToolStripMenuItem
            // 
            g1TTexureToolToolStripMenuItem.Name = "g1TTexureToolToolStripMenuItem";
            g1TTexureToolToolStripMenuItem.Size = new Size(250, 26);
            g1TTexureToolToolStripMenuItem.Text = "G1T Texure Tool";
            g1TTexureToolToolStripMenuItem.Click += g1TTexureToolToolStripMenuItem_Click;
            // 
            // localeToolStripMenuItem
            // 
            localeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { unpackLocalesToolStripMenuItem, packLocalesToolStripMenuItem });
            localeToolStripMenuItem.Name = "localeToolStripMenuItem";
            localeToolStripMenuItem.Size = new Size(66, 24);
            localeToolStripMenuItem.Text = "Locale";
            // 
            // unpackLocalesToolStripMenuItem
            // 
            unpackLocalesToolStripMenuItem.Name = "unpackLocalesToolStripMenuItem";
            unpackLocalesToolStripMenuItem.Size = new Size(194, 26);
            unpackLocalesToolStripMenuItem.Text = "Unpack Locales";
            unpackLocalesToolStripMenuItem.Click += unpackLocalesToolStripMenuItem_Click;
            // 
            // packLocalesToolStripMenuItem
            // 
            packLocalesToolStripMenuItem.Name = "packLocalesToolStripMenuItem";
            packLocalesToolStripMenuItem.Size = new Size(194, 26);
            packLocalesToolStripMenuItem.Text = "Pack Locales";
            packLocalesToolStripMenuItem.Click += packLocalesToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { infoToolStripMenuItem });
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(64, 24);
            aboutToolStripMenuItem.Text = "About";
            // 
            // infoToolStripMenuItem
            // 
            infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            infoToolStripMenuItem.Size = new Size(118, 26);
            infoToolStripMenuItem.Text = "Info";
            infoToolStripMenuItem.Click += infoToolStripMenuItem_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62.5F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37.5F));
            tableLayoutPanel1.Controls.Add(archiveList, 1, 0);
            tableLayoutPanel1.Controls.Add(toolStripStatusLabel, 1, 1);
            tableLayoutPanel1.Controls.Add(progressBarOperation, 1, 2);
            tableLayoutPanel1.Controls.Add(typeFilterComboBox, 1, 0);
            tableLayoutPanel1.Controls.Add(filterBox, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 28);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(882, 525);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // archiveList
            // 
            tableLayoutPanel1.SetColumnSpan(archiveList, 2);
            archiveList.Dock = DockStyle.Fill;
            archiveList.Location = new Point(3, 37);
            archiveList.Name = "archiveList";
            archiveList.Size = new Size(876, 459);
            archiveList.TabIndex = 0;
            archiveList.UseCompatibleStateImageBehavior = false;
            archiveList.KeyDown += archiveList_KeyDown;
            archiveList.MouseDoubleClick += archiveList_MouseDoubleClick;
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.AutoSize = true;
            toolStripStatusLabel.Dock = DockStyle.Bottom;
            toolStripStatusLabel.Location = new Point(3, 505);
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(545, 20);
            toolStripStatusLabel.TabIndex = 4;
            toolStripStatusLabel.Text = "None";
            // 
            // progressBarOperation
            // 
            progressBarOperation.Dock = DockStyle.Bottom;
            progressBarOperation.Location = new Point(554, 502);
            progressBarOperation.Name = "progressBarOperation";
            progressBarOperation.Size = new Size(325, 20);
            progressBarOperation.TabIndex = 5;
            // 
            // typeFilterComboBox
            // 
            typeFilterComboBox.Dock = DockStyle.Top;
            typeFilterComboBox.DrawMode = DrawMode.OwnerDrawVariable;
            typeFilterComboBox.DropDownHeight = 1;
            typeFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            typeFilterComboBox.FormattingEnabled = true;
            typeFilterComboBox.IntegralHeight = false;
            typeFilterComboBox.Location = new Point(554, 3);
            typeFilterComboBox.Name = "typeFilterComboBox";
            typeFilterComboBox.Size = new Size(325, 28);
            typeFilterComboBox.TabIndex = 6;
            typeFilterComboBox.ItemCheck += typeFilterComboBox_ItemCheck;
            // 
            // filterBox
            // 
            filterBox.Dock = DockStyle.Top;
            filterBox.Location = new Point(3, 3);
            filterBox.Name = "filterBox";
            filterBox.PlaceholderText = "Enter text to filter";
            filterBox.Size = new Size(545, 27);
            filterBox.TabIndex = 7;
            filterBox.TextChanged += toolStripTextBox1_TextChanged;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportWitchNameToolStripMenuItem });
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(76, 24);
            settingsToolStripMenuItem.Text = "Settings";
            // 
            // exportWitchToolStripMenuItem
            // 
            exportWitchNameToolStripMenuItem.Name = "exportWitchToolStripMenuItem";
            exportWitchNameToolStripMenuItem.Size = new Size(224, 26);
            exportWitchNameToolStripMenuItem.Text = "Export With Name";
            exportWitchNameToolStripMenuItem.Click += exportWitchToolStripMenuItem_Click;
            // 
            // ExplolerForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(882, 553);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(800, 400);
            Name = "ExplolerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ExplolerForm";
            Load += ExplolerForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private TableLayoutPanel tableLayoutPanel1;
        private ListView archiveList;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem grabNamesToolStripMenuItem;
        private ToolStripMenuItem upackBinArchiveToolStripMenuItem;
        private ToolStripMenuItem packBinArchiveToolStripMenuItem;
        private ToolStripMenuItem localeToolStripMenuItem;
        private ToolStripMenuItem unpackLocalesToolStripMenuItem;
        private ToolStripMenuItem packLocalesToolStripMenuItem;
        private ToolStripMenuItem extractAllToolStripMenuItem;
        private ToolStripMenuItem grabAllMagicHeadersToolStripMenuItem;
        private Label toolStripStatusLabel;
        private ProgressBar progressBarOperation;
        private ToolStripMenuItem g1TTexureToolToolStripMenuItem;
        private Controls.CheckedComboBox.CheckedComboBox typeFilterComboBox;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem infoToolStripMenuItem;
        private TextBox filterBox;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem exportWitchNameToolStripMenuItem;
    }
}