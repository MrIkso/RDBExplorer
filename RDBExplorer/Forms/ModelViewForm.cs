using Metanoia.Rendering;
using RDBExplorer.Core.Formats.G1M;
using RDBExplorer.Core.Models;

namespace RDBExplorer.Forms
{
    public partial class ModelViewForm : Form
    {
        public ModelViewer ModelViewer;
        private string _currentFileName;
        private byte[] _rawData;

        public ModelViewForm()
        {
            InitializeComponent();
            ModelViewer = new ModelViewer();
            ModelViewer.Dock = DockStyle.Fill;
            this.Controls.Add(ModelViewer);
        }


        public ModelViewForm(string entryName, byte[] data) : this()
        {
            _currentFileName = entryName;
            _rawData = data;

            _ = LoadModelAsync(data);
            UpdateTitle();
        }

        public ModelViewForm(RDBEntry entry, byte[] data) : this()
        {
            _currentFileName = entry.Name;
            _rawData = data;

            _ = LoadModelAsync(data);
            UpdateTitle();
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Model files (*.g1m)|*.g1m";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string selectedFile = ofd.FileName;
                _currentFileName = selectedFile;
                UpdateTitle();

                byte[] data = await Task.Run(() => File.ReadAllBytes(selectedFile));
                
                await LoadModelAsync(data);
            }
        }


        private async Task LoadModelAsync(byte[] data)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var genericModel = await Task.Run(() =>
                {
                    G1MImporter g1MImporter = new G1MImporter();
                    g1MImporter.Open(data);
                    return g1MImporter.ToGenericModel();
                });

                ModelViewer.SetModel(genericModel);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading model: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                FinalizeTitle();
            }
        }

        private void UpdateTitle()
        {
            if (_currentFileName != null)
            {
                this.Text = $"Model View (Loading...) - {Path.GetFileName(_currentFileName)}";
            }
            else
            {
                this.Text = "Model View";
            }
        }

        private void FinalizeTitle()
        {
            if (_currentFileName != null)
            {
                this.Text = $"Model View - {Path.GetFileName(_currentFileName)}";
            }
        }

        private void ModelViewForm_Load(object sender, EventArgs e)
        {
            UpdateTitle();
        }
    }
}
