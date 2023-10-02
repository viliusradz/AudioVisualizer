using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float animSpeed = 1;
    private Vector3 scale;
    private Vector3 stScale;
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
        transform.localScale = startScale;
        animSpeed = speed;
        stAnim = true;
    }
    private void StartAnimationUpdate()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, scale, animSpeed * Time.deltaTime);
        if (transform.localScale == scale)
            stAnim = false;
    }
}
