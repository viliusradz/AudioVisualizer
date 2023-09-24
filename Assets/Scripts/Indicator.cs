using System.Collections;
using System.Collections.Generic;
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

    private Material material;
    private Color startColor;
    private bool animationCalled = false;

    // Start is called before the first frame update
    void Start()
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
    }

    public void SetParameters(float angl, Vector3 scale)
    {
        angleZ = angl;
        transform.localScale = scale;
        startScale = transform.localScale;
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

    private void ColorAnimation()
    {
        material.color = Color.Lerp(material.color, startColor, colorSpeed * Time.deltaTime);
        if (material.color == startColor)
            colorChange = false;
    }
    private void ScaleAnimation()
    {
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
