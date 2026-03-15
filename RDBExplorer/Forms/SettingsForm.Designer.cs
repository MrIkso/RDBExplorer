namespace RDBExplorer.Forms
{
    partial class SettingsForm
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
            tableLayoutPanel1 = new TableLayoutPanel();
            rdbNamesDatabaseTb = new TextBox();
            selectPathDbBtn = new Button();
            label1 = new Label();
            selectModelAndTexturesDBBtn = new Button();
            modelAndTexturesDBTb = new TextBox();
            label2 = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            saveButton = new Button();
            cancelButton = new Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(rdbNamesDatabaseTb, 1, 0);
            tableLayoutPanel1.Controls.Add(selectPathDbBtn, 2, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(selectModelAndTexturesDBBtn, 2, 1);
            tableLayoutPanel1.Controls.Add(modelAndTexturesDBTb, 1, 1);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 18.75F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 18.75F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 62.5F));
            tableLayoutPanel1.Size = new Size(782, 253);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // rdbNamesDatabaseTb
            // 
            rdbNamesDatabaseTb.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            rdbNamesDatabaseTb.Location = new Point(275, 10);
            rdbNamesDatabaseTb.Name = "rdbNamesDatabaseTb";
            rdbNamesDatabaseTb.Size = new Size(402, 27);
            rdbNamesDatabaseTb.TabIndex = 0;
            // 
            // selectPathDbBtn
            // 
            selectPathDbBtn.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            selectPathDbBtn.Location = new Point(683, 9);
            selectPathDbBtn.Name = "selectPathDbBtn";
            selectPathDbBtn.Size = new Size(96, 29);
            selectPathDbBtn.TabIndex = 1;
            selectPathDbBtn.Text = "Select";
            selectPathDbBtn.UseVisualStyleBackColor = true;
            selectPathDbBtn.Click += selectPathDbBtn_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(3, 13);
            label1.Name = "label1";
            label1.Size = new Size(266, 20);
            label1.TabIndex = 2;
            label1.Text = "Rdb Names Database Path:";
            // 
            // selectModelAndTexturesDBBtn
            // 
            selectModelAndTexturesDBBtn.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            selectModelAndTexturesDBBtn.Location = new Point(683, 56);
            selectModelAndTexturesDBBtn.Name = "selectModelAndTexturesDBBtn";
            selectModelAndTexturesDBBtn.Size = new Size(96, 29);
            selectModelAndTexturesDBBtn.TabIndex = 4;
            selectModelAndTexturesDBBtn.Text = "Select";
            selectModelAndTexturesDBBtn.UseVisualStyleBackColor = true;
            selectModelAndTexturesDBBtn.Click += selectModelAndTexturesDBBtn_Click;
            // 
            // modelAndTexturesDBTb
            // 
            modelAndTexturesDBTb.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            modelAndTexturesDBTb.Location = new Point(275, 57);
            modelAndTexturesDBTb.Name = "modelAndTexturesDBTb";
            modelAndTexturesDBTb.Size = new Size(402, 27);
            modelAndTexturesDBTb.TabIndex = 3;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(3, 60);
            label2.Name = "label2";
            label2.Size = new Size(266, 20);
            label2.TabIndex = 5;
            label2.Text = "Model And Textures Database Path:";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.Controls.Add(saveButton, 1, 0);
            tableLayoutPanel2.Controls.Add(cancelButton, 2, 0);
            tableLayoutPanel2.Dock = DockStyle.Bottom;
            tableLayoutPanel2.Location = new Point(0, 211);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.Size = new Size(782, 42);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // saveButton
            // 
            saveButton.Dock = DockStyle.Fill;
            saveButton.Location = new Point(394, 3);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(189, 36);
            saveButton.TabIndex = 0;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Dock = DockStyle.Fill;
            cancelButton.Location = new Point(589, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(190, 36);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(782, 253);
            Controls.Add(tableLayoutPanel2);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Preferences";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Button saveButton;
        private Button cancelButton;
        private TextBox rdbNamesDatabaseTb;
        private Button selectPathDbBtn;
        private Label label1;
        private Button selectModelAndTexturesDBBtn;
        private TextBox modelAndTexturesDBTb;
        private Label label2;
    }
}