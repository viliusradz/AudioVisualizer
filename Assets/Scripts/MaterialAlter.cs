using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialAlter : MonoBehaviour
{
    public Material changeMat;
    public int xSize = 1024;
    public int ySize = 1024;
    public float saturation = 1;
    public float value = 1;
    public float bufferMulti = 4;
    public float changeDamp = 3;

    private FrequencyAnalizer analyze;
    private Texture2D texture;
    // Start is called before the first frame update
    void Start()
    {
        analyze = FrequencyAnalizer.inst;
        texture = new Texture2D(xSize, ySize, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        analyze.soundSamples.AddListener(ChangeHueValues);
    }

    // Update is called once per frame
    //void Update()
    //{
    //    for (int x = 0; x < res; x++)
    //    {
    //        for(int y = 0; y < res; y++)
    //        {
    //            texture.SetPixel(x, y, Color.HSVToRGB());
    //        }
    //    }
    //}
    private void ChangeHueValues(float[] buffer)
    {
        var colorValues = new float[buffer.Length];
        for (int i = 0; i < colorValues.Length; i++)
        {
            colorValues[i] = buffer[i];
            colorValues[i] *= Mathf.Pow(10, bufferMulti);
            colorValues[i] = Mathf.Clamp(colorValues[i], 0f, 360f);
        }
        int ind = 0;
        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                texture.SetPixel(x, y,Color.Lerp(Color.HSVToRGB(colorValues[ind],saturation, value),texture.GetPixel(x,y), changeDamp *Time.deltaTime));
                ind++;
            }
        }
        texture.Apply();
        changeMat.mainTexture = texture;
    }
}
