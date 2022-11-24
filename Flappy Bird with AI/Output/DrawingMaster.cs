using Flappy_Bird_with_AI.GameLogic;
using Flappy_Bird_with_AI.GameLogic.Components;
using Flappy_Bird_with_AI.GameLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flappy_Bird_with_AI.Output
{
    public class DrawingMaster
    {
        public static PictureBox PictureBox;
        public static bool IsDrawing { get; set; }


        private string _drawingText = "";

        private int fps;
        private Dictionary<Bird, IPlayer> _players;
        private List<Tube> _tubesList;
        private Background _background;
        private Gameplay _gameplay;
        public DrawingMaster(int fps, Dictionary<Bird, IPlayer> players, List<Tube> tubesList, Background background, Gameplay gameplay)
        {
            this.fps = fps;
            _players = players;
            _tubesList = tubesList;
            _background = background;
            _gameplay = gameplay;
        }

        public async void StartThread()
        {
            IsDrawing = true;
            while (IsDrawing)
            {
                await Task.Delay(1000 / fps);
                DrawCall();
            }
        }

        private void DrawCall()
        {
            Graphics g = Graphics.FromImage(PictureBox.Image);
            DrawCall(g);
            DrawText(g);
            try
            {
                PictureBox.Invoke(() => PictureBox.Refresh());
            }
            catch (Exception) { }
        }

        private void DrawCall(Graphics g)
        {
            _background.Draw(g);
            foreach (var tube in _tubesList)
            {
                tube.DrawCall(g);
            }
            foreach (var bird in _players.Keys)
            {
                bird.DrawCall(g);
            }
        }

        private void DrawText(Graphics g)
        {
            var counter = _players.Keys.Max(x => x.Counter);
            if (_gameplay.IsGameOver && int.TryParse(_drawingText, out int n))
            {
                _drawingText =
                    $"Game over!\nYou scored {counter} points.\n" +
                    $"Click 'R' to start new game!";
            }
            else
            if (!_gameplay.IsGameOver)
            {
                _drawingText = counter.ToString();
            }

            var drawFont = new Font("Times New Roman", 26f, FontStyle.Bold);
            var drawBrush = new SolidBrush(Color.White);
            var drawFormat = new StringFormat();
            g.DrawString(_drawingText, drawFont, drawBrush, 0, 0, drawFormat);
        }
    }
}