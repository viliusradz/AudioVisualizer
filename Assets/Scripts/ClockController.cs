using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockController : MonoBehaviour
{
    public GameObject secArrow;
    public GameObject minArrow;
    public GameObject hourArrow;
    public GameObject hIndicator;
    public GameObject quaterIndicator;
    public Transform indicatorHolder;
    [Header("Indicator settings")]
    public float markDistance = 15;
    public float errorBounds = 2f;

    private float degChangeSec = -6;
    private float degChangeMin = -6;
    private float degHChange = -360 / 12f;

    private List<Indicator> indicators = new();

    // Start is called before the first frame update
    void Start()
    {
        CreateMarks();
        UpdateArrowPosition();
    }

    void Update()
    {
        UpdateArrowPosition();
        IndicatorAnimation();
    }

    private void IndicatorAnimation()
    {
        float rotZ = 360 - secArrow.transform.rotation.eulerAngles.z;
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
        secArrow.transform.rotation = Quaternion.Euler(0, 0, degChangeSec * time.Second + (degChangeSec / 1000f) * time.Millisecond);
        minArrow.transform.rotation = Quaternion.Euler(0, 0, degChangeMin * time.Minute + (degChangeMin / 60f) * time.Second);
        hourArrow.transform.rotation = Quaternion.Euler(0, 0, degHChange * time.Hour + (degHChange / 60) * time.Minute);
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
                res = Instantiate(quaterIndicator, pos, rot, indicatorHolder);
            }
            else
                res = Instantiate(hIndicator, pos, rot, indicatorHolder);
            var indi = res.GetComponent<Indicator>();
            indi.SetParameters(-degChangeMin * i, res.transform.localScale);
            indicators.Add(indi);
        }
    }
}