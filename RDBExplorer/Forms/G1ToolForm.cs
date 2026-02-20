using RDBExplorer.Core.G1T;
using RDBExplorer.Utils;

namespace RDBExplorer.Forms
{
    public partial class G1ToolForm : Form
    {
        private G1T _currentG1T;
        private G1TTexture _selectedTexture;

        public G1ToolForm()
        {
            InitializeComponent();
            SetupEvents();
        }

        public G1ToolForm(string fileName, byte[] data) : this()
        {
            this.Text = $"G1Tool - {fileName}";
            _ = LoadWithDataAsync(data);
        }

        private void SetupEvents()
        {
            textureListView.SelectedIndexChanged += TextureListView_SelectedIndexChanged;
            mipsComboBox.SelectedIndexChanged += Control_PreviewChanged;
            layersComboBox.SelectedIndexChanged += Control_PreviewChanged;
        }

        public void LoadWithData(byte[] data)
        {
            try
            {
                _currentG1T = new G1T();
                _currentG1T.Load(data);
                PopulateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading G1T data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateUI()
        {
            if (_currentG1T?.G1TFile == null)
                return;

            textureListView.BeginUpdate();
            textureListView.Items.Clear();

            for (int i = 0; i < _currentG1T.G1TFile.Textures.Count; i++)
            {
                var tex = _currentG1T.G1TFile.Textures[i];
                var item = new ListViewItem(string.IsNullOrEmpty(tex.Name) ? $"Texture_{i}" : tex.Name);
                item.SubItems.Add($"{tex.Width}x{tex.Height} ({tex.Format})");
                item.Tag = tex;
                textureListView.Items.Add(item);
            }
            textureListView.EndUpdate();

            texrurePropertyGrid.SelectedObject = _currentG1T.G1TFile;
            toolStripStatusLabel.Text = $"Textures: {_currentG1T.G1TFile.Textures.Count} | Platform: {_currentG1T.G1TFile.Header.Platform}";

            if (textureListView.Items.Count > 0)
            {
                textureListView.Items[0].Selected = true;
            }
        }

        private void TextureListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && textureListView.FocusedItem != null)
            {
                var menu = new ContextMenuStrip();
                menu.Items.Add("Export this texture...", null, (s, args) =>
                {
                    ExportSelectedTexture();
                });
                menu.Show(Cursor.Position);
            }
        }

