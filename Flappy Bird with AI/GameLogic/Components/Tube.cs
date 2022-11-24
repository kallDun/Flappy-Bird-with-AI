using Flappy_Bird_with_AI.Parameters;
using System;
using System.Drawing;

namespace Flappy_Bird_with_AI.GameLogic.Components
{
    public class Tube
    {
        private readonly Image _tubeImage = Resource1.tube_image;
        private readonly Image _tubeDownImage = Resource1.tube_Down_image;
        private readonly GraphicsUnit _units = GraphicsUnit.Pixel;

        private readonly Random _rnd = new();

        public double X { get; private set; } = 1000;
        public int Ytop { get; private set; }
        public int Ybottom { get; private set; }
        public int Ycenter { get; private set; }

        public int DistanceBetweenTubes { get; private set; } = Config.Data.TubeHole; // set number of pixels between tubes
        public int TubeRandomPos { get; private set; }
        public bool IsToobClosed = false;
        public bool IsToobDestructable = false;

        public Ring Ring { get; private set; }
        public Star Star { get; private set; }

        private readonly int[] _tubePositionArray = Config.Data.TubesList;
        public int TubeIndex;

        public Tube(int thatRandomPos, int prevIndex)
        {
            if (Config.Data.IsTubesRandom)
            {
                do
                {
                    TubeRandomPos = _rnd.Next(1, 8);
                }
                while (TubeRandomPos == thatRandomPos);
            }
            else
            {
                TubeIndex = prevIndex % _tubePositionArray.Length;
                TubeRandomPos = _tubePositionArray[TubeIndex];
            }

            InitializeHeight();

            /*Ring = new Ring(TubeRandomPos, thatRandomPos, DistanceBetweenTubes, X);
            Star = _rnd.NextDouble() <= Config.Data.StarProbability
                ? new Star(X, Ring.Coordinates.Y) : null;*/
        }

        private void InitializeHeight()
        {
            Ytop = (-50 * TubeRandomPos) - (DistanceBetweenTubes / 2);
            Ybottom = Ytop + 540 + (DistanceBetweenTubes); // change the number 'betweenTubes' to regulate distance between
            Ycenter = Ytop + 540 + (DistanceBetweenTubes / 2);
        }

        public void DrawCall(Graphics g)
        {
            int x = (int)Math.Round(X);

            g.DrawImage(_tubeImage, x, Ytop, new Rectangle(0, 0, 225, 550), _units);
            g.DrawImage(_tubeDownImage, x, Ybottom, new Rectangle(0, 0, 225, 550), _units);

            Ring?.DrawCall(g, x);
            Star?.DrawCall(g, x);
        }

        public void EatTheRing() { Ring = null; }
        public void EatTheStar() { Star = null; }

        public void Update(double deltaTime)
        {
            if (IsToobClosed && DistanceBetweenTubes >= 0)
            {
                DistanceBetweenTubes -= 10;
                InitializeHeight();
            }

            X -= Gameplay.SpeedModifier * deltaTime;
        }
    }
}