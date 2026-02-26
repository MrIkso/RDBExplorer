using RDBExplorer.Core;
using RDBExplorer.Core.Formats.LangFile;
using RDBExplorer.Core.Formats.LayeredFile;
using RDBExplorer.Core.Models;
using RDBExplorer.Services;
using RDBExplorer.Utils;
using System.Collections.Concurrent;
using System.Text;
using static RDBExplorer.Utils.ListViewExtentions;

namespace RDBExplorer.Forms
{
    public partial class ExplolerForm : Form
    {
        private ArchiveExploler _archiveExploler;
        private ContextMenuStrip _contextMenu;
        private int _sortColumn = -1;
        private SortOrder _sortOrder = SortOrder.Ascending;
        private string _currentlyOpenedFile = string.Empty;
        private List<RDBEntry> _filteredDisplayList = new();
        private CancellationTokenSource _filterCts;
        private HashSet<long> _modifiedKtids = new();
        private string _version = "1.0.1";

        public ExplolerForm()
        {
            InitializeComponent();
            SetupListView();
            SetupContextMenu();
            archiveList.ColumnClick += ArchiveList_ColumnClick;
        }

        private void SetupListView()
        {
            archiveList.View = View.Details;
            archiveList.FullRowSelect = true;
            archiveList.GridLines = true;
            archiveList.Columns.Clear();
            archiveList.Columns.Add("Name", 250);
            archiveList.Columns.Add("Type", 200);
            archiveList.Columns.Add("Size", 100);
            archiveList.Columns.Add("Container", 200);
            archiveList.Columns.Add("Hash", 200);
            archiveList.VirtualMode = true;
            archiveList.VirtualListSize = 0;
            archiveList.RetrieveVirtualItem += ArchiveList_RetrieveVirtualItem;
        }

        private void SetupContextMenu()
        {
            _contextMenu = new ContextMenuStrip();

            var extractItem = new ToolStripMenuItem("Extract Selected");
            extractItem.Click += async (s, e) => await ExtractSelectedFiles();
            var renameItem = new ToolStripMenuItem("Rename File");
            renameItem.Click += (s, e) => RenameSelectedFile();

            var copyNameItem = new ToolStripMenuItem("Copy Name");
            copyNameItem.Click += (s, e) => CopySelectedSubItemsToClipboard(0);

            var copyTypeItem = new ToolStripMenuItem("Copy Type");
            copyTypeItem.Click += (s, e) => CopySelectedSubItemsToClipboard(1);

            var copyHashItem = new ToolStripMenuItem("Copy Hash");
            copyHashItem.Click += (s, e) => CopySelectedSubItemsToClipboard(2);

            var copyContainerPathItem = new ToolStripMenuItem("Copy Container Name");
            copyContainerPathItem.Click += (s, e) => CopySelectedSubItemsToClipboard(3);
            var injectData = new ToolStripMenuItem("Inject New Data");
            injectData.Click += async (s, e) => await UpdateEntryData();

            _contextMenu.Items.Add(extractItem);
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add(renameItem);
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add(copyNameItem);
            _contextMenu.Items.Add(copyTypeItem);
            _contextMenu.Items.Add(copyHashItem);
            _contextMenu.Items.Add(copyContainerPathItem);
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add(injectData);

            archiveList.ContextMenuStrip = _contextMenu;
        }

        private void CopySelectedSubItemsToClipboard(int mode)
        {
            if (archiveList.SelectedIndices.Count == 0)
                return;

            var item = _filteredDisplayList[archiveList.SelectedIndices[0]];

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

        private void ArchiveList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            _sortOrder = (e.Column == _sortColumn && _sortOrder == SortOrder.Ascending)
                         ? SortOrder.Descending
                         : SortOrder.Ascending;
            _sortColumn = e.Column;

            _filteredDisplayList.Sort((x, y) =>
            {
                int result = e.Column switch
                {
                    0 => string.Compare(x.Name, y.Name),
                    1 => string.Compare(x.TypeName, y.TypeName),
                    2 => x.FileSize.CompareTo(y.FileSize),
                    3 => string.Compare(x.Location.ContainerPath, y.Location.ContainerPath),
                    4 => x.FileKtid.CompareTo(y.FileKtid),
                    _ => 0
                };
                return (_sortOrder == SortOrder.Ascending) ? result : -result;
            });

            archiveList.Invalidate();
        }

