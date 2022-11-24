using System;
using System.Drawing;


namespace Flappy_Bird_with_AI.GameLogic.Components
{
    public class Ring
    {
        private int _positionTubeBefore, _positionTubeAfter, _betweenTubes;
        public Point Coordinates { get; private set; }

        public Ring(int positionTubeBefore, int positionTubeAfter, int betweenTubes, double x)
        {
            _positionTubeBefore = positionTubeBefore;
            _positionTubeAfter = positionTubeAfter;
            _betweenTubes = betweenTubes;
            GetCoordinates(x);
        }

        private void GetCoordinates(double x)
        {
            int y_center1 = (-50 * _positionTubeBefore) - (_betweenTubes / 2) + 530 + (_betweenTubes) - (_betweenTubes / 2);
            int y_center2 = (-50 * _positionTubeAfter) - (_betweenTubes / 2) + 530 + (_betweenTubes) - (_betweenTubes / 2);

            Coordinates = new Point((int)Math.Round((x + 105) - 250), (y_center1 + y_center2) / 2);
        }


        public void DrawCall(Graphics g, double x)
        {
            GetCoordinates(x);
            g.DrawEllipse(new Pen(new SolidBrush(Color.Gold), 3), Coordinates.X, Coordinates.Y, 15, 15);
        }
    }
}
