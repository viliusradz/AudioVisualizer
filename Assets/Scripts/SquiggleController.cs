using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SquiggleController : MonoBehaviour
{
    public LineRenderer traiSquiggle;
    public AnimationCurve equalizer;
    public float valueMulti = 3;
    public float damp = 3;
    public bool shapeChange = true;
    public float maxOut = 30f;

    [Header("CircularLayout")]
    public float range = 30;
    public int linePoints = 360;
    public bool animInvard = false;

    private Vector3[] squigglePos;
    private Vector3[] startSquigglePos;
    private FrequencyAnalizer analyzer;
    private float deg = 360f;
    private Shapes setShape;
    // Start is called before the first frame update
    void Start()
    {
        linePoints *= 2;
        analyzer = FrequencyAnalizer.inst;
        traiSquiggle.positionCount = linePoints;
        squigglePos = new Vector3[linePoints];
        startSquigglePos = new Vector3[squigglePos.Length];
        deg /= linePoints;
        RandomLineLayout();
        analyzer.soundSamples.AddListener(DrawSquiggle);
    }

    private void RandomLineLayout()
    {
        var res = UnityEngine.Random.Range(0, 0);
        switch (res)
        {
            case 0:
                setShape = Shapes.Circle;
                CircleLayout();
                break;
        }
    }
    private void Update()
    {
        for (int i = 0; i < squigglePos.Length; i++)
        {
            traiSquiggle.SetPosition(i, Vector3.Lerp(traiSquiggle.GetPosition(i), squigglePos[i], damp * Time.deltaTime));
        }
    }

    private void DrawSquiggle(float[] buffer)
    {

        if (setShape == Shapes.Circle && shapeChange)
        {
            float newDist = 0;
            int index = 0;
            for (int z = 0; z < 2; z++)
            {
                for (int i = 0; i < squigglePos.Length / 2; i++)
                {
                    if (z == 0)
                        newDist = range + buffer[i] * Mathf.Pow(10, valueMulti) * equalizer.Evaluate((float)i / linePoints);
                    else if (z == 1)
                        newDist = range + buffer[(squigglePos.Length / 2) - i] * Mathf.Pow(10, valueMulti) * equalizer.Evaluate((float)(squigglePos.Length / 2 - i) / linePoints);
                    newDist = Mathf.Clamp(newDist, 0, maxOut);
                    var rad = deg * index * Mathf.Deg2Rad;
                    var x = Mathf.Cos(rad) * newDist;
                    var y = Mathf.Sin(rad) * newDist;
                    if (animInvard)
                        squigglePos[index] = startSquigglePos[index] - Vector3.up * (y - range) - Vector3.right * (x - range);
                    else
                        squigglePos[index] = startSquigglePos[index] + Vector3.up * y + Vector3.right * x;
                    index++;
                }
            }
        }
        else
        {
            float newDist = 0;
            float avg = 0;
            for (int i = 0; i < 20000; i++)
            {
                avg += buffer[i];
            }
            avg /= 20000f;
            avg *= MathF.Pow(10, valueMulti);
            for (int i = 0; i < squigglePos.Length; i++)
            {
                if (animInvard)
                    newDist = range - avg;
                else
                    newDist = range + avg;
                var rad = deg * i * Mathf.Deg2Rad;
                var x = Mathf.Cos(rad) * newDist;
                var y = Mathf.Sin(rad) * newDist;
                squigglePos[i] = startSquigglePos[i] + Vector3.up * y + Vector3.right * x;
                i++;
            }
        }



    }

    private void CircleLayout()
    {
        for (int i = 0; i < linePoints; i++)
        {
            Vector3 pos = Vector3.zero;
            var rad = deg * i * Mathf.Deg2Rad;
            pos.x = Mathf.Cos(rad) * range;
            pos.y = Mathf.Sin(rad) * range;
            squigglePos[i] = pos;
        }
        squigglePos.CopyTo(startSquigglePos, 0);

    }
    public enum Shapes
    {
        Circle
    }
}
