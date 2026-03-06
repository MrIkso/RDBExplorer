using OpenTK.GLControl;
using OpenTK.Graphics;

namespace Metanoia.Rendering
{
    partial class ModelViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelViewer));
            Viewport = new GLControl();
            toolStrip1 = new ToolStrip();
            toolStripDropDownButton1 = new ToolStripDropDownButton();
            exportModelToolStripMenuItem = new ToolStripMenuItem();
            importAnimationToolStripMenuItem = new ToolStripMenuItem();
            exportAnimationToolStripMenuItem = new ToolStripMenuItem();
            exportButton = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            resetViewButton = new ToolStripButton();
            renderToFileButtton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripLabel1 = new ToolStripLabel();
            renderMode = new ToolStripComboBox();
            toolStripSeparator2 = new ToolStripSeparator();
            showBoneButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            modelPaneInfoButton = new ToolStripButton();
            animationTS = new ToolStrip();
            toolStripLabel2 = new ToolStripLabel();
            animationCB = new ToolStripComboBox();
            buttonBegin = new ToolStripButton();
            buttonPrevious = new ToolStripButton();
            buttonPlay = new ToolStripButton();
            buttonNext = new ToolStripButton();
            buttonEnd = new ToolStripButton();
            frameLabel = new ToolStripLabel();
            toolStrip1.SuspendLayout();
            animationTS.SuspendLayout();
            SuspendLayout();
            // 
            // Viewport
            // 
            Viewport.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            Viewport.APIVersion = new Version(3, 3, 0, 0);
            Viewport.BackColor = Color.Black;
            Viewport.Dock = DockStyle.Fill;
            Viewport.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            Viewport.IsEventDriven = true;
            Viewport.Location = new Point(0, 28);
            Viewport.Margin = new Padding(4, 5, 4, 5);
            Viewport.Name = "Viewport";
            Viewport.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
            Viewport.SharedContext = null;
            Viewport.Size = new Size(803, 529);
            Viewport.TabIndex = 0;
            Viewport.Load += Viewport_Load;
            Viewport.Paint += Viewport_Paint;
            Viewport.KeyPress += Viewport_KeyDown;
            Viewport.MouseDown += Viewport_MouseDown;
            Viewport.MouseMove += Viewport_MouseMove;
            Viewport.MouseUp += Viewport_MouseUp;
            Viewport.Resize += Viewport_Resize;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripDropDownButton1, exportButton, toolStripSeparator4, resetViewButton, renderToFileButtton, toolStripSeparator1, toolStripLabel1, renderMode, toolStripSeparator2, showBoneButton, toolStripSeparator3, modelPaneInfoButton });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(803, 28);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton1.DropDownItems.AddRange(new ToolStripItem[] { exportModelToolStripMenuItem, importAnimationToolStripMenuItem, exportAnimationToolStripMenuItem });
            toolStripDropDownButton1.Image = (Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new Size(46, 25);
            toolStripDropDownButton1.Text = "File";
            toolStripDropDownButton1.Visible = false;
            // 
            // exportModelToolStripMenuItem
            // 
            exportModelToolStripMenuItem.Name = "exportModelToolStripMenuItem";
            exportModelToolStripMenuItem.Size = new Size(226, 26);
            exportModelToolStripMenuItem.Text = "Export Model";
            exportModelToolStripMenuItem.Click += exportModelToolStripMenuItem_Click;
            // 
            // importAnimationToolStripMenuItem
            // 
            importAnimationToolStripMenuItem.Enabled = false;
            importAnimationToolStripMenuItem.Name = "importAnimationToolStripMenuItem";
            importAnimationToolStripMenuItem.Size = new Size(226, 26);
            importAnimationToolStripMenuItem.Text = "Import Animation(s)";
            // 
            // exportAnimationToolStripMenuItem
            // 
            exportAnimationToolStripMenuItem.Enabled = false;
            exportAnimationToolStripMenuItem.Name = "exportAnimationToolStripMenuItem";
            exportAnimationToolStripMenuItem.Size = new Size(226, 26);
            exportAnimationToolStripMenuItem.Text = "Export Animation";
            exportAnimationToolStripMenuItem.Click += exportAnimationToolStripMenuItem_Click;
            // 
            // exportButton
            // 
            exportButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            exportButton.Image = RDBExplorer.Properties.Resources.Export;
            exportButton.ImageTransparentColor = Color.Magenta;
            exportButton.Name = "exportButton";
            exportButton.Size = new Size(29, 25);
            exportButton.Text = "export button";
            exportButton.ToolTipText = "Export Model";
            exportButton.Click += exportButton_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 28);
            // 
            // resetViewButton
            // 
            resetViewButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            resetViewButton.Image = RDBExplorer.Properties.Resources.ResetView;
            resetViewButton.ImageTransparentColor = Color.Magenta;
            resetViewButton.Name = "resetViewButton";
            resetViewButton.Size = new Size(29, 25);
            resetViewButton.Text = "toolStripButton1";
            resetViewButton.ToolTipText = "Reset View";
            resetViewButton.Click += resetViewButton_Click;
            // 
            // renderToFileButtton
            // 
            renderToFileButtton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            renderToFileButtton.Image = RDBExplorer.Properties.Resources.PictureAndText;
            renderToFileButtton.ImageTransparentColor = Color.Magenta;
            renderToFileButtton.Name = "renderToFileButtton";
            renderToFileButtton.Size = new Size(29, 25);
            renderToFileButtton.Text = "toolStripButton3";
            renderToFileButtton.ToolTipText = "Render to Image";
            renderToFileButtton.Click += renderToFileButtton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 28);
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(102, 25);
            toolStripLabel1.Text = "Render Mode:";
            // 
            // renderMode
            // 
            renderMode.DropDownStyle = ComboBoxStyle.DropDownList;
            renderMode.Name = "renderMode";
            renderMode.Size = new Size(160, 28);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 28);
            // 
            // showBoneButton
            // 
            showBoneButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            showBoneButton.Image = RDBExplorer.Properties.Resources.ShowHotLines;
            showBoneButton.ImageTransparentColor = Color.Magenta;
            showBoneButton.Name = "showBoneButton";
            showBoneButton.Size = new Size(29, 25);
            showBoneButton.Text = "toolStripButton3";
            showBoneButton.ToolTipText = "Show/Hide Bones";
            showBoneButton.Click += showBoneButton_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 28);
            // 
            // modelPaneInfoButton
            // 
            modelPaneInfoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            modelPaneInfoButton.Image = RDBExplorer.Properties.Resources.ShowDetailsPane;
            modelPaneInfoButton.ImageTransparentColor = Color.Magenta;
            modelPaneInfoButton.Name = "modelPaneInfoButton";
            modelPaneInfoButton.Size = new Size(29, 25);
            modelPaneInfoButton.Text = "toolStripButton2";
            modelPaneInfoButton.ToolTipText = "Model Information";
            modelPaneInfoButton.Click += modelPaneInfoButton_Click;
            // 
            // animationTS
            // 
            animationTS.ImageScalingSize = new Size(20, 20);
            animationTS.Items.AddRange(new ToolStripItem[] { toolStripLabel2, animationCB, buttonBegin, buttonPrevious, buttonPlay, buttonNext, buttonEnd, frameLabel });
            animationTS.Location = new Point(0, 28);
            animationTS.Name = "animationTS";
            animationTS.Size = new Size(803, 28);
            animationTS.TabIndex = 2;
            animationTS.Text = "toolStrip2";
            animationTS.Visible = false;
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new Size(87, 25);
            toolStripLabel2.Text = "Animations:";
            // 
            // animationCB
            // 
            animationCB.DropDownStyle = ComboBoxStyle.DropDownList;
            animationCB.Name = "animationCB";
            animationCB.Size = new Size(172, 28);
            animationCB.SelectedIndexChanged += animationCB_SelectedIndexChanged;
            // 
            // buttonBegin
            // 
            buttonBegin.DisplayStyle = ToolStripItemDisplayStyle.Image;
            buttonBegin.ImageTransparentColor = Color.Magenta;
            buttonBegin.Name = "buttonBegin";
            buttonBegin.Size = new Size(29, 25);
            buttonBegin.Text = "Start";
            buttonBegin.Click += buttonBegin_Click;
            // 
            // buttonPrevious
            // 
            buttonPrevious.DisplayStyle = ToolStripItemDisplayStyle.Image;
            buttonPrevious.ImageTransparentColor = Color.Magenta;
            buttonPrevious.Name = "buttonPrevious";
            buttonPrevious.Size = new Size(29, 25);
            buttonPrevious.Text = "Previous";
            buttonPrevious.Click += buttonPrevious_Click;
            // 
            // buttonPlay
            // 
            buttonPlay.DisplayStyle = ToolStripItemDisplayStyle.Image;
            buttonPlay.ImageTransparentColor = Color.Magenta;
            buttonPlay.Name = "buttonPlay";
            buttonPlay.Size = new Size(29, 25);
            buttonPlay.Text = "Play/Pause";
            buttonPlay.Click += buttonPlay_Click;
            // 
            // buttonNext
            // 
            buttonNext.DisplayStyle = ToolStripItemDisplayStyle.Image;
            buttonNext.ImageTransparentColor = Color.Magenta;
            buttonNext.Name = "buttonNext";
            buttonNext.Size = new Size(29, 25);
            buttonNext.Text = "Next";
            buttonNext.Click += buttonNext_Click;
            // 
            // buttonEnd
            // 
            buttonEnd.DisplayStyle = ToolStripItemDisplayStyle.Image;
            buttonEnd.ImageTransparentColor = Color.Magenta;
            buttonEnd.Name = "buttonEnd";
            buttonEnd.Size = new Size(29, 25);
            buttonEnd.Text = "End";
            buttonEnd.Click += buttonEnd_Click;
            // 
            // frameLabel
            // 
            frameLabel.Name = "frameLabel";
            frameLabel.Size = new Size(87, 25);
            frameLabel.Text = "Frame: 0 / 0";
            // 
            // ModelViewer
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(Viewport);
            Controls.Add(animationTS);
            Controls.Add(toolStrip1);
            Margin = new Padding(4, 5, 4, 5);
            Name = "ModelViewer";
            Size = new Size(803, 557);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            animationTS.ResumeLayout(false);
            animationTS.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private GLControl Viewport;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton resetViewButton;
        private System.Windows.Forms.ToolStripButton modelPaneInfoButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox renderMode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton showBoneButton;
        private System.Windows.Forms.ToolStripButton renderToFileButtton;
        private System.Windows.Forms.ToolStripButton exportButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStrip animationTS;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox animationCB;
        private System.Windows.Forms.ToolStripButton buttonBegin;
        private System.Windows.Forms.ToolStripButton buttonPrevious;
        private System.Windows.Forms.ToolStripButton buttonPlay;
        private System.Windows.Forms.ToolStripButton buttonNext;
        private System.Windows.Forms.ToolStripButton buttonEnd;
        private System.Windows.Forms.ToolStripLabel frameLabel;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem importAnimationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAnimationToolStripMenuItem;
    }
}
