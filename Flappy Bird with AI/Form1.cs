using Flappy_Bird_with_AI.GameLogic;
using Flappy_Bird_with_AI.Input;
using Flappy_Bird_with_AI.Neat;
using Flappy_Bird_with_AI.Output;
using Flappy_Bird_with_AI.Parameters;
using Flappy_Bird_with_AI.Parameters.Enums;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Flappy_Bird_with_AI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            pictureBox1.Image = new Bitmap(1100, 700);
            DrawingMaster.PictureBox = pictureBox1;
            Logger.LogLabel = label_info;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Config.Data.StartType == StartType.SinglePlay)
            {
                SingleLivePlayer();
            }
            else
            {
                NeatExperiment();
            }            
        }

        private void NeatExperiment()
        {
            NeatEntryPoint entryPoint = new();

            switch (Config.Data.StartType)
            {
                case StartType.NeatLearning:
                    entryPoint.StartLearning();
                    break;

                case StartType.NeatBestPlay:
                    entryPoint.LoadAndBestPlay();
                    break;
            }
            
            FormClosing += (sender, args) => System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void SingleLivePlayer()
        {
            Gameplay gameplay = new SingleLivePlayerGameplay();
            gameplay.RestartGame();
            FormClosing += (sender, args) => gameplay.Dispose();
        }

        #region KEYBOARD
        private void Form1_KeyDown(object sender, KeyEventArgs e) => PressKey(e.KeyCode, true);
        private void Form1_KeyUp(object sender, KeyEventArgs e) => PressKey(e.KeyCode, false);
        private static void PressKey(Keys key, bool pressed)
        {
            switch (key)
            {
                case Keys.Space:
                    Keyboard.Pressed_Space = pressed;
                    break;
                case Keys.R:
                    Keyboard.Pressed_R = pressed;
                    break;
            }
        }
        #endregion
    }
}