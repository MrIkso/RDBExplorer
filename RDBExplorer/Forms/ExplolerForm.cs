using RDBExplorer.Core;
using RDBExplorer.Core.LangFile;
using RDBExplorer.Core.LayeredFile;
using RDBExplorer.Core.Models;
using RDBExplorer.Utils;
using System.Collections.Concurrent;

namespace RDBExplorer.Forms
{
    public partial class ExplolerForm : Form
    {
        private ArchiveExploler _archiveExploler;
        private ContextMenuStrip _contextMenu;
        private int _sortColumn = -1;
        private SortOrder _sortOrder = SortOrder.Ascending;
        private string _currentlyOpenedFile = string.Empty;

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
            archiveList.Columns.Add("Type", 120);
            archiveList.Columns.Add("Size", 100);
            archiveList.Columns.Add("Container", 200);
        }

        private void SetupContextMenu()
        {
            _contextMenu = new ContextMenuStrip();

            var extractItem = new ToolStripMenuItem("Extract Selected");
            extractItem.Click += async (s, e) => await ExtractSelectedFiles();
            var copyNameItem = new ToolStripMenuItem("Copy Name");
            copyNameItem.Click += (s, e) => CopySelectedSubItemsToClipboard(0);
            var copyTypeItem = new ToolStripMenuItem("Copy Type");
            copyTypeItem.Click += (s, e) => CopySelectedSubItemsToClipboard(1);
            var copyContainerPathItem = new ToolStripMenuItem("Copy Container Name");
            copyContainerPathItem.Click += (s, e) => CopySelectedSubItemsToClipboard(2);
            var injectData = new ToolStripMenuItem("Inject New Data");
            injectData.Click += async (s, e) => await UpdateEntryData();

            _contextMenu.Items.Add(extractItem);
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add(copyNameItem);
            _contextMenu.Items.Add(copyTypeItem);
            _contextMenu.Items.Add(copyContainerPathItem);
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add(injectData);

            archiveList.ContextMenuStrip = _contextMenu;
        }

        private void CopySelectedSubItemsToClipboard(int mode)
        {
            if (archiveList.SelectedItems.Count == 0)
                return;

            var item = archiveList.SelectedItems.Cast<ListViewItem>().First();

            RDBEntry dBEntry = item.Tag as RDBEntry;

            string textToCopy = string.Empty;
            if (mode == 0)
            {
                textToCopy = dBEntry.Name;
            }
            else if (mode == 1)
            {
                textToCopy = dBEntry.TypeInfoKtid.ToString("X");
            }
            else if (mode == 2)
            {
                textToCopy = dBEntry.Location.ContainerPath;
            }
            if (!string.IsNullOrEmpty(textToCopy))
            {
                Clipboard.SetText(textToCopy);
            }
        }

        private void ArchiveList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _sortColumn)
            {
                _sortOrder = (_sortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                _sortColumn = e.Column;
                _sortOrder = SortOrder.Ascending;
            }
            archiveList.ListViewItemSorter = new ListViewItemComparer(e.Column, _sortOrder);
            archiveList.Sort();
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

                    ShowFiles();
                }
            }
        }

        private void ShowFiles(string filter = "")
        {
            if (_archiveExploler?.RDBEntries == null)
                return;

            archiveList.BeginUpdate();
            archiveList.Items.Clear();

            string search = filter.ToLower().Trim();

            var filteredEntries = _archiveExploler.RDBEntries.Where(entry =>
            {
                if (string.IsNullOrEmpty(search))
                    return true;

                if (entry.Name != null && entry.Name.ToLower().Contains(search))
                {
                    return true;
                }

                string hexId = $"0x{entry.FileKtid:X8}".ToLower();
                if (hexId.Contains(search))
                {
                    return true;
                }

                if (entry.TypeName != null && entry.TypeName.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            });

            var items = filteredEntries.Select(entry =>
            {
                string displayName = !string.IsNullOrEmpty(entry.Name) ? entry.Name : $"0x{entry.FileKtid:X8}";
                ListViewItem item = new ListViewItem(displayName);
                item.SubItems.Add(entry.TypeName);

                var sizeSubItem = new ListViewItem.ListViewSubItem(item, Sizer.Suffix(entry.FileSize, 2));
                sizeSubItem.Tag = entry.FileSize;
                item.SubItems.Add(sizeSubItem);

                item.SubItems.Add(entry.Location.ContainerPath);
                item.Tag = entry;
                return item;
            }).ToArray();

            archiveList.Items.AddRange(items);
            archiveList.EndUpdate();

            toolStripStatusLabel.Text = $"Files shown: {items.Length} / Total: {_archiveExploler.RDBEntries.Count}";
        }

        private async Task UpdateEntryData()
        {
            if (archiveList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select an entry to update", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using var ofd = new OpenFileDialog();
            ofd.Title = "Select file to update entry data";
            ofd.Filter = "All files|*.*";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            ListViewItem selectedItem = archiveList.SelectedItems[0];
            var entry = (RDBEntry)selectedItem.Tag;
            var errors = new ConcurrentBag<string>();

            await Task.Run(() =>
            {

                byte[] data = File.ReadAllBytes(ofd.FileName);
                var result = _archiveExploler.InjectData(entry, data, _currentlyOpenedFile);
                if (!result.IsSuccessed)
                {
                    errors.Add($"[{entry.FileKtid:X8}] {result.ErrorMessage}");
                }
            });
            if (errors.Count > 0)
            {
                MessageBox.Show($"Inject with {errors.Count} errors.", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                selectedItem.SubItems[2].Text = Sizer.Suffix(entry.FileSize, 2);
                selectedItem.SubItems[2].Tag = entry.FileSize;
                selectedItem.SubItems[3].Text = entry.Location.ContainerPath;
                selectedItem.BackColor = Color.LightGreen;

                MessageBox.Show($"Successfully inject files! Updated: {Path.GetFileName(_currentlyOpenedFile)}\n{entry.Location.ContainerPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                entriesToProcess = archiveList.SelectedItems.Cast<ListViewItem>()
                    .Select(i => (RDBEntry)i.Tag).ToList();
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
                        var result = _archiveExploler.Extract(entry, outputDir);
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
            toolStripStatusLabel.Text = $"Finished. Errors: {errors.Count}";

            if (errors.Count > 0)
            {
                MessageBox.Show($"Completed with {errors.Count} errors.", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            ShowFiles(toolStripTextBox1.Text);
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
        }

        private void g1TTexureToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new G1ToolForm().Show();
        }

        private void archiveList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = archiveList.SelectedItems.Cast<ListViewItem>().First();
            RDBEntry dBEntry = item.Tag as RDBEntry;
            if (dBEntry != null)
            {
                if (dBEntry.TypeInfoKtid == 0xAD57EBBA || dBEntry.TypeInfoKtid == 0xAFBEC60C) {
                    byte[]? entryData = _archiveExploler.GetEntryData(dBEntry);
                    if (entryData != null)
                    {
                        G1ToolForm g1ToolForm = new G1ToolForm(dBEntry.Name, entryData);
                        g1ToolForm.Show();
                    }
                }
            }
        }
    }
}