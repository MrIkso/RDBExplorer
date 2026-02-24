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
            SuspendLayout();
            // 
            // entryListView
            // 
            entryListView.Columns.AddRange(new ColumnHeader[] { columnName, columnSize });
            entryListView.Dock = DockStyle.Fill;
            entryListView.FullRowSelect = true;
            entryListView.GridLines = true;
            entryListView.Location = new Point(0, 0);
            entryListView.Name = "entryListView";
            entryListView.Size = new Size(500, 300);
            entryListView.TabIndex = 0;
            entryListView.UseCompatibleStateImageBehavior = false;
            entryListView.View = View.Details;
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
            // EntryListViewControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(entryListView);
            Name = "EntryListViewControl";
            Size = new Size(500, 300);
            ResumeLayout(false);
        }
        private ListView entryListView;
        private ColumnHeader columnName;
        private ColumnHeader columnSize;
    }
}
