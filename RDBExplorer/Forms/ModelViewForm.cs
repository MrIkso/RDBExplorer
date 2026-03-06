using Metanoia.Modeling;
using Metanoia.Rendering;
using RDBExplorer.Core;
using RDBExplorer.Core.Formats.G1M;
using RDBExplorer.Core.Formats.G1T;
using RDBExplorer.Core.Models;
using RDBExplorer.Services;
using RDBExplorer.Utils;
using System.Diagnostics;

namespace RDBExplorer.Forms
{
    public partial class ModelViewForm : Form
    {
        public ModelViewer ModelViewer;
        private string _currentFileName;
        private byte[] _rawData;
        private ArchiveExploler _exploler;

        public ModelViewForm()
        {
            InitializeComponent();
            ModelViewer = new ModelViewer();
            ModelViewer.Dock = DockStyle.Fill;
            this.Controls.Add(ModelViewer);
            this.menuStrip1.SendToBack();
            ModelViewer.BringToFront();
        }


        public ModelViewForm(string entryName, byte[] data) : this()
        {
            _currentFileName = entryName;
            _rawData = data;

            _ = LoadModelAsync(data);
            UpdateTitle();
        }

        public ModelViewForm(RDBEntry entry, byte[] data, ArchiveExploler archiveExploler) : this()
        {
            _currentFileName = entry.Name;
            _rawData = data;
            _exploler = archiveExploler;

            _ = LoadModelAsync(data, entry.FileKtid);
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

        private async Task LoadModelAsync(byte[] data, uint? modelId = null)
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

             /*   if (modelId != null && TextureMapService.Instance.ModelToTextures.TryGetValue((uint)modelId, out uint[] textureHashes))
                {
                    await Task.Run(() =>
                    {
                        for (int i = 0; i < textureHashes.Length; i++)
                        {
                            uint texHash = textureHashes[i];
                            RDBEntry? entry = _exploler.FindEntryByKtId(texHash);
                            if (entry == null)
                                continue;
                            G1TParser g1TParser = new G1TParser();
                            g1TParser.Load(_exploler.GetEntryData(entry));
                            var g1tTexture = g1TParser.G1TFile.Textures[0];

                            byte[]? decodedData = TextureConverter.DecodeG1t(g1tTexture, 0, 0);
                            if (decodedData == null) 
                                continue;

                            var genTex = new GenericTexture
                            {
                                Name = $"Texture_{i}",
                                Width = g1tTexture.Width,
                                Height = g1tTexture.Height,
                                PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra
                            };
                            genTex.Mipmaps.Add(decodedData);

                            genericModel.TextureBank[genTex.Name] = genTex;
                        }

                        foreach (var matKvp in genericModel.MaterialBank)
                        {
                            string indexStr = matKvp.Key.Replace("Material_", "");
                            if (genericModel.TextureBank.ContainsKey($"Texture_{indexStr}"))
                            {
                                matKvp.Value.TextureDiffuse = $"Texture_{indexStr}";
                            }
                        }
                    });
                }
*/
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
