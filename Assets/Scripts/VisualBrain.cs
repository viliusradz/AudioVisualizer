using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class VisualBrain : MonoBehaviour
{
    public bool colorChange = false;
    public float maxScaleLessor = 1.3f;
    //public float changeInterval = 0.3f;
    //public float colorJumInterval = 0.1f;
    //public Gradient gr;
    public AnimationCurve colorCurveR;
    public AnimationCurve colorCurveG;
    public AnimationCurve colorCurveB;


    public bool changeScale = false;
    public bool changePosition = false;
    public float toScale = 1;
    public float sideScale = 1;
    public float smoothTime = 4f;
    public float scaleYBy = 4000;
    public float maxScale = 40;
    public float powToVal = 1;
    public float minScale = 0.1f;
    public float scalableTill = 0.2f;
    public float randomOffset = 6;
    public float scaleIfSm = 60;
    public float scaleMulti = 1;
    public Material defMat = null;
    public bool isWithOffset;

    [HideInInspector]
    public float trueScale = 0;

    private Vector3 positionOffset = Vector3.zero;

    Vector3 newScale;
    Vector3 stPos;
    float normScale = 0;
    Material noteMat;
    private float currentTime = 0;
    public void CubeChangeParams()
    {
        newScale = new(sideScale, 1, sideScale);
        stPos = transform.localPosition;
        noteMat = new Material(defMat);
        gameObject.GetComponent<Renderer>().material = noteMat;
    }
    public void ChangeYValue(float tScale, float additionalScale = 1)
    {
        trueScale = tScale;
        var yScale = tScale * scaleYBy * additionalScale;

        if (yScale < scalableTill)
            yScale = ChangeSmallScale(yScale);

        yScale = MathF.Pow(MathF.Abs(yScale), scaleMulti);

        yScale = Mathf.Clamp(yScale, minScale, maxScale);

        if (changePosition)
            positionOffset.y = yScale;

        if (changeScale)
            newScale.y = yScale;
    }
    private void Update()
    {
        if (changeScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, newScale, smoothTime * Time.deltaTime);
        }
        if (changePosition)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition,positionOffset + stPos, smoothTime *Time.deltaTime);
        }
        normScale = Mathf.Clamp01((transform.localScale.y - minScale) / ((maxScale/maxScaleLessor)- minScale));

        //noteMat.color = Mathf.CorrelatedColorTemperatureToRGB(Mathf.Clamp(normScale*1000, 1000,40000));
        if (colorChange)
        {
            //float gr = Mathf.Clamp01(MathF.Sin(MathF.Pow(normScale, 2)) + normScale);
            //float bl = Mathf.Clamp01(-Mathf.Pow(normScale, 3) + MathF.Pow(normScale, 2) + 2 * normScale);
            //noteMat.color = new Color(1 - Mathf.Clamp(gr, 0.45f, 1f), .4f, Mathf.Clamp(bl, 0.2f, 1f));
            noteMat.color = GenerateColorOnSound(normScale);
            //if (currentTime + changeInterval/(newScale.y/10) < Time.time)
            //{
            //    noteMat.color = Color.Lerp(ChangeColorValue(noteMat.color, colorJumInterval),noteMat.color, 1 *Time.deltaTime);
            //    currentTime = Time.time;
            //}
        }
        else
            noteMat.color = Color.gray;
        //noteMat.color = new Color(Mathf.Clamp01(gr-0.1f), Mathf.PerlinNoise(gr, bl + 0.1f), bl);

        if (isWithOffset)
            transform.localPosition = transform.localScale / 2 + stPos;
    }
    private float ChangeSmallScale(float yScale)
    {
        yScale *= scaleIfSm;
        yScale += minScale;
        return yScale;
    }

    private Color ChangeColorValue(Color color, float offset)
    {
        var r = Random.Range(Mathf.Clamp01(color.r - offset), Mathf.Clamp01(color.r + offset));
        var g = Random.Range(Mathf.Clamp01(color.g - offset), Mathf.Clamp01(color.g + offset));
        var b = Random.Range(Mathf.Clamp01(color.b - offset), Mathf.Clamp01(color.b + offset));
        return new Color(r, g, b);
    }
    private Color GenerateColorOnSound(float noteValue)
    {
        return new Color(Mathf.Clamp01(colorCurveR.Evaluate(noteValue)), Mathf.Clamp01(colorCurveG.Evaluate(noteValue)), Mathf.Clamp01(colorCurveB.Evaluate(noteValue)));
    }


}
