/*
MIT License

Copyright (c) 2020 Tony Pottier

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Diagnostics;

namespace ImageView
{
    /// <summary>
    /// A high performance and very powerful picture box control
    /// </summary>
    public partial class PictureBox : UserControl
    {

        #region Events
        [Category("Behavior"),
        Description("Occurs whenever the zoom value changes")]
        public event EventHandler<ZoomEventArgs> ZoomChanged;

        [Category("Behavior"),
        Description("Occurs whenever the cursor hovers over the image")]
        public event EventHandler<CoordinatesEventArgs> PixelCoordinatesChanged;

        public new event EventHandler<EventArgs> DoubleClick;

        public new event EventHandler<MouseEventArgs> MouseWheel;

        #endregion

        #region Hidden inherited properties
        [Obsolete("Not applicable in this class.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Image BackgroundImage { get; set; }

        [Obsolete("Not applicable in this class.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new ImageLayout BackgroundImageLayout { get; set; }

        [Obsolete("Not applicable in this class.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Font Font { get; set; }

        [Obsolete("Not applicable in this class.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Color ForeColor { get; set; }

        [Obsolete("Not applicable in this class.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new RightToLeft RightToLeft { get; set; }

        [Obsolete("Not applicable in this class.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool AutoScroll { get; set; }

        [Obsolete("Not applicable in this class.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Size AutoScrollMargin { get; set; }

        [Obsolete("Not applicable in this class.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Size AutoScrollMinSize { get; set; }

        #endregion

        #region Event Raise Methods

        /// <summary>
        /// Triggers whenever the zoom value changes.
        /// </summary>
        /// <param name="e">Zoom event information</param>
        protected virtual void OnZoomChanged(ZoomEventArgs e)
        {
            EventHandler<ZoomEventArgs> handler = ZoomChanged;
            if (handler != null)
                handler(this, e);
        }


        protected virtual void OnPixelCoordinatesChanged(CoordinatesEventArgs e)
        {
            EventHandler<CoordinatesEventArgs> handler = PixelCoordinatesChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// For some odd reason the double click event doesn't trigger on the control the picturebox is placed in\
        /// so this forces a doubleclick event raise.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panelPicture_DoubleClick(object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = DoubleClick;
            if (handler != null)
                handler(this, e);
        }



        #endregion

        #region Properties


        /// <summary>
        /// The bitmap to be drawn inside the picture box
        /// </summary>
        [Category("Appearance"),
        Description("The bitmap to be drawn inside the picture box")]
        public Bitmap Bitmap
        {
            get { return bitmap; }
            set
            {
                this.SuspendLayout();
                this.panelPicture.SuspendLayout();
                this.panelPicture.Visible = false;
                this.bitmap = value;
                calculateRect();
                this.panelPicture.Visible = true;
                this.panelPicture.ResumeLayout();
                this.panelPicture.Invalidate();
                this.ResumeLayout();
            }
        }

        [Category("Appearance"),
        Description("Bitmap used as a background with transparent images")]
        public Bitmap TransparentBackground
        {
            get { return transparentBackground; }
            set
            {
                if (value != transparentBackground)
                {
                    transparentBackground = value;
                    draw();
                }
            }
        }


        [Category("Behavior"),
        Description("Ordered list of valid zoom levels")]
        public List<float> ZoomSteps
        {
            get { return zoomSteps; }
        }

        [Category("Behavior"),
        Description("Value in % of total height that the scrollbars should go up or down using keys. Default is 10% (0.1f)")]
        public float VerticallScrollStep
        {
            get { return verticalScrollSep; }
            set
            {
                if (value < 0.0f)
                {
                    verticalScrollSep = 0;
                }
                else if (value > 1.0f)
                {
                    verticalScrollSep = 1.0f;
                }
                else
                {
                    verticalScrollSep = 1.0f;
                }
            }
        }

        [Category("Behavior"),
        Description("Modifier key to zoom out")]
        public Keys ZoomOutModifier
        {
            get;
            set;
        }


        [Category("Appearance"),
        Description("Current pixel coordinates of mouse hover")]
        public PointF PixelCoordinates
        {
            get { return pixelCoordinates; }
        }


        [Category("Appearance"),
        Description("Use a specific background for transparent image")]
        public bool UseBackgroundBrush
        {
            get;
            set;
        }


        [Category("Appearance"),
        Description("Use zoom cursors when mouse is hovering over image")]
        public bool UseZoomCursors
        {
            get;
            set;
        }

        [Category("Behavior"),
        Description("Mouse button used for zooming. Use 'None' to disable.")]
        public MouseButtons ZoomMouseButton
        {
            get { return zoomMouseButton; }
            set
            {
                if (UseZoomCursors && this.Cursor != this.DefaultCursor && value == MouseButtons.None)
                {
                    this.Cursor = this.DefaultCursor;
                }
                zoomMouseButton = value;
            }
        }

        [Category("Behavior"),
        Description("Enables or disables zooming when clicking the image with the mouse button")]
        public bool EnableClickToZoom
        {
            get { return enableClickToZoom; }
            set { enableClickToZoom = value; }
        }

        [Category("Behavior"),
        Description("Mouse button used for dragging. Use 'None' to disable.")]
        public MouseButtons DragMouseButton
        {
            get { return dragMouseButton; }
            set { dragMouseButton = value; }
        }

        [Category("Appearance"),
        Description("Cursor to be used when dragging")]
        public Icon DragCursor
        {
            get;
            set;
        }

        [Category("Appearance"),
        Description("Cursor to be used when zoom is active")]
        public Icon ZoomInCursor
        {
            get;
            set;
        }

        [Category("Appearance"),
        Description("Cursor to be used when zoom is active")]
        public Icon ZoomOutCursor
        {
            get;
            set;
        }


        [Category("Behavior"),
        Description("Controls if the mouse wheel should be able to control the scrollbars (if visible)")]
        public bool WheelScrollLock
        {
            get
            {
                return panelMain.ScrollLock;
            }
            set
            {
                panelMain.ScrollLock = value;
            }
        }

        [Category("Appearance"),
        Description("Interpolation used to draw the bitmap")]
        public InterpolationMode InterpolationMode
        {
            get { return interpolationMode; }
            set
            {
                if (value != interpolationMode)
                {
                    interpolationMode = value;
                    draw();
                }
            }
        }

        [Category("Appearance"),
        Description("Size mode used to display the picture")]
        public SizeMode SizeMode
        {
            get
            {
                return sizeMode;
            }
            set
            {
                if (value != SizeMode)
                {
                    this.sizeMode = value;
                    calculateRect();
                    draw();
                }
            }
        }

        [Category("Appearance"),
        Description("Zoom level currently in use")]
        public float Zoom
        {
            get { return zoom; }
            set
            {
                if (value > 0.0f)
                {
                    if (value != zoom)
                    {
                        this.sizeMode = SizeMode.Zoom;
                        this.zoom = value;
                        calculateRect();
                        draw();
                    }
                }
            }
        }

        #endregion

        #region Private variables
        private List<float> zoomSteps;
        private float zoom;
        private InterpolationMode interpolationMode;
        private RectangleF srcRect;
        private RectangleF dstRect;
        private Rectangle boxRect;
        private Bitmap bitmap;
        private Bitmap transparentBackground;
        private SizeMode sizeMode;
        private PointF pixelCoordinates;
        private Point mousePosition;
        private Cursor defaultCursor;
        private float verticalScrollSep;
        private MouseButtons dragMouseButton = MouseButtons.None;
        private MouseButtons zoomMouseButton = MouseButtons.None;
        private bool enableClickToZoom = true;

        #endregion

        #region Public methods

        public override void Refresh()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("PictureBox.Refresh()");
#endif
            if (this.bitmap != null)
            {
                calculateRect();
                draw();
            }
        }

        /// <summary>
        /// Adds a value to the zoom steps, and re-sorts the list. Duplicated values are automatically ignored.
        /// </summary>
        /// <param name="value">The new value to be added</param>
        public void AddZoomStep(float value)
        {
            if (value > 0)
            {
                if (!zoomSteps.Contains(value))
                {
                    zoomSteps.Add(value);
                    zoomSteps.Sort();
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        public void ScrollVertically(ScrollDirection direction)
        {
            if (panelMain.VerticalScroll.Visible)
            {
                Point scroll = panelMain.AutoScrollPosition;
                scroll.X *= -1;
                scroll.Y = -scroll.Y + ((int)((float)panelMain.Height * verticalScrollSep)) * (direction == ScrollDirection.Up ? -1 : 1);
                panelMain.AutoScrollPosition = scroll;

                refreshDrawingRect();
                draw();
            }
        }

        #endregion

        #region Private methods
        private void draw()
        {
            this.panelPicture.Invalidate();
        }
        private void calculateRect()
        {
            calculateRect(Point.Empty, -1.0f);
        }
        private void calculateRect(Point mouseCoord, float newZoom)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("calculateRect");
#endif
            if (bitmap != null)
            {
                switch (sizeMode)
                {
                    case SizeMode.BestFit:
                        calcBestFit();
                        break;
                    case SizeMode.FitToWidth:
                        calcFitToWidth();
                        break;
                    case SizeMode.FitToHeight:
                        calcFitToHeight();
                        break;
                    case SizeMode.Zoom:
                    case SizeMode.Normal:
                        calcZoom(mouseCoord, newZoom);
                        break;
                }
            }

        }
        private void calcZoom(Point mouseCoord, float newZoom)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("calcZoom");
#endif
            panelMain.AutoScroll = false;
            Size clientSize = panelMain.ClientSize;
            float oldZoom = this.zoom;

            if (SizeMode == SizeMode.Normal)
            {
                this.zoom = 1.0f;
                OnZoomChanged(new ZoomEventArgs(this.zoom));
            }
            if (newZoom != -1.0f && newZoom != oldZoom)
            {
                this.zoom = newZoom;
                OnZoomChanged(new ZoomEventArgs(this.zoom));
            }

            //image size
            boxRect.Width = (int)Math.Round(((float)bitmap.Width * zoom));
            boxRect.Height = (int)Math.Round(((float)bitmap.Height * zoom));
            boxRect.X = 0;
            boxRect.Y = 0;

            //center image if possible
            if (clientSize.Width > boxRect.Width)
            {
                boxRect.X = (clientSize.Width - boxRect.Width) >> 1;
            }
            if (clientSize.Height > boxRect.Height)
            {
                boxRect.Y = (panelMain.Height - boxRect.Height) >> 1;
            }

            //position picturebox
            panelPicture.Size = boxRect.Size;
            panelPicture.Location = boxRect.Location;
            panelMain.AutoScroll = true;

            //if a click point was provided, we center the image on this reference point
            if (mouseCoord != Point.Empty)
            {
                //translate mouse position to pixel coords
                float Xpx, Ypx;
                Xpx = mouseCoord.X / oldZoom;
                Ypx = mouseCoord.Y / oldZoom;

                //translate these pixel coords to new coords
                Xpx = Xpx * zoom;
                Ypx = Ypx * zoom;

                //center clicked ppoint
                Point panelHalfSize = new Point(panelMain.Width >> 1, panelMain.Height >> 1);
                panelMain.AutoScrollPosition = new Point((int)Xpx - panelHalfSize.X, (int)Ypx - panelHalfSize.Y);
            }

            refreshDrawingRect();
        }
        /*private void refreshDrawingRect()
        {
            Size clientSize = panelMain.ClientSize;
            dstRect.X = -panelMain.AutoScrollPosition.X;
            dstRect.Y = -panelMain.AutoScrollPosition.Y;
            dstRect.Width = clientSize.Width;
            dstRect.Height = clientSize.Height;
            srcRect.X = dstRect.X / zoom;
            srcRect.Y = dstRect.Y / zoom;
            srcRect.Width = dstRect.Width / zoom;
            srcRect.Height = dstRect.Height / zoom;
        }*/
        private void refreshDrawingRect()
        {
            dstRect.X = 0;
            dstRect.Y = 0;
            dstRect.Width = panelPicture.Width;
            dstRect.Height = panelPicture.Height;

            srcRect.X = 0;
            srcRect.Y = 0;
            srcRect.Width = bitmap.Width;
            srcRect.Height = bitmap.Height;
        }

        private void calcFitToHeight()
        {
            panelMain.AutoScroll = false;
            Size clientSize = panelMain.ClientSize;

            //the height is locked to be window height
            boxRect.Y = 0;
            boxRect.Height = clientSize.Height;

            //width is simply a function of the zoom
            float zoom = boxRect.Height / (float)bitmap.Height;
            boxRect.Width = (int)Math.Round(zoom * (float)bitmap.Width);

            //if an horizontal scrollbar is going to appear, we need to recalculate the size to take into account the height lost by the scrollbar's thickness
            if (boxRect.Width > clientSize.Width)
            {
                boxRect.Height = clientSize.Height - (panelMain.HorizontalScroll.Visible ? 0 : System.Windows.Forms.SystemInformation.HorizontalScrollBarHeight);
                zoom = boxRect.Height / (float)bitmap.Height;
                boxRect.Width = (int)Math.Round(zoom * (float)bitmap.Width);

            }

            //center x
            if (boxRect.Width < clientSize.Width)
            {
                boxRect.X = (clientSize.Width - boxRect.Width) >> 1;
            }
            else
            {
                boxRect.X = 0;
            }

            //position picture box
            panelPicture.Size = boxRect.Size;
            panelPicture.Location = boxRect.Location;

            //calculate rects for drawing
            dstRect.X = 0.0f;
            dstRect.Y = 0.0f;
            dstRect.Width = boxRect.Width;
            dstRect.Height = boxRect.Height;
            srcRect.X = 0.0f;
            srcRect.Y = 0.0f;
            srcRect.Width = dstRect.Width / zoom;
            srcRect.Height = dstRect.Height / zoom; // = image height

            panelMain.AutoScroll = true;

            if (this.zoom != zoom)
            {
                this.zoom = zoom;
                OnZoomChanged(new ZoomEventArgs(zoom));
            }


        }
        private void calcFitToWidth()
        {
            panelMain.AutoScroll = false;
            Size clientSize = panelMain.ClientSize;

            //the width is locked to be window width
            boxRect.X = 0;
            boxRect.Width = clientSize.Width;

            //height is simply a function of the zoom
            float zoom = boxRect.Width / (float)bitmap.Width;
            boxRect.Height = (int)Math.Round(zoom * (float)bitmap.Height);


            //if a vertical scrollbar is going to appear, we need to recalculate the size to take into account the width lost by the scrollbar's thickness
            //weirdly enough the behavior of winforms is strange here. If a scrollbar is visible then the client size area is reflected. But we need to account
            //for when this is not the case and so there's a test to check if the scroll bar is visible. If it is then we do not need to subsctract the thickness
            //of the bar since it's already accounted for in the panel ClientSize.
            if (boxRect.Height > panelMain.ClientSize.Height)
            {
                boxRect.Width = clientSize.Width - (panelMain.VerticalScroll.Visible ? 0 : System.Windows.Forms.SystemInformation.VerticalScrollBarWidth);
                zoom = boxRect.Width / (float)bitmap.Width;
                boxRect.Height = (int)Math.Round(zoom * (float)bitmap.Height);
            }

            //center y
            if (boxRect.Height < panelMain.ClientSize.Height)
            {
                boxRect.Y = (panelMain.ClientSize.Height - boxRect.Height) >> 1;
            }
            else
            {
                boxRect.Y = 0;
            }

            //position picture box
            panelPicture.Size = boxRect.Size;
            panelPicture.Location = boxRect.Location;

            //source portion of the image is locked to width of the image
            srcRect.X = 0;
            srcRect.Y = 0;
            srcRect.Width = bitmap.Width;
            srcRect.Height = (float)clientSize.Height / zoom;

            //target
            dstRect.X = 0;
            dstRect.Y = 0;
            dstRect.Width = boxRect.Width;
            dstRect.Height = clientSize.Height;

            panelMain.AutoScroll = true;

            if (this.zoom != zoom)
            {
                this.zoom = zoom;
                OnZoomChanged(new ZoomEventArgs(zoom));
            }

        }
        private void calcBestFit()
        {
            panelMain.AutoScroll = false;
            Size clientSize = panelMain.ClientSize;

            float zoom = 1.0f;

            boxRect.X = 0;
            boxRect.Y = 0;

            //image can fit entirely on the screen
            if (clientSize.Width >= bitmap.Width && clientSize.Height >= bitmap.Height)
            {
                boxRect.Width = bitmap.Width;
                boxRect.Height = bitmap.Height;

                //center image
                if (clientSize.Width > bitmap.Width)
                {
                    boxRect.X = (clientSize.Width - bitmap.Width) >> 1;
                }
                if (clientSize.Height > bitmap.Height)
                {
                    boxRect.Y = (clientSize.Height - bitmap.Height) >> 1;
                }
            }
            else
            {
                //either the width or height becomes the limiting factor, 
                //this is determined by the aspect ratio of the image and the aspect ratio of the panel containing said image
                float aspec = (float)bitmap.Width / (float)bitmap.Height;
                float windowAspec = (float)clientSize.Width / (float)clientSize.Height;
                if (aspec > windowAspec)
                {
                    zoom = ((float)clientSize.Width / (float)bitmap.Width);
                    boxRect.Width = clientSize.Width;
                    boxRect.Height = (int)Math.Round((boxRect.Width / aspec));

                    boxRect.Y = (clientSize.Height - boxRect.Height) >> 1;
                }
                else
                {
                    zoom = ((float)clientSize.Height / (float)bitmap.Height);
                    boxRect.Height = clientSize.Height;
                    boxRect.Width = (int)Math.Round((boxRect.Height * aspec));

                    boxRect.X = (clientSize.Width - boxRect.Width) >> 1;
                }
            }


            //position picture box
            panelPicture.Location = boxRect.Location;
            panelPicture.Size = boxRect.Size;


            //calculate rects for drawing
            dstRect.X = 0.0f;
            dstRect.Y = 0.0f;
            dstRect.Width = boxRect.Width;
            dstRect.Height = boxRect.Height;
            srcRect.X = 0.0f;
            srcRect.Y = 0.0f;
            srcRect.Width = dstRect.Width / zoom;
            srcRect.Height = dstRect.Height / zoom;

            //raise event if needed
            if (this.zoom != zoom)
            {
                this.zoom = zoom;
                OnZoomChanged(new ZoomEventArgs(zoom));
            }
        }
        private void PanelMain_MouseWheel(object sender, MouseEventArgs e)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("PanelMain_MouseWheel");
