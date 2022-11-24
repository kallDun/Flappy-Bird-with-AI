using System;
using System.Drawing;


namespace Flappy_Bird_with_AI.GameLogic.Components
{
    public class Star
    {
        private readonly Image _starImage = Resource1.star_image;
        private GraphicsUnit _units = GraphicsUnit.Pixel;

        public Point Coordinates { get; private set; }
        private double Y;

        public Star(double x, double y)
        {
            int add = y < 350 ? 220 : -220;
            Y = (y - 25) + add;
            InitializePosition(x);
        }

        public void InitializePosition(double x)
        {
            Coordinates = new Point((int)Math.Round((x + 105) - 275), (int)Math.Round(Y));
        }

        public void DrawCall(Graphics g, double x)
        {
            InitializePosition(x);
            g.DrawImage(_starImage, Coordinates.X, Coordinates.Y, new Rectangle(0, 0, 50, 50), _units);
        }
    }
}