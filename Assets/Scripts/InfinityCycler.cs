using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class InfinityCycler : MonoBehaviour
{
    public GameObject infiniPrefab;
    public InfinRotation iRot;
    public int spawnedMeshCount = 5;
    public float forwardSpeed = 10;
    public float meshOffset = 400;
    public int xSize = 10;
    public int ySize = 10;
    public bool autoSize = false;
    public float inputMulti = 1000;
    public float changeDamp = 3;

    public float distanceFromCamera = 20;

    public float cameraAxeleration = 3;
    public float axcelTimes = 1;
    public float maxCameraAxel = 6;
    public float minCameraAxel = 0.1f;

    [Header("Scaling Params")]
    public float scalePowTen = 4;
    public AnimationCurve equalizer;

    private float axelerationMulti = 1;

    FrequencyAnalizer analyze;

    [HideInInspector]
    public Vector3[] vertices;
    [HideInInspector]
    public Vector3[] reversedVertices;
    private bool fliped = true;
    private List<GameObject> meshes = new List<GameObject>();

    // pos change speed (dictated by WallDistanceCont)
    private float posChangeDamp = 2;
    // Start local position
    private Vector3 stLocPos;
    // pos to move towards
    private Vector3 posTo = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        stLocPos = transform.localPosition;
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
        SetPosition();
        //analyze.soundSamples.AddListener(UpdateValues);
        SpawnNewMesh();

    }

    public void Update()
    {
        DestroyMesh();
        if (meshes.Count < spawnedMeshCount)
            SpawnNewMesh();
        foreach (GameObject go in meshes)
        {
            go.transform.localPosition = go.transform.localPosition - transform.right * forwardSpeed * axelerationMulti * Time.deltaTime;
        }
        UpdateCyclesPos();
    }

    private void SetPosition()
    {
        if(iRot == InfinRotation.Up)
            stLocPos = new Vector3(0, Camera.main.transform.localRotation.y - distanceFromCamera, -ySize / 2f);
        else if(iRot == InfinRotation.Down)
            stLocPos = new Vector3(0, Camera.main.transform.localRotation.y + distanceFromCamera, ySize / 2f);        
        else if(iRot == InfinRotation.Left)
            stLocPos = new Vector3(0, -ySize / 2f, distanceFromCamera);        
        else if(iRot == InfinRotation.Right)
            stLocPos = new Vector3(0, ySize / 2f, -distanceFromCamera);
        transform.localPosition = stLocPos;
        posTo = stLocPos;
    }

    private void UpdateCyclesPos()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, posTo, posChangeDamp * Time.deltaTime);
    }


    private void SpawnNewMesh()
    {
        if (meshes.Count == 0)
        {
            var res = Instantiate(infiniPrefab, transform.localPosition + transform.forward * meshOffset * meshes.Count, transform.rotation, transform);
            res.GetComponent<MeshControl>().cycler = this;
            RotateCycler(res.transform);

            meshes.Add(res);
            fliped = false;
        }
        else
        {
            var pos = Vector3.zero;
            pos.x += meshes.Last().transform.localPosition.x + meshOffset;
            var res = Instantiate(infiniPrefab, pos, transform.rotation, transform);

            res.transform.localPosition = pos;
            res.GetComponent<MeshControl>().cycler = this;
            RotateCycler(res.transform);

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
    private void RotateCycler(Transform mesh)
    {
        if (iRot == InfinRotation.Down)
            mesh.Rotate(Vector3.right, 180);
        else if (iRot == InfinRotation.Right)
            mesh.Rotate(Vector3.right, 90);
        else if (iRot == InfinRotation.Left)
            mesh.Rotate(Vector3.right, 270);
    }
    private void DestroyMesh()
    {
        if (meshes[0].transform.localPosition.x + meshOffset < 0)
        {
            Destroy(meshes.First());
            meshes.RemoveAt(0);
        }

    }

    public void OffsetPosition(float offset)
    {
        if (iRot == InfinRotation.Up)
            posTo.y = stLocPos.y - offset;
        else if (iRot == InfinRotation.Down)
            posTo.y = stLocPos.y + offset;
        else if (iRot == InfinRotation.Left)
            posTo.z = stLocPos.z + offset;
        else if (iRot == InfinRotation.Right)
            posTo.z = stLocPos.z - offset;
    }

    public void SetValues(Vector3[] ver, Vector3[] rVer, float axcel)
    {
        vertices = ver;
        reversedVertices = rVer;
        axelerationMulti = axcel;
    }

    public enum InfinRotation
    {
        Up,
        Down,
        Left,
        Right
    }
}

