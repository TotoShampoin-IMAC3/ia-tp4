using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Explorer : MonoBehaviour
{
    public FishManager fishManager;
    public GotoInput gotoInput;
    public Image colorRenderer;
    public Button nextButton;

    // Start is called before the first frame update
    void Start()
    {
        gotoInput.SetValue(0);
        UpdateColorHue(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Goto()
    {
        fishManager.Goto(gotoInput.GetValue());
        UpdateNextButton();
    }
    public void Next()
    {
        gotoInput.SetValue(fishManager.Next());
        UpdateNextButton();
    }
    public void Prev()
    {
        gotoInput.SetValue(fishManager.Prev());
        UpdateNextButton();
    }

    public void UpdateColorHue(float value)
    {
        colorRenderer.color = Color.HSVToRGB(value / 360f, 1, 1);
    }
    public void UpdateNextButton()
    {
        if (fishManager.IsLastGeneration())
        {
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Gen";
        }
        else
        {
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
        }
    }
}
