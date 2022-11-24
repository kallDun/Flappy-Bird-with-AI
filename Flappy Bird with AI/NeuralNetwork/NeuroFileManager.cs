using btl.generic;
using Flappy_Bird_with_AI.Global;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flappy_Bird_with_AI.Files
{
    static class NeuroFileManager
    {
        public static void SaveNeuro(GA item)
        {
            string model = JsonConvert.SerializeObject(item);
            File.WriteAllText(GlobalNeuralParams.ModelFilePath, model);
            File.WriteAllText(GlobalNeuralParams.ModelLogsDirectoryPath + $"model_{DateTime.Now.ToString("dd.MM HH.mm.ss")}.json", model);
        }
        public static GA LoadNeuro()
        {
            string model = File.ReadAllText(GlobalNeuralParams.ModelFilePath);
            return JsonConvert.DeserializeObject<GA>(model);
        }
        public static void WriteBestFitness(IEnumerable<double> fitnesses)
        {
            if (GlobalNeuralParams.WriteBestFitnessesEveryItteration)
            {
                int bestCount = fitnesses.Count() < 5 ? fitnesses.Count() : 5;
                var path = $"{GlobalNeuralParams.ModelLogsDirectoryPath}\\fitness_logs\\fitness_{DateTime.Now.ToString("dd.MM HH.mm.ss")}.txt";
                var content = string.Join("\n", fitnesses.OrderBy(x => -x).Take(bestCount));
                File.WriteAllText(path, content);
            }
        }
    }
}