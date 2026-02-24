using RDBExplorer.Core.Formats.G1T;
using RDBExplorer.Utils;

namespace RDBExplorer.Forms
{
    public partial class G1ToolForm : Form
    {
        private G1TParser _currentG1T;
        private G1TTexture _selectedTexture;
        private string _currentFilePath;
        private bool _isModified;

        public G1ToolForm()
        {
            InitializeComponent();
            SetupEvents();
        }

        public G1ToolForm(string fileName, byte[] data) : this()
        {
            _currentFilePath = fileName;
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
                _currentG1T = new G1TParser();
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
            textutePrewierPictureBox.Bitmap?.Dispose();
            mipsComboBox.Items.Clear();
            layersComboBox.Items.Clear();

            textureListView.BeginUpdate();
            textureListView.Items.Clear();

            for (int i = 0; i < _currentG1T.G1TFile.Textures.Count; i++)
            {
                var tex = _currentG1T.G1TFile.Textures[i];
                AddTextureToListView(tex, i);
            }
            textureListView.EndUpdate();

            texrurePropertyGrid.SelectedObject = _currentG1T.G1TFile;

            if (textureListView.Items.Count > 0)
                textureListView.Items[0].Selected = true;
        }

        private void AddTextureToListView(G1TTexture tex, int index)
        {
            string name = string.IsNullOrEmpty(tex.Name) ? $"Texture_{index:D3}" : tex.Name;
            tex.Name = name;
            var item = new ListViewItem(name);
            item.SubItems.Add($"{tex.Width}x{tex.Height} ({tex.Format})");
            item.Tag = tex;
            textureListView.Items.Add(item);
        }

