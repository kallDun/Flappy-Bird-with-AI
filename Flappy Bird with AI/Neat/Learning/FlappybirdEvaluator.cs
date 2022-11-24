using Flappy_Bird_with_AI.Output;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Flappy_Bird_with_AI.Neat.Learning
{
    class FlappybirdEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        private ulong _evalCount;
        private bool _stopConditionSatisfied;
        public ulong EvaluationCount => _evalCount;
        public bool StopConditionSatisfied => _stopConditionSatisfied;


        public FitnessInfo Evaluate(IBlackBox box)
        {
            double seconds, verticalCloseness = 0;
            int tubes, stars, rings;
            NeatPlayer player = new(box);

            using (var gameplay = SimpleNeatLearningGameplay.GetSingleton(player))
            {
                gameplay.RestartGame();
                while (gameplay.IsGameOver is false)
                {
                    verticalCloseness = -Math.Abs(gameplay.GetVerticalTubeCloseness());

                    gameplay.Update();
                    if (gameplay.GetTubesScore() >= 240) break;
                }

                seconds = gameplay.GetLivedSeconds();
                tubes = gameplay.GetTubesScore();
                //stars = gameplay.GetStarsScore();
                //rings = gameplay.GetRingScore();
            }

            //double scores = tubes + rings * 0.25 + stars * 0.5;

            Logger.LogParameters("seconds", "closeness", new KeyValuePair<string, double>[] {
                new("seconds", seconds),
                new("closeness", verticalCloseness),
                //new("scores", tubes),
                new("tubes", tubes),
                //new("stars", stars),
                //new("rings", rings),
            });


            _evalCount++;
            if (tubes >= 60)
            {
                _stopConditionSatisfied = true;
            }

            return new(seconds, verticalCloseness);
        }

        public void Reset() { }
    }
}