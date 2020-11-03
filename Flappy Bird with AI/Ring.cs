using System.Drawing;


namespace Flappy_Bird_with_AI
{
    class Ring
    {
        private int thisRandomPos, thatRandomPos, betweenTubes;

        public Point coordinates { get; private set; }

        public Ring(int thisRandomPos, int thatRandomPos, int betweenTubes, int x)
        {
            this.thisRandomPos = thisRandomPos;
            this.thatRandomPos = thatRandomPos;
            this.betweenTubes = betweenTubes;
            getCoord(x);
        }

        private void getCoord(int x)
        {
            int y_center1 = (-50 * thisRandomPos) - (betweenTubes / 2) + 530 + (betweenTubes) - (betweenTubes / 2);
            int y_center2 = (-50 * thatRandomPos) - (betweenTubes / 2) + 530 + (betweenTubes) - (betweenTubes / 2);

            coordinates = new Point((x + 105) - 205, (y_center1 + y_center2) / 2);
        }


        public void draw(Graphics g, int x)
        {
            getCoord(x);
            g.DrawEllipse(new Pen(new SolidBrush(Color.Gold), 3), coordinates.X, coordinates.Y, 15, 15);
        }
    }
}
