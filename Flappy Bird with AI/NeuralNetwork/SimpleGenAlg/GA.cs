#region Using directives
using Flappy_Bird_with_AI;
using Flappy_Bird_with_AI.NeuralNetwork;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using Flappy_Bird_with_AI.Files;
using System.Linq;
using Flappy_Bird_with_AI.Global;
using Flappy_Bird_with_AI.GameLogic.Components;
using Flappy_Bird_with_AI.GameLogic.Obsolete;
#endregion

namespace btl.generic
{

    public delegate Task<double> GAFunction(Bird bird, params double[] values);

	/// <summary>a
	/// Genetic Algorithm class
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class GA
	{
		public ObsoleteGameplay Gameplay { get; set; }
		public List<BirdNeuroStruct> Birds { get; set; }

		/// <summary>
		/// Default constructor sets mutation rate to 5%, crossover to 80%, population to 100,
		/// and generations to 2000.
		/// </summary>
		public GA()
		{
			InitialValues();
			m_mutationRate = 0.05;
			m_crossoverRate = 0.80;
			m_populationSize = 100;
			m_generationSize = 2000;
			m_strFitness = "";
		}

		public GA(double crossoverRate, 
                  double mutationRate, 
                  int populationSize, 
                  int generationSize, 
                  int genomeSize)
		{
			InitialValues();
			m_mutationRate = mutationRate;
			m_crossoverRate = crossoverRate;
			m_populationSize = populationSize;
			m_generationSize = generationSize;
			m_genomeSize = genomeSize;
			m_strFitness = "";
		}

		public GA(int genomeSize)
		{
			InitialValues();
			m_genomeSize = genomeSize;
		}

		[JsonConstructor]
        public GA(int generationOn, double mutationRate, double crossoverRate, int populationSize, 
			int generationSize, int genomeSize, double totalFitness, string strFitness,
			bool elitism, List<Genome> thisGeneration, List<Genome> nextGeneration, List<double> fitnessTable)
        {
            GenerationOn = generationOn;
            m_mutationRate = mutationRate;
            m_crossoverRate = crossoverRate;
            m_populationSize = populationSize;
            m_generationSize = generationSize;
            m_genomeSize = genomeSize;
            m_totalFitness = totalFitness;
            m_strFitness = strFitness;
            m_elitism = elitism;
            m_thisGeneration = thisGeneration;
            m_nextGeneration = nextGeneration;
            m_fitnessTable = fitnessTable;
        }

        public void InitialValues()
		{
			m_elitism = false;
		}



		private bool IsStopped = false;
		public void Stop()
        {
			IsStopped = true;
		}

		/// <summary>
		/// Method which starts the GA executing.
		/// </summary>
		public async void Go()
		{
            /// -------------
            /// Preconditions
            /// -------------
			if (getFitness == null)
				throw new ArgumentNullException("Need to supply fitness function");
			if (m_genomeSize == 0)
				throw new IndexOutOfRangeException("Genome size not set");
            /// -------------

			//  Create the fitness table.
            m_fitnessTable = new List<double>();
            m_thisGeneration = new List<Genome>(m_generationSize);
            m_nextGeneration = new List<Genome>(m_generationSize);
            Genome.MutationRate = m_mutationRate;


			await Gameplay.Restart(m_populationSize);
			CreateGenomes();
			await RankPopulation();

			GenerationOn++;
			for (; GenerationOn <= m_generationSize + 1 && !IsStopped; GenerationOn++)
			{
				if (GlobalNeuralParams.AutoSave &&
					GenerationOn % GlobalNeuralParams.AutoSaveEveryNGenerations == 0)
                {
					NeuroFileManager.SaveNeuro(this);
                }

				await CreateNextGeneration();
				await RankPopulation();
            }
		}

		/// <summary>
		/// After ranking all the genomes by fitness, use a 'roulette wheel' selection
		/// method.  This allocates a large probability of selection to those with the 
		/// highest fitness.
		/// </summary>
		/// <returns>Random individual biased towards highest fitness</returns>
		private int RouletteSelection()
		{
			double randomFitness = m_random.NextDouble() * m_totalFitness;
			int idx = -1;
			int mid;
			int first = 0;
			int last = m_populationSize -1;
			mid = (last - first)/2;

			//  ArrayList's BinarySearch is for exact values only
			//  so do this by hand.
			while (idx == -1 && first <= last)
			{
                if (randomFitness < m_fitnessTable[mid])
				{
					last = mid;
				}
                else if (randomFitness > m_fitnessTable[mid])
                {
					first = mid;
				}
				mid = (first + last)/2;
				//  lies between i and i+1
				if ((last - first) == 1)
					idx = last;
			}
			return idx;
		}

        /// <summary>
		/// Rank population and sort in order of fitness.
		/// </summary>
		private async Task RankPopulation()
		{
			m_totalFitness = 0.0;

			List<Task<double>> tasks = new List<Task<double>>();
			foreach(Genome g in m_thisGeneration)
			{
				tasks.Add(FitnessFunction(g.bird, g.Genes()));
			}
			await Task.WhenAll(tasks);
            for (int i = 0; i < m_thisGeneration.Count; i++)
            {
				var g = m_thisGeneration[i];
				g.bird = null;
				var task = tasks[i];
				g.Fitness = task.Result;
				m_totalFitness += g.Fitness;
			}

            m_thisGeneration.Sort(delegate(Genome x, Genome y) 
                { return Comparer<double>.Default.Compare(x.Fitness, y.Fitness); });

            //  now sorted in order of fitness.
            double fitness = 0.0;
            m_fitnessTable.Clear();
            foreach(Genome t in m_thisGeneration)
			{
				fitness += t.Fitness;
				m_fitnessTable.Add(t.Fitness);
            }
        }

        /// <summary>
		/// Create the *initial* genomes by repeated calling the supplied fitness function
		/// </summary>
		private void CreateGenomes()
		{
			for (int i = 0; i < m_populationSize ; i++)
			{
				Genome g = new Genome(m_genomeSize);
				g.bird = Gameplay.Birds[i];
				m_thisGeneration.Add(g);
			}
		}

		private async Task CreateNextGeneration()
		{
			m_nextGeneration.Clear();
			Genome g = null;
			if (m_elitism)
				g = m_thisGeneration[m_populationSize - 1].DeepCopy();

			for (int i = 0 ; i < m_populationSize ; i+=2)
			{
				int pidx1 = RouletteSelection();
				int pidx2 = RouletteSelection();
				Genome parent1, parent2, child1, child2;
				parent1 = m_thisGeneration[pidx1];
				parent2 = m_thisGeneration[pidx2];

				if (m_random.NextDouble() < m_crossoverRate)
				{
					parent1.Crossover(ref parent2, out child1, out child2);
				}
				else
				{
					child1 = parent1;
					child2 = parent2;
				}
				child1.Mutate();
				child2.Mutate();

				m_nextGeneration.Add(child1);
				m_nextGeneration.Add(child2);
			}
			if (m_elitism && g != null)
				m_nextGeneration[0] = g;

			m_thisGeneration.Clear();
            foreach(Genome ge in m_nextGeneration) 
				m_thisGeneration.Add(ge);

			await Gameplay.Restart(m_populationSize);
			NeuroFileManager.WriteBestFitness(Birds.Select(x => x.Fitness));
			Birds.Clear();
			for (int i = 0; i < m_populationSize; i++)
            {
				m_thisGeneration[i].bird = Gameplay.Birds[i];
			}
		}

		[JsonProperty] public int GenerationOn { get; private set; } = 1;
		[JsonProperty] private double m_mutationRate;
		[JsonProperty] private double m_crossoverRate;
		[JsonProperty] private int m_populationSize;
		[JsonProperty] private int m_generationSize;
		[JsonProperty] private int m_genomeSize;
		[JsonProperty] private double m_totalFitness;
		[JsonProperty] private string m_strFitness;
		[JsonProperty] private bool m_elitism;

		[JsonProperty] private List<Genome> m_thisGeneration;
		[JsonProperty] private List<Genome> m_nextGeneration;
		[JsonProperty] private List<double> m_fitnessTable;

        static Random m_random = new Random();

		static private GAFunction getFitness;
		public GAFunction FitnessFunction
		{
			get	
			{
				return getFitness;
			}
			set
			{
				getFitness = value;
			}
		}

		//  Properties
		public int PopulationSize
		{
			get
			{
				return m_populationSize;
			}
			set
			{
				m_populationSize = value;
			}
		}
		public int Generations
		{
			get
			{
				return m_generationSize;
			}
			set
			{
				m_generationSize = value;
			}
		}
		public int GenomeSize
		{
			get
			{
				return m_genomeSize;
			}
			set
			{
				m_genomeSize = value;
			}
		}

		public double CrossoverRate
		{
			get
			{
				return m_crossoverRate;
			}
			set
			{
				m_crossoverRate = value;
			}
		}
		public double MutationRate
		{
			get
			{
				return m_mutationRate;
			}
			set
			{
				m_mutationRate = value;
			}
		}
		public string FitnessFile
		{
			get
			{
				return m_strFitness;
			}
			set
			{
				m_strFitness = value;
			}
		}

		/// <summary>
		/// Keep previous generation's fittest individual in place of worst in current
		/// </summary>
		public bool Elitism
		{
			get
			{
				return m_elitism;
			}
			set
			{
				m_elitism = value;
			}
		}
		public void GetBest(out double[] values, out double fitness)
		{
			Genome g = m_thisGeneration[m_populationSize-1];
            values = new double[g.Length];
            g.GetValues(ref values);
			fitness = g.Fitness;
		}

        public void GetWorst(out double[] values, out double fitness)
        {
			GetNthGenome(0, out values, out fitness);
		}

        public void GetNthGenome(int n, out double[] values, out double fitness)
        {
            /// Preconditions
            /// -------------
			if (n < 0 || n > m_populationSize-1)
				throw new ArgumentOutOfRangeException("n too large, or too small");
            /// -------------
			Genome g = m_thisGeneration[n];
            values = new double[g.Length];
            g.GetValues(ref values);
			fitness = g.Fitness;
		}
	}
}
