using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class InfinityCycler : MonoBehaviour
{
    public static InfinityCycler inst;

    public GameObject infiniPrefab;
    public float forwardSpeed = 10;
    public float meshOffset = 400;
    public int xSize = 10;
    public int ySize = 10;
    public bool autoSize = false;
    public float inputMulti = 1000;
    public float changeDamp = 3;

    public float cameraAxeleration = 3;
    public float maxCameraAxel = 6;
    public float minCameraAxel = 0.1f;

    [Header("Scaling Params")]
    public float scalePowTen = 4;

    private float axelerationMulti = 1;

    FrequencyAnalizer analyze;

    [HideInInspector]
    public Vector3[] vertices;
    [HideInInspector]
    public Vector3[] reversedVertices;
    private bool fliped = true;
    private List<GameObject> meshes = new List<GameObject>();
    // Start is called before the first frame update
    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(this);
    }
    void Start()
    {
        analyze = FrequencyAnalizer.inst;
        if (autoSize)
        {
            ySize = (int)Mathf.Sqrt(analyze.buffSize) - 1;
            xSize = ySize;
        }
        else
            ySize = (analyze.buffSize / (xSize + 1)) - 1;

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Vector3.zero;
        }
        reversedVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            reversedVertices[i] = Vector3.zero;
        }

        analyze.soundSamples.AddListener(UpdateValues);
        SpawnNewMesh();

    }
    public void Update()
    {
        DestroyMesh();
        if (meshes.Count < 3)
            SpawnNewMesh();
        foreach (GameObject go in meshes)
        {
            go.transform.localPosition = go.transform.localPosition + transform.right * forwardSpeed* axelerationMulti * Time.deltaTime;
        }
    }

    private void UpdateValues(float[] buffer)
    {
        ApplyReversingAndConnecting(buffer);
    }

    private void ApplyReversingAndConnecting(float[] buffer)
    {
        CycleSpeed(buffer.Take(20000).ToArray());

        buffer = ScaleBuffer(buffer);

        var time = Time.deltaTime;
        int i = 0;
        for (int k = 0; k < vertices.Length; k++)
        {
            vertices[k].y = Mathf.Lerp(vertices[k].y, buffer[k] * inputMulti, changeDamp * time);
        }
        for (int y = 0; y <= ySize; y++)
        {
            int ind = i + xSize;
            for (int x = 0; x <= xSize; x++)
            {
                reversedVertices[ind - x].y = Mathf.Lerp(reversedVertices[ind - x].y, buffer[i] * inputMulti, changeDamp * time);
                reversedVertices[i].y = Mathf.Lerp(reversedVertices[i].y, buffer[ind - x] * inputMulti, changeDamp * time);
                if (ind == i)
                {
                    for (int g = 0; g < 10; g++)
                    {
                        reversedVertices[i - g].y = vertices[i + g - xSize].y;
                        reversedVertices[i + g - xSize].y = vertices[i - g].y;
                    }
                }
                i++;
            }
        }
    }

    private float[] ScaleBuffer(float[] buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = buffer[i] * Mathf.Pow(10, scalePowTen);
        }
        return buffer;
    }

    private void SpawnNewMesh()
    {
        if (meshes.Count == 0)
        {
            meshes.Add(Instantiate(infiniPrefab, transform.position + transform.forward * meshOffset * meshes.Count, Quaternion.identity, transform));
            fliped = false;
        }
        else
        {
            var pos = Vector3.zero;
            pos.z += meshes.Last().transform.localPosition.z + meshOffset;
            var res = Instantiate(infiniPrefab, pos, Quaternion.identity, transform);
            res.transform.localPosition = pos;
            if (!fliped)
            {
                res.GetComponent<MeshControl>().revesedBuffer = true;
                fliped = true;
            }
            else
                fliped = false;

            meshes.Add(res);
        }
    }
    private void DestroyMesh()
    {
        if (meshes[0].transform.localPosition.z + meshOffset < 0)
        {
            Destroy(meshes.First());
            meshes.RemoveAt(0);
        }

    }

    private void CycleSpeed(float[] buffer)
    {
        float avg = 0;
        for (int i = 0; i < buffer.Length; i++)
        {
            avg += buffer[i];
        }
        avg/= buffer.Length;
        avg *= cameraAxeleration;
        axelerationMulti = Mathf.Clamp(avg, minCameraAxel, maxCameraAxel);
    }
}
