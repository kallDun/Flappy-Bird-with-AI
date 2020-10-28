using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flappy_Bird_with_AI
{
    class Star
    {
        private Image newImage = Resource1.star_image;
        private GraphicsUnit units = GraphicsUnit.Pixel;

        public Point coordinates { get; private set; }
        private int ringY;

        public Star(int x, int ringY)
        {
            int add = ringY < 350 ? 220 : -220;
            this.ringY = (ringY - 25) + add;
            initializePoint(x);
        }

        public void initializePoint(int x)
        {
            coordinates = new Point((x + 105) - 220, ringY);
        }

        public void draw(Graphics g, int x)
        {
            initializePoint(x);
            g.DrawImage(newImage, coordinates.X, coordinates.Y, new Rectangle(0, 0, 50, 50), units);
        }

    }
}
