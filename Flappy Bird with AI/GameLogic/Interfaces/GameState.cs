namespace Flappy_Bird_with_AI.GameLogic.Interfaces
{
    public struct GameState
    {
        public GameState(double horizontalTubeDistance, double verticalTubeDistance, double ringDistance, double starDistance)
        {
            HorizontalTubeDistance = horizontalTubeDistance;
            VerticalTubeDistance = verticalTubeDistance;
            RingDistance = ringDistance;
            StarDistance = starDistance;
        }

        public double HorizontalTubeDistance { get; set; }
        public double VerticalTubeDistance { get; set; }
        public double RingDistance { get; set; }
        public double StarDistance { get; set; }
    }
}