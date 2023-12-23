using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HourMark : MonoBehaviour
{

    public float animationSpeed = 1;
    private Vector3 startPostition;

    private void Start()
    {
        startPostition = transform.localPosition;        
    }

    public void OffsetAnimation(float offset)
    {
        transform.localPosition = Vector3.Lerp(startPostition, transform.localPosition + transform.forward * offset, Time.deltaTime *animationSpeed);
    }
}
