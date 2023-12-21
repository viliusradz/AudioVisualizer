using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float animSpeed = 1;
    private Vector3 scale;
    private Vector3 stScale;
    private Vector3 endpos;
    private bool stAnim = false;
    // Start is called before the first frame update
    void Update()
    {
        if (stAnim)
        {
            StartAnimationUpdate();
        }

    }

    public void StartArrowAnimation(float speed, Vector3 startScale)
    {
        scale = transform.localScale;
        endpos = transform.localPosition;
        transform.localPosition = new(transform.localPosition.x, transform.localPosition.y - transform.localScale.y/2, transform.localPosition.z);
        transform.localScale = startScale;
        animSpeed = speed;
        stAnim = true;
    }
    private void StartAnimationUpdate()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, scale, animSpeed * Time.deltaTime);
        transform.transform.position = Vector3.Lerp(transform.localPosition, endpos, animSpeed * Time.deltaTime);
        if (transform.localScale == scale)
            stAnim = false;
    }
}
