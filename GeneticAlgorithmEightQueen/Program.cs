using System;
using System.Collections.Generic;
using System.Linq;

namespace EightQueensProblem
{
    class Program
    {
        static Random rand = new Random();

        static void Main(string[] args)
        {
            int populationSize = 100;
            int geneCount = 8;

            int maxGenerations = 1000;

            double mutationProbability = 0.1;

            List<int[]> population = new List<int[]>();
            for (int i = 0; i < populationSize; i++)
            {
                int[] genes = Enumerable.Range(0, geneCount).OrderBy(g => rand.Next()).ToArray();
                population.Add(genes);
            }

            for (int generation = 0; generation < maxGenerations; generation++)
            {
                List<double> fitnessValues = new List<double>();
                foreach (var genes in population)
                {
                    double fitnessValue = CalcFitness(genes);
                    fitnessValues.Add(fitnessValue);
                }

                double maxFitnessValue = fitnessValues.Max();
                int[] bestGenes = population[fitnessValues.IndexOf(maxFitnessValue)];

                Console.WriteLine("Generation: {0}, Best fitness: {1}", generation, maxFitnessValue);

                if (maxFitnessValue == 1.0)
                {
                    Console.WriteLine("Solution found!");
                    PrintBoard(bestGenes);
                    break;
                }

                List<int[]> newPopulation = new List<int[]>();
                while (newPopulation.Count < populationSize)
                {
                    int[] parent1 = RouletteSelect(population, fitnessValues);
                    int[] parent2 = RouletteSelect(population, fitnessValues);

                    int[] child1, child2;
                    Crossover(parent1, parent2, out child1, out child2);

                    Mutate(child1, mutationProbability);
                    Mutate(child2, mutationProbability);

                    newPopulation.Add(child1);
                    newPopulation.Add(child2);
                }

                population = newPopulation;
            }

            Console.ReadLine();
        }

        static double CalcFitness(int[] genes)
        {
            int n = genes.Length;
            int diagonalCollisions = 0;
            int reverseDiagonalCollisions = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (genes[i] == genes[j] || genes[i] - i == genes[j] - j || genes[i] + i == genes[j] + j)
                    {
                        diagonalCollisions++;
                    }
                    if (genes[i] == genes[j] || genes[i] + i == genes[j] + j || genes[i] - i == genes[j] - j)
                    {
                        reverseDiagonalCollisions++;
                    }
                }
            }

            double fitnessValue = 1.0 / (1.0 + diagonalCollisions + reverseDiagonalCollisions);
            return fitnessValue;
        }

        static int[] RouletteSelect(List<int[]> population, List<double> fitnessValues)
        {
            double minFitnessValue = fitnessValues.Min();
            double maxFitnessValue = fitnessValues.Max();

            double totalFitness = 0.0;
            foreach (var fitnessValue in fitnessValues)
            {
                totalFitness += fitnessValue;
            }

            List<double> probabilities = new List<double>();
            foreach (var fitnessValue in fitnessValues)
            {
                double probability = (fitnessValue - minFitnessValue) / (maxFitnessValue - minFitnessValue);
                probability *= totalFitness;
                probabilities.Add(probability);
            }

            double randomValue = rand.NextDouble() * totalFitness;
            double sum = 0.0;
            for (int i = 0; i < population.Count; i++)
            {
                sum += probabilities[i];
                if (sum > randomValue)
                {
                    return population[i];
                }
            }

            return population[0];
        }

        static void Crossover(int[] parent1, int[] parent2, out int[] child1, out int[] child2)
        {
            int n = parent1.Length;
            int crossoverPoint = rand.Next(n);

            child1 = new int[n];
            child2 = new int[n];
            for (int i = 0; i < n; i++)
            {
                if (i < crossoverPoint)
                {
                    child1[i] = parent1[i];
                    child2[i] = parent2[i];
                }
                else
                {
                    child1[i] = parent2[i];
                    child2[i] = parent1[i];
                }
            }
        }

        static void Mutate(int[] genes, double mutationProbability)
        {
            int n = genes.Length;

            for (int i = 0; i < n; i++)
            {
                if (rand.NextDouble() < mutationProbability)
                {
                    int newIndex = rand.Next(n);
                    int temp = genes[i];
                    genes[i] = genes[newIndex];
                    genes[newIndex] = temp;
                }
            }
        }

        static void PrintBoard(int[] genes)
        {
            int n = genes.Length;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (genes[i] == j)
                    {
                        Console.Write("Q ");
                    }
                    else
                    {
                        Console.Write(". ");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}