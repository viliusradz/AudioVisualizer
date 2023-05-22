using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIValueConfig : MonoBehaviour
{
    public bool configActive = false;
    public int sampleCount = 20;
    public float threshold = 0.01f;
    public float stepIncr = 0.001f;

    public float valueMulti = 0.001f;

    private VisualBrain audio;
    private List<float> scales = new List<float>();
    public float avg = 0;
    private float sum = 0;
    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<VisualBrain>();
    }
    private void Update()
    {
        if (configActive)
            CalculateOffset();
        else
            valueMulti = 1;
    }
    private void CalculateOffset()
    {
        if (scales.Count != 0)
        {
            if (audio.trueScale != scales[scales.Count-1])
            {
                scales.Add(audio.trueScale);
                sum += audio.trueScale;
                count++;
            }
        }
        else
        {
            scales.Add(audio.trueScale);
            sum += audio.trueScale;
            count++;
        }
        avg = sum / count;
        if (avg > threshold)
            valueMulti -= stepIncr;
        if (avg < threshold)
            valueMulti += stepIncr;

        if (scales.Count > sampleCount)
        {
            sum -= scales[0];
            scales.RemoveAt(0);
            count--;
        }
    }

}
