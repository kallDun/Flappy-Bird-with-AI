using System;
using System.Drawing;

namespace Flappy_Bird_with_AI.GameLogic.Components
{
    public class Background
    {
        readonly GraphicsUnit _units = GraphicsUnit.Pixel;
        private readonly Image _city = Resource1.city_image;
        private readonly Image _sky = Resource1.sky_image;

        private double X = 0; 

        public void Update(double deltaTime)
        {
            X -= Gameplay.SpeedModifier * deltaTime;
            if (X <= -1100)
            {
                X += 1100;
            }
        }

        public void Draw(Graphics g)
        {
            int x = (int)Math.Round(X);
            g.DrawImage(_sky, 0, 0, new Rectangle(0, 0, 1100, 700), _units);
            g.DrawImage(_city, x, 0, new Rectangle(0, 0, 1100, 700), _units);
            g.DrawImage(_city, x + 1100, 0, new Rectangle(0, 0, 1100, 700), _units);
        }

    }
}