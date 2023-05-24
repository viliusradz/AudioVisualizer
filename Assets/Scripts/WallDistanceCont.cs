using CSCore.Streams.Effects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WallDistanceCont : MonoBehaviour
{
    public List<InfinityCycler> cyclers;
    public float powMulti = 3;
    FrequencyAnalizer analyze;

    public float forwardSpeed = 10;
    public float meshOffset = 400;
    public int xSize = 10;
    public int ySize = 10;
    public bool autoSize = false;
    public float inputMulti = 1000;
    public float changeDamp = 3;

    [Header("Scaling Params")]
    public float scalePowTen = 4;
    public AnimationCurve equalizer;

    public float cameraAxeleration = 3;
    public float axcelTimes = 1;
    public float maxCameraAxel = 6;
    public float minCameraAxel = 0.1f;

    [HideInInspector]
    public Vector3[] vertices;
    [HideInInspector]
    public Vector3[] reversedVertices;

    private float axelerationMulti = 1;

    // Start is called before the first frame update
    void Start()
    {
        analyze = FrequencyAnalizer.inst;
        analyze.soundSamples.AddListener(ChangeWallDistances);
        analyze.soundSamples.AddListener(UpdateValues);

        if (autoSize)
        {
            ySize = (int)Mathf.Sqrt(analyze.buffSize) - 1;
            xSize = ySize;
        }
        else
            ySize = (analyze.buffSize / (xSize + 1)) - 1;

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Vector3.zero;
        }
        reversedVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            reversedVertices[i] = Vector3.zero;
        }
    }

    private void ChangeWallDistances(float[] buffer)
    {
        float avg = 0;
        for (int i = 0; i < 20000; i++)
        {
            avg += buffer[i];
        }
        avg /= 20000;
        avg *= Mathf.Pow(10, powMulti);

        foreach (var item in cyclers)
        {
            item.OffsetPosition(avg);
        }

    }

    private void UpdateValues(float[] buff)
    {
        var buffer = new float[buff.Length];
        buff.CopyTo(buffer, 0);
        ApplyReversingAndConnecting(buffer);
    }

    private void ApplyReversingAndConnecting(float[] buffer)
    {
        CycleSpeed(buffer.Take(20000).ToArray());

        buffer = ScaleBuffer(buffer);

        var time = Time.deltaTime;
        int i = 0;
        for (int k = 0; k < vertices.Length; k++)
        {
            vertices[k].y = Mathf.Lerp(vertices[k].y, buffer[k] * inputMulti, changeDamp * time);
        }
        for (int y = 0; y <= ySize; y++)
        {
            int ind = i + xSize;
            for (int x = 0; x <= xSize; x++)
            {
                reversedVertices[ind - x].y = Mathf.Lerp(reversedVertices[ind - x].y, buffer[i] * inputMulti, changeDamp * time);
                reversedVertices[i].y = Mathf.Lerp(reversedVertices[i].y, buffer[ind - x] * inputMulti, changeDamp * time);
                if (ind == i)
                {
                    for (int g = 0; g < 30; g++)
                    {
                        reversedVertices[i - g].y = vertices[i + g - xSize].y;
                        reversedVertices[i + g - xSize].y = vertices[i - g].y;
                    }
                }
                i++;
            }
        }

        SetCyclerValues();
    }

    private float[] ScaleBuffer(float[] buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = buffer[i] * Mathf.Pow(10, scalePowTen) * equalizer.Evaluate((float)i / buffer.Length);
        }
        return buffer;
    }
    private void CycleSpeed(float[] buffer)
    {
        float avg = 0;
        for (int i = 0; i < buffer.Length; i++)
        {
            avg += buffer[i];
        }
        avg /= buffer.Length;
        avg *= Mathf.Pow(10, cameraAxeleration);
        avg = Mathf.Pow(avg, axcelTimes);
        axelerationMulti = Mathf.Clamp(avg, minCameraAxel, maxCameraAxel);
    }

    private void SetCyclerValues()
    {
        foreach (var item in cyclers)
        {
            item.SetValues(vertices, reversedVertices, axelerationMulti);
        }
    }
}
