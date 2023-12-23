using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class ClockController : MonoBehaviour
{
    public ArrowObject secArrow;
    public ArrowObject minArrow;
    public ArrowObject hourArrow;
    public GameObject hIndicator;
    public GameObject quaterIndicator;
    public Transform indicatorHolder;

    [Header("Hour Representation")]
    public List<Transform> hourText;

    [Header("Indicator settings")]
    public float markDistance = 15;
    public float errorBounds = 2f;
    public bool startAnimation = true;
    public float startAnimationSpeed = 0.05f;
    public float startAnimationPosition = 1f;

    private float degChangeSec = -6;
    private float degChangeMin = -6;
    private float degHChange = -360 / 12f;

    [Header("Clock spin effect")]
    public int minSample = 0;
    public int maxSample = 20000;
    public float tenToPower = 3;
    public float threshold = 1;



    private List<Indicator> indicators = new();

    // Start is called before the first frame update
    void Start()
    {
        FrequencyAnalizer.inst.soundSamples.AddListener(SpinClockOnMusic);

        CreateMarks();
        UpdateArrowPosition();
        if(startAnimation)
        {
            secArrow.arrowControl.StartArrowAnimation(startAnimationSpeed, Vector3.zero);
            minArrow.arrowControl.StartArrowAnimation(startAnimationSpeed, Vector3.zero);
            hourArrow.arrowControl.StartArrowAnimation(startAnimationSpeed, Vector3.zero);
        }
    }


    void Update()
    {
        UpdateArrowPosition();
        IndicatorAnimation();
    }

    private void IndicatorAnimation()
    {
        float rotZ = 360 - secArrow.arrowHolder.localRotation.eulerAngles.z;
        foreach (var indicator in indicators)
        {
            if (rotZ > indicator.angleZ - errorBounds && rotZ < indicator.angleZ + errorBounds)
            {
                indicator.Animation();
            }
        }
    }
    private void UpdateArrowPosition()
    {
        var time = DateTime.Now;
        secArrow.arrowHolder.localRotation = Quaternion.Euler(0, 0, degChangeSec * time.Second + (degChangeSec / 1000f) * time.Millisecond);
        minArrow.arrowHolder.localRotation = Quaternion.Euler(0, 0, degChangeMin * time.Minute + (degChangeMin / 60f) * time.Second);
        hourArrow.arrowHolder.localRotation = Quaternion.Euler(0, 0, degHChange * time.Hour + (degHChange / 60) * time.Minute);
    }


    private void CreateMarks()
    {
        GameObject res;
        for (int i = 0; i < 60; i++)
        {
            Vector3 pos = new Vector3(Mathf.Sin(-degChangeMin * i * Mathf.Deg2Rad) * markDistance, Mathf.Cos(-degChangeMin * i * Mathf.Deg2Rad) * markDistance, 0);
            var rot = Quaternion.Euler(new Vector3(0, 0, degChangeMin * i));
            if (i % 5 != 0)
            {
                res = Instantiate(quaterIndicator, indicatorHolder.position, rot, indicatorHolder);
            }
            else
                res = Instantiate(hIndicator, indicatorHolder.position, rot, indicatorHolder);
            var indi = res.GetComponent<Indicator>();
            indi.SetParameters(-degChangeMin * i, res.transform.localScale);
            if (startAnimation)
                indi.StartAnimation(startAnimation, startAnimationSpeed,
                    new Vector3(Mathf.Sin(-degChangeMin * i * Mathf.Deg2Rad) * startAnimationPosition, Mathf.Cos(-degChangeMin * i * Mathf.Deg2Rad) * startAnimationPosition, 0),
                    pos);
            else
                indi.StartAnimation(startAnimation, startAnimationSpeed, pos, pos);
            indicators.Add(indi);
        }
    }

    private void SpinClockOnMusic(float[] buff)
    {

        float avg = 0;
        for (int i = minSample; i < maxSample; i++)
        {
            avg += buff[i];
        }
        avg /= maxSample - minSample;

        MoveHourMarks(buff);

        var rotationAmount = -avg * Mathf.Pow(10f, tenToPower);


        if (MathF.Abs(rotationAmount) > threshold)
        {
            transform.Rotate(Vector3.forward, rotationAmount);

            CompensateTextRotation(rotationAmount);
        }
        else
        {
            transform.Rotate(Vector3.forward, 0.1f);
            CompensateTextRotation(0.1f);
        }

    }

    private void CompensateTextRotation(float rotationAmount)
    {
        foreach (var item in hourText)
        {
            item.Rotate(Vector3.forward, -rotationAmount);
        }
    }

    private void MoveHourMarks(float[] buff)
    {
        var averageNoteValue = 0f;
        for (int i = 2000; i < 4000; i++)
        {
            averageNoteValue += buff[i];
        }
        averageNoteValue /= 2000;

        var offsetValue = averageNoteValue * MathF.Pow(10, 5);
        foreach (var item in hourText)
        {
            item.transform.GetComponent<HourMark>().OffsetAnimation(offsetValue);
            offsetValue = -offsetValue; 
        }
    }
}
[System.Serializable]
public class ArrowObject
{
    public Transform arrowHolder;
    public Arrow arrowControl;
}