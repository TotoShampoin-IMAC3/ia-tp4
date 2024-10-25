using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    readonly List<Population> generations = new();
    readonly List<Fish> fishes = new();

    public GameObject fishPrefab;

    public float fitSize = 1f;
    public float fitColorHue = 0f;

    public int size = 50;
    public int elitism = 15;
    public float mutationRate = .1f;

    public int currentGeneration;

    void Start()
    {
        currentGeneration = 0;
        generations.Add(new Population());
        generations[currentGeneration].InitializeGeneRanges(new List<Tuple<float, float>> {
            new(.5f, 2f), // size
            new(0f, 360f), // colorHue
        });
        generations[currentGeneration].InitializePopulation(size);

        ShowGeneration(currentGeneration);
    }

    public bool IsLastGeneration()
    {
        return currentGeneration == generations.Count - 1;
    }

    public void SetFitSize(float value)
    {
        fitSize = value;
    }
    public void SetFitColorHue(float value)
    {
        fitColorHue = value;
    }

    public int Next()
    {
        if (currentGeneration >= generations.Count - 1)
        {
            GenerateNextPopulation();
        }
        currentGeneration++;
        ShowGeneration(currentGeneration);
        return currentGeneration;
    }
    public int Prev()
    {
        if (currentGeneration > 0)
        {
            currentGeneration--;
            ShowGeneration(currentGeneration);
        }
        return currentGeneration;
    }
    public void Goto(int to)
    {
        if (to >= 0 && to < generations.Count)
        {
            currentGeneration = to;
            ShowGeneration(currentGeneration);
        }
    }

    public void ShowGeneration(int index)
    {
        if (index < generations.Count)
        {
            foreach (Fish fish in fishes)
            {
                Destroy(fish.gameObject);
            }
            fishes.Clear();

            Population populationToShow = generations[index];
            List<Genome> population = populationToShow.population;
            for (int i = 0; i < population.Count; i++)
            {
                Fish fish = Instantiate(fishPrefab).GetComponent<Fish>();
                fish.genome = population[i];
                fish.ApplyGenotype();
                fish.ApplySelected(populationToShow.IsSelected(population[i]));
                fish.Place(
                    Mathf.Lerp(Mathf.PI * -(.5f - .125f), Mathf.PI * (.5f - .125f), (float)i / population.Count),
                    UnityEngine.Random.Range(0, 2 * Mathf.PI),
                    UnityEngine.Random.Range(5f, 12f)
                );
                fishes.Add(fish);
                fish.transform.parent = transform;
            }
        }
    }

    public void Evaluate(int i = -1)
    {
        if (i < 0) i = (generations.Count + i) % generations.Count;

        generations[i].EvaluatePopulation((Genome genome) =>
        {
            float size = genome.genes[0];
            float colorHue = genome.genes[1] / 360f * 10f;

            float fitColorHue = this.fitColorHue / 360f * 10f;

            float sizeFitness = 1f / (1f + Mathf.Pow(size - fitSize, 2));
            float colorFitness = 1f / (1f + Mathf.Pow(Mathf.Min(Mathf.Abs(colorHue - fitColorHue), 10f - Mathf.Abs(colorHue - fitColorHue)), 2));

            return colorFitness * sizeFitness;
        });
    }

    public void GenerateNextPopulation()
    {
        Evaluate(currentGeneration);
        generations.Add(generations[currentGeneration].NextGeneration(elitism, mutationRate));
    }
}
