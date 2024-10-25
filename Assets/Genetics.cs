using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Genome
{
    public List<float> genes = new();
    public float fitness;

    public void Evaluation(Func<Genome, float> fitnessFunction)
    {
        fitness = fitnessFunction(this);
    }
    public void Mutate(float mutationRate, List<float> geneMinValues, List<float> geneMaxValues)
    {
        for (int i = 0; i < genes.Count; i++)
        {
            if (UnityEngine.Random.Range(0f, 1f) < mutationRate)
            {
                genes[i] = UnityEngine.Random.Range(geneMinValues[i], geneMaxValues[i]);
            }
        }

    }
    public static Genome Crossover(Genome parent1, Genome parent2)
    {
        Genome child = new();
        for (int i = 0; i < parent1.genes.Count; i++)
        {
            child.genes.Add(UnityEngine.Random.Range(0, 2) == 0 ? parent1.genes[i] : parent2.genes[i]);
        }
        return child;
    }
}

public class Population
{
    public List<float> geneMinValues = new();
    public List<float> geneMaxValues = new();
    public List<Genome> population = new();
    public List<bool> selected = new();

    public void InitializeGeneMinValues(List<float> geneMinValues)
    {
        this.geneMinValues = geneMinValues;
    }
    public void InitializeGeneMaxValues(List<float> geneMaxValues)
    {
        this.geneMaxValues = geneMaxValues;
    }
    public void InitializeGeneRanges(List<Tuple<float, float>> geneRanges)
    {
        geneMinValues = geneRanges.Select(range => range.Item1).ToList();
        geneMaxValues = geneRanges.Select(range => range.Item2).ToList();
    }
    public void InitializePopulation(int populationSize)
    {
        foreach (int _ in new int[populationSize])
        {
            population.Add(CreateGenome());
        }
        MakeSelections();
    }
    public void MakeSelections()
    {
        selected = new List<bool>(new bool[population.Count]);
    }

    public bool IsSelected(Genome genome)
    {
        return selected[population.IndexOf(genome)];
    }

    public Genome CreateGenome()
    {
        Genome genome = new();
        for (int i = 0; i < geneMaxValues.Count; i++)
        {
            genome.genes.Add(UnityEngine.Random.Range(geneMinValues[i], geneMaxValues[i]));
        }
        return genome;
    }

    public Genome Select(List<Genome> excluded)
    {
        List<Genome> selectedGenomes = population
            .Where(genome => !excluded.Contains(genome))
            .ToList();

        float totalFitness = selectedGenomes.Sum(genome => genome.fitness);
        float randomValue = UnityEngine.Random.Range(0f, totalFitness);
        float currentFitness = 0;
        foreach (Genome genome in selectedGenomes)
        {
            currentFitness += genome.fitness;
            if (currentFitness >= randomValue)
            {
                return genome;
            }
        }
        return selectedGenomes.Last();
    }
    public Population Selection(int n)
    {
        List<Genome> selectedGenomes = new();
        for (int i = 0; i < n; i++)
        {
            var selectedOne = Select(selectedGenomes);
            selectedGenomes.Add(selectedOne);
            selected[population.IndexOf(selectedOne)] = true;
        }
        return new Population()
        {
            geneMinValues = geneMinValues,
            geneMaxValues = geneMaxValues,
            population = selectedGenomes
        };
    }

    public void EvaluatePopulation(Func<Genome, float> fitnessFunction)
    {
        foreach (Genome genome in population)
        {
            genome.Evaluation(fitnessFunction);
        }
    }
    public void MutatePopulation(float mutationRate)
    {
        foreach (Genome genome in population)
        {
            genome.Mutate(mutationRate, geneMinValues, geneMaxValues);
        }
    }
    public void CrossoverPopulation(int count)
    {
        List<Genome> newPopulation = new();
        for (int i = 0; i < count; i++)
        {
            Genome parent1 = population[UnityEngine.Random.Range(0, population.Count)];
            Genome parent2 = population[UnityEngine.Random.Range(0, population.Count)];
            newPopulation.Add(Genome.Crossover(parent1, parent2));
        }
        population.AddRange(newPopulation);
    }

    public Population NextGeneration(int elitism, float mutationRate)
    {
        Population nextGeneration = Selection(elitism);
        nextGeneration.CrossoverPopulation(population.Count - elitism);
        nextGeneration.MutatePopulation(mutationRate);
        nextGeneration.MakeSelections();
        return nextGeneration;
    }
}
