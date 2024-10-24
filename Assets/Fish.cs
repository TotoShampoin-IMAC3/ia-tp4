using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fish : MonoBehaviour
{
    readonly static float bowlRadius = 10f;

    public Genome genome = new();
    private float size = 1f;
    private float colorHue = 0f;

    public float angleAlpha = 0;
    public float angleBeta = 0;

    TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        genome.On("genesChanged", (List<float> genes) =>
        {
            ApplyGenotype();
        });

        text = GetComponentInChildren<TextMeshPro>();

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

        text.transform.LookAt(Camera.main.transform);
        text.transform.Rotate(0, 180, 0);
        text.text = genome.fitness.ToString("F2");
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
        transform.localScale = new Vector3(size, size, size);
        GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(colorHue / 360, 1, 1);
    }

    public void ApplySelected(bool selected)
    {
        GetComponent<MeshRenderer>().material.SetColor("_FresnelColor", selected ? Color.white : Color.black);
    }
}
