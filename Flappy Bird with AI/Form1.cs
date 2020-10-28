using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flappy_Bird_with_AI
{
    public partial class Form1 : Form
    {
        private Gameplay gameplay = new Gameplay();
        private Background background = new Background();
        private string drawingText = "";

        public Form1()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Start();
            tubeCreateTimer.Start();
            timerDraw.Start();
        }


        private void timer_Tick(object sender, EventArgs e) {
            if (!gameplay.gameOver) background.update();
            gameplay.update(); 
        }


        private void tubeCreateTimer_Tick(object sender, EventArgs e) { gameplay.createTube(); }

        private void draw()
        {
            pictureBox1.Image = new Bitmap(1100,700);
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            background.draw(g);
            gameplay.draw(g);
            drawText(g);
            pictureBox1.Refresh();
        }

        private void drawText(Graphics g)
        {
            if (gameplay.gameOver && int.TryParse(drawingText, out int n))
            {
                tubeCreateTimer.Stop();
                drawingText = $"Game over!\nYou scored {gameplay.counter} points.\n" +
                $"Click the 'R' to start new game!";
            }
            else
            if (!gameplay.gameOver)
                drawingText = gameplay.counter.ToString();

            var drawFont = new Font("Times New Roman", 26f, FontStyle.Bold);
            var drawBrush = new SolidBrush(Color.White);
            var drawFormat = new StringFormat();
            g.DrawString(drawingText, drawFont, drawBrush, 0, 0, drawFormat);
        }

        private void timerDraw_Tick(object sender, EventArgs e) { draw(); }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                if (!gameplay.gameOver) gameplay.Click();
            }
            else 
            if (e.KeyChar == 'r')
            {
                drawingText = "";
                gameplay.restart();
                tubeCreateTimer.Start();
            }
        }
    }

    
}
