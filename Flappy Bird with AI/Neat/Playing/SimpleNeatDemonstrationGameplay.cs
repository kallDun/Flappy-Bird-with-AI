using Flappy_Bird_with_AI.GameLogic;
using Flappy_Bird_with_AI.GameLogic.Components;
using Flappy_Bird_with_AI.Input;
using Flappy_Bird_with_AI.Output;
using Flappy_Bird_with_AI.Parameters;

namespace Flappy_Bird_with_AI.Neat.Playing
{
    class SimpleNeatDemonstrationGameplay : Gameplay
    {
        private readonly Background _background = new();

        public SimpleNeatDemonstrationGameplay(NeatPlayer player)
        {
            SetParams(Config.Data.Fps, new() { { new Bird(), player } });
            OnUpdate += (deltaTime) =>
            {
                if (IsGameOver && Keyboard.Pressed_R)
                {
                    RestartGame();
                }
            };
            new DrawingMaster(Config.Data.DrawFps, Players, _tubesList, _background, this).StartThread();
        }

        protected override void ThreadCall(double deltaTime)
        {
            base.ThreadCall(deltaTime);
            if (!IsGameOver) _background.Update(deltaTime);
        }

        public override void Dispose()
        {
            base.Dispose();
            DrawingMaster.IsDrawing = false;
        }
    }
}