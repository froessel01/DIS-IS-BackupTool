using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BackupTool
{
    public sealed class RoundedPanel : Panel
    {
        private int _borderRadius = 12;
        private int _borderThickness = 1;
        private Color _borderColor = Color.FromArgb(52, 54, 62);

        [DefaultValue(12)]
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                Invalidate();
            }
        }

        [DefaultValue(1)]
        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;
                Invalidate();
            }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        public RoundedPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Avoid default background to prevent corner artifacts; we paint manually.
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var rect = ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using var path = BuildRoundRect(rect, _borderRadius);
            Region = new Region(path);

            using var clear = new SolidBrush(Parent?.BackColor ?? BackColor);
            e.Graphics.FillRectangle(clear, rect);

            using var fill = new SolidBrush(BackColor);
            e.Graphics.FillPath(fill, path);

            if (_borderThickness > 0)
            {
                using var pen = new Pen(_borderColor, _borderThickness);
                pen.Alignment = PenAlignment.Inset;
                e.Graphics.DrawPath(pen, path);
            }
        }

        private static GraphicsPath BuildRoundRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
