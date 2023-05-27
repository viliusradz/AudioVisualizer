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


    Vector3 pos;
    TrailRenderer trail;
    Material trMat;
    float trailEm = 0;

    private void Start()
    {
        transform.LookAt(transform.parent);
        pos = transform.localPosition;
        trail = GetComponent<TrailRenderer>();
        trail.material = new Material(trail.materials[0]);
        trMat = trail.material;
        defColor = Color.white;
        if (isStaticColor)
            defColor = Color.gray;



    }
    public void BeatValueChange(float val)
    {
        if (val != 0)
        {
            if (monochrome)
            {
                var res = defColor.grayscale;
                defColor = Color.white * res;
            }
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
    private void Update()
    {

        transform.LookAt(transform.parent);
        trMat.SetColor("_EmissionColor", Color.Lerp(trMat.GetColor("_EmissionColor"), defColor * trailEm, 20 * Time.deltaTime));
        if (Input.GetKeyDown(KeyCode.M))
            monochrome = !monochrome;


        if (doParentOffset)
        {
            transform.LookAt(transform.parent);
            transform.localPosition = Vector3.Lerp(transform.localPosition, pos + transform.forward * positionTo, parentOffsetSpeed*Time.deltaTime);
        }
    }
}
