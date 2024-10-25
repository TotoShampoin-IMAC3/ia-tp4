using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public Genome<float> genome = new();
    private float size = 1f;
    private float colorHue = 0f;
    private float speed = 1f;
    public float bowlRadius = 10f;

    public float angleAlpha = 0;
    public float angleBeta = 0;

    public MeshRenderer fishRenderer;

    void Update()
    {
        var time = Time.time;

        transform.position = new Vector3(
            bowlRadius * Mathf.Cos(angleAlpha + time * speed) * Mathf.Cos(angleBeta),
            bowlRadius * Mathf.Sin(angleBeta),
            bowlRadius * Mathf.Sin(angleAlpha + time * speed) * Mathf.Cos(angleBeta)
        );
        transform.LookAt(new Vector3(
            bowlRadius * Mathf.Cos(angleAlpha + (time + 0.1f) * speed) * Mathf.Cos(angleBeta),
            bowlRadius * Mathf.Sin(angleBeta),
            bowlRadius * Mathf.Sin(angleAlpha + (time + 0.1f) * speed) * Mathf.Cos(angleBeta)
        ));
    }

    // public void Place(float angleB, float angleA, float radius)
    public void Place(float angleB, float angleA)
    {
        angleBeta = angleB;
        angleAlpha = angleA;
        // bowlRadius = radius;
    }

    public void ApplyGenotype()
    {
        size = genome.genes[0];
        colorHue = genome.genes[1];
        speed = genome.genes[2];
        bowlRadius = genome.genes[3];

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
