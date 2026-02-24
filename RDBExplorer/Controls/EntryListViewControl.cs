using RDBExplorer.Core;
using RDBExplorer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RDBExplorer.Controls
{
    public partial class EntryListViewControl : UserControl
    {
        public event EventHandler<EntryData>? OnExportRequested;

        public EntryListViewControl()
        {
            InitializeComponent();
            SetupContextMenu();
        }

        private void SetupContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            var exportItem = new ToolStripMenuItem("Export Selected...");
            exportItem.Click += (s, e) =>
            {
                if (entryListView.SelectedItems.Count > 0)
                {
                    var entry = entryListView.SelectedItems[0].Tag as EntryData;
                    if (entry != null)
                    {
                        OnExportRequested?.Invoke(this, entry);
                    }
                }
            };

            menu.Items.Add(exportItem);
            entryListView.ContextMenuStrip = menu;
        }

        public void ShowEntries(List<EntryData> entries)
        {
            entryListView.Items.Clear();
            if (entries == null)
                return;

            foreach (var entry in entries)
            {
                var item = new ListViewItem(entry.Name);
                string sizeStr = entry.Data != null ? Sizer.GetDisplayBytes(entry.Data.Length) : "0 B";
                item.SubItems.Add(sizeStr);

                item.Tag = entry;
                entryListView.Items.Add(item);
            }
        }
    }
}
