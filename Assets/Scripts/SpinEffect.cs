using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class SpinEffect : MonoBehaviour
{
    public float maxRotationSpeed = 100f;
    public float rotationSpeed = 100f;
    public float exadurationMulti = 2f;
    public float orderIndex = 1;

    [Header("Reverse spin")]
    public bool reverseSpin = false;
    public bool inverseAtPoint = false;
    public float inverseCoeficient = 20;

    //[Header("Change Position")]
    //public bool changeDistance = true;
    //public float lowerBy = 10;

    [Header("Spectrum bounds")]
    public int minBound = 0;
    public int maxbound = 100;

    public float distanceSpeed = 10;

    Vector3 startPos;
    Vector3 posTo = new Vector3();
    private Vector3 rotateTo = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        FrequencyAnalizer.inst.soundSamples.AddListener(Spinny);
        startPos = transform.position;
        if (maxbound < 0)
            maxbound = FrequencyAnalizer.inst.buffSize;
    }
    private void Update()
    {
        transform.Rotate(rotateTo * Time.deltaTime);
        //if (changeDistance)
        //    transform.position = Vector3.Lerp(transform.position, posTo, distanceSpeed*Time.deltaTime);
    }

    //void SpinParent(float[] values)
    //{
    //    float average = 0;
    //    float currentValue = 0;
    //    float avgNegative = 0;
    //    int noVal = 0;
    //    int valIndex = 0;
    //    foreach (float value in values)
    //    {
    //        currentValue = Mathf.Abs(value);
    //        valIndex++;
    //        if (System.MathF.Round(currentValue, 5) == 0)
    //            noVal++;
    //        else
    //            noVal = 0;

    //        if (noVal < 20)
    //        {
    //            average += currentValue;
    //            avgNegative += value;
    //        }

    //        if (noVal > 1000)
    //            break;
    //    }
    //    average /= valIndex;
    //    var res = Mathf.Pow(rotationSpeed * average, exadurationMulti);
    //    rotateTo.z = res;
    //    if (changeDistance)
    //    {
    //        if (avgNegative < 0)
    //            res = -res;
    //        posTo = startPos;
    //        posTo.z -= res / lowerBy;
    //    }

    //}

    private void Spinny(float[] buff)
    {
        float sum = 0;
        for (int i = minBound; i < maxbound; i++)
        {
            sum += buff[i];
        }
        sum = sum / (maxbound - minBound);
        sum *= Mathf.Pow(orderIndex, 2) * rotationSpeed;
        //sum *= orderIndex;
        //sum *= maxbound - minBound;
        sum = Mathf.Pow(sum, exadurationMulti);
        sum = Mathf.Clamp(sum, -maxRotationSpeed, maxRotationSpeed);
        if (inverseAtPoint)
        {
            if (sum > inverseCoeficient)
                sum = -sum;
        }
        if (reverseSpin)
            rotateTo.z = -sum;
        else
            rotateTo.z = sum;

    }
}
