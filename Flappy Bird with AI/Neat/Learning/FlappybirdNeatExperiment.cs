using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace Flappy_Bird_with_AI.Neat.Learning
{
    class FlappybirdNeatExperiment : SimpleNeatExperiment
    {
        public override IPhenomeEvaluator<IBlackBox> PhenomeEvaluator => new FlappybirdEvaluator();

        public override int InputCount => 2;

        public override int OutputCount => 1;

        public override bool EvaluateParents => true;
    }
}
