using FastColoredTextBoxNS;

namespace RDBExplorer.Forms
{
    partial class AssetViewForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssetViewForm));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            saveAsRawToolStripMenuItem = new ToolStripMenuItem();
            openInImHexToolStripMenuItem = new ToolStripMenuItem();
            tabControl = new TabControl();
            resourceRawViewTabPage = new TabPage();
            hexBox = new Be.Windows.Forms.HexBox();
            statusStrip = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            fileSizeToolStripStatusLabel = new ToolStripStatusLabel();
            resourceViewTabPage = new TabPage();
            fastColoredTextBox = new FastColoredTextBox();
            resourceDetailsTabPage = new TabPage();
            propertyResGrid = new PropertyGrid();
            menuStrip1.SuspendLayout();
            tabControl.SuspendLayout();
            resourceRawViewTabPage.SuspendLayout();
            statusStrip.SuspendLayout();
            resourceViewTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)fastColoredTextBox).BeginInit();
            resourceDetailsTabPage.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(812, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveAsRawToolStripMenuItem, openInImHexToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // saveAsRawToolStripMenuItem
            // 
            saveAsRawToolStripMenuItem.Name = "saveAsRawToolStripMenuItem";
            saveAsRawToolStripMenuItem.Size = new Size(191, 26);
            saveAsRawToolStripMenuItem.Text = "Save as Raw";
            // 
            // openInImHexToolStripMenuItem
            // 
            openInImHexToolStripMenuItem.Name = "openInImHexToolStripMenuItem";
            openInImHexToolStripMenuItem.Size = new Size(191, 26);
            openInImHexToolStripMenuItem.Text = "Open in ImHex";
            // 
            // tabControl
            // 
            tabControl.Controls.Add(resourceRawViewTabPage);
            tabControl.Controls.Add(resourceViewTabPage);
            tabControl.Controls.Add(resourceDetailsTabPage);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 28);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(812, 415);
            tabControl.TabIndex = 1;
            // 
            // resourceRawViewTabPage
            // 
            resourceRawViewTabPage.Controls.Add(hexBox);
            resourceRawViewTabPage.Controls.Add(statusStrip);
            resourceRawViewTabPage.Location = new Point(4, 29);
            resourceRawViewTabPage.Name = "resourceRawViewTabPage";
            resourceRawViewTabPage.Padding = new Padding(3);
            resourceRawViewTabPage.Size = new Size(804, 382);
            resourceRawViewTabPage.TabIndex = 0;
            resourceRawViewTabPage.Text = "Raw Hex";
            resourceRawViewTabPage.UseVisualStyleBackColor = true;
            // 
            // hexBox
            // 
            hexBox.BorderStyle = BorderStyle.None;
            hexBox.ColumnInfoVisible = true;
            hexBox.Dock = DockStyle.Fill;
            hexBox.Font = new Font("Consolas", 10F);
            hexBox.GroupSeparatorVisible = true;
            hexBox.GroupSize = 8;
            hexBox.LineInfoVisible = true;
            hexBox.Location = new Point(3, 3);
            hexBox.Margin = new Padding(4);
            hexBox.Name = "hexBox";
            hexBox.ShadowSelectionColor = Color.FromArgb(100, 60, 188, 255);
            hexBox.Size = new Size(798, 354);
            hexBox.StringViewVisible = true;
            hexBox.TabIndex = 1;
            hexBox.UseFixedBytesPerLine = true;
            hexBox.VScrollBarVisible = true;
            hexBox.CurrentLineChanged += Position_Changed;
            hexBox.CurrentPositionInLineChanged += Position_Changed;
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(20, 20);
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel, fileSizeToolStripStatusLabel });
            statusStrip.Location = new Point(3, 357);
            statusStrip.Name = "statusStrip";
            statusStrip.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            statusStrip.Size = new Size(798, 22);
            statusStrip.SizingGrip = false;
            statusStrip.TabIndex = 0;
            statusStrip.Text = "statusStrip";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(0, 16);
            // 
            // fileSizeToolStripStatusLabel
            // 
            fileSizeToolStripStatusLabel.Name = "fileSizeToolStripStatusLabel";
            fileSizeToolStripStatusLabel.Size = new Size(0, 16);
            // 
            // resourceViewTabPage
            // 
            resourceViewTabPage.Controls.Add(fastColoredTextBox);
            resourceViewTabPage.Location = new Point(4, 29);
            resourceViewTabPage.Name = "resourceViewTabPage";
            resourceViewTabPage.Padding = new Padding(3);
            resourceViewTabPage.Size = new Size(804, 382);
            resourceViewTabPage.TabIndex = 2;
            resourceViewTabPage.Text = "Resource Preview";
            resourceViewTabPage.UseVisualStyleBackColor = true;
            // 
            // fastColoredTextBox
            // 
            fastColoredTextBox.AutoCompleteBracketsList = new char[]
    {
    '(',
    ')',
    '{',
    '}',
    '[',
    ']',
    '"',
    '"',
    '\'',
    '\''
    };
            fastColoredTextBox.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
            fastColoredTextBox.AutoScrollMinSize = new Size(31, 18);
            fastColoredTextBox.BackBrush = null;
            fastColoredTextBox.CharHeight = 18;
            fastColoredTextBox.CharWidth = 10;
            fastColoredTextBox.DefaultMarkerSize = 8;
            fastColoredTextBox.DisabledColor = Color.FromArgb(100, 180, 180, 180);
            fastColoredTextBox.Dock = DockStyle.Fill;
            fastColoredTextBox.Hotkeys = resources.GetString("fastColoredTextBox.Hotkeys");
            fastColoredTextBox.IsReplaceMode = false;
            fastColoredTextBox.Location = new Point(3, 3);
            fastColoredTextBox.Name = "fastColoredTextBox";
            fastColoredTextBox.Paddings = new Padding(0);
            fastColoredTextBox.SelectionColor = Color.FromArgb(60, 0, 0, 255);
            fastColoredTextBox.ServiceColors = (ServiceColors)resources.GetObject("fastColoredTextBox.ServiceColors");
            fastColoredTextBox.Size = new Size(798, 376);
            fastColoredTextBox.TabIndex = 0;
            fastColoredTextBox.Zoom = 100;
            // 
            // resourceDetailsTabPage
            // 
            resourceDetailsTabPage.Controls.Add(propertyResGrid);
            resourceDetailsTabPage.Location = new Point(4, 29);
            resourceDetailsTabPage.Name = "resourceDetailsTabPage";
            resourceDetailsTabPage.Padding = new Padding(3);
            resourceDetailsTabPage.Size = new Size(804, 382);
            resourceDetailsTabPage.TabIndex = 1;
            resourceDetailsTabPage.Text = "Details";
            resourceDetailsTabPage.UseVisualStyleBackColor = true;
            // 
            // propertyResGrid
            // 
            propertyResGrid.BackColor = SystemColors.Control;
            propertyResGrid.Dock = DockStyle.Fill;
            propertyResGrid.Location = new Point(3, 3);
            propertyResGrid.Name = "propertyResGrid";
            propertyResGrid.Size = new Size(798, 376);
            propertyResGrid.TabIndex = 0;
            // 
            // AssetViewForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(812, 443);
            Controls.Add(tabControl);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(820, 480);
            Name = "AssetViewForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "AssetViewForm";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tabControl.ResumeLayout(false);
            resourceRawViewTabPage.ResumeLayout(false);
            resourceRawViewTabPage.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            resourceViewTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)fastColoredTextBox).EndInit();
            resourceDetailsTabPage.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem saveAsRawToolStripMenuItem;
        private ToolStripMenuItem openInImHexToolStripMenuItem;
        private TabControl tabControl;
        private TabPage resourceRawViewTabPage;
        private TabPage resourceDetailsTabPage;
        private PropertyGrid propertyResGrid;
        private TabPage resourceViewTabPage;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabel;
        private ToolStripStatusLabel fileSizeToolStripStatusLabel;
        private FastColoredTextBox fastColoredTextBox;
    }
}