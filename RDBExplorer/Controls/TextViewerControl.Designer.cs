using FastColoredTextBoxNS;

namespace RDBExplorer.Controls
{
    partial class TextViewerControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            textBox = new FastColoredTextBox();
            SuspendLayout();
            // 
            // panel1
            // 
            textBox.Dock = DockStyle.Fill;
            textBox.Location = new Point(0, 0);
            textBox.Name = "panel1";
            textBox.Size = new Size(500, 300);
            textBox.TabIndex = 0;
            // 
            // TextViewerControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(textBox);
            Name = "TextViewerControl";
            Size = new Size(500, 300);
            ResumeLayout(false);
        }

        private FastColoredTextBox textBox;
    }
}