        private void ArchiveList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex >= 0 && e.ItemIndex < _filteredDisplayList.Count)
            {
                var entry = _filteredDisplayList[e.ItemIndex];
                string displayName = !string.IsNullOrEmpty(entry.Name) ? entry.Name : $"0x{entry.FileKtid:X8}";

                ListViewItem lvi = new ListViewItem(displayName);
                lvi.SubItems.Add(entry.TypeName ?? "");
                lvi.SubItems.Add(Sizer.Suffix(entry.FileSize, 2));
                lvi.SubItems.Add(entry.Location.ContainerPath ?? "");
                lvi.SubItems.Add($"0x{entry.FileKtid:X8}");
                lvi.Tag = entry;
                if (_modifiedKtids.Contains(entry.FileKtid))
                {
                    lvi.BackColor = Color.LightGreen;
                }
                e.Item = lvi;
            }
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select RDB Database File";
                ofd.Filter = "RDB Files|*.rdb";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    toolStripStatusLabel.Text = "Loading RDB and RDX index...";

                    await Task.Run(() =>
                    {
                        _currentlyOpenedFile = ofd.FileName;
                        SetTitle();
                        _archiveExploler = null;
                        _archiveExploler = new ArchiveExploler();
                        _archiveExploler.Browse(_currentlyOpenedFile);
                    });

                    _modifiedKtids.Clear();
                    _filteredDisplayList.Clear();
                    extractAllToolStripMenuItem.Enabled = true;
                    grabNamesToolStripMenuItem.Enabled = true;
                    grabAllMagicHeadersToolStripMenuItem.Enabled = true;

