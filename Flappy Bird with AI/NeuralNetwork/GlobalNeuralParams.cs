using Flappy_Bird_with_AI.Enums;
using System;

namespace Flappy_Bird_with_AI.Global
{
    public static class GlobalNeuralParams
    {
        public static string ModelFilePath => $"{Environment.CurrentDirectory}\\model.json";
        public static string ModelLogsDirectoryPath => $"{Environment.CurrentDirectory}\\prev_models\\";
        public static NeuralNetworkType NeuralType => NeuralNetworkType.LearnFromStart;
        public static double CrossoverRate => 0.6;
        public static double MutationRate => 0.1;
        public static int PopulationSize => 20;
        public static int GenerationSize => 10000;
        public static bool Elitism => true;
        public static bool WriteBestFitnessesEveryItteration => true;
        public static bool AutoSave => true;
        public static int AutoSaveEveryNGenerations => 3;
    }
}