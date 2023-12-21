using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    [HideInInspector]
    public float angleZ;
    [Header("Arrow Hover Animation")]
    public float expandSpeed = 2;
    public float expandedScale = 1.3f;
    public float contractSpeed = 1;
    private bool expanded = false;
    private bool expand = false;
    private bool colorChange = false;
    public float colorSpeed = 1;
    public Color changeColor = Color.white;

    private Vector3 startScale;
    private Vector3 pos;
    private bool loadAnimation = false;
    private float loadSpeed = 1;
    private Vector3 endScale;

    private Material material;
    private Color startColor;
    private bool animationCalled = false;

    // Start is called before the first frame update
    void Awake()
    {
        material = GetComponent<Renderer>().material;
        material = new Material(material);
        GetComponent<Renderer>().material = material;
        startColor = material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (expand)
            ScaleAnimation();
        if (colorChange)
            ColorAnimation();
        if (!expand && !colorChange)
            animationCalled = false;
        if(loadAnimation)
            StartAnimationUpdate();
    }

    public void SetParameters(float angl, Vector3 scale)
    {
        angleZ = angl;
        transform.localScale = scale;
        startScale = transform.localScale;
    }

    public void StartAnimation(bool animation, float speed, Vector3 stPos, Vector3 endPos)
    {
        if (animation)
        {
            loadAnimation = true;
            endScale = transform.localScale;
            transform.localScale = Vector3.zero;

        }
        transform.localPosition = stPos;
        pos =endPos;
        loadSpeed = speed; 
    }

    public void Animation()
    {
        if (!animationCalled)
        {
            changeColor = Color.white;
            material.color = changeColor;
            expand = true;
            colorChange = true;

            animationCalled = true;
        }
    }

    private void StartAnimationUpdate()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, pos, loadSpeed*Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, endScale, loadSpeed*Time.deltaTime);
        if (transform.position == pos && transform.localScale == endScale)
            loadAnimation = false;
    }

    private void ColorAnimation()
    {
        material.color = Color.Lerp(material.color, startColor, colorSpeed * Time.deltaTime);
        if (material.color == startColor)
            colorChange = false;
    }
    private void ScaleAnimation()
    {
        if (animationCalled) return;
        if (!expanded)
            transform.localScale = Vector3.Lerp(transform.localScale, startScale * expandedScale, expandSpeed * Time.deltaTime);
        else
            transform.localScale = Vector3.Lerp(transform.localScale, startScale, contractSpeed * Time.deltaTime);

        if (transform.localScale == expandedScale * startScale)
        {
            expanded = true;
        }
        if (expanded && transform.localScale == startScale)
        {
            expanded = false;
            expand = false;
        }
    }
}
