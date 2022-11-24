using System;
using System.Drawing;

namespace Flappy_Bird_with_AI.GameLogic.Components
{
    public class Bird
    {
        private readonly Image _birdImg = Resource1.bird_image;
        private readonly GraphicsUnit _units = GraphicsUnit.Pixel;

        public double X { get; private set; }
        public double Y { get; private set; }

        public int Counter { get; private set; }
        public double SecondsLive { get; private set; }
        public int RingCollected { get; private set; }
        public int StarsCollected { get; private set; }

        public bool IsGameOver { get; private set; }


        private double _acceler; // base acceleration

        public Bird()
        {
            SetNewGame();
        }

        public void SetNewGame()
        {
            IsGameOver = false;
            X = 300;
            Y = 250;
            _acceler = 0;
            Counter = 0;
            SecondsLive = 0;
            RingCollected = 0;
            StarsCollected = 0;
        }

        public void SetGameOver()
        {
            _acceler = 30;
            Jump();
            IsGameOver = true;
        }

        public void Jump()
        {
            _acceler = -Gameplay.JumpModifier;
        }

        public void DrawCall(Graphics g)
        {
            if (!IsGameOver)
            {
                int x = (int)Math.Round(X);
                int y = (int)Math.Round(Y);

                g.DrawImage(_birdImg, x, y, new Rectangle(0, 0, 60, 60), _units);
            }
        }

        public void Update(double deltaTime)
        {
            SecondsLive += deltaTime;

            if (Y < 700)
            {
                _acceler += Gameplay.AccelerationModifier * deltaTime;
                Y += _acceler * Gameplay.GravitationModifier * deltaTime;
            }
        }


        public void EatTheRing() => RingCollected++;
        public void EatTheStar() => StarsCollected++;
        public void IncrementCounter() => Counter++;
    }
}