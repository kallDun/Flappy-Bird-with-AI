using NeuronDotNet.Core.Backpropagation;
using btl.generic;
using System.Threading.Tasks;
using System.Collections.Generic;
using Flappy_Bird_with_AI.Global;
using Flappy_Bird_with_AI.Files;
using System.Windows.Forms;
using Flappy_Bird_with_AI.GameLogic;
using Flappy_Bird_with_AI.GameLogic.Components;
using Flappy_Bird_with_AI.GameLogic.Obsolete;

namespace Flappy_Bird_with_AI.NeuralNetwork
{
    partial class GeneticAlgorithm
    {
        public int GetGenerationNumber => algorithm.GenerationOn;
        private ObsoleteGameplay _gameplay;
        private GA algorithm;
        private List<BirdNeuroStruct> _birds = new List<BirdNeuroStruct>();
        public GeneticAlgorithm(ObsoleteGameplay gameplay)
        {
            _gameplay = gameplay;
        }

        #region Logic
        public void StartAlgorithm()
        {
            if (GlobalNeuralParams.NeuralType == Enums.NeuralNetworkType.LearnFromStart)
            {
                algorithm = new GA(GlobalNeuralParams.CrossoverRate, GlobalNeuralParams.MutationRate, GlobalNeuralParams.PopulationSize, GlobalNeuralParams.GenerationSize, genomeSize: 40)
                {
                    FitnessFunction = new GAFunction(FitnessFunction),
                    Elitism = GlobalNeuralParams.Elitism,
                    Gameplay = _gameplay,
                    Birds = _birds
                };
                algorithm.Go();
            }
            else
            if (GlobalNeuralParams.NeuralType == Enums.NeuralNetworkType.LoadAndLearn)
            {
                algorithm = NeuroFileManager.LoadNeuro();
                algorithm.FitnessFunction = new GAFunction(FitnessFunction);
                algorithm.Generations = GlobalNeuralParams.GenerationSize;
                algorithm.Gameplay = _gameplay;
                algorithm.Birds = _birds;
                algorithm.Go();
            }
            else
            if (GlobalNeuralParams.NeuralType == Enums.NeuralNetworkType.LoadAndBestPlay)
            {
                algorithm = NeuroFileManager.LoadNeuro();
                algorithm.FitnessFunction = new GAFunction(FitnessFunction);
                algorithm.Gameplay = _gameplay;
                algorithm.Birds = _birds;

                algorithm.GetBest(out double[] weights, out double fitness);
                var network = GenerateNetwork();
                SetNetworkWeights(network, weights);
                StartSingleGeneticPlay(network);
            }
        }
        public void Save() => NeuroFileManager.SaveNeuro(algorithm);
        private async void StartSingleGeneticPlay(BackpropagationNetwork network)
        {
            await _gameplay.Restart(1);
            _birds.Add(new BirdNeuroStruct
            {
                Bird = _gameplay.Birds[0],
                Network = network
            });
        }
        #endregion

        #region NeuralFunctions
        private BackpropagationNetwork GenerateNetwork()
        {
            ActivationLayer inputLayer = new LinearLayer(3);
            ActivationLayer hiddenLayer = new LinearLayer(5);
            ActivationLayer outputLayer = new LinearLayer(1);

            BackpropagationConnector connector = new BackpropagationConnector(inputLayer, hiddenLayer);
            BackpropagationConnector connector2 = new BackpropagationConnector(hiddenLayer, outputLayer);

            BackpropagationNetwork network;
            network = new BackpropagationNetwork(inputLayer, outputLayer);
            network.Initialize();
            return network;
        }
        public void SetNetworkWeights(BackpropagationNetwork aNetwork, double[] weights)
        {
            // Setup the network's weights.
            int index = 0;

            foreach (BackpropagationConnector connector in aNetwork.Connectors)
            {
                foreach (BackpropagationSynapse synapse in connector.Synapses)
                {
                    synapse.Weight = weights[index++];
                    synapse.SourceNeuron.bias = (weights[index++]);
                }
            }
        }
        #endregion

        #region GeneticFunctions
        public async Task<double> FitnessFunction(Bird bird, double[] weights)
        {
            var network = GenerateNetwork();
            SetNetworkWeights(network, weights);
            BirdNeuroStruct item = new BirdNeuroStruct
            {
                Bird = bird,
                Network = network,
                Fitness = 0
            };
            _birds.Add(item);

            do
            {
                await Task.Delay(100);
            } 
            while (bird.IsGameOver is false);

            //item.Fitness += bird.Counter * 5;
            return item.Fitness;
        }

        double _updateCalls = 0, _updateFreq = 0.2;
        public void UpdateCall(double deltaTime)
        {
            _updateCalls += _updateFreq;
            while (_updateCalls >= 1)
            {
                _updateCalls--;
                foreach (var item in _birds)
                {
                    if (item.Bird.IsGameOver) continue;
                    else
                    {
                        var distToTube = _gameplay.GetDistanceToTube(item.Bird);
                        distToTube /= 1000;

                        double fitness = 0.01;
                        if (distToTube == 0) fitness = 2;
                        else fitness /= distToTube;
                        if (fitness > 2) fitness = 2;

                        item.Fitness += fitness;
                        item.Fitness += 0.2;
                    }

                    double formWidth = 1280;
                    double formHeigth = 720;

                    var (horizontalTube, verticalTube, top, bottom, tube) = _gameplay.GetGameState(item.Bird);

                    var output = item.Network.Run(new double[] 
                    { 
                        horizontalTube, 
                        verticalTube, 
                        //top / formHeigth, 
                        //bottom / formHeigth, 
                        tube
                    })[0];
                    
                    if (output < 0)
                    {
                        item.Bird.Jump();
                    }
                }
            }
        }
        #endregion
    }
}