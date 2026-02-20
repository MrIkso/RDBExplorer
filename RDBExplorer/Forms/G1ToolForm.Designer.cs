namespace RDBExplorer.Forms
{
    partial class G1ToolForm
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
            exportImagesToolStripMenuItem = new ToolStripMenuItem();
            openFileDialog = new OpenFileDialog();
            tableLayoutPanel1 = new TableLayoutPanel();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            textureListView = new ListView();
            textureName = new ColumnHeader();
            textureSize = new ColumnHeader();
            texturePreviewTabControl = new TabControl();
            textureTabPage = new TabPage();
            tableLayoutPanel2 = new TableLayoutPanel();
            layersComboBox = new ComboBox();
            label2 = new Label();
            textutePrewierPictureBox = new PictureBox();
            label1 = new Label();
            mipsComboBox = new ComboBox();
            textureDetailsTabPage = new TabPage();
            texrurePropertyGrid = new PropertyGrid();
            menuStrip1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            texturePreviewTabControl.SuspendLayout();
            textureTabPage.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)textutePrewierPictureBox).BeginInit();
            textureDetailsTabPage.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(8, 3, 0, 3);
            menuStrip1.Size = new Size(1132, 30);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, exportImagesToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(187, 26);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            // 
            // exportImagesToolStripMenuItem
            // 
            exportImagesToolStripMenuItem.Name = "exportImagesToolStripMenuItem";
            exportImagesToolStripMenuItem.Size = new Size(224, 26);
            exportImagesToolStripMenuItem.Text = "Export Images";
            exportImagesToolStripMenuItem.Click += exportImagesToolStripMenuItem_Click;
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(statusStrip1, 0, 1);
            tableLayoutPanel1.Controls.Add(splitContainer1, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 30);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(1132, 613);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // statusStrip1
            // 
            tableLayoutPanel1.SetColumnSpan(statusStrip1, 2);
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel });
            statusStrip1.Location = new Point(0, 587);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1132, 26);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(78, 20);
            toolStripStatusLabel.Text = "Textures: 0";
            // 
            // splitContainer1
            // 
            tableLayoutPanel1.SetColumnSpan(splitContainer1, 2);
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(3, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(textureListView);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(texturePreviewTabControl);
            splitContainer1.Size = new Size(1126, 581);
            splitContainer1.SplitterDistance = 298;
            splitContainer1.TabIndex = 1;
            // 
            // textureListView
            // 
            textureListView.Columns.AddRange(new ColumnHeader[] { textureName, textureSize });
            textureListView.Dock = DockStyle.Fill;
            textureListView.GridLines = true;
            textureListView.Location = new Point(0, 0);
            textureListView.Name = "textureListView";
            textureListView.Size = new Size(298, 581);
            textureListView.TabIndex = 0;
            textureListView.UseCompatibleStateImageBehavior = false;
            textureListView.View = View.Details;
            textureListView.MouseClick += TextureListView_MouseClick;
            // 
            // textureName
            // 
            textureName.Text = "Name";
            textureName.Width = 150;
            // 
            // textureSize
            // 
            textureSize.Text = "Size";
            textureSize.Width = 90;
            // 
            // texturePreviewTabControl
            // 
            texturePreviewTabControl.Controls.Add(textureTabPage);
            texturePreviewTabControl.Controls.Add(textureDetailsTabPage);
            texturePreviewTabControl.Dock = DockStyle.Fill;
            texturePreviewTabControl.Location = new Point(0, 0);
            texturePreviewTabControl.Name = "texturePreviewTabControl";
            texturePreviewTabControl.SelectedIndex = 0;
            texturePreviewTabControl.Size = new Size(824, 581);
            texturePreviewTabControl.TabIndex = 0;
            // 
            // textureTabPage
            // 
            textureTabPage.Controls.Add(tableLayoutPanel2);
            textureTabPage.Location = new Point(4, 29);
            textureTabPage.Name = "textureTabPage";
            textureTabPage.Padding = new Padding(3);
            textureTabPage.Size = new Size(816, 548);
            textureTabPage.TabIndex = 0;
            textureTabPage.Text = "Texture";
            textureTabPage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 6;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.Controls.Add(layersComboBox, 3, 1);
            tableLayoutPanel2.Controls.Add(label2, 2, 1);
            tableLayoutPanel2.Controls.Add(textutePrewierPictureBox, 0, 0);
            tableLayoutPanel2.Controls.Add(label1, 0, 1);
            tableLayoutPanel2.Controls.Add(mipsComboBox, 1, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 93.87755F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 6.122449F));
            tableLayoutPanel2.Size = new Size(810, 542);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // layersComboBox
            // 
            layersComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            layersComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            layersComboBox.FormattingEnabled = true;
            layersComboBox.Location = new Point(286, 511);
            layersComboBox.Name = "layersComboBox";
            layersComboBox.Size = new Size(180, 28);
            layersComboBox.TabIndex = 4;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(233, 515);
            label2.Name = "label2";
            label2.Size = new Size(47, 20);
            label2.TabIndex = 3;
            label2.Text = "Layer:";
            // 
            // textutePrewierPictureBox
            // 
            textutePrewierPictureBox.BackColor = Color.Gray;
            tableLayoutPanel2.SetColumnSpan(textutePrewierPictureBox, 6);
            textutePrewierPictureBox.Dock = DockStyle.Fill;
            textutePrewierPictureBox.Location = new Point(3, 3);
            textutePrewierPictureBox.Name = "textutePrewierPictureBox";
            textutePrewierPictureBox.Size = new Size(804, 502);
            textutePrewierPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            textutePrewierPictureBox.TabIndex = 0;
            textutePrewierPictureBox.TabStop = false;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(3, 515);
            label1.Name = "label1";
            label1.Size = new Size(38, 20);
            label1.TabIndex = 1;
            label1.Text = "Mip:";
            // 
            // mipsComboBox
            // 
            mipsComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            mipsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            mipsComboBox.FormattingEnabled = true;
            mipsComboBox.Location = new Point(47, 511);
            mipsComboBox.Name = "mipsComboBox";
            mipsComboBox.Size = new Size(180, 28);
            mipsComboBox.TabIndex = 2;
            // 
            // textureDetailsTabPage
            // 
            textureDetailsTabPage.Controls.Add(texrurePropertyGrid);
            textureDetailsTabPage.Location = new Point(4, 29);
            textureDetailsTabPage.Name = "textureDetailsTabPage";
            textureDetailsTabPage.Padding = new Padding(3);
            textureDetailsTabPage.Size = new Size(816, 548);
            textureDetailsTabPage.TabIndex = 1;
            textureDetailsTabPage.Text = "Details";
            textureDetailsTabPage.UseVisualStyleBackColor = true;
            // 
            // texrurePropertyGrid
            // 
            texrurePropertyGrid.BackColor = SystemColors.Control;
            texrurePropertyGrid.Dock = DockStyle.Fill;
            texrurePropertyGrid.Location = new Point(3, 3);
            texrurePropertyGrid.Name = "texrurePropertyGrid";
            texrurePropertyGrid.Size = new Size(810, 542);
            texrurePropertyGrid.TabIndex = 0;
            // 
            // G1ToolForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1132, 643);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 5, 4, 5);
            Name = "G1ToolForm";
            Text = "G1Tool";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            texturePreviewTabControl.ResumeLayout(false);
            textureTabPage.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)textutePrewierPictureBox).EndInit();
            textureDetailsTabPage.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private ToolStripMenuItem exportImagesToolStripMenuItem;
        private TableLayoutPanel tableLayoutPanel1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel;
        private SplitContainer splitContainer1;
        private ListView textureListView;
        private TabControl texturePreviewTabControl;
        private TabPage textureTabPage;
        private TabPage textureDetailsTabPage;
        private TableLayoutPanel tableLayoutPanel2;
        private PictureBox textutePrewierPictureBox;
        private ColumnHeader textureName;
        private ColumnHeader textureSize;
        private ComboBox layersComboBox;
        private Label label2;
        private Label label1;
        private ComboBox mipsComboBox;
        private PropertyGrid texrurePropertyGrid;
    }
}