#endif
            if (ModifierKeys == Keys.Control)
            {
                if (SizeMode != SizeMode.Zoom)
                {
                    SizeMode = SizeMode.Zoom;
                }
                const float zoomFactor = 1.2f;
                float newZoom;

                if (e.Delta > 0)
                {
                    newZoom = Zoom * zoomFactor;
                }
                else
                {
                    newZoom = Zoom / zoomFactor;
                }
                newZoom = Math.Max(0.05f, Math.Min(newZoom, 16.0f));

                this.panelPicture.SuspendLayout();
                calculateRect(e.Location, newZoom);
                this.panelPicture.ResumeLayout();

                this.panelPicture.Invalidate();
            }
            else if (!WheelScrollLock && panelPicture.Height > panelMain.ClientSize.Height)
            {
                refreshDrawingRect();
            }
            else
            {
                EventHandler<MouseEventArgs> handler = MouseWheel;
                if (handler != null)
                    handler(this, e);
            }

        }
        private void PanelMain_Scroll(object sender, ScrollEventArgs e)
        {
            refreshDrawingRect();
        }
        private void panelPicture_Paint(object sender, PaintEventArgs e)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("panelPicture_Paint");
#endif 
            if (bitmap != null)
            {
                Graphics g = e.Graphics;
                g.CompositingMode = CompositingMode.SourceOver;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;

                if (Image.IsAlphaPixelFormat(bitmap.PixelFormat))
                {
                    using (var brush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.DarkGray, Color.Gray))
                    {
                        e.Graphics.FillRectangle(brush, dstRect);
                    }
                }

                else if (UseBackgroundBrush && transparentBackground != null)
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    using (TextureBrush textureBrush = new TextureBrush(transparentBackground, WrapMode.Tile))
                    {
                        g.FillRectangle(textureBrush, dstRect);
                    }
                }

                g.InterpolationMode = this.interpolationMode == InterpolationMode.Invalid ? InterpolationMode.Default : this.interpolationMode;
                try
                {
                    g.DrawImage(bitmap, dstRect, srcRect, GraphicsUnit.Pixel);
                }
                catch (ArgumentException ex)
                {
                    // Handle the case where the bitmap is invalid or disposed
                    Debug.WriteLine($"Error drawing image: {ex.Message}");
                }
            }
        }

        private void PictureBox_SizeChanged(object sender, EventArgs args)
        {
            Refresh();
        }

        private float getNextZoomValue(ZoomMode zoomMode)
        {
            float newZoom = zoom;

            if (zoomSteps.Count > 0)
            {
                float maxZoom = zoomSteps[zoomSteps.Count - 1];

                if (zoomMode == ZoomMode.In)
                {
                    //find next bigger zoom step
                    if (newZoom < maxZoom)
                    {
                        for (int i = 0; i < zoomSteps.Count; i++)
                        {
                            if (zoomSteps[i] > newZoom)
                            {
                                newZoom = zoomSteps[i];
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //find the next smaller zoom step
                    for (int i = zoomSteps.Count - 1; i >= 0; i--)
                    {
                        if (zoomSteps[i] < newZoom)
                        {
                            newZoom = zoomSteps[i];
                            break;
                        }
                    }
                }
            }

            return newZoom;

        }
        private void PictureBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (UseZoomCursors && this.ZoomInCursor != null && ZoomMouseButton != MouseButtons.None)
            {
                if ((e.KeyCode & ZoomOutModifier) != Keys.None)
                {
                    var coord = panelPicture.PointToClient(Cursor.Position);
                    if (panelPicture.DisplayRectangle.Contains(coord))
                    {
                        Cursor = new Cursor(ZoomInCursor.Handle);
                    }
                    else
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void PictureBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (UseZoomCursors && this.ZoomOutCursor != null && ZoomMouseButton != MouseButtons.None)
            {
                if ((e.KeyCode & ZoomOutModifier) != Keys.None)
                {
                    var coord = panelPicture.PointToClient(Cursor.Position);
                    if (panelPicture.DisplayRectangle.Contains(coord))
                    {
                        this.Cursor = new Cursor(ZoomOutCursor.Handle);
                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Allows catching arrows key as part of the WM_KEYDOWN message
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }
        private void panelPicture_MouseDown(object sender, MouseEventArgs e)
        {
            this.Focus();
            if (e.Button == ZoomMouseButton)
            {

            }
            else if (e.Button == DragMouseButton)
            {
                mousePosition = e.Location;
                if (DragCursor != null)
                {
                    this.Cursor = new Cursor(DragCursor.Handle);
                }

            }
        }

        private void panelPicture_MouseUp(object sender, MouseEventArgs e)
        {
            if (enableClickToZoom && e.Button == ZoomMouseButton)
            {
                float newZoom;
                if ((Control.ModifierKeys & ZoomOutModifier) != Keys.None)
                {
                    newZoom = getNextZoomValue(ZoomMode.Out);
                }
                else
                {
                    newZoom = getNextZoomValue(ZoomMode.In);
                }

                sizeMode = SizeMode.Zoom;
                calculateRect(e.Location, newZoom);
                this.panelPicture.Invalidate();
            }
            else if (e.Button == DragMouseButton)
            {
                if (UseZoomCursors && ZoomMouseButton != MouseButtons.None && ZoomInCursor != null && ZoomOutCursor != null)
                {
                    this.Cursor = new Cursor(ZoomInCursor.Handle);
                }
                else
                {
                    this.Cursor = defaultCursor;
                }
            }
        }
        private void panelPicture_MouseEnter(object sender, EventArgs e)
        {
            if (UseZoomCursors && ZoomMouseButton != MouseButtons.None && ZoomInCursor != null && ZoomOutCursor != null)
            {
                this.Cursor = new Cursor(ZoomInCursor.Handle);
            }
        }
        private void panelPicture_MouseLeave(object sender, EventArgs e)
        {
            if (this.Cursor != defaultCursor)
                this.Cursor = defaultCursor;

            OnPixelCoordinatesChanged(new CoordinatesEventArgs(true));
        }
        private void panelPicture_MouseMove(object sender, MouseEventArgs e)
        {
            if (bitmap == null)
            {
                return;
            }
            //pixel coords
            pixelCoordinates.X = e.Location.X / zoom;
            pixelCoordinates.Y = e.Location.Y / zoom;
            OnPixelCoordinatesChanged(new CoordinatesEventArgs(pixelCoordinates));

            if (DragMouseButton != MouseButtons.None && e.Button == DragMouseButton)
            {
                Point changePoint = Point.Empty;
                if (panelMain.VerticalScroll.Visible)
                {
                    changePoint.Y = e.Location.Y - mousePosition.Y;
                }
                if (panelMain.HorizontalScroll.Visible)
                {
                    changePoint.X = e.Location.X - mousePosition.X;
                }
                Point scroll = new Point(-panelMain.AutoScrollPosition.X - changePoint.X, -panelMain.AutoScrollPosition.Y - changePoint.Y);
                panelMain.AutoScrollPosition = scroll;

                refreshDrawingRect();
                draw();
            }
        }

        #endregion

        #region Constructor
        public PictureBox()
        {
            initialize([0.05f, 0.10f, 0.25f, 0.5f, 0.75f, 1.0f, 1.25f, 1.50f, 2.0f, 4.0f, 8.0f, 16.0f]);
        }

        public PictureBox(IEnumerable<float> zoomSteps)
        {
            initialize(zoomSteps);
        }

        private void initialize(IEnumerable<float> zoomSteps)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw, true);

            this.panelMain.Scroll += PanelMain_Scroll;
            this.panelMain.MouseWheel += PanelMain_MouseWheel;
            this.Bitmap = null;
            this.transparentBackground = null;
            this.zoom = 1.0f;
            this.verticalScrollSep = 0.1f; //10% is default scroll step
            this.verticalScrollSep = 0.1f; //10% is default scroll step
            this.srcRect = new RectangleF();
            this.dstRect = new RectangleF();
            this.boxRect = new Rectangle();
            this.WheelScrollLock = false;
            this.ZoomMouseButton = MouseButtons.Left;
            this.zoomSteps = new List<float>();
            this.pixelCoordinates = new PointF();
            this.zoomSteps.AddRange(zoomSteps);
            this.mousePosition = new Point();
            this.defaultCursor = this.Cursor;
            this.ZoomOutModifier = Keys.Alt | Keys.Menu;
            this.DragCursor = null;
            this.ZoomInCursor = null;
            this.ZoomOutCursor = null;
            this.UseZoomCursors = true;
            this.KeyDown += PictureBox_KeyDown;
            this.KeyUp += PictureBox_KeyUp;
            this.SizeChanged += PictureBox_SizeChanged;

        }
        #endregion

        #region Event Args
        public class ZoomEventArgs : EventArgs
        {
            private float zoom;

            public float Zoom { get { return zoom; } }

            public ZoomEventArgs(float zoom)
            {
                this.zoom = zoom;
            }
        }


        public class CoordinatesEventArgs : EventArgs
        {
            private PointF coord;
            private bool outOfBounds;

            public PointF PixelCoordinates { get { return coord; } }
            public bool OutOfBounds { get { return outOfBounds; } }

            public CoordinatesEventArgs(PointF coord)
            {
                this.coord = coord;
                this.outOfBounds = false;
            }

            public CoordinatesEventArgs(bool outOfBounds)
            {
                this.outOfBounds = outOfBounds;
                this.coord = PointF.Empty;

            }

        }
        #endregion

    }

    #region Enums
    public enum ZoomMode
    {
        In,
        Out
    }

    public enum ScrollDirection
    {
        Up,
        Down
    }

    public enum SizeMode : int
    {
        BestFit = 0,
        FitToWidth = 1,
        FitToHeight = 2,
        Normal = 3,
        Stretch = 4,
        Zoom = 5
    }

    #endregion

}
