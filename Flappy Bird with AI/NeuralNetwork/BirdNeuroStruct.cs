using Flappy_Bird_with_AI.GameLogic.Components;
using NeuronDotNet.Core.Backpropagation;

namespace Flappy_Bird_with_AI.NeuralNetwork
{
    public class BirdNeuroStruct
    {
        public Bird Bird { get; set; }
        public BackpropagationNetwork Network { get; set; }
        public double Fitness { get; set; }
    }
}