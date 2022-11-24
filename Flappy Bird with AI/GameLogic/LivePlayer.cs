using Flappy_Bird_with_AI.GameLogic.Interfaces;
using Flappy_Bird_with_AI.Input;

namespace Flappy_Bird_with_AI.GameLogic
{
    class LivePlayer : IPlayer
    {
        public bool IsJumping(GameState gameState)
        {
            if (Keyboard.Pressed_Space)
            {
                Keyboard.Pressed_Space = false;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}