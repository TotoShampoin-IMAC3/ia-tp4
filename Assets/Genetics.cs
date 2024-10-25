using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/* Genetic Algorithm implementation */

/// <summary>
///     Represents an individual genome in a genetic algorithm, with a genotype and fitness value.
/// </summary>
/// <typeparam name="GeneType">
///     The type of the gene, which must implement IComparable.
/// </typeparam>
public class Genome<GeneType> where GeneType : IComparable
{
    public List<GeneType> genes = new();
    public float fitness;

    /// <summary>
    ///     Evaluates the fitness of the genome using a fitness function.
    /// </summary>
    /// <param name="fitnessFunction">
    ///     A function that takes a genome and returns its fitness.
    /// </param>
    public void Evaluation(Func<Genome<GeneType>, float> fitnessFunction)
    {
        fitness = fitnessFunction(this);
    }
    /// <summary>
    ///     Mutates the genome by changing some of its genes randomly.
    /// </summary>
    public void Mutate(float mutationProbability, List<GeneType> geneMinValues, List<GeneType> geneMaxValues)
    {
        for (int i = 0; i < genes.Count; i++)
        {
            if (UnityEngine.Random.Range(0f, 1f) < mutationProbability)
            {
                genes[i] = (GeneType)Convert.ChangeType(
                    UnityEngine.Random.Range(
                        Convert.ToSingle(geneMinValues[i]),
                        Convert.ToSingle(geneMaxValues[i])
                    ),
                    typeof(GeneType)
                );
            }
        }
    }
    /// <summary>
    ///     Creates a new genome by combining the genes of two parent genomes.
    /// </summary>
    /// <returns>
    ///     The child genome that the parents give birth to.
    /// </returns>
    public static Genome<GeneType> Crossover(Genome<GeneType> parent1, Genome<GeneType> parent2)
    {
        Genome<GeneType> child = new();
        for (int i = 0; i < parent1.genes.Count; i++)
        {
            child.genes.Add(UnityEngine.Random.Range(0, 2) == 0 ? parent1.genes[i] : parent2.genes[i]);
        }
        return child;
    }
}



/// <summary>
///     Represents one generation of a population of individuals, as well as the gene ranges.
/// </summary>
/// <typeparam name="GeneType">
///     It remembers which individuals are selected for the next generation.
/// </typeparam>
public class Population<GeneType> where GeneType : IComparable
{
    public List<GeneType> geneMinValues = new();
    public List<GeneType> geneMaxValues = new();
    public List<Genome<GeneType>> population = new();
    public List<bool> selected = new();

    /// <summary>
    ///     Initializes the gene ranges of the population.
    /// </summary>
    /// <param name="geneRanges">
    ///     A list of (min, max) tuples for each gene.
    /// </param>
    public void InitializeGeneRanges(List<Tuple<GeneType, GeneType>> geneRanges)
    {
        geneMinValues = geneRanges.Select(range => range.Item1).ToList();
        geneMaxValues = geneRanges.Select(range => range.Item2).ToList();
    }
    /// <summary>
    ///     Initializes the population with random genomes.
    /// </summary>
    public void InitializePopulation(int populationSize)
    {
        foreach (int _ in new int[populationSize])
        {
            population.Add(CreateGenome());
        }
        MakeSelections();
    }
    /// <summary>
    ///     Marks all individuals in the population as unselected.
    /// </summary>
    public void MakeSelections()
    {
        selected = new List<bool>(new bool[population.Count]);
    }

    /// <summary>
    ///     Returns whether a genome is selected for the next generation.
    /// </summary>
    public bool IsSelected(Genome<GeneType> genome)
    {
        int index = population.IndexOf(genome);
        if (index == -1)
        {
            return false;
        }
        return selected[index];
    }

