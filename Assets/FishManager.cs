using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    readonly List<Population> generations = new();
    readonly List<Fish> fishes = new();

    List<float> geneMinValues = new List<float> { .5f, 0f };
    List<float> geneMaxValues = new List<float> { 2f, 360f };

    public GameObject fishPrefab;

    public float fitSize = 1f;
    public float fitColorHue = 0f;

    public int elitism = 15;
    public float mutationRate = .1f;

    // Start is called before the first frame update
    void Start()
    {
        generations.Add(new Population());
        // generations[0].InitializeGeneMaxValues(new List<int> { 3, 6 });
        generations[0].InitializeGeneRanges(new List<Tuple<float, float>> {
            new(.5f, 2f), // size
            new(0f, 360f), // colorHue
        });
        generations[0].InitializePopulation(100);

        ShowGeneration(0);
    }

    // Update is called once per frame
    void Update()
    {
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

            Population population = generations[index];
            for (int i = 0; i < population.population.Count; i++)
            {
                Fish fish = Instantiate(fishPrefab).GetComponent<Fish>();
                fish.genome = population.population[i];
                fish.ApplyGenotype();
                fish.Place(
                    UnityEngine.Random.Range(0, 2 * Mathf.PI),
                    i / ((float)population.population.Count) * Mathf.PI - Mathf.PI / 2
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
            float colorHue = genome.genes[0];

            float sizeFitness = 1f - Mathf.Abs(size - fitSize) / fitSize;
            float colorFitness = 1f - Mathf.Abs(colorHue - fitColorHue) / 360f;

            return sizeFitness + colorFitness;
        });
    }

    public void GenerateNextPopulation()
    {
        int i = generations.Count - 1;
        Evaluate(i);
        generations.Add(generations[i].NextGeneration(elitism, mutationRate));
        ShowGeneration(i + 1);
    }
}
