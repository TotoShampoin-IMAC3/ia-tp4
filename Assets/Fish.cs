using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    readonly static float bowlRadius = 10f;

    public Genome genome = new();
    private float size = 1f;
    private float colorHue = 0f;

    public float angleAlpha = 0;
    public float angleBeta = 0;

    // Start is called before the first frame update
    void Start()
    {
        genome.On("genesChanged", (List<float> genes) =>
        {
            ApplyGenotype();
        });

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

    public void Place(float angleB, float angleA)
    {
        angleBeta = angleB;
        angleAlpha = angleA;
    }

    public void ApplyGenotype()
    {
        size = genome.genes[0];
        colorHue = genome.genes[1];

        ApplyPhenotype();
    }

    public void ApplyPhenotype()
    {
        transform.localScale = new Vector3(size / 2f, size / 2f, size);
        GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(colorHue / 360, 1, 1);
    }
}
