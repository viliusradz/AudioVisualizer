using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public SpinAxis axisAround;
    public float spinMulti = 4;
    public float exaduration = 1;
    public float maxSpin = 30;
    public float minSpin = 0;
    public bool reverseSpin = false;

    private float rotateBy = 0;
    private FrequencyAnalizer analyze;
    // Start is called before the first frame update
    void Start()
    {
        analyze = FrequencyAnalizer.inst;
        analyze.soundSamples.AddListener(RotationSet);
    }

    // Update is called once per frame
    void Update()
    {
        if (axisAround == SpinAxis.X)
            transform.Rotate(Vector3.right, rotateBy * Time.deltaTime);
        else if (axisAround == SpinAxis.Y)
            transform.Rotate(Vector3.up, rotateBy * Time.deltaTime);
        else if (axisAround == SpinAxis.Z)
            transform.Rotate(Vector3.forward, rotateBy * Time.deltaTime);
    }
    private void RotationSet(float[] buffer)
    {
        float avg = 0;
        for (int i = 0; i < 20000; i++)
        {
            avg += buffer[i];
        }
        avg /= 20000;
        avg *= Mathf.Pow(10, spinMulti);
        avg = Mathf.Pow(avg, exaduration);
        avg = Mathf.Clamp(avg, minSpin, maxSpin);
        if(reverseSpin)
            avg = -avg;
        rotateBy = avg;
    }

    public enum SpinAxis
    {
        X, Y, Z
    }
}
