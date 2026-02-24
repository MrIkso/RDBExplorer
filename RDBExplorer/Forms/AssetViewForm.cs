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
                    MessageBox.Show($"This file type: {type} not supported!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    string? jsonData = await Task.Run(() => parser.GetJsonData());
                    textViewer.SetText(jsonData ?? "// No data");
                    viewer = textViewer;
                }
                else
                {
                    var listViewer = new EntryListViewControl();
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
    }
}