        private void ExportSelectedTexture()
        {
            if (_selectedTexture == null) 
                return;

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|TGA Image|*.tga|HDR Image|*.hdr|EXR Image|*.exr";
                sfd.FileName = string.IsNullOrEmpty(_selectedTexture.Name) ? "ExportedTexture" : _selectedTexture.Name;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    int mip = mipsComboBox.SelectedIndex;
                    int layer = layersComboBox.SelectedIndex;

                    TextureConverter.SaveImage(_selectedTexture, mip, layer, sfd.FileName);
                    MessageBox.Show("Texture saved!");
                }
            }
        }

        public async Task LoadWithDataAsync(byte[] data)
        {
            try
            {
                SetUIState(false);
                toolStripStatusLabel.Text = "Loading texture data...";

                _currentG1T = await Task.Run(() =>
                {
                    var g1t = new G1T();
                    g1t.Load(data);
                    return g1t;
                });

                PopulateUI();
                toolStripStatusLabel.Text = $"Ready | Textures: {_currentG1T.G1TFile.Textures.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading G1T: {ex.Message}");
            }
            finally
            {
                SetUIState(true);
            }
        }

        private void SetUIState(bool enabled)
        {
            textureListView.Enabled = enabled;
            menuStrip1.Enabled = enabled;
            this.Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
        }

        private async void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "KT Textures|*.g1t";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Text = $"G1Tool - {Path.GetFileName(openFileDialog.FileName)}";
                    byte[] data = await File.ReadAllBytesAsync(openFileDialog.FileName);
                    await LoadWithDataAsync(data);
                }
            }
        }

        private void TextureListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (textureListView.SelectedItems.Count == 0)
            {
                return;
            }

            _selectedTexture = (G1TTexture)textureListView.SelectedItems[0].Tag;

            mipsComboBox.SelectedIndexChanged -= Control_PreviewChanged;
            layersComboBox.SelectedIndexChanged -= Control_PreviewChanged;

            mipsComboBox.Items.Clear();
            for (int i = 0; i < _selectedTexture.MipMaps.Count; i++)
            {
                mipsComboBox.Items.Add($"Mip {i} ({_selectedTexture.MipMaps[i].Width}x{_selectedTexture.MipMaps[i].Height})");
            }

            mipsComboBox.SelectedIndex = 0;

            layersComboBox.Items.Clear();
            uint totalLayers = _selectedTexture.GetTotalLayers();
            for (int i = 0; i < totalLayers; i++)
            {
                string label = _selectedTexture.LoadType == G1TLoadType.CUBE || _selectedTexture.LoadType == G1TLoadType.CUBE_ARRAY
                    ? GetCubeFaceName(i)
                    : $"Layer {i}";
                layersComboBox.Items.Add(label);
            }

            layersComboBox.SelectedIndex = 0;
            mipsComboBox.SelectedIndexChanged += Control_PreviewChanged;
            layersComboBox.SelectedIndexChanged += Control_PreviewChanged;

            UpdatePreview();
        }

        private void Control_PreviewChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private async void UpdatePreview()
        {
            if (_selectedTexture == null)
                return;

            int mipIdx = mipsComboBox.SelectedIndex;
            int layerIdx = layersComboBox.SelectedIndex;
            if (mipIdx < 0 || layerIdx < 0)
                return;

            toolStripStatusLabel.Text = "Decoding image...";

            Bitmap? bmp = await Task.Run(() =>
            {
                byte[]? data = TextureConverter.DecodeG1t(_selectedTexture, mipIdx, layerIdx);
                if (data == null)
                    return null;
                return TextureConverter.CreateBitmapFromRawData(data, (int)_selectedTexture.MipMaps[mipIdx].Height, (int)_selectedTexture.MipMaps[mipIdx].Width);
            });

            if (bmp == null)
            {
                MessageBox.Show($"Unable to preview texture", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var oldImg = textutePrewierPictureBox.Image;
            textutePrewierPictureBox.Image = bmp;
            oldImg?.Dispose();

            toolStripStatusLabel.Text = "Ready";
        }

        private string GetCubeFaceName(int index)
        {
            string[] faces = { "Positive X", "Negative X", "Positive Y", "Negative Y", "Positive Z", "Negative Z" };
            int faceIdx = index % 6;
            int arrayIdx = index / 6;
            return _selectedTexture.ArraySize > 1 ? $"Layer {arrayIdx} - {faces[faceIdx]}" : faces[faceIdx];
        }

        private async void exportImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentG1T == null || _currentG1T.G1TFile.Textures.Count == 0)
                return;

            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string folder = fbd.SelectedPath;
                    SetUIState(false);

                    int total = _currentG1T.G1TFile.Textures.Count;
                    toolStripStatusLabel.Text = $"Exporting 0/{total}...";

                    await Task.Run(() =>
                    {
                        for (int i = 0; i < total; i++)
                        {
                            var tex = _currentG1T.G1TFile.Textures[i];
                            string name = string.IsNullOrEmpty(tex.Name) ? $"Texture_{i}" : tex.Name;
                            string outPath = Path.Combine(folder, name + ".png");

                            try
                            {
                                TextureConverter.SaveImage(tex, 0, 0, outPath);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to export {name}: {ex.Message}");
                            }

                            int current = i + 1;
                            this.Invoke(new Action(() => {
                                toolStripStatusLabel.Text = $"Exporting {current}/{total}...";
                            }));
                        }
                    });

                    SetUIState(true);
                    toolStripStatusLabel.Text = "Export finished.";
                    MessageBox.Show("All textures exported successfully!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}