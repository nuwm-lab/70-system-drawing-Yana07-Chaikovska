using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GraphProject
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new GraphForm());
        }
    }

    public class GraphForm : Form
    {
        private const double XStart = 2.3;
        private const double XEnd = 7.8;
        private const double Step = 0.01;

        private readonly Pen axisPen = new Pen(Color.Black, 2);
        private readonly Pen graphPen = new Pen(Color.Red, 2);

        public GraphForm()
        {
            this.Text = "Графік функції";
            this.Width = 900;
            this.Height = 600;

            this.DoubleBuffered = true;

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);

            this.ResizeRedraw = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle rect = this.ClientRectangle;
            if (rect.Width < 100 || rect.Height < 100)
                return;

            int left = 60;
            int right = rect.Width - 20;
            int top = 20;
            int bottom = rect.Height - 40;

            g.DrawLine(axisPen, left, rect.Height / 2, right, rect.Height / 2);
            g.DrawLine(axisPen, left, top, left, bottom);

            List<PointF> points = GeneratePoints(rect, left, right, top, bottom);
            DrawGraph(g, points);
        }

        private List<PointF> GeneratePoints(Rectangle rect, int left, int right, int top, int bottom)
        {
            var points = new List<PointF>();
            List<double> ys = new List<double>();

            for (double x = XStart; x <= XEnd; x += Step)
            {
                double d = Math.Sin(3 * x) - x;
                if (Math.Abs(d) < 1e-6) continue;

                double y = (6 * x + 4) / d;
                if (!double.IsInfinity(y) && !double.IsNaN(y))
                    ys.Add(y);
            }

            double yMin = ys.Min();
            double yMax = ys.Max();
            double yRange = yMax - yMin;
            if (yRange == 0) yRange = 1;

            double scaleX = (right - left) / (XEnd - XStart);
            double scaleY = (bottom - top) / yRange;

            for (double x = XStart; x <= XEnd; x += Step)
            {
                double d = Math.Sin(3 * x) - x;
                if (Math.Abs(d) < 1e-6)
                {
                    points.Add(PointF.Empty);
                    continue;
                }

                double y = (6 * x + 4) / d;

                if (double.IsInfinity(y) || double.IsNaN(y))
                {
                    points.Add(PointF.Empty);
                    continue;
                }

                float sx = (float)(left + (x - XStart) * scaleX);
                float sy = (float)(bottom - (y - yMin) * scaleY);

                points.Add(new PointF(sx, sy));
            }

            return points;
        }

        private void DrawGraph(Graphics g, List<PointF> points)
        {
            PointF? prev = null;

            foreach (var p in points)
            {
                if (p == PointF.Empty)
                {
                    prev = null;
                    continue;
                }

                if (prev != null)
                    g.DrawLine(graphPen, prev.Value, p);

                prev = p;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                axisPen.Dispose();
                graphPen.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
