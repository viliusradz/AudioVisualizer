using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CircularNotes : MonoBehaviour
{
    FrequencyAnalizer analizer;

    public GameObject notePref;

    [Header("Change Position")]
    public bool changeDistance = true;
    public bool inverseChange = false;
    public float distanceChange = 100f;
    public float offsetSpeed = 3;
    public float exadurationMulti = 2f;
    public float maxOffset = 5;
    public float orderIndex = 1;

    [Header("Change Color")]
    public bool colorChange = false;
    public float timeInterval = 0.5f;
    public float intensityMulti = 5;
    public float powIntensity = 1;
    public float maxEmmision = 5;
    private float lastColorChange = 0;
    private Color colorCurrent = Color.white;

    [Header("Spectrum bounds")]
    public int minBound = 0;
    public int maxbound = 100;

    [Header("NoteRotationDeactivator")]
    public bool rotationDeactivate = false;
    public float rotationCheckInterv = 0.1f;
    public float rotateCoefficient = 2f;
    private Vector3 lastPoint = Vector3.zero;
    private float rotationLastCheck = 0;
    private bool rotationChildrenActive = true;


    public int visualAmount = 100;
    public float range = 5f;
    public int startFromSample = 0;
    public int samplesPerNote = 100;
    public Material noteMat;

    private List<SphereNote> visualList = new();
    private int samplesPerVisual;

    // Start is called before the first frame update
    void Start()
    {
        analizer = FrequencyAnalizer.inst;
        analizer.soundSamples.AddListener(UpdateVisuals);
        if (samplesPerNote == 0)
            samplesPerVisual = (analizer.buffSize - 94000) / visualAmount;
        else if (samplesPerNote < 0)
            samplesPerVisual = analizer.buffSize / visualAmount;
        else
            samplesPerVisual = samplesPerNote;


        //float curNote = 1;
        //bool diffAdded = false;
        var angle = 360f / visualAmount;
        for (int i = 0; i < visualAmount; i++)
        {
            var rad = angle * i * Mathf.Deg2Rad;
            var x = Mathf.Cos(rad);
            var y = Mathf.Sin(rad);
            var res = Instantiate(notePref, transform.position, Quaternion.identity, transform).GetComponent<SphereNote>();
            res.name = "Sphere" + visualList.Count.ToString();
            res.transform.localPosition = new Vector3(x, y) * range;
            visualList.Add(res);
            //if (i > 50)
            //    curNote += 0.1f;
            //if (samplesPerNote * i > 950 && !diffAdded)
            //{
            //    curNote += 3;
            //    diffAdded = true;
            //}
        }
    }
    private void Update()
    {
        if (rotationDeactivate)
        {
            if (rotationLastCheck + rotationCheckInterv < Time.time)
            {
                CheckDistanceTraveled();
                rotationLastCheck = Time.time;
            }
        }
    }

    void UpdateVisuals(float[] buff)
    {
        buff = buff.Skip(minBound).Take(maxbound - minBound).ToArray();
        if (changeDistance)
            DistanceChanger(buff);
        if (colorChange)
            ChangeColor(buff);

        //int currInd = startFromSample;
        //foreach (var cube in visualList)
        //{
        //    float total = 0;
        //    for (int i = 0; i < samplesPerVisual; i++)
        //    {
        //        total += MathF.Abs(buff[currInd]);
        //        currInd++;
        //    }
        //    float scaleNote = total / samplesPerVisual;
        //    cube.BeatValueChange(scaleNote);
        //}
    }
    void ChangeColor(float[] buff)
    {
        float sum = 0;
        foreach (var item in buff)
        {
            sum += item;
        }
        sum = sum / (maxbound - minBound);
        sum *= Mathf.Pow(orderIndex, 2) * distanceChange;
        sum *= intensityMulti;
        sum = Mathf.Pow(sum, powIntensity);
        if (Time.time > timeInterval + lastColorChange)
        {
            colorCurrent = UnityEngine.Random.ColorHSV();
            lastColorChange = Time.time;
        }
        sum = Mathf.Clamp(sum, 0, maxEmmision);
        foreach (var cube in visualList)
        {
            cube.ChangeNoteColor(colorCurrent, sum);
        }
    }

    private void DistanceChanger(float[] buff)
    {
        float sum = 0;
        foreach (var item in buff)
        {
            sum += item;
        }
        sum = sum / (maxbound - minBound);
        sum *= Mathf.Pow(orderIndex, 2) * distanceChange;
        sum = Mathf.Pow(sum, exadurationMulti);
        sum = Mathf.Clamp(sum, -maxOffset, maxOffset);
        if (inverseChange)
            sum = -sum;
        foreach (var cube in visualList)
        {
            cube.OffsetPosition(sum, offsetSpeed);
        }
    }

    private void CheckDistanceTraveled()
    {
        var res = Vector3.Distance(transform.up * range + transform.position, lastPoint);
        if (res < rotateCoefficient)
        {
            if (rotationChildrenActive)
            {
                foreach (var cube in visualList)
                    cube.gameObject.SetActive(false);
                rotationChildrenActive = false;
            }
        }
        else
        {
            if (!rotationChildrenActive)
            {
                foreach (var cube in visualList)
                    cube.gameObject.SetActive(true);
                rotationChildrenActive = true;
            }
        }
        lastPoint = transform.up * range + transform.position;
    }

    //private void TrailLengthChange()
    //{
    //    if (lastTrailChange + changeTimeIntv < Time.time)
    //    {
    //        lastTrailChange = Time.time;
    //        var res = transform.rotation.z - lastRotation;
    //        lastRotation = transform.rotation.z;

    //        res *= changeAmount;
    //        res = Math.Abs(1/res);
    //        foreach (var cube in visualList)
    //        {
    //            cube.ChangeTrailLength(res, dampTime);
    //        }
    //    }
    //}
    //[Header("TrailLength")]
    //public bool changeTrailLength = true;
    //public float changeAmount = 10;
    //public float exadurate = 3;
    //public float changeTimeIntv = 0.1f;
    //public float dampTime = 5;
    //private float lastRotation = 0;
    //private float lastTrailChange = 0;
}
