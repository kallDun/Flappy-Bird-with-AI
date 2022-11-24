using Flappy_Bird_with_AI.GameLogic;
using Flappy_Bird_with_AI.Neat.Learning;
using Flappy_Bird_with_AI.Neat.Playing;
using Flappy_Bird_with_AI.Output;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace Flappy_Bird_with_AI.Neat
{
    class NeatEntryPoint
    {
        private string INITIAL_DIRECTORY_PATH => $"{Environment.CurrentDirectory}\\Neat";
        private string LAST_MODEL_PATH => $"{INITIAL_DIRECTORY_PATH}\\model_last.xml";
        private string CHAMPION_MODEL_PATH => $"{INITIAL_DIRECTORY_PATH}\\model_champion.xml";

        private NeatEvolutionAlgorithm<NeatGenome> _ea;
        private double _maxFitness = 0;

        public void StartLearning()
        {
            SimpleNeatExperiment experiment = GenerateExperimentObject();

            _ea = experiment.CreateEvolutionAlgorithm();
            _ea.UpdateEvent += new EventHandler(UpdateEvent);

            _ea.StartContinue();
        }

        public void LoadAndBestPlay()
        {
            SimpleNeatExperiment experiment = GenerateExperimentObject();
            NeatGenome genome;

            try
            {
                OpenFileDialog openFileDialog = new();
                openFileDialog.Filter = "model file (.xml)|*.xml";
                openFileDialog.InitialDirectory = INITIAL_DIRECTORY_PATH;
                DialogResult result = openFileDialog.ShowDialog();
                if (result != DialogResult.OK) Application.Exit();

                XmlReader reader = XmlReader.Create(openFileDialog.FileName);
                var genomes = NeatGenomeXmlIO.ReadCompleteGenomeList(reader, false, 
                    experiment.CreateGenomeFactory() as NeatGenomeFactory);
                genome = genomes[0];
            }
            catch (Exception e)
            {
                Logger.LogLabel.Text = $"Something gone wrong! Exception: {e}";
                return;
            }

            IBlackBox blackBox = experiment.CreateGenomeDecoder().Decode(genome);
            NeatPlayer player = new(blackBox);
            Gameplay gameplay = new SimpleNeatDemonstrationGameplay(player);
            gameplay.RestartGame();
        }


        private static SimpleNeatExperiment GenerateExperimentObject()
        {
            SimpleNeatExperiment experiment = new FlappybirdNeatExperiment();

            // Load config XML.
            XmlDocument xmlConfig = new();
            xmlConfig.Load("Parameters\\neat.flappybird.config.xml");
            experiment.Initialize("Flappy Bird", xmlConfig.DocumentElement);
            return experiment;
        }
        private void UpdateEvent(object sender, EventArgs e)
        {
            Logger.LogGeneration(string.Format("gen={0:N0}", _ea.CurrentGeneration));

           // Save the best genome to file
           var doc = NeatGenomeXmlIO.SaveComplete(new List<NeatGenome>() { _ea.CurrentChampGenome }, false);
            doc.Save(LAST_MODEL_PATH);

            if (_maxFitness <= _ea.Statistics._maxFitness)
            {
                _maxFitness = _ea.Statistics._maxFitness;
                doc = NeatGenomeXmlIO.SaveComplete(new List<NeatGenome>() { _ea.CurrentChampGenome }, false);
                doc.Save(CHAMPION_MODEL_PATH);
            }
        }
    }
}