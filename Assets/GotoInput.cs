using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoInput : MonoBehaviour
{
    int value = 0;

    void Refresh()
    {
        GetComponent<TMPro.TMP_InputField>().text = value.ToString();
    }

    public void SetValue(int value)
    {
        this.value = value;
        Refresh();
    }
    public void SetValue(string value)
    {
        this.value = int.Parse(value);
        Refresh();
    }
    public int GetValue()
    {
        return value;
    }
    public string GetValueString()
    {
        return value.ToString();
    }
}