                    PopulateTypeFilter();
                    ShowFiles();
                }
            }
        }

        private async void ShowFiles(string filter = "")
        {
            if (_archiveExploler?.RDBEntries == null)
                return;

            _filterCts?.Cancel();
            _filterCts = new CancellationTokenSource();
            var token = _filterCts.Token;

            toolStripStatusLabel.Text = "Filtering...";

            var search = filter.ToLower().Trim();
            var checkedTypes = new HashSet<string>(typeFilterComboBox.CheckedItems.Cast<string>());
            bool hasTypeFilter = checkedTypes.Count > 0;
            bool hasTextFilter = !string.IsNullOrEmpty(search);

            try
            {
                var results = await Task.Run(() =>
                {
                    IEnumerable<RDBEntry> query = _archiveExploler.RDBEntries;
                    if (hasTypeFilter)
                    {
                        query = query.Where(e => checkedTypes.Contains(e.TypeName ?? "Unknown"));
                    }
                    if (hasTextFilter)
                    {
                        query = query.Where(entry =>
                            (entry.Name != null && entry.Name.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                            (entry.TypeName != null && entry.TypeName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                            $"0x{entry.FileKtid:X8}".Contains(search, StringComparison.OrdinalIgnoreCase)
                        );
                    }

                    return query.ToList();
                }, token);

                _filteredDisplayList = results;
                archiveList.VirtualListSize = _filteredDisplayList.Count;
                archiveList.Invalidate();

                toolStripStatusLabel.Text = $"Files shown: {_filteredDisplayList.Count} / Total: {_archiveExploler.RDBEntries.Count}";
            }
            catch (OperationCanceledException) { }
        }

        private async Task UpdateEntryData()
        {
            if (archiveList.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Select an entry to update", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int selectedIndex = archiveList.SelectedIndices[0];
            var entry = _filteredDisplayList[selectedIndex];

            using var ofd = new OpenFileDialog();
            ofd.Title = "Select file to update entry data";
            ofd.Filter = "All files|*.*";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() != DialogResult.OK) return;

            string sourceFilePath = ofd.FileName;
            string errorMessage = null;

            toolStripStatusLabel.Text = "Injecting data...";

            await Task.Run(() =>
            {
                try
                {
                    byte[] data = File.ReadAllBytes(sourceFilePath);
                    var result = _archiveExploler.InjectData(entry, data, _currentlyOpenedFile);
                    if (!result.IsSuccessed)
                    {
                        errorMessage = result.ErrorMessage;
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            });

            if (errorMessage != null)
            {
                MessageBox.Show($"Inject failed: {errorMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                _modifiedKtids.Add(entry.FileKtid);
                archiveList.RedrawItems(selectedIndex, selectedIndex, false);

                toolStripStatusLabel.Text = "Inject successful.";
                MessageBox.Show($"Successfully injected! Updated: {Path.GetFileName(_currentlyOpenedFile)}\nContainer: {entry.Location.ContainerPath}",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task ExtractFiles(bool extractAll)
        {
            List<RDBEntry> entriesToProcess;

            if (extractAll)
            {
                entriesToProcess = _archiveExploler.RDBEntries;
            }
            else
            {
                entriesToProcess = archiveList.SelectedIndices.Cast<int>().Select(idx => _filteredDisplayList[idx]).ToList();
            }

            if (entriesToProcess.Count == 0) return;

            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string outputDir = fbd.SelectedPath;
            int total = entriesToProcess.Count;

            _contextMenu.Enabled = false;
            archiveList.Enabled = false;
            filterBox.Enabled = false;
            typeFilterComboBox.Enabled = false;
            progressBarOperation.Value = 0;
            progressBarOperation.Maximum = total;

            var progress = new Progress<int>(count =>
            {
                progressBarOperation.Value = count;
                int percent = (int)((double)count / total * 100);
                toolStripStatusLabel.Text = $"Extracting: {count} / {total} ({percent}%)";
            });

            var errors = new ConcurrentBag<string>();

            await Task.Run(() =>
            {
                int processedCount = 0;
                foreach (var entry in entriesToProcess)
                {
                    try
                    {
                        var result = _archiveExploler.Extract(entry, outputDir, exportWitchNameToolStripMenuItem.Checked);
                        if (!result.IsSuccessed)
                        {
                            errors.Add($"[{entry.FileKtid:X8}] {result.ErrorMessage}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"[{entry.FileKtid:X8}] Critical error: {ex.Message}");
                    }

                    processedCount++;
                    ((IProgress<int>)progress).Report(processedCount);
                }
            });

            _contextMenu.Enabled = true;
            archiveList.Enabled = true;
            filterBox.Enabled = true;
            typeFilterComboBox.Enabled = true;
            toolStripStatusLabel.Text = $"Finished. Errors: {errors.Count}";

            if (errors.Count > 0)
            {
                string errorLog = string.Join(Environment.NewLine, errors.Take(10));
                if (errors.Count > 10) errorLog += Environment.NewLine + "...and more.";

                MessageBox.Show($"Completed with {errors.Count} errors.{Environment.NewLine}{Environment.NewLine}Log:{Environment.NewLine}{errorLog}",
                    "Finished", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show($"Successfully extracted {total} files!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task ExtractSelectedFiles()
        {
            await ExtractFiles(false);
        }

        private async Task GrabNames(bool isGrabMagic)
        {
            if (_archiveExploler?.RDBEntries == null)
                return;

            NameGrabber nameGrabber = new NameGrabber();
            int total = _archiveExploler.RDBEntries.Count;
            int current = 0;
            int grabbedCount = 0;

            toolStripStatusLabel.Text = "Grabbing internal names...";

            await Task.Run(() =>
            {
                foreach (var entry in _archiveExploler.RDBEntries)
                {
                    current++;

                    if (isGrabMagic)
                    {
                        byte[]? data = _archiveExploler.GetEntryData(entry);

                        if (data != null)
                        {
                            nameGrabber.Load(data, entry.FileKtid, true);
                            grabbedCount++;
                        }
                    }
                    else
                    {
                        bool isRelevant = entry.Name != null && (
                            entry.Name.EndsWith(".g1cox") ||
                            entry.Name.EndsWith(".g1mx") ||
                            entry.Name.EndsWith(".g1p"));

                        if (isRelevant)
                        {
                            byte[]? data = _archiveExploler.GetEntryData(entry);

                            if (data != null)
                            {
                                nameGrabber.Load(data, entry.FileKtid, false);
                                grabbedCount++;
                            }
                        }
                    }

                    if (current % 50 == 0)
                    {
                        this.Invoke(new Action(() =>
                        {
                            toolStripStatusLabel.Text = $"Scanning: {current}/{total} | Found: {nameGrabber.GrabbedNames.Count}";
                        }));
                    }
                }

                string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "grabbed_names.csv");
                nameGrabber.SaveToFile(savePath);
            });

            MessageBox.Show($"Done! Grabbed {nameGrabber.GrabbedNames.Count} names.\nSaved to: grabbed_names.csv",
                            "Name Grabber", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveDictionary(Dictionary<uint, string> dictionary, string path)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<uint, string> kv in dictionary)
            {
                sb.Append($"0x{kv.Key:X8}, {kv.Value}");
                sb.AppendLine();
            }

            File.WriteAllText(path, sb.ToString());
        }

        private async void grabNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await GrabNames(false);
        }

        private async void upackBinArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select archive file";
                ofd.Filter = "Supported files (*.bin, *.lnk)|*.bin;*.lnk";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string binArchivePath = ofd.FileName;

                    if (!File.Exists(binArchivePath))
                        return;

                    string archiveName = Path.GetFileNameWithoutExtension(binArchivePath);
                    if (!archiveName.ToLower().StartsWith("archive"))
                    {
                        MessageBox.Show("Select correct bin archive with name like archive_00.bin (*.lnk)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string rootDir = Path.GetDirectoryName(binArchivePath);
                    string unpackArchivePath = Path.Combine(rootDir, $"{archiveName}_extracted");

                    try
                    {
                        this.Cursor = Cursors.WaitCursor;
                        await Task.Run(() =>
                        {
                            LFMBParser lFMBParser = new LFMBParser();
                            lFMBParser.UnpackBinArchive(binArchivePath, unpackArchivePath);
                        });
                        MessageBox.Show("Unpacking completed successfully!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error during unpacking: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        private async void packBinArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select archive manifest file";
                ofd.Filter = "Manifest file |*.json";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string manifestArchivePath = ofd.FileName;

                    if (!File.Exists(manifestArchivePath))
                        return;

                    string rootDir = Path.GetDirectoryName(manifestArchivePath);

                    string packArchivePath = Path.Combine(rootDir, "Packed");
                    if (!Directory.Exists(packArchivePath))
                    {
                        Directory.CreateDirectory(packArchivePath);
                    }

                    try
                    {
                        this.Cursor = Cursors.WaitCursor;
                        await Task.Run(() =>
                        {
                            LFMBParser lFMBParser = new LFMBParser();
                            lFMBParser.PackBinArchive(manifestArchivePath, packArchivePath);
                        });
                        MessageBox.Show("Packing completed successfully!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error during packing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        private async void unpackLocalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string selectedDir = fbd.SelectedPath;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                await Task.Run(() =>
                {
                    BatchLangProcessor.ParseFromDir(selectedDir, selectedDir);
                });
                MessageBox.Show("Locales unpacked to CSV successfully!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void packLocalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string selectedDir = fbd.SelectedPath;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                await Task.Run(() =>
                {
                    BatchLangProcessor.ConvertToBinary(selectedDir, selectedDir);
                });
                MessageBox.Show("Locales converted to binary successfully!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await ExtractFiles(true);
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            ShowFiles(filterBox.Text);
        }


        private async void grabAllMagicHeadersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await GrabNames(true);
        }

        private void SetTitle()
        {
            this.Invoke(new Action(() =>
            {
                if (string.IsNullOrEmpty(_currentlyOpenedFile))
                {
                    this.Text = "RDB Explorer";
                }
                else
                {
                    this.Text = $"RDB Explorer - {Path.GetFileName(_currentlyOpenedFile)}";
                }
            }));
        }

        private void ExplolerForm_Load(object sender, EventArgs e)
        {
            SetTitle();
            exportWitchNameToolStripMenuItem.Checked = SettingsService.Instance.Config.ExportWithNames;
        }

        private void g1TTexureToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new G1ToolForm().Show();
        }


        private async void archiveList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (archiveList.SelectedIndices.Count == 0)
            {
                return;
            }

            int selectedIndex = archiveList.SelectedIndices[0];
            if (selectedIndex < 0 || selectedIndex >= _filteredDisplayList.Count)
            {
                return;
            }

            var item = _filteredDisplayList[selectedIndex];
            await AssetLauncher.OpenEntry(item, _archiveExploler);
        }

        private void PopulateTypeFilter()
        {
            if (_archiveExploler?.RDBEntries == null)
            {
                return;
            }
            typeFilterComboBox.Text = "Filter by Type";
            var uniqueTypes = _archiveExploler.RDBEntries.Select(e => e.TypeName ?? "Unknown").Distinct().OrderBy(t => t).ToArray();

            this.Invoke(new Action(() =>
            {
                typeFilterComboBox.Items.Clear();
                foreach (var type in uniqueTypes)
                {
                    typeFilterComboBox.Items.Add(type);
                }
            }));
        }

        private void typeFilterComboBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                ShowFiles(filterBox.Text);
            }));
        }

        private void SelectAllItems()
        {
            if (archiveList.VirtualListSize == 0)
            {
                return;
            }

            archiveList.Focus();

            LVITEM lvi = new LVITEM();
            lvi.state = LVIS_SELECTED;
            lvi.stateMask = LVIS_SELECTED;

            SendMessage(archiveList.Handle, LVM_SETITEMSTATE, -1, ref lvi);
        }

        private void archiveList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                SelectAllItems();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"RDB Explorer - by MrIkso\n\nVersion {_version}");
        }

        private void RenameSelectedFile()
        {
            if (archiveList.SelectedIndices.Count == 0)
            {
                return;
            }

            int index = archiveList.SelectedIndices[0];
            var entry = _filteredDisplayList[index];

            string newName = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter new name for this file:",
                "Rename File",
                entry.Name);

            if (!string.IsNullOrEmpty(newName) && newName != entry.Name)
            {
                TypeIDHelper.Instance.UpdateName(entry.FileKtid, newName);
                foreach (var item in _archiveExploler.RDBEntries.Where(e => e.FileKtid == entry.FileKtid))
                {
                    item.Name = newName;
                }
                archiveList.Invalidate();
            }
        }

        private void exportWitchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool newState = !exportWitchNameToolStripMenuItem.Checked;
            exportWitchNameToolStripMenuItem.Checked = newState;
            SettingsService.Instance.Config.ExportWithNames = newState;
            SettingsService.Instance.Save();
        }
    }
}