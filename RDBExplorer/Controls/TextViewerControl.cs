using FastColoredTextBoxNS;
using System.Text;

namespace RDBExplorer.Controls
{
    public partial class TextViewerControl : UserControl
    {
        private string _tempFilePath;
        private const long MaxMemorySize = 2 * 1024 * 1024; // 2 MB
        private bool isBindedFile = false;
        public TextViewerControl()
        {
            InitializeComponent();
            textBox.ReadOnly = true;
            textBox.Language = Language.JSON;
        }

        public void SetText(string text)
        {
            CleanUp();

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (text.Length * sizeof(char) > MaxMemorySize)
            {
                LoadLargeText(text);
            }
            else
            {
                textBox.Text = text;
            }
        }

        private void LoadLargeText(string text)
        {
            try
            {
                _tempFilePath = Path.GetTempFileName();
                File.WriteAllText(_tempFilePath, text, Encoding.UTF8);
                textBox.OpenBindingFile(_tempFilePath, Encoding.UTF8);
                textBox.IsChanged = false;
                isBindedFile = true;
                textBox.ClearUndo();
                textBox.SyntaxHighlighter = null;
                GC.Collect();
                GC.GetTotalMemory(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading large text: {ex.Message}");
            }
        }

        public async Task LoadFromFileAsync(string path)
        {
            CleanUp();

            try
            {
                textBox.Text = "Loading large text...";
                textBox.Enabled = false;

                _tempFilePath = path;

                textBox.OpenBindingFile(_tempFilePath, Encoding.UTF8);
                isBindedFile = true;
                textBox.IsChanged = false;
                textBox.ClearUndo();
                textBox.SyntaxHighlighter = null;
                textBox.Language = Language.Custom;


                await Task.Run(() =>
                {
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                    GC.WaitForPendingFinalizers();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading large text: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                textBox.Enabled = true;
            }
        }

        public void CleanUp()
        {
            if (isBindedFile)
            {
                textBox.CloseBindingFile();
                isBindedFile = false;
            }
            if (!string.IsNullOrEmpty(_tempFilePath) && File.Exists(_tempFilePath))
            {
                try
                {
                    File.Delete(_tempFilePath);
                }
                catch
                {
                }
                _tempFilePath = null;
                GC.Collect();
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            CleanUp();
            base.OnHandleDestroyed(e);
        }
    }
}
