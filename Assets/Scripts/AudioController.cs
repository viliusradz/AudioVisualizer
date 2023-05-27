using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioController : MonoBehaviour
{

    //SpawnCubesFromCenter
    //MakeWithoughtOffset

    FrequencyAnalizer analizer;

    public GameObject cubePref;
    public AnimationCurve valueOffset;
    public int cubeAmount = 100;
    public float cubeOffset = 1.2f;
    public float sideScale = 0.3f;
    public float timeToScale = 4f;
    public int startFromSample = 0;
    public int samplesPerNote = 100;
    public Material noteMat;

    public bool isScaledWithOffset = false;
    public bool spawnFromCenter = false;


    private List<VisualBrain> cubeList = new();
    //public Dictionary<VisualBrain, AIValueConfig> cubeConfig = new();
    private int samplesPerVisual;
    float hightestValue = 0.0001f;
    // Start is called before the first frame update
    void Start()
    {
        analizer = FrequencyAnalizer.inst;
        analizer.soundSamples.AddListener(UpdateVisuals);
        if (samplesPerNote <= 0)
            samplesPerVisual = 20000 / cubeAmount;
        else
            samplesPerVisual = samplesPerNote;
        var pos = transform.position;
        float posOff = 0;
        float curNote = 1;
        bool diffAdded = false;
        for (int i = 0; i < cubeAmount; i++)
        {
            var res = Instantiate(cubePref, pos + new Vector3(posOff, 0), transform.rotation, transform).GetComponent<VisualBrain>();
            //cubeConfig.Add(res, res.GetComponent<AIValueConfig>());

            res.name = "cube" + i.ToString();
            res.powToVal = curNote;
            res.isWithOffset = isScaledWithOffset;
            res.defMat = noteMat;
            res.CubeChangeParams();
            cubeList.Add(res);

            if (spawnFromCenter)
            {
                if (i % 2 == 0)
                    posOff += cubeOffset;
                posOff = -posOff;
            }
            else
                posOff += cubeOffset;

            if (i > 50)
                curNote += 0.1f;
            if (samplesPerNote * i > 950 && !diffAdded)
            {
                curNote += 3;
                diffAdded = true;
            }
        }
    }

    void UpdateVisuals(float[] buff)
    {
        int cubeIndex = 0;
        int currInd = startFromSample;
        foreach (var cube in cubeList)
        {
            float total = 0;
            for (int i = 0; i < samplesPerVisual; i++)
            {
                if (buff[currInd] > hightestValue)
                {
                    hightestValue = buff[currInd];
                }
                total += (buff[currInd] / hightestValue) * valueOffset.Evaluate((float)currInd / 20000);
                //total += (buff[currInd] / hightestValue) * valueOffset.Evaluate(currInd / cubeAmount) * cubeConfig[cube].valueMulti;
                currInd++;
            }
            float scaleNote = total / samplesPerVisual;
            cube.ChangeYValue(scaleNote);
            cubeIndex++;
        }
    }



}
