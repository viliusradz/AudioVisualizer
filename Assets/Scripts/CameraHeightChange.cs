using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHeightChange : MonoBehaviour
{

    [Header("Camera settings")]
    public float cameraYHeight = 3;
    public float heightChangeMulti = 100;
    public float exaduration = 2;
    public float maxHeight = 30;
    public float damp = 3;
    public bool cameraChangeYHeight = true;
    private Camera cam;
    private Vector3 camStPos;

    private FrequencyAnalizer analyze;
    // Start is called before the first frame update
    void Start()
    {
        analyze = FrequencyAnalizer.inst;
        cam = Camera.main;
        camStPos = cam.transform.position;
        camStPos.y = cameraYHeight;

        analyze.soundSamples.AddListener(FrequencyAvg);
    }

    private void FrequencyAvg(float[] buffer)
    {
        float avg = 0;
        for (int i = 0; i < 20000; i++)
        {
            avg += buffer[i];
        }
        avg /= 20000f;
        avg *= Mathf.Pow(10, heightChangeMulti);
        avg = Mathf.Pow(avg, exaduration);
        avg = Mathf.Clamp(avg, 0, maxHeight);
        var res = camStPos + Vector3.up* avg;
        cam.transform.position = Vector3.Lerp(cam.transform.position, res, damp *Time.deltaTime);
    }
}
