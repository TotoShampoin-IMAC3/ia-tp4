using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fish : MonoBehaviour
{

    public Genome genome = new();
    private float size = 1f;
    private float colorHue = 0f;

    public float angleAlpha = 0;
    public float angleBeta = 0;
    public float bowlRadius = 10f;

    public MeshRenderer fishRenderer;

    // Start is called before the first frame update
    void Start()
    {
        genome.On("genesChanged", (List<float> genes) =>
        {
            ApplyGenotype();
        });

        // text = GetComponentInChildren<TextMeshPro>();

        // angleAlpha = Random.Range(0, 2 * Mathf.PI);
        // angleBeta = Random.Range(-Mathf.PI / 2, Mathf.PI / 2);
    }

    // Update is called once per frame
    void Update()
    {
        var time = Time.time;

        transform.position = new Vector3(
            bowlRadius * Mathf.Cos(angleAlpha + time) * Mathf.Cos(angleBeta),
            bowlRadius * Mathf.Sin(angleBeta),
            bowlRadius * Mathf.Sin(angleAlpha + time) * Mathf.Cos(angleBeta)
        );
        transform.LookAt(new Vector3(
            bowlRadius * Mathf.Cos(angleAlpha + time + 0.1f) * Mathf.Cos(angleBeta),
            bowlRadius * Mathf.Sin(angleBeta),
            bowlRadius * Mathf.Sin(angleAlpha + time + 0.1f) * Mathf.Cos(angleBeta)
        ));
    }

    public void Place(float angleB, float angleA, float radius)
    {
        angleBeta = angleB;
        angleAlpha = angleA;
        bowlRadius = radius;
    }

    public void ApplyGenotype()
    {
        size = genome.genes[0];
        colorHue = genome.genes[1];

        ApplyPhenotype();
    }

    public void ApplyPhenotype()
    {
        transform.localScale = new Vector3(size, size, size);
        fishRenderer.material.color = Color.HSVToRGB(colorHue / 360, 1, 1);
    }

    public void ApplySelected(bool selected)
    {
        fishRenderer.material.SetColor("_FresnelColor", selected ? Color.white : Color.black);
    }
}
