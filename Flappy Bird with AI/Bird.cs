using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flappy_Bird_with_AI
{
    class Bird
    {

        private Image newImage = Resource1.bird_image;
        private GraphicsUnit units = GraphicsUnit.Pixel;

        public int x { get; private set; }
        public int y { get; private set; }
        private int acceler; // base acceleration

        public Bird(){ newGame(); }

        public void newGame()
        {
            x = 300;
            y = 250;
            acceler = 0;
        }

        public void gameOver()
        {
            acceler = 30;
            clicked();
        }

        public void clicked(){ acceler = -16; }

        public void draw(Graphics g)
        {
            g.DrawImage(newImage, x, y, new Rectangle(0, 0, 60, 60), units);
        }

        public void update()
        {
            if (y < 700)
            {
                acceler += 2;
                y += acceler;
            }
        }

    }
}
