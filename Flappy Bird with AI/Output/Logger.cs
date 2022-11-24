using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Flappy_Bird_with_AI.Output
{
    static class Logger
    {
        public static Label LogLabel;

        public static void LogGeneration(string message)
        {
            LogLabel.Invoke(() =>
            {
                LogLabel.Text =
                    $"{message},  " +
                    $"{string.Join(",  ", _bestParameters.Select(x => string.Format("{0}={1:0.00000}", x.Key, x.Value)))}" +
                    $"\n{LogLabel.Text}";
            });

            _bestFitness = double.MinValue;
            _bestAlternativeFitness = double.MinValue;
        }


        private static double _bestFitness = double.MinValue;
        private static double _bestAlternativeFitness = double.MinValue;
        private static KeyValuePair<string, double>[] _bestParameters;

        public static void LogParameters(string fitness, string alternativeFitness, params KeyValuePair<string, double>[] parameters)
        {
            double _fitness = parameters.First(x => x.Key == fitness).Value;
            double _alternativeFitness = parameters.First(x => x.Key == alternativeFitness).Value;
            
            if (_fitness >= _bestFitness && _alternativeFitness >= _bestAlternativeFitness)
            {
                _bestFitness = _fitness;
                _bestAlternativeFitness = _alternativeFitness;
                _bestParameters = parameters;
            }
        }
    }
}