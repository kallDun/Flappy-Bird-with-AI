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
        private static string url_imageCITY = @"D:\Users7\Igor\Desktop\VIDEO_\UNIVERSITY PROJECTS\FlappyBird\Resized Images\City.png";
        private static string url_imageSKY = @"D:\Users7\Igor\Desktop\VIDEO_\UNIVERSITY PROJECTS\FlappyBird\Resized Images\Sky.png";
        
        GraphicsUnit units = GraphicsUnit.Pixel;
        private Image city1 = Image.FromFile(url_imageCITY);
        private Image city2 = Image.FromFile(url_imageCITY);
        private Image sky = Image.FromFile(url_imageSKY);

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
            g.DrawImage(city1, x, 0, new Rectangle(0, 0, 1100, 700), units);
            g.DrawImage(city2, x + 1100, 0, new Rectangle(0, 0, 1100, 700), units);
        }

    }
}
