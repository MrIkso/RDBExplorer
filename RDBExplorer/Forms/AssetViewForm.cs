using Be.Windows.Forms;
using RDBExplorer.Controls;
using RDBExplorer.Core;
using RDBExplorer.Core.Models;
using RDBExplorer.Utils;

namespace RDBExplorer.Forms
{
    public partial class AssetViewForm : Form
    {
        private HexBox hexBox;
        private string _currentFileName;
        private byte[] _rawData;
        private RDBEntry _entry;
        private bool _isResourceLoaded = false;
        private TextViewerControl textViewer;
        public IResourceParser CurrentParser { get; private set; }

        public AssetViewForm()
        {
            InitializeComponent();
        }

        public AssetViewForm(RDBEntry entry, byte[] data) : this()
        {
            _entry = entry;
            _rawData = data;
            _currentFileName = entry.Name;
            this.Text = $"Asset View - {_currentFileName}";

            ShowByteData(data);
            UpdateFileSizeStatus();
        }

        private async void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {

            bool isParsedViewActive = tabControl.SelectedTab == resourceViewTabPage;
            bool isDetailsViewActive = tabControl.SelectedTab == resourceDetailsTabPage;

            if ((isParsedViewActive || isDetailsViewActive) && !_isResourceLoaded)
            {
                KTFileType fileType = (KTFileType)_entry.TypeInfoKtid;
                await LoadResourceAsync(fileType, _rawData);
            }
        }

        void Position_Changed(object sender, EventArgs e)
        {
            if (hexBox == null)
            {
                this.toolStripStatusLabel.Text = string.Empty;
                return;
            }

            var provider = hexBox.ByteProvider;
            long offset = 0;
            long selectionLength = 0;

            if (provider != null)
            {
                offset = Math.Max(0, hexBox.SelectionStart);
                selectionLength = Math.Max(0, hexBox.SelectionLength);
            }



            // string byteInfo = currentByte.HasValue ? $"  Byte: 0x{currentByte.Value:X2} ({bitPresentation})" : string.Empty;
            this.toolStripStatusLabel.Text = string.Format(
                "Offset: 0x{0:X8}  Selected: {1}",
                offset, selectionLength);
        }

        public async Task LoadResourceAsync(KTFileType type, byte[] data)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                IResourceParser? parser = await Task.Run(() => ResourceFactory.GetLoadedParser(type, data));

                if (parser == null)
                {
                    Label placeholder = new Label();
                    placeholder.Text = $"Parser for '{type}' not found.\nOnly Raw Hex view is available.";
                    placeholder.TextAlign = ContentAlignment.MiddleCenter;
                    placeholder.Dock = DockStyle.Fill;
                    placeholder.ForeColor = Color.Gray;
                    placeholder.Font = new Font(this.Font.FontFamily, 12, FontStyle.Regular);

                    resourceViewTabPage.Controls.Add(placeholder);

                    _isResourceLoaded = true;
                    saveParsedResultToolStripMenuItem.Enabled = false;
                    return;
                }

                CurrentParser = parser;
                _isResourceLoaded = true;

                textViewer = null;
                resourceViewTabPage.Controls.Clear();

                if (parser.IsConvertedToText)
                {
                    textViewer = new TextViewerControl();
                    textViewer.Dock = DockStyle.Fill;
                    resourceViewTabPage.Controls.Add(textViewer);
                    saveParsedResultToolStripMenuItem.Enabled = true;

                    string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");
                    using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                    {
                        await parser.SerializeJsonToStreamAsync(fs);
                    }
                    await textViewer.LoadFromFileAsync(tempFile);
                }
                else
                {
                    var listViewer = new EntryListViewControl();
                    listViewer.OnExportRequested += (sender, entry) => ExportEntry(entry);
                    listViewer.Dock = DockStyle.Fill;
                    resourceViewTabPage.Controls.Add(listViewer);

                    List<EntryData>? entries = await Task.Run(() => parser.GetEntries());
                    listViewer.ShowEntries(entries ?? new List<EntryData>());
                    saveParsedResultToolStripMenuItem.Enabled = false;
                }

                propertyResGrid.SelectedObject = parser.RawModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while loading: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }


        private async void saveAsRawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.FileName = Path.GetFileName(_currentFileName);
                sfd.Filter = "All files (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        await File.WriteAllBytesAsync(sfd.FileName, _rawData);
                        MessageBox.Show("Raw data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to save raw data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void saveParsedResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentParser == null || !CurrentParser.IsConvertedToText)
                return;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.FileName = Path.ChangeExtension(_currentFileName, ".json");
                sfd.Filter = "JSON file (*.json)|*.json|All files (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;
                        using (var fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                        {
                            await CurrentParser.SerializeJsonToStreamAsync(fs);
                        }
                        MessageBox.Show("JSON result saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to save JSON: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }


        private void ShowByteData(byte[] data)
        {
            DynamicByteProvider dynamicByteProvider = new DynamicByteProvider(data);
            hexBox.ByteProvider = dynamicByteProvider;
            hexBox.ReadOnly = true;
        }

        void UpdateFileSizeStatus()
        {
            if (this.hexBox.ByteProvider == null)
            {
                this.fileSizeToolStripStatusLabel.Text = string.Empty;
            }
            else
            {
                this.fileSizeToolStripStatusLabel.Text = Sizer.GetDisplayBytes(this.hexBox.ByteProvider.Length);
            }
        }

        private async void ExportEntry(EntryData entry)
        {
            if (entry.Data == null || entry.Data.Length == 0)
            {
                return;
            }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.FileName = entry.Name;
                sfd.Filter = "All files (*.*)|*.*";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    await File.WriteAllBytesAsync(sfd.FileName, entry.Data);
                }
            }
        }

        private void AssetViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void hexBox_SelectionLengthChanged(object sender, EventArgs e)
        {
            Position_Changed(sender, e);
        }
    }
}