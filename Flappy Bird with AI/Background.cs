using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Flappy_Bird_with_AI
{
    class Background
    {
        GraphicsUnit units = GraphicsUnit.Pixel;
        private Image city = Resource1.city_image;
        private Image sky = Resource1.sky_image;

        private int x = 0; 

        public void update()
        {
            x -= 5;
            if (x <= -1100)
            {
                x = 0;
            }
        }

        public void draw(Graphics g)
        {
            g.DrawImage(sky, 0, 0, new Rectangle(0, 0, 1100, 700), units);
            g.DrawImage(city, x, 0, new Rectangle(0, 0, 1100, 700), units);
            g.DrawImage(city, x + 1100, 0, new Rectangle(0, 0, 1100, 700), units);
        }

    }
}
