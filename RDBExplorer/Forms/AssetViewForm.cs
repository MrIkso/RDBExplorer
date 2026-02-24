using Be.Windows.Forms;
using RDBExplorer.Controls;
using RDBExplorer.Core;
using RDBExplorer.Core.Models;
using RDBExplorer.Core.Wrappers;
using RDBExplorer.Utils;

namespace RDBExplorer.Forms
{
    public partial class AssetViewForm : Form
    {

        private HexBox hexBox;
        private string _currentFilePath;
        public IResourceParser CurrentParser { get; private set; }

        public AssetViewForm()
        {
            InitializeComponent();
        }

        public AssetViewForm(RDBEntry entry, byte[] data) : this()
        {
            string name = entry.Name;
            _currentFilePath = name;
            this.Text = $"Asset View - {name}";
            ShowByteData(data);
            UpdateFileSizeStatus();
            InitLoadResource(entry, data);
        }

        void UpdateFileSizeStatus()
        {
            if (this.hexBox.ByteProvider == null)
                this.fileSizeToolStripStatusLabel.Text = string.Empty;
            else
                this.fileSizeToolStripStatusLabel.Text = Sizer.GetDisplayBytes(this.hexBox.ByteProvider.Length);
        }

        private void ShowByteData(byte[] data)
        {
            DynamicByteProvider dynamicByteProvider = new DynamicByteProvider(data);
            hexBox.ByteProvider = dynamicByteProvider;
        }

        void Position_Changed(object sender, EventArgs e)
        {
            this.toolStripStatusLabel.Text = string.Format("Ln {0}    Col {1}",
                hexBox.CurrentLine, hexBox.CurrentPositionInLine);

            string bitPresentation = string.Empty;

            byte? currentByte = hexBox.ByteProvider != null && hexBox.ByteProvider.Length > hexBox.SelectionStart
                ? hexBox.ByteProvider.ReadByte(hexBox.SelectionStart)
                : (byte?)null;

           /* BitInfo bitInfo = currentByte != null ? new BitInfo((byte)currentByte, hexBox.SelectionStart) : null;

            if (bitInfo != null)
            {
                byte currentByteNotNull = (byte)currentByte;
                bitPresentation = string.Format("Bits of Byte {0}: {1}"
                    , hexBox.SelectionStart
                    , bitInfo.ToString()
                    );
            }

            this.bitToolStripStatusLabel.Text = bitPresentation;

            this.bitControl1.BitInfo = bitInfo;*/
        }


        private async void InitLoadResource(RDBEntry entry, byte[] data)
        {
            KTFileType fileType = (KTFileType)entry.TypeInfoKtid;
            await LoadResourceAsync(fileType, data);
        }

        public async Task LoadResourceAsync(KTFileType type, byte[] data)
        {
            try
            {
                IResourceParser? parser = await Task.Run(() => ResourceFactory.GetLoadedParser(type, data));
                if (parser == null)
                {
                    MessageBox.Show($"The file type '{type}' is not supported for specialized parsing. Data will be displayed in Raw Hex.",
                        "Parser Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                CurrentParser = parser;
                resourceViewTabPage.Controls.Clear();
                Control viewer;

                if (parser.IsConvertedToText)
                {
                    var textViewer = new TextViewerControl();
                    textViewer.Dock = DockStyle.Fill;
                    resourceViewTabPage.Controls.Add(textViewer);
                    viewer = textViewer;

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
                    listViewer.OnExportRequested += (sender, entry) =>
                    {
                        ExportEntry(entry);
                    };

                    listViewer.Dock = DockStyle.Fill;
                    resourceViewTabPage.Controls.Add(listViewer);

                    List<EntryData>? entries = await Task.Run(() => parser.GetEntries());
                    listViewer.ShowEntries(entries ?? new List<EntryData>());
                    viewer = listViewer;
                }

                propertyResGrid.SelectedObject = parser.RawModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while loading: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ExportEntry(EntryData entry)
        {
            if (entry.Data == null || entry.Data.Length == 0)
            {
                MessageBox.Show("No data to export.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.FileName = entry.Name;
                sfd.Filter = "All files (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        await Task.Run(() => File.WriteAllBytes(sfd.FileName, entry.Data));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to save file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
