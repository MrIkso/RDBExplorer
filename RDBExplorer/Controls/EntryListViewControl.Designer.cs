namespace RDBExplorer.Controls
{
    partial class EntryListViewControl
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
            entryListView = new ListView();
            columnName = new ColumnHeader();
            columnSize = new ColumnHeader();
            tableLayoutPanel1 = new TableLayoutPanel();
            extractAllDataBtn = new Button();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // entryListView
            // 
            entryListView.Columns.AddRange(new ColumnHeader[] { columnName, columnSize });
            entryListView.Dock = DockStyle.Fill;
            entryListView.FullRowSelect = true;
            entryListView.GridLines = true;
            entryListView.Location = new Point(3, 3);
            entryListView.Name = "entryListView";
            entryListView.Size = new Size(494, 259);
            entryListView.TabIndex = 0;
            entryListView.UseCompatibleStateImageBehavior = false;
            entryListView.View = View.Details;
            entryListView.MouseDoubleClick += entryListView_MouseDoubleClick;
            // 
            // columnName
            // 
            columnName.Text = "Name";
            columnName.Width = 300;
            // 
            // columnSize
            // 
            columnSize.Text = "Size";
            columnSize.Width = 100;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(entryListView, 0, 0);
            tableLayoutPanel1.Controls.Add(extractAllDataBtn, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(500, 300);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // extractAllDataBtn
            // 
            extractAllDataBtn.Enabled = false;
            extractAllDataBtn.Location = new Point(3, 268);
            extractAllDataBtn.Name = "extractAllDataBtn";
            extractAllDataBtn.Size = new Size(94, 29);
            extractAllDataBtn.TabIndex = 1;
            extractAllDataBtn.Text = "Extract All";
            extractAllDataBtn.UseVisualStyleBackColor = true;
            extractAllDataBtn.Click += extractAllDataBtn_Click;
            // 
            // EntryListViewControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "EntryListViewControl";
            Size = new Size(500, 300);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }
        private ListView entryListView;
        private ColumnHeader columnName;
        private ColumnHeader columnSize;
        private TableLayoutPanel tableLayoutPanel1;
        private Button extractAllDataBtn;
    }
}
