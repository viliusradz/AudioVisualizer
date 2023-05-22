using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FFTransform : MonoBehaviour
{
    int SampleRate = 44100;
    int BufferMilliseconds = 20;
    float[] audioValues;
    private void Start()
    {
        audioValues = new float[SampleRate * BufferMilliseconds / 1000];
    }
}
