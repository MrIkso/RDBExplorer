namespace RDBExplorer.Controls
{
    partial class DepedencyListControl
    {
        private System.ComponentModel.IContainer components = null;

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
            tableLayoutPanel1 = new TableLayoutPanel();
            depedencyListView = new ListView();
            nameHeader = new ColumnHeader();
            typeHeader = new ColumnHeader();
            hashHeader = new ColumnHeader();
            sizeHeader = new ColumnHeader();
            statusLabel = new Label();
            filterTextBox = new TextBox();
            extractAllDataBtn = new Button();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(depedencyListView, 0, 2);
            tableLayoutPanel1.Controls.Add(statusLabel, 0, 0);
            tableLayoutPanel1.Controls.Add(filterTextBox, 0, 1);
            tableLayoutPanel1.Controls.Add(extractAllDataBtn, 0, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(538, 318);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // depedencyListView
            // 
            depedencyListView.Columns.AddRange(new ColumnHeader[] { nameHeader, typeHeader, hashHeader, sizeHeader });
            depedencyListView.Dock = DockStyle.Fill;
            depedencyListView.FullRowSelect = true;
            depedencyListView.GridLines = true;
            depedencyListView.Location = new Point(3, 56);
            depedencyListView.Name = "depedencyListView";
            depedencyListView.Size = new Size(532, 224);
            depedencyListView.TabIndex = 0;
            depedencyListView.UseCompatibleStateImageBehavior = false;
            depedencyListView.View = View.Details;
            depedencyListView.VirtualMode = true;
            depedencyListView.ColumnClick += depedencyListView_ColumnClick;
            depedencyListView.RetrieveVirtualItem += DepedencyListView_RetrieveVirtualItem;
            depedencyListView.DoubleClick += depedencyListView_DoubleClick;
            // 
            // nameHeader
            // 
            nameHeader.Text = "Name";
            nameHeader.Width = 200;
            // 
            // typeHeader
            // 
            typeHeader.Text = "Type";
            typeHeader.Width = 220;
            // 
            // hashHeader
            // 
            hashHeader.Text = "Hash";
            hashHeader.Width = 100;
            // 
            // sizeHeader
            // 
            sizeHeader.Text = "Size";
            sizeHeader.Width = 80;
            // 
            // statusLabel
            // 
            statusLabel.Dock = DockStyle.Top;
            statusLabel.Location = new Point(3, 0);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(532, 20);
            statusLabel.TabIndex = 1;
            statusLabel.Text = "Dependencies";
            // 
            // filterTextBox
            // 
            filterTextBox.Dock = DockStyle.Top;
            filterTextBox.Location = new Point(3, 23);
            filterTextBox.Name = "filterTextBox";
            filterTextBox.PlaceholderText = "Enter text to filter";
            filterTextBox.Size = new Size(532, 27);
            filterTextBox.TabIndex = 2;
            filterTextBox.TextChanged += filterTextBox_TextChanged;
            // 
            // extractAllDataBtn
            // 
            extractAllDataBtn.Location = new Point(3, 286);
            extractAllDataBtn.Name = "extractAllDataBtn";
            extractAllDataBtn.Size = new Size(94, 29);
            extractAllDataBtn.TabIndex = 3;
            extractAllDataBtn.Text = "Extract All Data";
            extractAllDataBtn.UseVisualStyleBackColor = true;
            extractAllDataBtn.Click += extractAllDataBtn_Click;
            // 
            // DepedencyListControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "DepedencyListControl";
            Size = new Size(538, 318);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }
        private TableLayoutPanel tableLayoutPanel1;
        private ListView depedencyListView;
        private ColumnHeader nameHeader;
        private ColumnHeader typeHeader;
        private ColumnHeader hashHeader;
        private Label statusLabel;
        private TextBox filterTextBox;
        private ColumnHeader sizeHeader;
        private Button extractAllDataBtn;
    }
}
