using RDBExplorer.Core;
using RDBExplorer.Core.Formats.ObjectDatabaseFile;
using RDBExplorer.Core.Models;
using RDBExplorer.Services;
using RDBExplorer.Utils;
using System.Data;

namespace RDBExplorer.Controls
{
    public partial class DepedencyListControl : UserControl
    {
        private List<RDBEntry> _fullDepedencyList = new();
        private List<RDBEntry> _filteredDisplayList = new();
        private CancellationTokenSource _filterCts;
        private ArchiveExploler _archiveExplorer;
        private SortOrder _sortOrder = SortOrder.Ascending;
        private int _sortColumn = 0;
        private ContextMenuStrip _contextMenu;
        public event EventHandler<bool> OnDependencyStatusChanged;

        public DepedencyListControl()
        {
            InitializeComponent();
            SetupContextMenu();
        }

        private void DepedencyListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex >= 0 && e.ItemIndex < _filteredDisplayList.Count)
            {
                var entry = _filteredDisplayList[e.ItemIndex];
                e.Item = new ListViewItem(entry.Name);
                e.Item.SubItems.Add(entry.TypeName);
                e.Item.SubItems.Add($"0x{entry.FileKtid:X8}");
                e.Item.SubItems.Add(Sizer.Suffix(entry.FileSize, 2));
                e.Item.Tag = entry;
            }
        }

        public async void BuildDepedencyList(KidsOdbObjectFile kidsOdbObjectFile, ArchiveExploler archiveExploler)
        {
            _archiveExplorer = archiveExploler;
            if (kidsOdbObjectFile == null)
                return;

            _fullDepedencyList = await Task.Run(() =>
            {
                var uniqueKtids = new HashSet<uint>();
                foreach (var obj in kidsOdbObjectFile.Objects)
                {
                    foreach (var col in obj.Columns)
                    {
                        OBJDBPropertyType propertyType = col.Type;
                        bool hasName = !string.IsNullOrEmpty(col.PropertyName);
                        bool shouldCheck = (hasName && col.PropertyName.Contains("Hash", StringComparison.OrdinalIgnoreCase))
                        || (!hasName && propertyType == OBJDBPropertyType.UInt32);

                        if (shouldCheck)
                        {
                            foreach (var val in col.Values)
                            {
                                if (val is uint ktid && ktid > 1000)
                                {
                                    uniqueKtids.Add(ktid);
                                }
                            }
                        }
                    }
                }

                var list = new List<RDBEntry>();
                foreach (var ktid in uniqueKtids)
                {
                    var entry = _archiveExplorer.FindEntryByKtId(ktid);
                    if (entry != null)
                    {
                        list.Add(entry);
                    }
                }
                return list.OrderBy(x => x.Name).ToList();
            });

            bool hasData = _fullDepedencyList.Count > 0;
            ApplyFilter("");
            statusLabel.Text = $"Dependencies: {_fullDepedencyList.Count}";

            OnDependencyStatusChanged?.Invoke(this, hasData);
        }

        public async void ApplyFilter(string filter)
        {
            _filterCts?.Cancel();
            _filterCts = new CancellationTokenSource();
            var token = _filterCts.Token;

            var search = filter.ToLower().Trim();
            bool hasTextFilter = !string.IsNullOrEmpty(search);

            try
            {
                var results = await Task.Run(() =>
                {
                    IEnumerable<RDBEntry> query = _fullDepedencyList;
                    if (hasTextFilter)
                    {
                        query = query.Where(entry =>
                            (entry.Name != null && entry.Name.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                            $"0x{entry.FileKtid:X8}".Contains(search, StringComparison.OrdinalIgnoreCase)
                        );
                    }
                    var list = query.ToList();

                    ApplySortToList(list);

                    return list;
                }, token);

                _filteredDisplayList = results;
                depedencyListView.VirtualListSize = _filteredDisplayList.Count;
                depedencyListView.Invalidate();
            }
            catch (OperationCanceledException) { }
        }

        private void ApplySortToList(List<RDBEntry> list)
        {
            if (_sortColumn < 0) return;

            list.Sort((x, y) =>
            {
                int result = _sortColumn switch
                {
                    0 => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase),
                    1 => string.Compare(x.TypeName, y.TypeName, StringComparison.OrdinalIgnoreCase),
                    2 => x.FileKtid.CompareTo(y.FileKtid),
                    3 => x.FileSize.CompareTo(y.FileSize),
                    _ => 0
                };
                return (_sortOrder == SortOrder.Ascending) ? result : -result;
            });
        }

        private void filterTextBox_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter(filterTextBox.Text);
        }

        private void depedencyListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            _sortOrder = (e.Column == _sortColumn && _sortOrder == SortOrder.Ascending)
                      ? SortOrder.Descending
                      : SortOrder.Ascending;
            _sortColumn = e.Column;

            ApplySortToList(_filteredDisplayList);
            depedencyListView.Invalidate();
        }

        private async void depedencyListView_DoubleClick(object sender, EventArgs e)
        {
            if (depedencyListView.SelectedIndices.Count == 0)
                return;

            int index = depedencyListView.SelectedIndices[0];
            if (index < 0 || index >= _filteredDisplayList.Count)
                return;

            var item = _filteredDisplayList[index];
            await AssetLauncher.OpenEntry(item, _archiveExplorer);
        }

        private async void extractAllDataBtn_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string outputDir = fbd.SelectedPath;
            int total = _fullDepedencyList.Count;

            if (total == 0)
            {
                return;
            }
            extractAllDataBtn.Enabled = false;
            bool withName = SettingsService.Instance.Config.ExportWithNames;
            this.Cursor = Cursors.WaitCursor;
            try
            {
                await Task.Run(() =>
                {
                    for (int i = 0; i < total; i++)
                    {
                        var entry = _fullDepedencyList[i];
                        _archiveExplorer.Extract(entry, outputDir, withName);
                        int current = i + 1;
                        this.Invoke(new Action(() =>
                        {
                            statusLabel.Text = $"Extracting: {current} / {total} ({entry.Name})";
                        }));
                    }
                });

                MessageBox.Show($"Successfully extracted {total} files.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                extractAllDataBtn.Enabled = true;
                this.Cursor = Cursors.Default;
                statusLabel.Text = $"Dependencies: {total}";
            }
        }

        private void SetupContextMenu()
        {
            _contextMenu = new ContextMenuStrip();
            var copyNameItem = new ToolStripMenuItem("Copy Name");
            copyNameItem.Click += (s, e) => CopySelectedSubItemsToClipboard(0);

            var copyTypeItem = new ToolStripMenuItem("Copy Type");
            copyTypeItem.Click += (s, e) => CopySelectedSubItemsToClipboard(1);

            var copyHashItem = new ToolStripMenuItem("Copy Hash");
            copyHashItem.Click += (s, e) => CopySelectedSubItemsToClipboard(2);

            var copyContainerPathItem = new ToolStripMenuItem("Copy Container Name");
            copyContainerPathItem.Click += (s, e) => CopySelectedSubItemsToClipboard(3);

            _contextMenu.Items.Add(copyNameItem);
            _contextMenu.Items.Add(copyTypeItem);
            _contextMenu.Items.Add(copyHashItem);
            _contextMenu.Items.Add(copyContainerPathItem);

            depedencyListView.ContextMenuStrip = _contextMenu;
        }

        private void CopySelectedSubItemsToClipboard(int mode)
        {
            if (depedencyListView.SelectedIndices.Count == 0)
                return;

            var item = _filteredDisplayList[depedencyListView.SelectedIndices[0]];

            string textToCopy = string.Empty;
            if (mode == 0)
            {
                textToCopy = item.Name;
            }
            else if (mode == 1)
            {
                textToCopy = $"0x{item.TypeInfoKtid:X8}";
            }
            else if (mode == 2)
            {
                textToCopy = $"0x{item.FileKtid:X8}";
            }
            else if (mode == 3)
            {
                textToCopy = item.Location.ContainerPath;
            }
            if (!string.IsNullOrEmpty(textToCopy))
            {
                Clipboard.SetText(textToCopy);
            }
        }
    }
}
