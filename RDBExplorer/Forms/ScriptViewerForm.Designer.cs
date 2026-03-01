using FastColoredTextBoxNS;

namespace RDBExplorer.Forms
{
    partial class ScriptViewerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptViewerForm));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveResultToolStripMenuItem = new ToolStripMenuItem();
            modeToolStripMenuItem = new ToolStripMenuItem();
            modeToolStripMenuItem1 = new ToolStripMenuItem();
            decompilerToolStripMenuItem = new ToolStripMenuItem();
            dissasemblerToolStripMenuItem = new ToolStripMenuItem();
            ftb = new FastColoredTextBox();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ftb).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, modeToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, saveResultToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(224, 26);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // saveResultToolStripMenuItem
            // 
            saveResultToolStripMenuItem.Enabled = false;
            saveResultToolStripMenuItem.Name = "saveResultToolStripMenuItem";
            saveResultToolStripMenuItem.Size = new Size(224, 26);
            saveResultToolStripMenuItem.Text = "Save result";
            saveResultToolStripMenuItem.Click += saveResultToolStripMenuItem_Click;
            // 
            // modeToolStripMenuItem
            // 
            modeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { modeToolStripMenuItem1 });
            modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            modeToolStripMenuItem.Size = new Size(98, 24);
            modeToolStripMenuItem.Text = "View Mode";
            // 
            // modeToolStripMenuItem1
            // 
            modeToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { decompilerToolStripMenuItem, dissasemblerToolStripMenuItem });
            modeToolStripMenuItem1.Name = "modeToolStripMenuItem1";
            modeToolStripMenuItem1.Size = new Size(224, 26);
            modeToolStripMenuItem1.Text = "Mode";
            // 
            // decompilerToolStripMenuItem
            // 
            decompilerToolStripMenuItem.Name = "decompilerToolStripMenuItem";
            decompilerToolStripMenuItem.Size = new Size(224, 26);
            decompilerToolStripMenuItem.Text = "Decompiler";
            decompilerToolStripMenuItem.Click += decompilerToolStripMenuItem_Click;
            // 
            // dissasemblerToolStripMenuItem
            // 
            dissasemblerToolStripMenuItem.Name = "dissasemblerToolStripMenuItem";
            dissasemblerToolStripMenuItem.Size = new Size(224, 26);
            dissasemblerToolStripMenuItem.Text = "Dissasembler";
            dissasemblerToolStripMenuItem.Click += dissasemblerToolStripMenuItem_Click;
            // 
            // ftb
            // 
            ftb.AutoCompleteBracketsList = new char[]
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
            ftb.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
            ftb.AutoScrollMinSize = new Size(31, 18);
            ftb.BackBrush = null;
            ftb.CharHeight = 18;
            ftb.CharWidth = 10;
            ftb.DefaultMarkerSize = 8;
            ftb.DisabledColor = Color.FromArgb(100, 180, 180, 180);
            ftb.Dock = DockStyle.Fill;
            ftb.Hotkeys = resources.GetString("ftb.Hotkeys");
            ftb.IsReplaceMode = false;
            ftb.Location = new Point(0, 28);
            ftb.Name = "ftb";
            ftb.Paddings = new Padding(0);
            ftb.ReadOnly = true;
            ftb.SelectionColor = Color.FromArgb(60, 0, 0, 255);
            ftb.ServiceColors = (ServiceColors)resources.GetObject("ftb.ServiceColors");
            ftb.Size = new Size(800, 422);
            ftb.TabIndex = 1;
            ftb.Zoom = 100;
            // 
            // ScriptViewerForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ftb);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "ScriptViewerForm";
            Text = "Script Viewer";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ftb).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem modeToolStripMenuItem;
        private ToolStripMenuItem modeToolStripMenuItem1;
        private ToolStripMenuItem decompilerToolStripMenuItem;
        private ToolStripMenuItem dissasemblerToolStripMenuItem;
        private FastColoredTextBox ftb;
        private ToolStripMenuItem saveResultToolStripMenuItem;
    }
}