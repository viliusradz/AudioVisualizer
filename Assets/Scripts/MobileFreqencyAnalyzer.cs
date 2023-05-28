using CSCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileFreqencyAnalyzer : MonoBehaviour
{

    private float[] buffer;
    // Start is called before the first frame update
    void Start()
    {
        //var buffSize = (int)Math.Floor(Math.Log(sampleSource.WaveFormat.SampleRate * sampleSource.WaveFormat.Channels, 2));
        //buffSize = (int)Math.Pow(2, buffSize);
        print(Microphone.GetPosition(null));

    }

    // Update is called once per frame
    void Update()
    {

    }
}
