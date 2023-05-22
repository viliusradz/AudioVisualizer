using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InfinityCycler : MonoBehaviour
{
    public static InfinityCycler inst;

    public GameObject infiniPrefab;
    public float forwardSpeed = 10;
    public float meshOffset = 400;


    [HideInInspector]
    public float[] buffer;
    FrequencyAnalizer analyze;

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
            go.transform.localPosition = go.transform.localPosition + transform.right * forwardSpeed * Time.deltaTime;
        }
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
    private void UpdateValues(float[] buff)
    {
        buffer = buff;
    }
}
