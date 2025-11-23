using System;
using System.Drawing;
using System.Windows.Forms;

namespace GraphProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Resize += (s, e) => this.Invalidate(); // Перемальовує графік при зміні розміру
            this.DoubleBuffered = true; // Усуває мерехтіння
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Розміри області малювання
            int w = ClientSize.Width;
            int h = ClientSize.Height;

            // Фон
            g.Clear(Color.White);

            // Вісь X та Y
            Pen axisPen = new Pen(Color.Black, 2);
            g.DrawLine(axisPen, 40, h / 2, w - 20, h / 2); // X-вісь
            g.DrawLine(axisPen, 40, 20, 40, h - 20);      // Y-вісь

            // Параметри функції
            double xStart = 2.3;
            double xEnd = 7.8;
            double step = 0.9;

            // Масштаб
            double scaleX = (w - 60) / (xEnd - xStart); // ширина графіка
            double scaleY = 50;                        // коефіцієнт розтягування по Y

            // Кисть для графіка
            Pen graphPen = new Pen(Color.Red, 2);

            // Малювання графіка
            PointF? prevPoint = null;

            for (double x = xStart; x <= xEnd; x += step)
            {
                double denominator = Math.Sin(3 * x) - x;

                if (Math.Abs(denominator) < 0.0001)
                    continue; // уникнення ділення на 0

                double y = (6 * x + 4) / denominator;

                // Перетворення координат
                float screenX = (float)(40 + (x - xStart) * scaleX);
                float screenY = (float)(h / 2 - y * scaleY);

                PointF currentPoint = new PointF(screenX, screenY);

                if (prevPoint != null)
                    g.DrawLine(graphPen, prevPoint.Value, currentPoint);

                prevPoint = currentPoint;
            }
        }
    }
}
