using System.Drawing;

namespace ImageView
{
    partial class PictureBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelMain = new ImageView.ScrollablePanel();
            this.panelPicture = new ImageView.DoubleBufferedPanel();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelPicture);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.ScrollLock = false;
            this.panelMain.Size = new System.Drawing.Size(224, 246);
            this.panelMain.TabIndex = 0;
            // 
            // panelPicture
            // 
            this.panelPicture.Location = new System.Drawing.Point(0, 0);
            this.panelPicture.Margin = new System.Windows.Forms.Padding(0);
            this.panelPicture.Name = "panelPicture";
            this.panelPicture.Size = new System.Drawing.Size(189, 213);
            this.panelPicture.TabIndex = 0;
            this.panelPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.panelPicture_Paint);
            this.panelPicture.DoubleClick += new System.EventHandler(this.panelPicture_DoubleClick);
            this.panelPicture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelPicture_MouseDown);
            this.panelPicture.MouseEnter += new System.EventHandler(this.panelPicture_MouseEnter);
            this.panelPicture.MouseLeave += new System.EventHandler(this.panelPicture_MouseLeave);
            this.panelPicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelPicture_MouseMove);
            this.panelPicture.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelPicture_MouseUp);
            // 
            // PictureBox
            // 
            this.Controls.Add(this.panelMain);
            this.Name = "PictureBox";
            this.Size = new System.Drawing.Size(224, 246);
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        //private System.Windows.Forms.Panel panelMain;
        private ImageView.ScrollablePanel panelMain;
        private ImageView.DoubleBufferedPanel panelPicture;
    }
}