        private void TextureListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && textureListView.FocusedItem != null)
            {
                var menu = new ContextMenuStrip();
                menu.Items.Add("Export this texture (All Mips/Layers)...", null, (s, a) => ExportSelectedTexture());
                menu.Items.Add("Import from folder (Replace Mips/Layers)...", null, (s, a) => InitImportTexture());
                menu.Show(Cursor.Position);
            }
        }

        private void ExportSelectedTexture()
        {
            if (_selectedTexture == null)
                return;

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|TGA Image|*.tga|DDS Image|.*dds|HDR Image|*.hdr|EXR Image|*.exr";
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

        private void G1ToolForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isModified)
            {
                var result = MessageBox.Show("You have unsaved changes. Exit anyway?", "Unsaved Changes",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            textutePrewierPictureBox.Bitmap?.Dispose();
        }

        private async void InitImportTexture()
        {
            if (_selectedTexture == null)
                return;

            using (var ofd = new FolderBrowserDialog())
            {
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string path = ofd.SelectedPath;
                    try
                    {
                        SetUIState(false);
                        toolStripStatusLabel.Text = "Importing DDS files...";

                        await Task.Run(() => TextureConverter.ConvertDdsToG1T(_selectedTexture, path));

                        if (textureListView.SelectedItems.Count > 0)
                        {
                            var item = textureListView.SelectedItems[0];
                            item.SubItems[1].Text = $"{_selectedTexture.Width}x{_selectedTexture.Height} ({_selectedTexture.Format})";
                            item.BackColor = Color.LightGreen;
                        }

                        texrurePropertyGrid.Refresh();
                        UpdatePreview();
                        UpdateModifiedState(true);

                        toolStripStatusLabel.Text = "Import completed.";
                        MessageBox.Show("Texture replaced. Save file to apply changes.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Import failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        SetUIState(true);
                    }
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
                    var g1t = new G1TParser();
                    g1t.Load(data);
                    return g1t;
                });

                PopulateUI();
                toolStripStatusLabel.Text = $"Ready | Textures: {_currentG1T.G1TFile.Textures.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading G1T: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                exportImagesToolStripMenuItem.Enabled = true;
                updateAllTexturesToolStripMenuItem.Enabled = true;
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
                    _currentFilePath = openFileDialog.FileName;
                    this.Text = $"G1Tool - {Path.GetFileName(_currentFilePath)}";
                    byte[] data = await File.ReadAllBytesAsync(_currentFilePath);
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
                return TextureConverter.CreateBitmapFromRawData(data, (int)_selectedTexture.MipMaps[mipIdx].Width, (int)_selectedTexture.MipMaps[mipIdx].Height);
            });

            if (bmp == null)
            {
                MessageBox.Show($"Unable to preview texture", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            textutePrewierPictureBox.Bitmap?.Dispose();
            textutePrewierPictureBox.Bitmap = bmp;
            toolStripStatusLabel.Text = "Ready";
        }

        private string GetCubeFaceName(int index)
        {
            string[] faces = { "Positive X", "Negative X", "Positive Y", "Negative Y", "Positive Z", "Negative Z" };
            int faceIdx = index % 6;
            int arrayIdx = index / 6;
            return _selectedTexture.ArraySize > 1 ? $"Layer {arrayIdx} - {faces[faceIdx]}" : faces[faceIdx];
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            textutePrewierPictureBox.Refresh();
        }

        private async void exportImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentG1T == null || _currentG1T.G1TFile.Textures.Count == 0)
            {
                return;
            }

            using (var fbd = new FolderBrowserDialog())
            {

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string baseFolder = fbd.SelectedPath;
                    string exportRoot = Path.Combine(baseFolder, $"{Path.GetFileNameWithoutExtension(_currentFilePath)}_Exported");

                    if (!Directory.Exists(exportRoot))
                    {
                        Directory.CreateDirectory(exportRoot);
                    }

                    SetUIState(false);

                    int texTotal = _currentG1T.G1TFile.Textures.Count;

                    await Task.Run(() =>
                    {
                        for (int i = 0; i < texTotal; i++)
                        {
                            var tex = _currentG1T.G1TFile.Textures[i];
                            string texName = string.IsNullOrEmpty(tex.Name) ? $"Texture_{i:D3}" : tex.Name;
                            string ext = ".dds";
                            this.Invoke(new Action(() =>
                            {
                                toolStripStatusLabel.Text = $"Exporting {texName} ({i + 1}/{texTotal})...";
                            }));

                            string targetDir = exportRoot;
                            if (tex.MipMaps.Count > 1 || tex.GetTotalLayers() > 1)
                            {
                                targetDir = Path.Combine(exportRoot, texName);
                                Directory.CreateDirectory(targetDir);
                            }

                            for (int m = 0; m < tex.MipMaps.Count; m++)
                            {
                                uint layerCount = (uint)tex.MipMaps[m].Layers.Count;
                                for (int l = 0; l < layerCount; l++)
                                {
                                    string fileName = $"{texName}_M{m:D2}_L{l:D2}{ext}";

                                    if (tex.LoadType == G1TLoadType.CUBE || tex.LoadType == G1TLoadType.CUBE_ARRAY)
                                    {
                                        fileName = $"{texName}_M{m:D2}_L{l:D2}_{GetCubeFaceName(l)}{ext}";
                                    }
                                    string outPath = Path.Combine(targetDir, fileName);
                                    try
                                    {
                                        TextureConverter.SaveImage(tex, m, l, outPath);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error exporting {fileName}: {ex.Message}");
                                    }
                                }
                            }
                        }
                    });

                    SetUIState(true);
                    toolStripStatusLabel.Text = "Ready";
                    MessageBox.Show($"Export complete!\nLocation: {exportRoot}", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        #region Save file
        private async void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentG1T == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(_currentFilePath) || !File.Exists(_currentFilePath))
            {
                saveAsToolStripMenuItem_Click(sender, e);
                return;
            }

            await SaveFileAsync(_currentFilePath);
            UpdateModifiedState(false);
        }

        private async void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentG1T == null)
                return;

            using (var sfd = new SaveFileDialog
            {
                Filter = "G1T Texture Archive|*.g1t",
                FileName = Path.GetFileName(_currentFilePath)
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    _currentFilePath = sfd.FileName;
                    await SaveFileAsync(_currentFilePath);
                    this.Text = $"G1Tool - {Path.GetFileName(_currentFilePath)}";
                    UpdateModifiedState(false);
                }
            }
        }

        private async Task SaveFileAsync(string path)
        {
            SetUIState(false);
            toolStripStatusLabel.Text = "Saving file...";
            try
            {
                byte[] data = await Task.Run(() => _currentG1T.Save());
                await File.WriteAllBytesAsync(path, data);
                MessageBox.Show("File saved successfully!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Save failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetUIState(true);
                toolStripStatusLabel.Text = "Ready";
            }
        }

        #endregion

        private void UpdateModifiedState(bool modified)
        {
            _isModified = modified;
            string fileName = string.IsNullOrEmpty(_currentFilePath) ? "Untitled" : Path.GetFileName(_currentFilePath);
            this.Text = $"G1Tool - {fileName}{(modified ? "*" : "")}";
            saveToolStripMenuItem.Enabled = modified;
            saveAsToolStripMenuItem.Enabled = true;
        }

        private async void updateAllTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentG1T?.G1TFile == null)
                return;

            using (var ofd = new FolderBrowserDialog())
            {
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string path = ofd.SelectedPath;
                    await UpdateAllFromFolderAsync(path);
                }
            }
        }

        public async Task UpdateAllFromFolderAsync(string folderPath)
        {
            SetUIState(false);
            int count = 0;
            int total = textureListView.Items.Count;

            try
            {
                await Task.Run(() =>
                {
                    foreach (ListViewItem item in textureListView.Items)
                    {
                        var tex = (G1TTexture)item.Tag;
                        TextureConverter.ConvertDdsToG1T(tex, folderPath);

                        count++;

                        int currentCount = count;
                        this.Invoke(new Action(() => {
                            toolStripStatusLabel.Text = $"Updating all textures: {currentCount}/{total}...";
                            item.SubItems[1].Text = $"{tex.Width}x{tex.Height} ({tex.Format})";
                            item.BackColor = Color.LightGreen;
                        }));
                    }
                });

                UpdatePreview();
                texrurePropertyGrid.Refresh();
                UpdateModifiedState(true);
                MessageBox.Show($"Batch update finished. {count} textures processed.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Batch import failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetUIState(true);
                toolStripStatusLabel.Text = "Ready";
            }
        }

    }
}