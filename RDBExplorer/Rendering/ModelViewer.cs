using Metanoia.GUI;
using Metanoia.Modeling;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Metanoia.Rendering
{
    public partial class ModelViewer : UserControl
    {
        public Matrix4 Camera;

        private GenericRenderer GenericRenderer;
        private bool _glReady = false;
        private Vector3 _target = Vector3.Zero;
        private float _yaw = 0f;
        private float _pitch = 20f;
        private float _distance = 100f;
        private Vector3 _defaultTarget = Vector3.Zero;
        private float _defaultDistance = 100f;

        private bool ShowBones = false;

        private int Frame { get => _frame; set { _frame = value; frameLabel.Text = $"Frame: {_frame} / {MaxFrame}"; } }
        private int MaxFrame { get => _maxFrame; set { _maxFrame = value; frameLabel.Text = $"Frame: {Frame} / {_maxFrame}"; } }
        private int _frame = 0, _maxFrame = 0;
        private bool _isPlaying = false;
        private bool IsPlaying
        {
            get => _isPlaying;
            set { _isPlaying = value; }
        }

        private GenericModel Model { get; set; }
        private ModelInfoPanel ModelPanel = new ModelInfoPanel();
        private int _prevX, _prevY;

        public ModelViewer()
        {
            InitializeComponent();

            foreach (var value in Enum.GetValues(typeof(RenderMode)))
                renderMode.ComboBox.Items.Add(value);
            renderMode.ComboBox.SelectedIndex = 0;

            animationTS.Visible = false;

            var timer = new System.Timers.Timer(1000.0 / 60.0);
            timer.Elapsed += (s, a) =>
            {
                if (Viewport.IsDisposed)
                {
                    timer.Stop(); timer.Dispose();
                    return;
                }
                Viewport.Invalidate();
            };
            timer.Start();
        }

        private void UpdateCamera()
        {
            float yawRad = MathHelper.DegreesToRadians(_yaw);
            float pitchRad = MathHelper.DegreesToRadians(_pitch);

            var camPos = _target + new Vector3(
                _distance * MathF.Cos(pitchRad) * MathF.Sin(yawRad),
                _distance * MathF.Sin(pitchRad),
                _distance * MathF.Cos(pitchRad) * MathF.Cos(yawRad)
            );

            var view = Matrix4.LookAt(camPos, _target, Vector3.UnitY);

            float aspect = (Viewport.Height > 0 && Viewport.Width > 0)
                ? Viewport.Width / (float)Viewport.Height : 1f;
            var proj = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(60f), aspect, 0.01f, 10_000_000f);

            Camera = view * proj;
        }

        private void SetupViewport()
        {
            Viewport.MakeCurrent();
#if DEBUG
            Debug.WriteLine($"[GL] {GL.GetString(StringName.Vendor)} | {GL.GetString(StringName.Renderer)} | {GL.GetString(StringName.Version)}");
#endif
            GL.ClearColor(0.18f, 0.18f, 0.18f, 1f);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            _glReady = true;
            UpdateCamera();

            GenericRenderer = new GenericRenderer();
            renderMode.ComboBox.SelectedValueChanged += UpdateRenderMode;
            Viewport.MouseWheel += OnMouseWheel;
        }

        private void UpdateRenderMode(object sender, EventArgs args)
        {
            GenericRenderer.RenderMode = (RenderMode)renderMode.SelectedItem;
            Viewport.Invalidate();
        }

        public void SetModel(GenericModel model)
        {
            if (model == null)
                return;
            Model = model;
            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);
            bool hasVerts = false;

            foreach (var mesh in model.Meshes)
            {
                foreach (var v in mesh.Vertices)
                {
                    min = Vector3.ComponentMin(min, v.Pos);
                    max = Vector3.ComponentMax(max, v.Pos);
                    hasVerts = true;
                }
            }

            if (hasVerts)
            {
                _defaultTarget = (min + max) * 0.5f;
                _defaultDistance = (max - min).Length * 1.5f;
                if (_defaultDistance < 1f)
                    _defaultDistance = 10f;

#if DEBUG
                Debug.WriteLine($"[SetModel] center={_defaultTarget} dist={_defaultDistance}");
#endif
            }

            ResetView();

            ModelPanel.SetModel(model);
            if (GenericRenderer != null)
            {
                GenericRenderer.SetGenericModel(Model);
            }
        }

        public void ResetView()
        {
            _target = _defaultTarget;
            _distance = _defaultDistance;
            _yaw = 0f;
            _pitch = 20f;
            UpdateCamera();
            Viewport.Invalidate();
        }

        private void RenderAxisGizmo()
        {
            int vpW = Viewport.Width;
            int vpH = Viewport.Height;

            const int GizmoSize = 80;
            const int GizmoPad = 20;
            int gizmoX = GizmoPad;
            int gizmoY = GizmoPad;

            GL.Viewport(gizmoX, gizmoY, GizmoSize, GizmoSize);

            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(-1.6, 1.6, -1.6, 1.6, -10, 10);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();


            float yawRad = MathHelper.DegreesToRadians(_yaw);
            float pitchRad = MathHelper.DegreesToRadians(_pitch);

            var rotView = Matrix4.CreateRotationX(-pitchRad) * Matrix4.CreateRotationY(-yawRad);
            GL.LoadMatrix(ref rotView);

            GL.Disable(EnableCap.DepthTest);
            GL.LineWidth(2.5f);

            float axisLen = 1.0f;
            float tipSize = 0.07f;

            DrawAxis(Vector3.Zero, new Vector3(axisLen, 0, 0),
                     new Vector3(-axisLen * 0.3f, 0, 0),
                     new Color4(0.95f, 0.22f, 0.22f, 1f),
                     new Color4(0.6f, 0.1f, 0.1f, 1f),
                     "X", tipSize);

            DrawAxis(Vector3.Zero, new Vector3(0, axisLen, 0),
                     new Vector3(0, -axisLen * 0.3f, 0),
                     new Color4(0.22f, 0.85f, 0.22f, 1f),
                     new Color4(0.1f, 0.5f, 0.1f, 1f),
                     "Y", tipSize);

            DrawAxis(Vector3.Zero, new Vector3(0, 0, axisLen),
                     new Vector3(0, 0, -axisLen * 0.3f),
                     new Color4(0.22f, 0.45f, 0.95f, 1f),
                     new Color4(0.1f, 0.2f, 0.6f, 1f),
                     "Z", tipSize);

            GL.PointSize(6f);
            GL.Begin(PrimitiveType.Points);
            GL.Color4(0.9f, 0.9f, 0.9f, 1f);
            GL.Vertex3(0f, 0f, 0f);
            GL.End();

            GL.Enable(EnableCap.DepthTest);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();

            GL.Viewport(0, 0, vpW, vpH);
        }

        private void DrawAxis(Vector3 origin, Vector3 tip, Vector3 negativeTip,
                              Color4 color, Color4 dimColor, string label, float tipSize)
        {
            GL.LineWidth(2.5f);
            GL.Begin(PrimitiveType.Lines);
            GL.Color4(color);
            GL.Vertex3(origin);
            GL.Vertex3(tip);
            GL.End();

            GL.LineWidth(1.5f);
            GL.Enable(EnableCap.LineStipple);
            GL.LineStipple(2, 0xAAAA);
            GL.Begin(PrimitiveType.Lines);
            GL.Color4(dimColor);
            GL.Vertex3(origin);
            GL.Vertex3(negativeTip);
            GL.End();
            GL.Disable(EnableCap.LineStipple);

            GL.PointSize(10f);
            GL.Begin(PrimitiveType.Points);
            GL.Color4(color);
            GL.Vertex3(tip);
            GL.End();

            DrawSphere(tip, tipSize, color);
        }


        private void DrawSphere(Vector3 center, float radius, Color4 color)
        {
            GL.Color4(color);

            int segments = 12;
            for (int plane = 0; plane < 3; plane++)
            {
                GL.LineWidth(1.5f);
                GL.Begin(PrimitiveType.LineLoop);
                for (int i = 0; i < segments; i++)
                {
                    float a = MathF.PI * 2f * i / segments;
                    float x = MathF.Cos(a) * radius;
                    float y = MathF.Sin(a) * radius;
                    switch (plane)
                    {
                        case 0:
                            GL.Vertex3(center.X + x, center.Y + y, center.Z);
                            break;
                        case 1:
                            GL.Vertex3(center.X + x, center.Y, center.Z + y);
                            break;
                        case 2:
                            GL.Vertex3(center.X, center.Y + x, center.Z + y);
                            break;
                    }
                }
                GL.End();
            }
        }

        private void Viewport_Paint(object sender, PaintEventArgs e) => RenderScene();

        private void RenderScene()
        {
            if (!_glReady || GenericRenderer == null)
                return;

            Viewport.MakeCurrent();

            var err = GL.GetError();
            if (err != ErrorCode.NoError)
                Console.WriteLine($"[GL ERR] {err}");

            GL.Viewport(0, 0, Viewport.Width, Viewport.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            UpdateCamera();

            float yawRad = MathHelper.DegreesToRadians(_yaw);
            float pitchRad = MathHelper.DegreesToRadians(_pitch);
            var camPos = _target + new Vector3(
                _distance * MathF.Cos(pitchRad) * MathF.Sin(yawRad),
                _distance * MathF.Sin(pitchRad),
                _distance * MathF.Cos(pitchRad) * MathF.Cos(yawRad));

            var viewOnly = Matrix4.LookAt(camPos, _target, Vector3.UnitY);
            float aspect = Viewport.Width / (float)Math.Max(Viewport.Height, 1);
            var projOnly = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(60f), aspect, 0.01f, 10_000_000f);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projOnly);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref viewOnly);

            RenderFloor();

            if (!GenericRenderer.HasModelSet && Model != null)
                GenericRenderer.SetGenericModel(Model);
            if (IsPlaying)
                buttonNext_Click(null, null);
            GenericRenderer.RenderShader(Camera, ShowBones);

            RenderAxisGizmo();

            Viewport.SwapBuffers();
        }

        private void RenderFloor()
        {
            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.Disable(EnableCap.DepthTest);
            GL.LineWidth(1f);

            float unit = _defaultDistance / 10f;
            int lines = 20;
            float size = unit * lines;

            GL.Begin(PrimitiveType.Lines);
            for (int i = -lines; i <= lines; i++)
            {
                float t = i * unit;
                bool isAxis = (i == 0);
                GL.Color3(isAxis ? 0.6f : 0.3f, isAxis ? 0.6f : 0.3f, isAxis ? 0.6f : 0.3f);
                GL.Vertex3(-size, _target.Y - _defaultDistance * 0.3f, t);
                GL.Vertex3(size, _target.Y - _defaultDistance * 0.3f, t);
                GL.Vertex3(t, _target.Y - _defaultDistance * 0.3f, -size);
                GL.Vertex3(t, _target.Y - _defaultDistance * 0.3f, size);
            }
            GL.End();

            GL.Enable(EnableCap.DepthTest);
            GL.PopAttrib();
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            int dx = e.X - _prevX;
            int dy = e.Y - _prevY;

            if (e.Button == MouseButtons.Left)
            {
                _yaw += dx * 0.5f;
                _pitch -= dy * 0.5f;
                _pitch = Math.Clamp(_pitch, -89f, 89f);
                UpdateCamera();
                Viewport.Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                float speed = (_distance * 0.001f);

                float yawRad = MathHelper.DegreesToRadians(_yaw);
                var right = new Vector3(MathF.Cos(yawRad), 0, -MathF.Sin(yawRad));
                var up = Vector3.UnitY;

                _target -= right * dx * speed;
                _target += up * dy * speed;

                UpdateCamera();
                Viewport.Invalidate();
            }

            _prevX = e.X;
            _prevY = e.Y;
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            float factor = e.Delta > 0 ? 0.9f : 1.1f;
            _distance *= factor;
            float minZoom = 0.1f;
            float maxZoom = Math.Max(_defaultDistance * 20f, 5000f);

            _distance = Math.Clamp(_distance, minZoom, maxZoom);

            UpdateCamera();
            Viewport.Invalidate();
        }

        private void Viewport_KeyDown(object sender, KeyPressEventArgs e)
        {
            float step = _distance * 0.1f;
            float yawRad = MathHelper.DegreesToRadians(_yaw);
            var forward = new Vector3(MathF.Sin(yawRad), 0, MathF.Cos(yawRad));

            if (e.KeyChar == 'w' || e.KeyChar == 'W')
            {
                _target += forward * step;
                UpdateCamera();
                Viewport.Invalidate();
            }
            if (e.KeyChar == 's' || e.KeyChar == 'S')
            {
                _target -= forward * step;
                UpdateCamera();
                Viewport.Invalidate();
            }
            if (e.KeyChar == 'f' || e.KeyChar == 'F')
            {
                ResetView(); // F = focus/reset
            }
        }

        private void Viewport_Load(object sender, EventArgs e) => SetupViewport();

        private void Viewport_Resize(object sender, EventArgs e)
        {
            if (_glReady)
            {
                UpdateCamera();
                Viewport.Invalidate();
            }
        }

        private void resetViewButton_Click(object sender, EventArgs e) => ResetView();

        private void modelPaneInfoButton_Click(object sender, EventArgs e)
        {
            if (Model == null)
                return;
            if (!ModelPanel.Visible)
                ModelPanel.Show();
        }

        private void showBoneButton_Click(object sender, EventArgs e)
        {
            ShowBones = !ShowBones;
            showBoneButton.Checked = ShowBones;
            Viewport.Invalidate();
        }

        private void renderToFileButtton_Click(object sender, EventArgs e)
        {
            if (Model == null)
                return;
            RenderToFile(Viewport.Width, Viewport.Height);
        }

        private void exportButton_Click(object sender, EventArgs e) => ExportModel();
        private void exportModelToolStripMenuItem_Click(object sender, EventArgs e) => ExportModel();
        public void ExportModel() { /* TODO */ }

        public void EnableAnimation()
        {
            animationTS.Visible = true;
            Frame = 0;
        }

        public void AddAnimation(GenericAnimation animation)
        {
            animationCB.Items.Add(animation);
            animationCB.SelectedItem = animation;
            EnableAnimation();
            if (Model?.Skeleton != null)
                animation.UpdateSkeleton(Frame, Model.Skeleton);
            ResetView();
        }

        private void animationCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (animationCB.SelectedItem is GenericAnimation anim)
                MaxFrame = anim.FrameCount;
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            Frame = (Frame + 1) % Math.Max(MaxFrame, 1);
            if (Model?.Skeleton != null && animationCB.SelectedItem is GenericAnimation anim)
                anim.UpdateSkeleton(Frame, Model.Skeleton);
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            Frame = (Frame - 1 + MaxFrame) % Math.Max(MaxFrame, 1);
            if (Model?.Skeleton != null && animationCB.SelectedItem is GenericAnimation anim)
                anim.UpdateSkeleton(Frame, Model.Skeleton);
        }

        private void buttonPlay_Click(object sender, EventArgs e) => IsPlaying = !IsPlaying;
        private void buttonBegin_Click(object sender, EventArgs e)
        {
            Frame = 0;
            if (Model?.Skeleton != null && animationCB.SelectedItem is GenericAnimation a)
                a.UpdateSkeleton(Frame, Model.Skeleton);
        }
        private void buttonEnd_Click(object sender, EventArgs e)
        {
            Frame = MaxFrame;
            if (Model?.Skeleton != null && animationCB.SelectedItem is GenericAnimation a)
                a.UpdateSkeleton(Frame, Model.Skeleton);
        }

        private void exportAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (animationCB.SelectedItem is GenericAnimation anim)
            {
                IsPlaying = false;
                Frame = 0;
                MessageBox.Show("Exported!");
            }
        }


        private void RenderToFile(int width, int height)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.Viewport(0, 0, width, height);
            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GenericRenderer.RenderShader(Camera, false);
            GL.PopAttrib();

            var bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] pixels = new byte[width * height * 4];
            GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
            var bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            Marshal.Copy(pixels, 0, bd.Scan0, pixels.Length);
            bmp.UnlockBits(bd);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            bmp.Save("Render.png");
            bmp.Dispose();
        }

        private void Viewport_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Viewport.Cursor = Cursors.SizeAll;
            }
            else if (e.Button == MouseButtons.Left)
            {
                Viewport.Cursor = Cursors.Hand;
            }
        }

        private void Viewport_MouseUp(object sender, MouseEventArgs e)
        {
            Viewport.Cursor = Cursors.Default;
        }
    }
}