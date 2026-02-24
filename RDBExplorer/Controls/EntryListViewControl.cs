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
        public EntryListViewControl()
        {
            InitializeComponent();
        }


        public void ShowEntries(List<EntryData> entries)
        {
            entryListView.Items.Clear();
            if (entries == null) return;

            foreach (var entry in entries)
            {
                var item = new ListViewItem(entry.Name);
                // Використовуємо ваш утилітний клас Sizer для розміру
                string sizeStr = entry.Data != null ? Sizer.GetDisplayBytes(entry.Data.Length) : "0 B";
                item.SubItems.Add(sizeStr);

                item.Tag = entry; // Зберігаємо об'єкт для подальшого експорту
                entryListView.Items.Add(item);
            }
        }
    }
}
