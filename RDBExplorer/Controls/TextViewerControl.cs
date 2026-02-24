namespace RDBExplorer.Controls
{
    public partial class TextViewerControl : UserControl
    {
        public TextViewerControl()
        {
            InitializeComponent();
        }

        public void SetText(string text) {
            textBox.Text = text;
            textBox.ReadOnly = true;
        }
    }
}
