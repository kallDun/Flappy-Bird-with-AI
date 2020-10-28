using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flappy_Bird_with_AI
{
    class Tube
    {
        private Image tubeImage = Resource1.tube_image;
        private Image tubeDownImage = Resource1.tube_Down_image;
        private GraphicsUnit units = GraphicsUnit.Pixel;

        private Random random = new Random();

        public int x { get; private set; } = 1000;
        public int y_tUp { get; private set; } 
        public int y_tDown { get; private set; }
        public int y_center { get; private set; }

        public int betweenTubes { get; private set; } = 130; // set number of pixels between tubes
        public int thisRandomPos { get; private set; } //except repeat position of the next Tube
        public bool closeToob = false;
        public bool forDestruct = false;

        public Ring ring { get; private set; }
        public Star star { get; private set; }

        public Tube(int thatRandomPos)
        {
            // random from - 475 to -50 ~ from 1 to 8 except last Tube position and *= (-54)
            do thisRandomPos = random.Next(1, 8);
            while (thisRandomPos == thatRandomPos);

            y_initialize();

            ring = new Ring(thisRandomPos, thatRandomPos, betweenTubes, x);
            star = random.Next(7) == 0? new Star(x, ring.coordinates.Y) : null; // 1 to 8 chance to generate star
        }

        private void y_initialize()
        {
            y_tUp = (-50 * thisRandomPos) - (betweenTubes / 2);
            y_tDown = y_tUp + 530 + (betweenTubes); // change the number 'betweenTubes' to regulate distance between
        }

        public void draw(Graphics g)
        {
            g.DrawImage(tubeImage, x, y_tUp, new Rectangle(0, 0, 225, 550), units);
            g.DrawImage(tubeDownImage, x, y_tDown, new Rectangle(0, 0, 225, 550), units);

            ring?.draw(g, x);
            star?.draw(g, x);
        }

        public void eatTheRing() { ring = null; }
        public void eatTheStar() { star = null; }

        public void update()
        {
            if (closeToob && betweenTubes >= 0)
            {
                betweenTubes -= 10;
                y_initialize();
            }

            x -= 5;
        }

    }
}
