using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public int maxFrequency = 20000;
    public float cameraShakeDamp = 10;
    public float cameraShakeMulti = 10;
    FrequencyAnalizer analyzer;
    Camera cam;
    private float camFOV;

    // Start is called before the first frame update
    void Start()
    {
        analyzer = FrequencyAnalizer.inst;
        analyzer.soundSamples.AddListener(CameraShake);
        cam = Camera.main;
        camFOV = cam.fieldOfView;
    }

    private void CameraShake(float[] buffer)
    {
        float avgVal = 0;
        for (int i = 0; i < maxFrequency; i++)
        {
            avgVal += buffer[i];
        }
        avgVal /= maxFrequency;
        avgVal *= cameraShakeMulti;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, camFOV - avgVal, cameraShakeDamp * Time.deltaTime);
    }
}