    /// <summary>
    ///     Creates a new genome with random genes.
    /// </summary>
    /// <returns>
    ///     The new genome.
    /// </returns>
    public Genome<GeneType> CreateGenome()
    {
        Genome<GeneType> genome = new();
        for (int i = 0; i < geneMaxValues.Count; i++)
        {
            genome.genes.Add((GeneType)Convert.ChangeType(
                UnityEngine.Random.Range(
                    Convert.ToSingle(geneMinValues[i]),
                    Convert.ToSingle(geneMaxValues[i])
                ),
                typeof(GeneType)
            ));
        }
        return genome;
    }

    /// <summary>
    ///     Selects a genome from the population, excluding a list of genomes.
    ///     The selected genome is not marked as selected.
    /// </summary>
    /// <param name="excluded">
    ///     A list of genomes that should not be selected.
    /// </param>
    /// <returns>
    ///     The selected genome.
    /// </returns>
    public Genome<GeneType> Select(List<Genome<GeneType>> excluded)
    {
        List<Genome<GeneType>> selectedGenomes = population
            .Where(genome => !excluded.Contains(genome))
            .ToList();

        float totalFitness = selectedGenomes.Sum(genome => genome.fitness);
        float randomValue = UnityEngine.Random.Range(0f, totalFitness);
        float currentFitness = 0;
        foreach (Genome<GeneType> genome in selectedGenomes)
        {
            currentFitness += genome.fitness;
            if (currentFitness >= randomValue)
            {
                return genome;
            }
        }
        return selectedGenomes.Last();
    }
    /// <summary>
    ///     Selects a number of genomes from the population.
    /// </summary>
    /// <returns>
    ///     A new population with the selected genomes.
    /// </returns>
    public Population<GeneType> Selection(int amount)
    {
        List<Genome<GeneType>> selectedGenomes = new();
        for (int i = 0; i < amount; i++)
        {
            var selectedOne = Select(selectedGenomes);
            selectedGenomes.Add(selectedOne);
            selected[population.IndexOf(selectedOne)] = true;
        }
        return new Population<GeneType>()
        {
            geneMinValues = geneMinValues,
            geneMaxValues = geneMaxValues,
            population = selectedGenomes
        };
    }

    /// <summary>
    ///     Evaluates the fitness of the population using a fitness function.
    /// </summary>
    public void EvaluatePopulation(Func<Genome<GeneType>, float> fitnessFunction)
    {
        foreach (Genome<GeneType> genome in population)
        {
            genome.Evaluation(fitnessFunction);
        }
    }
    /// <summary>
    ///     Mutates the population by changing some of the genes of each genome randomly.
    /// </summary>
    public void MutatePopulation(float mutationProbability)
    {
        foreach (Genome<GeneType> genome in population)
        {
            genome.Mutate(mutationProbability, geneMinValues, geneMaxValues);
        }
    }
    /// <summary>
    ///     Creates a new population by crossing over the genomes of the current population.
    /// </summary>
    public void CrossoverPopulation(int amountToAdd)
    {
        List<Genome<GeneType>> newPopulation = new();
        for (int i = 0; i < amountToAdd; i++)
        {
            Genome<GeneType> parent1 = population[UnityEngine.Random.Range(0, population.Count)];
            Genome<GeneType> parent2 = population[UnityEngine.Random.Range(0, population.Count)];
            newPopulation.Add(Genome<GeneType>.Crossover(parent1, parent2));
        }
        population.AddRange(newPopulation);
    }

    /// <summary>
    ///     Creates the next generation of the population using elitism, mutation, and crossover.
    /// </summary>
    /// <param name="elitism">
    ///     The amount of genomes to take from this generation to the next.
    /// </param>
    /// <returns>
    ///     The next generation of the population.
    /// </returns>
    public Population<GeneType> NextGeneration(int elitism, float mutationProbability)
    {
        Population<GeneType> nextGeneration = Selection(elitism);
        nextGeneration.CrossoverPopulation(population.Count - elitism);
        nextGeneration.MutatePopulation(mutationProbability);
        nextGeneration.MakeSelections();
        return nextGeneration;
    }
}
