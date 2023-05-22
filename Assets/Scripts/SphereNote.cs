using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class SphereNote : MonoBehaviour
{
    public float minOff = -4;
    public float maxOff = 10;
    public float scalingSpeed = 30;
    public float scalingMulti = 2;

    public bool monochrome = false;
    public bool isStaticColor = false;
    public Color defColor = Color.white;
    public float timeFromColors = 0.3f;

    public bool doCenterOffest = false;
    public float closestToCenter = 1;
    public float furthestFromCenter = 3;
    public float posChangeSpeed = 30;
    public float posScalingMulti = 2;

    public bool changingTrailLenght = false;
    public float lengthChange = 10;
    public float lengthScalingMulti = 2;
    public float lengthDampSpeed = 3;

    public bool doParentOffset = true;
    public float maxOffset = 5;
    private float positionTo = 0;


    public bool changeSphereColor = false;
    public float emmisionMulti = 1.2f;

    private float parentOffsetSpeed = 2;


    //private float changeSpeed = 0;
    //private float dampenTime = 0;

    Vector3 pos;
    TrailRenderer trail;
    Material trMat;
    float beatOffset;
    float trailEm = 0;
    float nowTime = 0;
    float trailTime = 0;

    private void Start()
    {
        transform.LookAt(transform.parent);
        pos = transform.localPosition;
        trail = GetComponent<TrailRenderer>();
        trail.material = new Material(trail.materials[0]);
        trMat = trail.material;
        //trMat.EnableKeyword("_EMISSION");
        defColor = Color.white;
        if (isStaticColor)
            defColor = Color.gray;
        //trMat.color = Color.green;
        trailTime = trail.time;


    }
    public void BeatValueChange(float val)
    {
        beatOffset = val;
        if (val != 0)
        {
            if (monochrome)
            {
                var res = defColor.grayscale;
                defColor = Color.white * res;
            }
            //trailEm = MathF.Pow(beatOffset * scalingSpeed, scalingMulti);
            //trailEm = Mathf.Clamp(trailEm, minOff, maxOff);
        }
    }
    public void OffsetPosition(float value, float offsetSpeed)
    {
        positionTo = value;
        parentOffsetSpeed = offsetSpeed;
    }
    public void ChangeNoteColor(Color color, float intensityMulti)
    {
        defColor = color;
        trailEm = intensityMulti;
    }

    //public void ChangeTrailLength(float changeVal, float dampTime)
    //{
    //    changeSpeed = changeVal;
    //    dampenTime = dampTime;
    //}
    private void Update()
    {
        //var scale = Mathf.Lerp(minOff, maxOff, Mathf.Pow(beatOffset * scalingSpeed, scalingMulti) * Time.deltaTime);
        //if(trailEmmision != 0)
        transform.LookAt(transform.parent);
        trMat.SetColor("_EmissionColor", Color.Lerp(trMat.GetColor("_EmissionColor"), defColor * trailEm, 20 * Time.deltaTime));
        if (Input.GetKeyDown(KeyCode.M))
            monochrome = !monochrome;

        //if (doCenterOffest)
        //{
        //    var posOffset = Mathf.Lerp(closestToCenter, furthestFromCenter, Mathf.Pow(beatOffset * scalingSpeed, scalingMulti) * Time.deltaTime);
        //    transform.localPosition = transform.forward * posOffset + pos;
        //}
        if (doParentOffset)
        {
            transform.LookAt(transform.parent);
            transform.localPosition = Vector3.Lerp(transform.localPosition, pos + transform.forward * positionTo, parentOffsetSpeed*Time.deltaTime);
        }
        //if(changingTrailLenght)
        //{
        //    if(MathF.Round(Mathf.Abs(beatOffset), 3)>0)
        //        trail.time =Mathf.Lerp(trail.time,trailTime / (Mathf.Pow(beatOffset * lengthChange, lengthScalingMulti) * Time.deltaTime), Time.deltaTime *lengthDampSpeed);
        //}
        //if(changingTrailLenght)
        //{
        //    trail.time = Mathf.Lerp(trail.time,trailTime*changeSpeed, Time.deltaTime * dampenTime);
        //}
    }
}
