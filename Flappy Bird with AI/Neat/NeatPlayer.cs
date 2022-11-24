using Flappy_Bird_with_AI.GameLogic.Interfaces;
using SharpNeat.Phenomes;

namespace Flappy_Bird_with_AI.Neat
{
    class NeatPlayer : IPlayer
    {
        public IBlackBox Brain { get; private set; }
        public NeatPlayer(IBlackBox brain)
        {
            Brain = brain;
        }

        public bool IsJumping(GameState gameState)
        {
            Brain.ResetState();

            Brain.InputSignalArray[0] = gameState.HorizontalTubeDistance;
            Brain.InputSignalArray[1] = gameState.VerticalTubeDistance;
            //Brain.InputSignalArray[2] = gameState.RingDistance;
            //Brain.InputSignalArray[3] = gameState.StarDistance;
            
            Brain.Activate();

            return Brain.OutputSignalArray[0] > 0.5;
        }
    }
}