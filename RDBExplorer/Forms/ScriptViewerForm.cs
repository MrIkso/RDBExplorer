using RDBExplorer.Core.Formats.Bytecode;
using System.Text;

namespace RDBExplorer.Forms
{
    public partial class ScriptViewerForm : Form
    {
        private ScriptFile _scriptFile;
        private ScriptParser _scriptParser = new ScriptParser();
        private string _scriptFilePath;

        private string _decompiledCache;
        private string _disassembledCache;

        public ScriptViewerForm()
        {
            InitializeComponent();
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select compiled script";
            openFileDialog.Filter = "Bytecode file|*.bytecode;*.dat|All files|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                await StartParseFileAsync(openFileDialog.FileName);
            }
        }

        private async Task StartParseFileAsync(string fileName)
        {
            try
            {
                SetLoadingState(true);
                _scriptFilePath = fileName;

                _decompiledCache = null;
                _disassembledCache = null;

                byte[] data = await Task.Run(() => File.ReadAllBytes(fileName));
                _scriptFile = await Task.Run(() => _scriptParser.Parse(data));

                UpdateTitle();
                saveResultToolStripMenuItem.Enabled = true;
                await UpdateDisplayContentAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private async Task UpdateDisplayContentAsync()
        {
            if (_scriptFile == null) return;

            SetLoadingState(true);
            string resultText = string.Empty;

            try
            {
                if (decompilerToolStripMenuItem.Checked)
                {
                    if (string.IsNullOrEmpty(_decompiledCache))
                    {
                        _decompiledCache = await Task.Run(() =>
                        {
                            var astNodes = _scriptParser.BuildAst(_scriptFile);
                            return _scriptParser.GenerateCode(astNodes);
                        });
                    }
                    resultText = _decompiledCache;
                }
                else
                {
                    if (string.IsNullOrEmpty(_disassembledCache))
                    {
                        _disassembledCache = await Task.Run(() => _scriptParser.Disassemble(_scriptFile));
                    }
                    resultText = _disassembledCache;
                }

                ftb.Text = resultText;
                ftb.SelectionStart = 0;
                ftb.DoSelectionVisible();
            }
            catch (Exception ex)
            {
                ftb.Text = $"/* DECOMPILATION ERROR:\n{ex} \n*/";
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private void SetLoadingState(bool isLoading)
        {
            this.Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
            menuStrip1.Enabled = !isLoading;
        }

        private void UpdateTitle()
        {
            this.Text = $"Script Viewer - {Path.GetFileName(_scriptFilePath)}";
        }

        private async void decompilerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            decompilerToolStripMenuItem.Checked = true;
            dissasemblerToolStripMenuItem.Checked = false;
            await UpdateDisplayContentAsync();
        }

        private async void dissasemblerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            decompilerToolStripMenuItem.Checked = false;
            dissasemblerToolStripMenuItem.Checked = true;
            await UpdateDisplayContentAsync();
        }

        private void saveResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ftb.Text))
                return;

            using SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save decoded result";
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(_scriptFilePath);
            saveFileDialog.FileName = Path.ChangeExtension(Path.GetFileName(_scriptFilePath), ".txt");

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, ftb.Text, Encoding.UTF8);
            }
        }
    }
}
